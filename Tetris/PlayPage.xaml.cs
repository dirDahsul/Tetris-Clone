using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

namespace Tetris
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlayPage : ContentPage
    {
        public PlayPage()
        {
            InitializeComponent();

            lblMPersonalBest.Text = Preferences.Get("MarathonHighScore", 0).ToString();
            lblQGPersonalBest.Text = Preferences.Get("QuickplayHighScore", 0).ToString();
        }

        private void btnNewQuick_Clicked(object sender, EventArgs e)
        {
            Application.Current.MainPage = new QuickGame();
        }

        private void btnNewMarathon_Clicked(object sender, EventArgs e)
        {
            Application.Current.MainPage = new GamePage();
        }
    }
}