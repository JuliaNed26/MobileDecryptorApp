using Microcharts;
using Microcharts.Maui;
using SkiaSharp;

namespace EncryptingAndDecryptingMobileApp.FrequencyHistogram;

internal sealed class ChartBuilder
{
	private List<ChartEntry> _entries;

	public void FillEntriesUsingText(string text)
	{
		_entries = new List<ChartEntry>();

		var frequencyDictionary = LettersFrequencyCounter.CountFrequency(text);

		foreach (var letterFrequencyPair in frequencyDictionary)
		{
			_entries.Add(new ChartEntry(letterFrequencyPair.Value)
			{
				Label = letterFrequencyPair.Key.ToString(),
				ValueLabel = letterFrequencyPair.Value.ToString(),
				Color = SKColor.Parse("#266489")
			});
		}
	}

	public Chart Build()
	{
		var chart = new BarChart()
		{
			Entries = _entries,
			LabelTextSize = 32,
		};

		return chart;
	}
}

