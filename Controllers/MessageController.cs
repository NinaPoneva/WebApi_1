using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text.RegularExpressions;
using WebApi_1;

[Route("api/")]
[ApiController]
public class MessageController : ControllerBase
{
	private readonly DatabaseService _databaseService;
	private readonly SwiftMessageParser _swiftMessageParser;

	public MessageController(DatabaseService databaseService,SwiftMessageParser swiftMessageParser)
	{
		_databaseService = databaseService;
	}


	[HttpPost("upload-body")]
	public async Task<IActionResult> UploadBody([FromBody] MessageModel message)
	{

		if (message == null)
		{
			return BadRequest(new { message = "The message body is required." });
		}

		Console.WriteLine($"Received message with Block1: {message.Block1}");

		try
		{
			await _databaseService.AddRecordAsync(message);
			return Ok("Message received and processed successfully.");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"An error occurred while saving the message: {ex.Message}");
			return StatusCode(500, "An error occurred while processing the message.");
		}
	}

	[HttpPost("read-from-text-file")]
	public async Task<IActionResult> ReadFromTextFile([FromBody] string filePath)
	{
		if (string.IsNullOrWhiteSpace(filePath))
		{
			return BadRequest("Invalid file path.");
		}

		if (!System.IO.File.Exists(filePath))
		{
			return NotFound("File not found.");
		}

		try
		{
			var content = await System.IO.File.ReadAllTextAsync(filePath);
			return Ok(new { content });
		}
		catch (IOException ex)
		{
			return StatusCode(500, $"Internal server error: {ex.Message}");
		}
	}

	[HttpPost("upload-file")]
	public async Task<IActionResult> UploadFile(IFormFile file)
	{
		if (file == null || file.Length == 0)
		{
			return BadRequest("No file uploaded.");
		}

		try
		{
			using (var reader = new StreamReader(file.OpenReadStream()))
			{
				var content = await reader.ReadToEndAsync();

				var parser = new SwiftMessageParser();
				var message = parser.Parse(content);

				await _databaseService.AddRecordAsync(message);
			}

			return Ok("Message received and processed successfully.");
		}
		catch (IOException ex)
		{
			return StatusCode(500, $"Internal server error: {ex.Message}");
		}
	}
}