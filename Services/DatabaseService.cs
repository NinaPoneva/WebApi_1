using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Threading.Tasks;

public class DatabaseService
{
	private readonly string _connectionString;

	public DatabaseService(string connectionString)
	{
		_connectionString = connectionString;
		CreateTableIfNotExists();
	}

	private void CreateTableIfNotExists()
	{
		using (var connection = new SqliteConnection(_connectionString))
		{
			connection.Open();

			var command = connection.CreateCommand();
			command.CommandText = @"
                CREATE TABLE IF NOT EXISTS MessagesMT799 (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Block1 TEXT,
                    Block2 TEXT,
                    Block3 TEXT,
                    Block4 TEXT,
                    Block5 TEXT
                );";
			command.ExecuteNonQuery();
		}
	}

	public async Task<List<MessageModel>> GetAllRecordsAsync()
	{
		var messages = new List<MessageModel>();

		using (var connection = new SqliteConnection(_connectionString))
		{
			await connection.OpenAsync();

			var command = connection.CreateCommand();
			command.CommandText = "SELECT Block1, Block2, Block3, Block4, Block5 FROM MessagesMT799";

			using (var reader = await command.ExecuteReaderAsync())
			{
				while (await reader.ReadAsync())
				{
					var message = new MessageModel
					{
						Block1 = reader.IsDBNull(0) ? null : reader.GetString(0),
						Block2 = reader.IsDBNull(1) ? null : reader.GetString(1),
						Block3 = reader.IsDBNull(2) ? null : reader.GetString(2),
						Block4 = reader.IsDBNull(3) ? null : reader.GetString(3),
						Block5 = reader.IsDBNull(4) ? null : reader.GetString(4),
					};

					messages.Add(message);
				}
			}
		}

		return messages;
	}

	public async Task AddRecordAsync(MessageModel message)
	{
		using (var connection = new SqliteConnection(_connectionString))
		{
			await connection.OpenAsync();

			var command = connection.CreateCommand();
			command.CommandText = @"
            INSERT INTO MessagesMT799 (Block1, Block2, Block3, Block4, Block5) 
            VALUES ($block1, $block2, $block3, $block4, $block5)";
			command.Parameters.AddWithValue("$block1", message.Block1);
			command.Parameters.AddWithValue("$block2", message.Block2);
			command.Parameters.AddWithValue("$block3", message.Block3);
			command.Parameters.AddWithValue("$block4", message.Block4);
			command.Parameters.AddWithValue("$block5", message.Block5);

			await command.ExecuteNonQueryAsync();
		}
	}
}
