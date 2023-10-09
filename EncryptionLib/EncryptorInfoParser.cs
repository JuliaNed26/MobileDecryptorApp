using System.Text.RegularExpressions;
using EncryptionLib.Dto;

namespace EncryptionLib;

public static class EncryptorInfoParser
{
	public static EncryptionInfoDto ParseEncryptionInfo(string fileContent)
	{
		var fileRegex = @"^Method:\s*(Caesar|AES)\r?\n(Shift:\s*(\d+)\r?\n)?Text:\s*'(.*)'$";

		Match match = Regex.Match(fileContent, fileRegex, RegexOptions.IgnoreCase);
		if (!match.Success)
		{
			throw new ArgumentException("Can not retrieve required data from file");
		}

		var method = match.Groups[1].Value.ToLower() == "caesar"
			         ? EncryptionMethod.Caesar
			         : EncryptionMethod.Aes;
		var text = match.Groups[4].Value;

		if (method == EncryptionMethod.Caesar)
		{
			return new EncryptionInfoDto
			{
				Method = method,
				Shift = int.Parse(match.Groups[3].Value),
				Text = text
			};
		}
		return new EncryptionInfoDto
		{
			Method = method,
			Text = text
		};
	}


	public static DecryptionInfoDto ParseDecryptionInfo(string fileContent)
	{
		var fileRegex = @"^Method:\s*(Caesar|AES)\r?\n(Shift|Key):\s*([a-zA-Z0-9=]+)\r?\n"+
		                      @"Text:\s*'(.*)'$";

		Match match = Regex.Match(fileContent, fileRegex, RegexOptions.IgnoreCase);
		if (!match.Success)
		{
			throw new ArgumentException("Can not retrieve required data from file");
		}

		var method = match.Groups[1].Value.ToLower() == "caesar"
			? EncryptionMethod.Caesar
			: EncryptionMethod.Aes;
		var text = match.Groups[4].Value; 

		if (method == EncryptionMethod.Caesar && !int.TryParse(match.Groups[3].Value, out var _))
		{
			throw new ArgumentException("Shift should be a digit");
		}

		return new DecryptionInfoDto
		{
			Method = method,
			AdditionalData = match.Groups[3].Value,
			Text = text
		};
	}
}
