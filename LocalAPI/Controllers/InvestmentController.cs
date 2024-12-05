using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using LocalAPI.Model;
using LocalAPI.Services;
using ClassLibrary;


namespace LocalAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class InvestmentController : ControllerBase
	{
		private readonly DatabaseService _databaseService;

		public InvestmentController(DatabaseService databaseService)
		{
			_databaseService = databaseService;
		}

		// GET: api/Cities
		[HttpGet]
		public IActionResult GetInv()
		{
			//started making changes
			//don't trust
			List<Investment> investments = new List<Investment>();

			using (MySqlConnection connection = _databaseService.GetConnection())
			{
				connection.Open();
				string query = "SELECT * FROM inv_list";
				MySqlCommand command = new MySqlCommand(query, connection);
				MySqlDataReader reader = command.ExecuteReader();

				while (reader.Read())
				{
					var city = new Investment
					{
						inv_Id = reader.GetInt32(0),
						user_id = reader.GetInt32(1),
						type_id = reader.GetInt32(2),
						option_id = reader.GetInt32(3),
						amount_invested = reader.GetDecimal(4),
						investment_date = reader.GetDateTime(5)
					};
					investments.Add(city);
				}

				reader.Close();
			}

			return Ok(investments);
		}


		// Get: investment/GetCitiesAbovePopulation/veriable
		[HttpGet("GetUserInvestment/{user}")]
		public IActionResult GetUserInvestment(int user)
		{
			List<Investment> largeCities = new List<Investment>(); // רשימה שתכיל את הערים

			using (MySqlConnection connection = _databaseService.GetConnection())
			{
				connection.Open();

				// שאילתה שמחזירה ערים עם אוכלוסייה מעל המספר המינימלי
				string query = "SELECT * FROM inv_list WHERE user_id = @user";
				MySqlCommand command = new MySqlCommand(query, connection);
				command.Parameters.AddWithValue("@user", user); // שימוש בפרמטר

				MySqlDataReader reader = command.ExecuteReader();

				while (reader.Read())
				{
					Investment city = new Investment
					{
						inv_Id = reader.GetInt32(0),
						user_id = reader.GetInt32(1),
						type_id = reader.GetInt32(2),
						option_id = reader.GetInt32(3),
						amount_invested = reader.GetDecimal(4),
						investment_date = reader.GetDateTime(5)
					};
					largeCities.Add(city); // הוספת העיר לרשימה
				}

				reader.Close();
			}

			return Ok(largeCities); // החזרת התוצאות בפורמט JSON
		}

		// Get: investment/Get_users
		[HttpGet("insert_inv/{user}/{type}/{option}/{amount}")]
		public IActionResult insert_inv(int user, int type, int option, int amount)
		{
			using (MySqlConnection connection = _databaseService.GetConnection())
			{
				connection.Open();

				// שאילתה שמחזירה ערים עם אוכלוסייה מעל המספר המינימלי
				string query = "insert into inv_list (user_id, type_id, option_id,amount_invested,investment_date) values(@user,@type,@option,@amount,now())";
				MySqlCommand command = new MySqlCommand(query, connection);
				command.Parameters.AddWithValue("@user", user); // שימוש בפרמטר
				command.Parameters.AddWithValue("@type", type);
				command.Parameters.AddWithValue("@option", option);
				command.Parameters.AddWithValue("@amount", amount);

				MySqlDataReader reader = command.ExecuteReader();



				reader.Close();
				return Ok();
			}

		}
		// httppost
		[HttpGet("remove_inv/{inv_id}")]
		public IActionResult remove_inv(int inv_id)
		{
			using (MySqlConnection connection = _databaseService.GetConnection())
			{
				connection.Open();

				// שאילתה שמחזירה ערים עם אוכלוסייה מעל המספר המינימלי
				string query = "DELETE FROM inv_list WHERE investment_id = @investment_id";
				MySqlCommand command = new MySqlCommand(query, connection);
				command.Parameters.AddWithValue("@investment_id", inv_id);

				MySqlDataReader reader = command.ExecuteReader();



				reader.Close();
				return Ok();
			}

		}

		[HttpGet("Find_user/{password}/{username}")]
		public IActionResult Find_users(int password, string username)
		{
            List<User> users = new List<User>();

            using (MySqlConnection connection = _databaseService.GetConnection())
			{
				connection.Open();

				// שאילתה שמחזירה ערים עם אוכלוסייה מעל המספר המינימלי
				string query = "SELECT * FROM users where password = @password and username = @username";
				MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@password", password);
                command.Parameters.AddWithValue("@username", username);

                MySqlDataReader reader = command.ExecuteReader();
                

                while (reader.Read())
                {
                    User user = new User
                    {
                        user_id = reader.GetInt32(0),
                        admin = reader.GetInt32(1),
                        username = reader.GetString(2),
                        email = reader.GetString(3),
                        password = reader.GetString(4)
                    };
                    users.Add(user); // הוספת העיר לרשימה
                }

                reader.Close();
				return Ok(users);
            }

		}
	}
}