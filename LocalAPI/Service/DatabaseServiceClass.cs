﻿using MySql.Data.MySqlClient;

namespace LocalAPI.Services
{
	public class DatabaseService
	{
		private readonly string _connectionString;

		public DatabaseService(IConfiguration configuration)
		{
			_connectionString = configuration.GetConnectionString("DefaultConnection");
		}

		public MySqlConnection GetConnection()
		{
			return new MySqlConnection(_connectionString);
		}
	}
}

