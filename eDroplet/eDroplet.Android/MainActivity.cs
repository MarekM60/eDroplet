using Acr.UserDialogs;
using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Prism;
using Prism.Ioc;

namespace eDroplet.Droid
{
    [Activity(Label = "eDroplet", Icon = "@mipmap/ic_launcher", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public static IUserDialogs dialogUI;

        protected override void OnCreate(Bundle bundle)
        {
            UserDialogs.Init(() => this);
            dialogUI = UserDialogs.Instance;

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App(new AndroidInitializer()));

            this.RequestPermissions(new[]
           {
                Manifest.Permission.AccessCoarseLocation,
                Manifest.Permission.BluetoothPrivileged,
                Manifest.Permission.BluetoothAdmin,
                Manifest.Permission.Bluetooth,
                Manifest.Permission.Nfc,
                Manifest.Permission.Vibrate
            }, 0);

        }
    }

    public class AndroidInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IUserDialogs>();
        }
    }
}

