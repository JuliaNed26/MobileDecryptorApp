using EncryptionLib;
using System.IO;
using System.Text;
using CommunityToolkit.Maui.Storage;

namespace EncryptingAndDecryptingMobileApp;

public partial class EncryptPage : ContentPage
{
    private IFileSaver _fileSaver;

	public EncryptPage(IFileSaver fileSaver)
	{
		InitializeComponent();
        EncryptMethodPicker.ItemsSource = EncryptMethods.ToList();
        EncryptMethodPicker.SelectedIndex = 0;
        _fileSaver = fileSaver;
	}

    public IEnumerable<string> EncryptMethods { get; } = new List<string> { "Caesar", "AES" };
    public string EncryptedText { get; private set; }

    private void EncryptBtnClicked(object sender, EventArgs e)
    {
        var i = 0;
        var textToEncrypt = TextInputEntry.Text;
        var encryptionMethod = EncryptMethodPicker.SelectedItem;

        switch (encryptionMethod.ToString())
        {
            case "Caesar":
				EncryptCaesar();
	            break;
            case "AES":
				EncryptAes();
                break;
            default: 
	            throw new InvalidOperationException("Do not have such encrypting method");
		}

        void EncryptCaesar()
		{
			if (!int.TryParse(CaesarShiftInputEntry.Text, out var shift))
			{
				ShowErrorMessage("You should enter shift for caesar method. Shift is a digit.");
			}
			else
			{
				var caesarEncryptor = new CaesarEncryptStrategy(shift);
				EncryptedOutputLabel.Text = caesarEncryptor.Encrypt(textToEncrypt);
			}
		}

        void EncryptAes()
		{
			if (string.IsNullOrEmpty(AesKeyOutputLabel.Text))
			{
				ShowErrorMessage("Press button to create key for aes method");
			}
			else
			{
				var aesEncryptor = new AesEncryptStrategy(AesKeyOutputLabel.Text);
				EncryptedOutputLabel.Text = aesEncryptor.Encrypt(textToEncrypt);
			}
		}
    }

    private void AesKeyGeneratorButton_OnClicked(object sender, EventArgs e)
    {
	    AesKeyOutputLabel.Text = AesEncryptStrategy.GenerateKey();
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
	    using var streamWithResultInfo = GetStreamWithEncryptionInfo();
        await _fileSaver.SaveAsync($"result_{Guid.NewGuid().ToString()}.txt", 
	                                       streamWithResultInfo, 
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
		EncryptedOutputLabel.Text = string.Empty;

		var encryptionInfo = EncryptorInfoParser.ParseEncryptionInfo(fileText);

		EncryptMethodPicker.SelectedItem = encryptionInfo.Method == EncryptionMethod.Caesar
			? EncryptMethods.ElementAt(0)
			: EncryptMethods.ElementAt(1);

		if (encryptionInfo.Method == EncryptionMethod.Caesar)
		{
			CaesarShiftInputEntry.Text = encryptionInfo.Shift.ToString();
		}

		TextInputEntry.Text = encryptionInfo.Text;
	}

    private MemoryStream GetStreamWithEncryptionInfo()
    {
	    var method = $"{EncryptMethodPicker.SelectedItem}";
	    var shift = $"{CaesarShiftInputEntry.Text}";
	    var key = $"{AesKeyOutputLabel.Text}";
	    var encryptedText = $"{EncryptedOutputLabel.Text}";

	    MemoryStream stream = new MemoryStream();
	    stream.Write(Encoding.Default.GetBytes($"Method: {method}\n"));
	    stream.Write(Encoding.Default.GetBytes
	    (method == EncryptMethods.ElementAt(0)
		    ? $"Shift: {shift}\n"
		    : $"Key: {key}\n"));
	    stream.Write(Encoding.Default.GetBytes($"Encrypted text: {encryptedText}"));
	    return stream;
    }
}