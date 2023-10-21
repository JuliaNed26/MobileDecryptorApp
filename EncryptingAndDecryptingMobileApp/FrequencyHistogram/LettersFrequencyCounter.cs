namespace EncryptingAndDecryptingMobileApp.FrequencyHistogram;
internal static class LettersFrequencyCounter
{
	public static Dictionary<char, int> CountFrequency(string text)
	{
		var frequencyDictionary = new Dictionary<char, int>();
		foreach (var symbol in text)
		{
			frequencyDictionary[symbol] = frequencyDictionary.TryGetValue(symbol, out var frequency)
				                          ? frequency + 1
				                          : 1;
		}

		return frequencyDictionary;
	}
}
