using System.Reflection.PortableExecutable;
using System.Text.RegularExpressions;
using WebApi_1.Models;

public class SwiftMessageModel
{
	public int Id { get; set; }
	public string BasicHeaderBlock { get; set; }
	public string ApplicationHeaderBlock { get; set; }
	public long MessageAuthenticationCode { get; set; }
	public long Checksum { get; set; }
	public SwiftMT799Model SwiftMT799 { get; set; }
}
