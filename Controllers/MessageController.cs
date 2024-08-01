using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class MessageController : ControllerBase
{
	private readonly DatabaseService _databaseService;

	public MessageController(DatabaseService databaseService)
	{
		_databaseService = databaseService;
	}

	[HttpPost("import")]
	public async Task<IActionResult> ImportMessages([FromForm] IFormFile file)
	{
		if (file == null || file.Length == 0)
		{
			return BadRequest("No file uploaded.");
		}

		try
		{
			var filePath = Path.GetTempFileName();
			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				await file.CopyToAsync(stream);
			}

			_databaseService.ImportFromFile(filePath);
			System.IO.File.Delete(filePath);

			return Ok("File imported successfully.");
		}
		catch (Exception ex)
		{
			return BadRequest($"Error importing file: {ex.Message}");
		}
	}


	[HttpPost]
	public IActionResult PostMessage([FromBody] string message)
	{
		try
		{
			var parts = ParseMessage(message);
			_databaseService.InsertMessage(parts.Item1, parts.Item2, parts.Item3, parts.Item4);
			return Ok();
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Exception: {ex.Message}");
			return BadRequest($"Error inserting message: {ex.Message}");
		}
	}

	[HttpGet]
	public IActionResult GetMessages()
	{
		try
		{
			var messages = _databaseService.GetMessages();
			return Ok(messages);
		}
		catch (Exception ex)
		{
			return BadRequest($"Error retrieving messages: {ex.Message}");
		}
	}

	private (string, string, string, string) ParseMessage(string message)
	{
		
		var trimmedMessage = message.Trim('{', '}');

		
		var parts = trimmedMessage.Split("}{", StringSplitOptions.RemoveEmptyEntries);

		
		if (parts.Length < 4)
		{
			throw new ArgumentException("Invalid message format");
		}

	
		var part1 = parts[0].Trim();
		var part2 = parts[1].Trim();
		var part3 = parts[2].Trim();
		var part4 = parts[3].Trim();

		return (part1, part2, part3, part4);
	
}
}
