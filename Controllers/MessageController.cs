using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text.RegularExpressions;

[Route("api/")]
[ApiController]
public class MessageController : ControllerBase
{
	private readonly DatabaseService _databaseService;

	public MessageController(DatabaseService databaseService)
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
}
