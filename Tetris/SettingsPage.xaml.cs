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
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();

            sMusic.IsToggled = Preferences.Get("Music", true);
            sSounds.IsToggled = Preferences.Get("Sounds", true);
            sVibration.IsToggled = Preferences.Get("Vibration", false);
            sGhostPiece.IsToggled = Preferences.Get("GhostPiece", true);
            FieldHeight.Value = Preferences.Get("nFieldHeight", 20);
            FieldWidth.Value = Preferences.Get("nFieldWidth", 10);
            TouchSensitivity.Value = Preferences.Get("nTouchSensitivity", 0);

            //FieldWidth.Value = Preferences.Get("FieldSizeX", 10);
            //FieldHeight.Value = Preferences.Get("FieldSizeY", 20);
            //StretchToFit.On = Preferences.Get("StoF", true);
            //FigureColors = JsonConvert.DeserializeObject<Color[]>(Preferences.Get("FColor", JsonConvert.SerializeObject(FigureColors)));

            //if (Preferences.ContainsKey("FColor")) FigureColors = JsonConvert.DeserializeObject<List<Color>>(Preferences.Get("FColor", "default_value"));
            //else FigureColors = new List<Color>() { Color.DarkBlue, Color.DarkGreen, Color.DarkRed, Color.DarkViolet, Color.DarkOrange, Color.DarkCyan, Color.HotPink };

            FieldHeight.ValueChanged += OnHeightSliderValueChanged;
            FieldWidth.ValueChanged += OnWidthSliderValueChanged;
            TouchSensitivity.ValueChanged += OnSensitivitySliderValueChanged;
        }

        async void OnRootPageButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
            // За премахване на анимацията при прехода
            // можете да използвате следващия ред, а за да включите анимацията
            // е необходимо да добавите параметър true, вместо false
            // await Navigation.PopToRootAsync(false);
        }

        void OnHeightSliderValueChanged(object sender, ValueChangedEventArgs e)
        {
            var StepValue = 1.0;
            var newStep = Math.Round(e.NewValue / StepValue);

            //((Slider)sender).Value = newStep * StepValue;
            FieldHeight.Value = newStep * StepValue;

            Preferences.Set("nFieldHeight", (int)FieldHeight.Value);
        }
        void OnWidthSliderValueChanged(object sender, ValueChangedEventArgs e)
        {
            var StepValue = 1.0;
            var newStep = Math.Round(e.NewValue / StepValue);

            //((Slider)sender).Value = newStep * StepValue;
            FieldWidth.Value = newStep * StepValue;

            Preferences.Set("nFieldWidth", (int)FieldWidth.Value);
        }
        void OnSensitivitySliderValueChanged(object sender, ValueChangedEventArgs e)
        {
            var StepValue = 1.0;
            var newStep = Math.Round(e.NewValue / StepValue);

            //((Slider)sender).Value = newStep * StepValue;
            TouchSensitivity.Value = newStep * StepValue;

            Preferences.Set("nTouchSensitivity", (int)TouchSensitivity.Value);
        }

        private async void btnHowToPlay_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new HowToPlayPage());
        }

        private void sMusic_Toggled(object sender, ToggledEventArgs e)
        {
            Preferences.Set("Music", sMusic.IsToggled);
            if (sMusic.IsToggled)
            {
                App.player.Load("menu.mp3");
                //App.player.Loop = true;
                //App.player.Volume = 75;
                App.player.Play();
            }
            else
            {
                App.player.Pause();
            }
        }

        private void sSounds_Toggled(object sender, ToggledEventArgs e)
        {
            Preferences.Set("Sounds", sSounds.IsToggled);
        }

        private void sVibration_Toggled(object sender, ToggledEventArgs e)
        {
            Preferences.Set("Vibation", sVibration.IsToggled);

            if (sVibration.IsToggled)
                    Vibration.Vibrate();
        }

        private void sGhostPiece_Toggled(object sender, ToggledEventArgs e)
        {
            Preferences.Set("GhostPiece", sGhostPiece.IsToggled);
        }

        private async void sRestoreDefaults_Toggled(object sender, ToggledEventArgs e)
        {
            sRestoreDefaults.IsEnabled = false;

            if (!sMusic.IsToggled)
            {
                sMusic.IsToggled = true;
                Preferences.Set("Music", true);
                App.player.Load("menu.mp3");
                //App.player.Loop = true;
                //App.player.Volume = 75;
                App.player.Play();
            }

            if (FieldHeight.Value != 20)
            {
                FieldHeight.Value = 20;
                Preferences.Set("nFieldHeight", 20);
            }

            if (FieldWidth.Value != 10)
            {
                FieldWidth.Value = 10;
                Preferences.Set("nFieldWidth", 10);
            }

            if (TouchSensitivity.Value != 0.0)
            {
                TouchSensitivity.Value = 0.0;
                Preferences.Set("nTouchSensitivity", 0);
            }

            if(!sSounds.IsToggled)
            {
                sSounds.IsToggled = true;
                Preferences.Set("Sounds", true);
            }

            if(sVibration.IsToggled)
            {
                sVibration.IsToggled = false;
                Preferences.Set("Vibation", false);
            }

            if(!sGhostPiece.IsToggled)
            {
                sGhostPiece.IsToggled = true;
                Preferences.Set("GhostPiece", true);
            }

            await Task.Delay(1200);
            sRestoreDefaults.IsToggled = false;
            sRestoreDefaults.IsEnabled = true;
        }
    }
}