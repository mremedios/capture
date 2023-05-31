using System.Net;

namespace Database.Models;

public record Data
(
	Dictionary<string, string> Headers,
	string SipMessage,
	string CallId,
	IPEndPoint Host,
	DateTime ReceivingTime,
	Details Details
);