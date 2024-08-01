using System.Text.RegularExpressions;

namespace WebApi_1
{
	using System.Text.RegularExpressions;

	public class SwiftMessageParser
	{
		public MessageModel Parse(string message)
		{
			if (string.IsNullOrWhiteSpace(message))
			{
				throw new ArgumentException("Message cannot be null or empty", nameof(message));
			}

			var swiftMessage = new MessageModel();

			// Regular expressions to extract each block
			var block1Regex = new Regex(@"\{1:(.*?)\}", RegexOptions.Singleline);
			var block2Regex = new Regex(@"\{2:(.*?)\}", RegexOptions.Singleline);
			var block3Regex = new Regex(@"\{3:(.*?)\}", RegexOptions.Singleline);
			var block4Regex = new Regex(@"\{4:(.*?)\}", RegexOptions.Singleline);
			var block5Regex = new Regex(@"\{5:(.*?)\}", RegexOptions.Singleline);

			// Extract blocks
			swiftMessage.Block1 = ExtractBlock(message, block1Regex);
			swiftMessage.Block2 = ExtractBlock(message, block2Regex);
			swiftMessage.Block3 = ExtractBlock(message, block3Regex);
			swiftMessage.Block4 = ExtractBlock(message, block4Regex);
			swiftMessage.Block5 = ExtractBlock(message, block5Regex);

			return swiftMessage;
		}

		private string ExtractBlock(string message, Regex regex)
		{
			var match = regex.Match(message);
			return match.Success ? match.Groups[1].Value.Trim() : null;
		}
	}
}
