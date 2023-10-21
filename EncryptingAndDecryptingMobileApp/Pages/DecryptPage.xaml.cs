using System.Text;
using CommunityToolkit.Maui.Storage;
using EncryptingAndDecryptingMobileApp.FrequencyHistogram;
using EncryptionLib;
using Microcharts.Maui;
using static System.Net.Mime.MediaTypeNames;
using Application = Microsoft.Maui.Controls.Application;

namespace EncryptingAndDecryptingMobileApp;

public partial class DecryptPage : ContentPage
{
	private IFileSaver _fileSaver;
	public DecryptPage(IFileSaver fileSaver)
	{
		InitializeComponent();
		DecryptMethodPicker.ItemsSource = EncryptMethods.ToList();
		DecryptMethodPicker.SelectedIndex = 0;
		_fileSaver = fileSaver;
	}

	public IEnumerable<string> EncryptMethods { get; } = new List<string> { "Caesar", "AES" };

	private void DecryptBtnClicked(object sender, EventArgs e)
	{
		var i = 0;
		var textToDecrypt = TextInputEntry.Text;
		var encryptionMethod = DecryptMethodPicker.SelectedItem;

		BuildChart();

		switch (encryptionMethod.ToString())
		{
			case "Caesar":
				CaesarDecrypt();
				break;
			case "AES":
				AesDecrypt();
				break;
			default:
				throw new InvalidOperationException("Do not have such encrypting method");
		}

		void BuildChart()
		{
			var builder = new ChartBuilder();
			builder.FillEntriesUsingText(textToDecrypt);
			chartView.Chart = builder.Build();
		}

		void CaesarDecrypt()
		{
			if (!int.TryParse(CaesarShiftInputEntry.Text, out var shift))
			{
				ShowErrorMessage("You should enter shift for caesar method. Shift is a digit.");
			}
			else
			{
				var caesarEncryptor = new CaesarEncryptStrategy(shift);
				DecryptedOutputEntry.Text = caesarEncryptor.Decrypt(textToDecrypt);
			}
		}

		void AesDecrypt()
		{
			try
			{
				var aesEncryptor = new AesEncryptStrategy(AesKeyInputEntry.Text);
				DecryptedOutputEntry.Text = aesEncryptor.Decrypt(textToDecrypt);
			}
			catch (Exception exception)
			{
				ShowErrorMessage(exception.Message);
			}
		}
	}

	private async void UploadFileButton_Clicked(object sender, EventArgs e)
	{
		var fileWithInfo = await FilePicker.PickAsync(new PickOptions
		{
			PickerTitle = "Choose file with data (text file)",
			FileTypes = new FilePickerFileType(
				new Dictionary<DevicePlatform, IEnumerable<string>>
				{
					{ DevicePlatform.iOS, new[] { "public.txt" } },
					{ DevicePlatform.Android, new[] { "text/plain" } },
					{ DevicePlatform.UWP, new[] { ".txt" } },
					{ DevicePlatform.macOS, new[] { "public.utf8-plain-text" } },
				}),
		});

		if (fileWithInfo != null)
		{
			var text = await GetTextFromFile(fileWithInfo);
			try
			{
			    FillInfoFromFileText(text);
			}
			catch (Exception exception)
			{
				ShowErrorMessage(exception.Message);
			}
		}
	}

	private async void SaveToFileButton_Clicked(object sender, EventArgs e)
	{
		using var streamWithResult = GetStreamWithDecryptionResult();

		await _fileSaver.SaveAsync($"result_{Guid.NewGuid().ToString()}.txt", 
			                               streamWithResult, 
			                               default);

	}

	private async void ShowErrorMessage(string message)
	{
		await Application.Current.MainPage.DisplayAlert("Error", message, "Ok");
	}

	private async Task<string> GetTextFromFile(FileResult file)
	{
		var stream = await file.OpenReadAsync();
		var reader = new StreamReader(stream);
		var text = await reader.ReadToEndAsync();
		return text;
	}

	private void FillInfoFromFileText(string fileText)
	{
		DecryptedOutputEntry.Text = string.Empty;

		var encryptionInfo = EncryptorInfoParser.ParseDecryptionInfo(fileText);

		DecryptMethodPicker.SelectedItem = encryptionInfo.Method == EncryptionMethod.Caesar
			? EncryptMethods.ElementAt(0)
			: EncryptMethods.ElementAt(1);

		switch (encryptionInfo.Method)
		{
			case EncryptionMethod.Caesar:
				CaesarShiftInputEntry.Text = encryptionInfo.AdditionalData;
				break;
			case EncryptionMethod.Aes:
				AesKeyInputEntry.Text = encryptionInfo.AdditionalData;
				break;
			default:
				throw new ArgumentException("Such encryption method does not exist");
		}

		TextInputEntry.Text = encryptionInfo.Text;
	}

	private MemoryStream GetStreamWithDecryptionResult()
	{
		var method = $"{DecryptMethodPicker.SelectedItem}";
		var shift = $"{CaesarShiftInputEntry.Text}";
		var key = $"{AesKeyInputEntry.Text}";
		var decryptedText = $"{DecryptedOutputEntry.Text}";

		MemoryStream stream = new MemoryStream();
		stream.Write(Encoding.Default.GetBytes($"Method: {method}\n"));
		stream.Write(Encoding.Default.GetBytes
		(method == EncryptMethods.ElementAt(0)
			? $"Shift: {shift}\n"
			: $"Key: {key}\n"));
		stream.Write(Encoding.Default.GetBytes($"Decrypted text: '{decryptedText}'"));
		return stream;
	}

	private void OnEntryTapped(object sender, TappedEventArgs e)
	{
		if (sender is Entry entry)
		{
			Clipboard.SetTextAsync(entry.Text);
		}
	}
}