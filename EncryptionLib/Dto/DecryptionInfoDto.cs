namespace EncryptionLib.Dto;

public record DecryptionInfoDto
{
	public EncryptionMethod Method { get; init; }

	//Key for Aes method or shift for Caesar
	public string AdditionalData { get; init; }
	public string Text { get; init; }
}
