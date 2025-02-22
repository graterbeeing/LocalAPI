using ClassLibrary;
using LocalAPI.Services;
using LocalAPI.Model;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LocalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DatabaseService _databaseService;

        public UserController(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        // GET: api/<UserController>
        [HttpGet("Find_user/{password}/{username}")]
        public IActionResult Find_users(string password, string username)
        {
            List<User> users = new List<User>();

            using (MySqlConnection connection = _databaseService.GetConnection())
            {
                connection.Open();

                // שאילתה שמחזירה ערים עם אוכלוסייה מעל המספר המינימלי
                string query = "SELECT * FROM users where username = @username";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@username", username);

                MySqlDataReader reader = command.ExecuteReader();
                //under development (need to unhash the password and check


                while (reader.Read())
                {
                    string stored_pass = reader.GetString(4);
                    if(!VerifyPassword(password, stored_pass))
                    {
                        return Unauthorized("Invalid email or password.");
                    }
                    else
                    {
                        User user = new User();
                        user.user_id = reader.GetInt32(0);
                        user.admin = reader.GetInt32(1);
                        user.username = reader.GetString(2);
                        user.email = reader.GetString(3);
                        user.password = reader.GetString(4);
                        users.Add(user);
                    }
                }

                reader.Close();
                return Ok(users);
            }



        }


        // POST api/<UserController>
        [HttpPost("Sign-in")]
        public async Task<IActionResult> Sign_in([FromBody] User user)
        {
            if (user == null || string.IsNullOrWhiteSpace(user.password))
            {
                return BadRequest("Invalid registration data.");
            }

            var hashedPassword = HashPassword(user.password); // Hash the password

            using var connection = _databaseService.GetConnection();
            await connection.OpenAsync();
            var query = "SELECT * FROM users where username = @username";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@username", user.username);
            MySqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Close();
                return BadRequest("username already in use");
            }
            else
            {
                reader.Close();
                query = "INSERT INTO users (username, Email, password) VALUES (@username, @Email, @Password)";
                using var command2 = new MySqlCommand(query, connection);
                command2.Parameters.AddWithValue("@username", user.username);
                command2.Parameters.AddWithValue("@Email", user.email);
                command2.Parameters.AddWithValue("@Password", hashedPassword); // Store hashed password
                await command2.ExecuteNonQueryAsync();
                return Ok("Registration successful.");
                // plan to make it automatically log in. returning the new user and using user_service
            }

        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }


        private string HashPassword(string password)
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            // Combine hash and salt for storage
            return $"{Convert.ToBase64String(salt)}:{hashed}";
        }

        private bool VerifyPassword(string enteredPassword, string storedHash)
        {
            var parts = storedHash.Split(':');
            var salt = Convert.FromBase64String(parts[0]);
            var storedHashedPassword = parts[1];

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: enteredPassword,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return hashed == storedHashedPassword;
        }

    }
}
