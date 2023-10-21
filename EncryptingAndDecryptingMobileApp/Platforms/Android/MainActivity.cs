using Android.App;
using Android.Content.PM;
using Android.OS;
using Microcharts.Maui;
using Microsoft.Maui.Controls.Compatibility;

namespace EncryptingAndDecryptingMobileApp;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
}
