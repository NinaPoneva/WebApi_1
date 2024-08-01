using Microsoft.Data.Sqlite;

public class DatabaseService
{
	private readonly string _connectionString;

	public DatabaseService(string connectionString)
	{
		_connectionString = connectionString;
		CreateTable();
	}

	private void CreateTable()
	{
		using (var connection = new SqliteConnection(_connectionString))
		{
			connection.Open();
			var command = connection.CreateCommand();
			command.CommandText =
			@"
                CREATE TABLE IF NOT EXISTS message (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    part1 TEXT,
                    part2 TEXT,
                    part3 TEXT,
                    part4 TEXT
                );
            ";
			command.ExecuteNonQuery();
		}
	}

	public IEnumerable<string> GetMessages()
	{
		var messages = new List<string>();

		using (var connection = new SqliteConnection(_connectionString))
		{
			connection.Open();
			var command = connection.CreateCommand();
			command.CommandText =
			@"
                SELECT part1, part2, part3, part4 FROM message;
            ";

			using (var reader = command.ExecuteReader())
			{
				while (reader.Read())
				{
					var part1 = reader.GetString(0);
					var part2 = reader.GetString(1);
					var part3 = reader.GetString(2);
					var part4 = reader.GetString(3);
					messages.Add($"{part1} | {part2} | {part3} | {part4}");
				}
			}
		}

		return messages;
	}

	public void InsertMessage(string part1, string part2, string part3, string part4)
	{
		if (part1 == null) part1 = string.Empty;
		if (part2 == null) part2 = string.Empty;
		if (part3 == null) part3 = string.Empty;
		if (part4 == null) part4 = string.Empty;

		using (var connection = new SqliteConnection(_connectionString))
		{
			connection.Open();
			using (var transaction = connection.BeginTransaction())
			{
				var command = connection.CreateCommand();
				command.Transaction = transaction;
				command.CommandText =
				@"
                INSERT INTO message (part1, part2, part3, part4)
                VALUES ($part1, $part2, $part3, $part4);
            ";
				command.Parameters.AddWithValue("$part1", part1);
				command.Parameters.AddWithValue("$part2", part2);
				command.Parameters.AddWithValue("$part3", part3);
				command.Parameters.AddWithValue("$part4", part4);
				command.ExecuteNonQuery();

				transaction.Commit();
			}
		}
	}
	public void ImportFromFile(string filePath)
	{
	
		Console.WriteLine($"Importing from file: {filePath}");

		var fileContent = File.ReadAllText(filePath);
		Console.WriteLine($"File content length: {fileContent.Length}");


		var parts = ParseData(fileContent);
		Console.WriteLine($"Parsed parts count: {parts.Count}");

		foreach (var part in parts)
		{
			Console.WriteLine($"Inserting message with part1: {part.Item1}");
			InsertMessage(part.Item1, part.Item2, part.Item3, part.Item4);
		}
	}


	private List<Tuple<string, string, string, string>> ParseData(string data)
	{
		var parts = new List<Tuple<string, string, string, string>>();

		var matches = System.Text.RegularExpressions.Regex.Matches(data, @"{(.*?)}");

		for (int i = 0; i < matches.Count; i += 4)
		{
			string part1 = i < matches.Count ? matches[i].Groups[1].Value.Trim() : string.Empty;
			string part2 = (i + 1) < matches.Count ? matches[i + 1].Groups[1].Value.Trim() : string.Empty;
			string part3 = (i + 2) < matches.Count ? matches[i + 2].Groups[1].Value.Trim() : string.Empty;
			string part4 = (i + 3) < matches.Count ? matches[i + 3].Groups[1].Value.Trim() : string.Empty;

			parts.Add(Tuple.Create(part1, part2, part3, part4));
		}

		return parts;
	}
}
