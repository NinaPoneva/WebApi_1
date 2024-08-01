using System;
using System.Globalization;
using System.Text.RegularExpressions;
using WebApi_1.Models;

public class SwiftParser
{
	public SwiftMessageModel ParseSwiftMessage(string message)
	{
		var swiftMessageModel = new SwiftMessageModel();

		swiftMessageModel.BasicHeaderBlock = ExtractHeaderBlock(message, 1);

		swiftMessageModel.ApplicationHeaderBlock = ExtractHeaderBlock(message, 2);

		string block4Content = ExtractBlock4Content(message);

		swiftMessageModel.SwiftMT799 = ParseBlock4Content(block4Content);

		var (mac, checksum) = ExtractBlock5Content(message);
		swiftMessageModel.MessageAuthenticationCode = mac;
		swiftMessageModel.Checksum = checksum;

		return swiftMessageModel;
	}

	private string ExtractHeaderBlock(string message, int blockNumber)
	{
		string pattern = $@"\{{{blockNumber}:(.*?)\}}";
		var match = Regex.Match(message, pattern);
		return match.Success ? match.Groups[1].Value : string.Empty;
	}

	private string ExtractBlock4Content(string message)
	{
		string pattern = @"\{4:(.*?)\}";
		var match = Regex.Match(message, pattern, RegexOptions.Singleline);
		return match.Success ? match.Groups[1].Value : string.Empty;
	}

	private (long, long) ExtractBlock5Content(string message)
	{
		string pattern = @"\{5:.*?MAC:(\d+).*?CHK:(\d+)\}";
		var match = Regex.Match(message, pattern);
		if (match.Success)
		{
			long mac = long.Parse(match.Groups[1].Value);
			long checksum = long.Parse(match.Groups[2].Value);
			return (mac, checksum);
		}
		return (0, 0);
	}

	private SwiftMT799Model ParseBlock4Content(string block4Content)
	{
		var swiftMT799Model = new SwiftMT799Model();

		swiftMT799Model.TransactionReferenceNumber = ExtractField(block4Content, "20");

		swiftMT799Model.RelatedReference = ExtractField(block4Content, "21");

		string narrative = ExtractField(block4Content, "79");
		swiftMT799Model.Narrative = narrative;

		decimal amount = ExtractAmount(narrative);
		swiftMT799Model.Amount = amount;

		return swiftMT799Model;
	}

	private string ExtractField(string content, string tag)
	{
		string pattern = $@":{tag}:(.*?)(?=(\r?\n:|$))";
		var match = Regex.Match(content, pattern, RegexOptions.Singleline);
		return match.Success ? match.Groups[1].Value.Trim() : string.Empty;
	}

	private decimal ExtractAmount(string narrative)
	{
		string pattern = @"STOYNOST BGN (\d+\.\d{3},\d{2})";
		var match = Regex.Match(narrative, pattern);
		if (match.Success)
		{
			string amountString = match.Groups[1].Value;
			amountString = amountString.Replace(".", "").Replace(",", ".");
			return decimal.Parse(amountString, CultureInfo.InvariantCulture);
		}
		return 0m;
	}
}
