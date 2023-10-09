namespace EncryptionLib.Dto;

public record EncryptionInfoDto
{
	public EncryptionMethod Method { get; init; }
	public int? Shift { get; init; }
	public string Text { get; init; }
}
