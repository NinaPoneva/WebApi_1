using System.Text.RegularExpressions;

public class MessageModel
{
    public string Block1 { get; set; }
	public string Block2 { get; set; }
	public string Block3 { get; set; }
	public string Block4 { get; set; }
	public string Block5 { get; set; }
}

public class SwiftMessageParser
{
	public MessageModel Parse(string message)
	{
		if (string.IsNullOrWhiteSpace(message))
		{
			throw new ArgumentException("Message cannot be null or empty", nameof(message));
		}

	
		var swiftMessage = new MessageModel();

		var block1Regex = new Regex(@"^{1:(.*?)\n", RegexOptions.Singleline);
		var block2Regex = new Regex(@"^{2:(.*?)\n", RegexOptions.Singleline);
		var block3Regex = new Regex(@"^{3:(.*?)\n", RegexOptions.Singleline);
		var block4Regex = new Regex(@"^{4:(.*?)\n", RegexOptions.Singleline);
		var block5Regex = new Regex(@"^{5:(.*?)\n", RegexOptions.Singleline);

		swiftMessage.Block1 = block1Regex.Match(message).Groups[1].Value.Trim();
		swiftMessage.Block2 = block2Regex.Match(message).Groups[1].Value.Trim();
		swiftMessage.Block3 = block3Regex.Match(message).Groups[1].Value.Trim();
		swiftMessage.Block4 = block4Regex.Match(message).Groups[1].Value.Trim();
		swiftMessage.Block5 = block5Regex.Match(message).Groups[1].Value.Trim();

		return swiftMessage;
	}
	private string ExtractBlock(string content, Regex regex)
	{
		var match = regex.Match(content);
		return match.Success ? match.Groups[1].Value.Trim() : null;
	}
}
