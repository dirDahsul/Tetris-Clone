using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using Plugin.SimpleAudioPlayer;

namespace Tetris
{
    public partial class App : Application
    {
        public static int screenHeight, screenWidth;
        public static ISimpleAudioPlayer player = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();

        public App()
        {
            InitializeComponent();

            if(Preferences.Get("DailiesDate", DateTime.Today.ToString()) != DateTime.Today.ToString())
                Preferences.Set("TodayStreak", -1);
            else
                Preferences.Set("TodayStreak", Preferences.Get("TodayStreak", -1) + 1);

            int login_streak = Preferences.Get("LoginStreak", 0);
            int today_streak = Preferences.Get("TodayStreak", -1);
            if (login_streak < today_streak)
                Preferences.Set("LoginStreak", today_streak);

            //MainPage = new MainPage();
            MainPage = new NavigationPage(new MainPage());
            ((NavigationPage)Application.Current.MainPage).BarBackgroundColor = Color.FromHex("#20a6c9");

            if (Preferences.Get("Music", true) == true)
            {
                player.Load("menu.mp3");
                player.Loop = true;
                player.Volume = 75;
                player.Play();
            }
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
            player.Pause();
        }

        protected override void OnResume()
        {
            player.Play();
        }
    }
}
