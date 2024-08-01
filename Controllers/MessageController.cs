using Microsoft.AspNetCore.Mvc;


namespace WebApi_1.Controllers
{
	[Route("api/")]
	[ApiController]
	public class MessageController : ControllerBase
	{
		private readonly DatabaseService _databaseService;
		private readonly SwiftParser _swiftMessageParser;

		public MessageController(DatabaseService databaseService, SwiftParser swiftMessageParser)
		{
			_databaseService = databaseService;
			_swiftMessageParser = swiftMessageParser;
		}

		/// <summary>
		/// Processes the upload of a SWIFT MT 799 file, parses the content, and writes the parsed message to the database.
		/// </summary>
		/// <param name="file">
		/// The file to be uploaded and processed. Must not be null and must contain data.
		/// </param>
		/// <returns>
		///  An IActionResult that represents the result of the file upload operation:
		/// - Returns a BadRequest with an error message if no file is uploaded or if parsing fails.
		/// - Returns a StatusCode 500 with an error message if there's an internal server error during file processing or database operations.
		/// - Returns an Ok response indicating successful processing and saving of the message if everything succeeds.
		/// </returns>
		[HttpPost("upload-file")]
		public async Task<IActionResult> UploadFile(IFormFile file)
		{
			if (file == null || file.Length == 0)
			{
				return BadRequest("No file uploaded.");
			}

			try
			{
				string content;
				using (var reader = new StreamReader(file.OpenReadStream()))
				{
					content = await reader.ReadToEndAsync();
				}

				var message = _swiftMessageParser.ParseSwiftMessage(content);

				if (message == null)
				{
					return BadRequest("Failed to parse the message.");
				}

				try
				{
					await _databaseService.AddRecordAsync(message);
				}
				catch (Exception ex)
				{
					return StatusCode(500, "Internal server error while saving the record.");
				}

				return Ok("Message received and processed successfully.");
			}
			catch (IOException ex)
			{
				
				return StatusCode(500, $"Internal server error while reading the file: {ex.Message}");
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}
	}
}
