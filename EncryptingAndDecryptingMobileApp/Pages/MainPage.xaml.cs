using CommunityToolkit.Maui.Storage;

namespace EncryptingAndDecryptingMobileApp;

public partial class MainPage : ContentPage
{
	private IFileSaver _fileSaver;

	public MainPage(IFileSaver fileSaver)
	{
		InitializeComponent();
		_fileSaver = fileSaver;
	}

    private async void OpenEncryptPage(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new EncryptPage(_fileSaver));
    }

    private async void OpenDecryptPage(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new DecryptPage(_fileSaver));
	}

    private async void OpenAuthorsInfoPage(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new AuthorInfoPage());
	}
}

