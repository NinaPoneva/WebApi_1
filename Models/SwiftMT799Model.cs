using System.Security.Cryptography.Xml;

namespace WebApi_1.Models
{
	public class SwiftMT799Model
	{
		public long Id { get; set; }
		public string TransactionReferenceNumber { get; set; }
		public string RelatedReference { get; set; }
		public string Narrative { get; set; }
		public decimal Amount { get; set; }

	}
}
