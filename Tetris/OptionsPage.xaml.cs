using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using Newtonsoft.Json;

namespace Tetris
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OptionsPage : ContentPage
    {
        private Color[] FigureColors = new Color[7] // Apply a color for every figure
        {
            Color.DarkBlue,
            Color.DarkGreen,
            Color.DarkRed,
            Color.DarkViolet,
            Color.DarkOrange,
            Color.DarkCyan,
            Color.HotPink
        };

        public OptionsPage()
        {
            InitializeComponent();

            //FieldWidth.Value = Preferences.Get("FieldSizeX", 10);
            //FieldHeight.Value = Preferences.Get("FieldSizeY", 20);
            StretchToFit.On = Preferences.Get("StoF", true);
            //FigureColors = JsonConvert.DeserializeObject<Color[]>(Preferences.Get("FColor", JsonConvert.SerializeObject(FigureColors)));

            //if (Preferences.ContainsKey("FColor")) FigureColors = JsonConvert.DeserializeObject<List<Color>>(Preferences.Get("FColor", "default_value"));
            //else FigureColors = new List<Color>() { Color.DarkBlue, Color.DarkGreen, Color.DarkRed, Color.DarkViolet, Color.DarkOrange, Color.DarkCyan, Color.HotPink };
            
            FieldWidth.ValueChanged += OnSliderValueChanged;
        }

        async void OnRootPageButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
            // За премахване на анимацията при прехода
            // можете да използвате следващия ред, а за да включите анимацията
            // е необходимо да добавите параметър true, вместо false
            // await Navigation.PopToRootAsync(false);
        }
        
        private void FieldWidth_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            double width = FieldPanel.Width / FieldWidth.Value;

            if (StretchToFit.On == false)
            {
                double height = FieldPanel.Height / FieldHeight.Value;
                width = width > height ? height : width;
            }

            // Draw the outlines on the play field --- ignoring the outlines of the phone/device, no lines there, sooo start at 1
            for (int xf = 1; xf < FieldWidth.Value; xf++) // Draw the X outlines with boxviews
            {
                BoxView xfBV = new BoxView()
                {
                    WidthRequest = 1,
                    HeightRequest = FieldPanel.Height,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Start,
                    TranslationX = (xf) * width - 0.5,
                    Color = Color.FromRgb(16, 16, 16)
                };

                FieldPanel.Children.Add(xfBV); // Add Field X BoxView to content pageeee so it can be displayed, yeeet
                                                      // And add it after the tetromino ... soo insert at 5
            }
        }

        private void FieldHeight_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            double height = FieldPanel.Height / FieldHeight.Value;

            if (StretchToFit.On == false)
            {
                double width = FieldPanel.Width / FieldWidth.Value;
                height = width > height ? height : width;
            }

            for (int yf = 1; yf < FieldHeight.Value; yf++) // Draw the Y outlines with boxviews
            {
                BoxView yfBV = new BoxView()
                {
                    WidthRequest = FieldPanel.Width,
                    HeightRequest = 1,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Start,
                    TranslationY = (yf) * height - 0.5,
                    Color = Color.FromRgb(16, 16, 16)
                };

                FieldPanel.Children.Add(yfBV); // Add Field Y BoxView to content pageeee so it can be displayed, yeeet
                                                      // And add it after the tetromino ... soo insert at 5
            }
        }

        private void StretchToFit_OnChanged(object sender, ToggledEventArgs e)
        {
            FieldWidth_ValueChanged(sender, null);
            FieldHeight_ValueChanged(sender, null);
            //SelectedFigure_SelectedIndexChanged(sender, null);
        }

        private void SelectedFigure_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selIndex = SelectedFigure.SelectedIndex;
            switch (selIndex)
            {
                case 0:
                    TetrominoPiece1.TranslationX = -(TetrominoPiece1.WidthRequest + (TetrominoPiece1.WidthRequest / 2));
                    TetrominoPiece1.TranslationY = 0;

                    TetrominoPiece2.TranslationX = TetrominoPiece1.TranslationX + TetrominoPiece2.WidthRequest;
                    TetrominoPiece2.TranslationY = 0;

                    TetrominoPiece3.TranslationX = TetrominoPiece2.TranslationX + TetrominoPiece3.WidthRequest;
                    TetrominoPiece3.TranslationY = 0;

                    TetrominoPiece4.TranslationX = TetrominoPiece3.TranslationX + TetrominoPiece4.WidthRequest;
                    TetrominoPiece4.TranslationY = 0;
                    break;
                case 1:
                    TetrominoPiece1.TranslationX = -TetrominoPiece1.WidthRequest;
                    TetrominoPiece1.TranslationY = -(TetrominoPiece1.HeightRequest / 2);

                    TetrominoPiece2.TranslationX = TetrominoPiece1.TranslationX;
                    TetrominoPiece2.TranslationY = TetrominoPiece1.TranslationY + TetrominoPiece2.HeightRequest;

                    TetrominoPiece3.TranslationX = TetrominoPiece2.TranslationX + TetrominoPiece3.WidthRequest;
                    TetrominoPiece3.TranslationY = TetrominoPiece2.TranslationY;

                    TetrominoPiece4.TranslationX = TetrominoPiece3.TranslationX + TetrominoPiece4.WidthRequest;
                    TetrominoPiece4.TranslationY = TetrominoPiece3.TranslationY;
                    break;
                case 2:
                    TetrominoPiece1.TranslationX = TetrominoPiece1.WidthRequest;
                    TetrominoPiece1.TranslationY = -(TetrominoPiece1.HeightRequest / 2);

                    TetrominoPiece2.TranslationX = TetrominoPiece1.TranslationX;
                    TetrominoPiece2.TranslationY = TetrominoPiece1.TranslationY + TetrominoPiece2.HeightRequest;

                    TetrominoPiece3.TranslationX = TetrominoPiece2.TranslationX - TetrominoPiece3.WidthRequest;
                    TetrominoPiece3.TranslationY = TetrominoPiece2.TranslationY;

                    TetrominoPiece4.TranslationX = TetrominoPiece3.TranslationX - TetrominoPiece4.WidthRequest;
                    TetrominoPiece4.TranslationY = TetrominoPiece3.TranslationY;
                    break;
                case 3:
                    TetrominoPiece1.TranslationX = TetrominoPiece1.WidthRequest / 2;
                    TetrominoPiece1.TranslationY = TetrominoPiece1.HeightRequest / 2;

                    TetrominoPiece2.TranslationX = TetrominoPiece2.WidthRequest / 2;
                    TetrominoPiece2.TranslationY = -(TetrominoPiece2.HeightRequest / 2);

                    TetrominoPiece3.TranslationX = -(TetrominoPiece3.WidthRequest / 2);
                    TetrominoPiece3.TranslationY = TetrominoPiece3.HeightRequest / 2;

                    TetrominoPiece4.TranslationX = -(TetrominoPiece4.WidthRequest / 2);
                    TetrominoPiece4.TranslationY = TetrominoPiece4.HeightRequest / 2;
                    break;
                case 4:
                    TetrominoPiece1.TranslationX = -TetrominoPiece1.WidthRequest;
                    TetrominoPiece1.TranslationY = TetrominoPiece1.HeightRequest / 2;

                    TetrominoPiece2.TranslationX = TetrominoPiece1.TranslationX + TetrominoPiece2.WidthRequest;
                    TetrominoPiece2.TranslationY = TetrominoPiece1.TranslationY;

                    TetrominoPiece3.TranslationX = TetrominoPiece2.TranslationX;
                    TetrominoPiece3.TranslationY = TetrominoPiece2.TranslationY + TetrominoPiece3.HeightRequest;

                    TetrominoPiece4.TranslationX = TetrominoPiece3.TranslationX + TetrominoPiece4.WidthRequest;
                    TetrominoPiece4.TranslationY = TetrominoPiece3.TranslationY;
                    break;
                case 5:
                    TetrominoPiece1.TranslationX = -TetrominoPiece1.WidthRequest;
                    TetrominoPiece1.TranslationY = TetrominoPiece1.HeightRequest / 2;

                    TetrominoPiece2.TranslationX = TetrominoPiece1.TranslationX + TetrominoPiece2.WidthRequest;
                    TetrominoPiece2.TranslationY = TetrominoPiece1.TranslationY;

                    TetrominoPiece3.TranslationX = TetrominoPiece2.TranslationX;
                    TetrominoPiece3.TranslationY = TetrominoPiece2.TranslationY + TetrominoPiece3.HeightRequest;

                    TetrominoPiece4.TranslationX = TetrominoPiece2.TranslationX + TetrominoPiece4.WidthRequest;
                    TetrominoPiece4.TranslationY = TetrominoPiece2.TranslationY;
                    break;
                case 6:
                    TetrominoPiece1.TranslationX = -TetrominoPiece1.WidthRequest;
                    TetrominoPiece1.TranslationY = -(TetrominoPiece1.HeightRequest / 2);

                    TetrominoPiece2.TranslationX = TetrominoPiece1.TranslationX + TetrominoPiece2.WidthRequest;
                    TetrominoPiece2.TranslationY = TetrominoPiece1.TranslationY;

                    TetrominoPiece3.TranslationX = TetrominoPiece2.TranslationX;
                    TetrominoPiece3.TranslationY = TetrominoPiece2.TranslationY + TetrominoPiece3.HeightRequest;

                    TetrominoPiece4.TranslationX = TetrominoPiece3.TranslationX + TetrominoPiece4.WidthRequest;
                    TetrominoPiece4.TranslationY = TetrominoPiece3.TranslationY;
                    break;
                default:
                    TetrominoPiece1.Color = Color.Black;
                    TetrominoPiece2.Color = Color.Black;
                    TetrominoPiece3.Color = Color.Black;
                    TetrominoPiece4.Color = Color.Black;
                    break;
            }

            TetrominoPiece1.Color = FigureColors[selIndex];
            TetrominoPiece2.Color = FigureColors[selIndex];
            TetrominoPiece3.Color = FigureColors[selIndex];
            TetrominoPiece4.Color = FigureColors[selIndex];

            SelectedColor.SelectedIndex = selIndex;
        }

        private void SelectedColor_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selIndex = SelectedColor.SelectedIndex;
            int selFigure = SelectedFigure.SelectedIndex;
            switch (selIndex)
            {
                case 0:
                    FigureColors[selFigure] = Color.DarkBlue;
                    break;
                case 1:
                    FigureColors[selFigure] = Color.DarkGreen;
                    break;
                case 2:
                    FigureColors[selFigure] = Color.DarkRed;
                    break;
                case 3:
                    FigureColors[selFigure] = Color.DarkViolet;
                    break;
                case 4:
                    FigureColors[selFigure] = Color.DarkOrange;
                    break;
                case 5:
                    FigureColors[selFigure] = Color.DarkCyan;
                    break;
                case 6:
                    FigureColors[selFigure] = Color.HotPink;
                    break;
                default:
                    FigureColors[selFigure] = Color.Black;
                    break;
            }

            TetrominoPiece1.Color = FigureColors[selFigure];
            TetrominoPiece2.Color = FigureColors[selFigure];
            TetrominoPiece3.Color = FigureColors[selFigure];
            TetrominoPiece4.Color = FigureColors[selFigure];
        }

        private void BtnSave_Clicked(object sender, EventArgs e)
        {
            //Preferences.Set("FieldSizeX", FieldWidth.Value);
            //Preferences.Set("FieldSizeY", FieldHeight.Value);
            Preferences.Set("StoF", StretchToFit.On);
            //Preferences.Set("FColor", JsonConvert.SerializeObject(FigureColors));
        }

        void OnSliderValueChanged(object sender, ValueChangedEventArgs e)
        {
            var StepValue = 1.0;
            var newStep = Math.Round(e.NewValue / StepValue);

            FieldWidth.Value = newStep * StepValue;


        }
    }
}