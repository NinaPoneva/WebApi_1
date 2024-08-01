using System;
using System.Data.SQLite;
using WebApi_1.Models;

public class DatabaseService
{
	private readonly string _connectionString;

	public DatabaseService(string connectionString)
	{
		_connectionString = connectionString;
	}

	public async Task<long> AddRecordAsync(SwiftMessageModel message)
	{
		if (message.SwiftMT799 == null)
		{
			throw new ArgumentException("SwiftMT799 is required.");
		}

		using (var connection = new SQLiteConnection(_connectionString))
		{
			await connection.OpenAsync();
			using (var transaction = connection.BeginTransaction())
			{
				try
				{
					var swiftMT799Id = await InsertSwiftMT799Async(message.SwiftMT799, connection, transaction);
					message.SwiftMT799.Id = swiftMT799Id;  
					await InsertSwiftMessageAsync(message, connection, transaction); 
					transaction.Commit();
					return swiftMT799Id;
				}
				catch (Exception ex)
				{
					transaction.Rollback();
					throw new InvalidOperationException("Failed to insert record.", ex);
				}
			}
		}
	}

	private async Task<long> InsertSwiftMT799Async(SwiftMT799Model swiftMT799, SQLiteConnection connection, SQLiteTransaction transaction)
	{
		var insertSwiftMT799 = new SQLiteCommand(
			"INSERT INTO SwiftMT799 (TransactionReferenceNumber, RelatedReference, Narrative, Amount) " +
			"VALUES (@TransactionReferenceNumber, @RelatedReference, @Narrative, @Amount); " +
			"SELECT last_insert_rowid();",
			connection, transaction);

		insertSwiftMT799.Parameters.AddWithValue("@TransactionReferenceNumber", (object)swiftMT799.TransactionReferenceNumber ?? DBNull.Value);
		insertSwiftMT799.Parameters.AddWithValue("@RelatedReference", (object)swiftMT799.RelatedReference ?? DBNull.Value);
		insertSwiftMT799.Parameters.AddWithValue("@Narrative", (object)swiftMT799.Narrative ?? DBNull.Value);
		insertSwiftMT799.Parameters.AddWithValue("@Amount", swiftMT799.Amount);

		try
		{
			var swiftMT799Id = (long)await insertSwiftMT799.ExecuteScalarAsync();
			return swiftMT799Id;
		}
		catch (Exception ex)
		{
			Console.WriteLine($"An error occurred while inserting SwiftMT799 record: {ex.Message}");
			throw;
		}
	}

	private async Task InsertSwiftMessageAsync(SwiftMessageModel message, SQLiteConnection connection, SQLiteTransaction transaction)
	{
		var insertSwiftMessage = new SQLiteCommand(
			"INSERT INTO SwiftMessage (BasicHeaderBlock, ApplicationHeaderBlock, MessageAuthenticationCode, Checksum, SwiftMT799Id) " +
			"VALUES (@BasicHeaderBlock, @ApplicationHeaderBlock, @MessageAuthenticationCode, @Checksum, @SwiftMT799Id);",
			connection, transaction);

		insertSwiftMessage.Parameters.AddWithValue("@BasicHeaderBlock", message.BasicHeaderBlock);
		insertSwiftMessage.Parameters.AddWithValue("@ApplicationHeaderBlock", message.ApplicationHeaderBlock);
		insertSwiftMessage.Parameters.AddWithValue("@MessageAuthenticationCode", message.MessageAuthenticationCode);
		insertSwiftMessage.Parameters.AddWithValue("@Checksum", message.Checksum);
		insertSwiftMessage.Parameters.AddWithValue("@SwiftMT799Id", message.SwiftMT799.Id);

		try
		{
			await insertSwiftMessage.ExecuteNonQueryAsync();
		}
		catch (Exception ex)
		{
			Console.WriteLine($"An error occurred while inserting SwiftMessage record: {ex.Message}");
			throw;
		}
	}
}
