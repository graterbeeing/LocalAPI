using ClassLibrary;
using LocalAPI.Services;
using LocalAPI.Model;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity.Data;
using Org.BouncyCastle.Asn1.Ocsp;
using Microsoft.Extensions.Options;
using System.Data;

namespace LocalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OptionsController : Controller
    {
        private readonly DatabaseService _databaseService;

        public OptionsController(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        // POST api/<OptionsController>
        [HttpPost("Insert_stock")]
        public async Task<IActionResult> Insert_stock([FromBody] OptionsRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Optionsymbol) || request.type < 1 || request.type > 2)
            {
                return BadRequest("Invalid request data.");
            }

            using var connection = _databaseService.GetConnection();
            await connection.OpenAsync();

            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                // 1️ Insert into `options` table
                var insertOptionsQuery = "insert into options_list(symbol,type_id) values(@symbol,@type); SELECT LAST_INSERT_ID();";
                using var optionsCommand = new MySqlCommand(insertOptionsQuery, connection, (MySqlTransaction)transaction);
                optionsCommand.Parameters.AddWithValue("@symbol", request.Optionsymbol);
                optionsCommand.Parameters.AddWithValue("@type", request.type);

                var optionId = Convert.ToInt32(await optionsCommand.ExecuteScalarAsync());

                // 2️ Insert into `stock_list` table using the generated OptionID
                var insertStockQuery = "insert into stock_list (option_id, stock_name) values(@OptionID, @StockName)";
                using var stockCommand = new MySqlCommand(insertStockQuery, connection, (MySqlTransaction)transaction);
                stockCommand.Parameters.AddWithValue("@OptionID", optionId);
                stockCommand.Parameters.AddWithValue("@StockName", request.StockName);

                await stockCommand.ExecuteNonQueryAsync();

                // Commit transaction
                await transaction.CommitAsync();

                return Ok(new { Message = "Data inserted successfully" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Database error: {ex.Message}");
            }

        }


        // can't decide how to code the body for the search in class Library
        [HttpGet("Sort_type/{type}")]
        public IActionResult Sort_type(int type)
        {
            List<OptionsData> options = new List<OptionsData>();

            using (MySqlConnection connection = _databaseService.GetConnection())
            {
                connection.Open();

                // שאילתה שמחזירה ערים עם אוכלוסייה מעל המספר המינימלי
                string query = "SELECT * FROM options_list where type_id = @type";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@type", type);

                MySqlDataReader reader = command.ExecuteReader();
                //under development (need to unhash the password and check


                while (reader.Read())
                {
                    var Option = new OptionsData
                    {
                        Id = reader.GetInt32(0),
                        Symbol = reader.GetString(1),
                        type = reader.GetInt32(2)
                    };
                    options.Add(Option);
                    
                }

                reader.Close();
                return Ok(options);
            }
        }

        [HttpGet("GetOptionId/{id}")]
        public IActionResult GetOptionId(int id)
        {
            List<OptionsData> options = new List<OptionsData>();

            using (MySqlConnection connection = _databaseService.GetConnection())
            {
                connection.Open();

                // שאילתה שמחזירה ערים עם אוכלוסייה מעל המספר המינימלי
                string query = "SELECT * FROM options_list where option_id = @id";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);

                MySqlDataReader reader = command.ExecuteReader();
                //under development (need to unhash the password and check


                while (reader.Read())
                {
                    var Option = new OptionsData
                    {
                        Id = reader.GetInt32(0),
                        Symbol = reader.GetString(1),
                        type = reader.GetInt32(2)
                    };
                    options.Add(Option);

                }

                reader.Close();
                return Ok(options);
            }
        }

        [HttpGet("GetOptionsBySymbol/{symbol}")]
        public IActionResult GetOptionsBySymbol(string symbol)
        {
            List<OptionsData> options = new List<OptionsData>();

            using (MySqlConnection connection = _databaseService.GetConnection())
            {
                connection.Open();

                string query = "SELECT * FROM options_list WHERE symbol = @symbol";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@symbol", symbol);

                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var option = new OptionsData
                    {
                        Id = reader.GetInt32(0),
                        Symbol = reader.GetString(1),
                        type = reader.GetInt32(2)
                    };
                    options.Add(option);
                }

                reader.Close();
                return Ok(options);
            }
        }

        [HttpGet("SearchLikeSymbol/{search?}")]
        public async Task<IActionResult> SearchLikeSymbol(string? search)
        {
            try
            {
                using var connection = _databaseService.GetConnection();
                await connection.OpenAsync();

                var query = "SELECT * FROM options_list WHERE symbol LIKE @search";
                using var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@search", $"{search}%");

                using var reader = await command.ExecuteReaderAsync();

                var result = new List<OptionsData>(); // Make sure OptionsData model matches your table

                while (await reader.ReadAsync())
                {
                    result.Add(new OptionsData
                    {
                        Id = reader.GetInt32("option_id"),
                        Symbol = reader.GetString("symbol"),
                        type = reader.GetInt32("type_id")
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Database error: {ex.Message}");
            }
        }


    }
}
