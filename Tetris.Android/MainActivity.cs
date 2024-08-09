using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace Tetris.Droid
{
    [Activity(Label = "Filtris", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Icon = "@drawable/blocky", Theme = "@style/MyTheme.Splash", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            //-- Настройваме първичната тема на приложението --
            base.SetTheme(Resource.Style.MainTheme);
            //-------------------------------------------------

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            #region For screen Height & Width  
            var pixels = Resources.DisplayMetrics.WidthPixels;
            var scale = Resources.DisplayMetrics.Density;
            var dps = (double)((pixels - 0.5f) / scale);
            var ScreenWidth = (int)dps;
            App.screenWidth = ScreenWidth;
            pixels = Resources.DisplayMetrics.HeightPixels;
            dps = (double)((pixels - 0.5f) / scale);
            var ScreenHeight = (int)dps;
            App.screenHeight = ScreenHeight;
            #endregion
            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public override async void OnBackPressed()
        {
            if (Xamarin.Forms.Application.Current.MainPage is Xamarin.Forms.NavigationPage
                && Xamarin.Forms.Application.Current.MainPage.Navigation.NavigationStack.Count == 1)
            {
                bool answer = await Xamarin.Forms.Application.Current.MainPage.Navigation.NavigationStack[0].DisplayAlert("Confirm exit", "Are you serious?", "Yes", "No");
                //if (Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Confirm exit", "Are you serious?", "Yes", "No"))
                if (answer)
                {
                    //base.OnBackPressed();

                    System.Environment.Exit(0);
                }
            }
            else
                base.OnBackPressed();
        }
    }
}