using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Tetris
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AboutPage : ContentPage
    {
        public AboutPage()
        {
            InitializeComponent();
            //((NavigationPage)this).BarBackgroundColor = Color.FromHex("#007da7");
            //((NavigationPage)Application.Current.MainPage).BarBackgroundColor = Color.FromHex("#007da7");
        }

        async void OnRootPageButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
            // За премахване на анимацията при прехода
            // можете да използвате следващия ред, а за да включите анимацията
            // е необходимо да добавите параметър true, вместо false
            // await Navigation.PopToRootAsync(false);
        }
    }
}