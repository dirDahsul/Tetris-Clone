using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
using Newtonsoft.Json;
using Xamarin.CommunityToolkit.Effects;
using Xamarin.CommunityToolkit.Converters;

using System.Threading;
using System.Globalization;
using Plugin.SimpleAudioPlayer;

namespace Tetris
{
    public partial class MainPage : ContentPage
    {
        private readonly Random random = new Random();

        //Task tAnimateBackground;
        //Task tCreateBackgroundFigures;
        //Task tAnimateLogo;

        /*public class MultiplicationConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return ((double)parameter) * ((double)value);
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return (double)value;
            }
        }*/

        public MainPage()
        {
            //Preferences.Clear();
            InitializeComponent();

            LoadUserData();
            /*tAnimateBackground = */Task.Run(AnimateBackground);
            /*tCreateBackgroundFigures = */Task.Run(CreateBackgroundFigures);
            ///*tAnimateLogo = */Task.Run(AnimateLogo);
        }

        private void UpdateDailyTimer()
        {
            // You could use TimeSpan.FromDays(1) as well
            var remaining = TimeSpan.FromHours(24) - DateTime.Now.TimeOfDay;
            if (remaining.Minutes == 59)
                lblDailyTimeHome.Text = (remaining.Hours + 1) + "h " + "00" + "m";
            else
                lblDailyTimeHome.Text = remaining.Hours + "h " + (remaining.Minutes + 1) + "m";

            string dailies_date = Preferences.Get("DailiesDate", null);
            if (DateTime.Today.ToString() != dailies_date)
                UpdateDailies();
        }

        private double NextDouble(double minValue, double maxValue)
        {
            return random.NextDouble() * (maxValue - minValue) + minValue;
        }

        /*private async void AnimateLogo()
        {
            _ = Task.Run(async () =>
            {
                while (true)
                {
                    await imgTetris.RelScaleTo(0.3, 1000);
                    await imgTetris.RelScaleTo(-0.3, 1000);
                }
            });

            while (true)
                await imgTetris.RotateTo(imgTetris.Rotation + 360, 10000);
        }*/

        private async void CreateBackgroundFigures()
        {
            while (true)
            {
                double imagePosX = NextDouble(0.0, App.screenWidth);
                double imageSize = NextDouble(App.screenWidth / 8, App.screenWidth / 3);
                string imageName = "tetromino" + random.Next(1, 3) + ".png";
                SpawnBackgroundFigure(imagePosX, -imageSize, imageSize, imageName);

                //await DisplayAlert("Alert", imagePosX.ToString(), "OK");
                await Task.Delay(random.Next(1500, 3000));
            }
        }

        private async void SpawnBackgroundFigure(double x, double y, double size, string image)
        {
            Image Tetromino = new Image()
            {
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Start,
                TranslationX = x,
                TranslationY = y,
                WidthRequest = size,
                //HeightRequest = size,
                Source = image,
                Aspect = Aspect.AspectFit
            };

            Device.BeginInvokeOnMainThread(() =>
            {
                bdGrid.Children.Insert(bdGrid.Children.Count - 1, Tetromino);
            });

            uint time = (uint)(size*200);
            _ = Tetromino.TranslateTo(x/2.5, NextDouble(this.Height/3, this.Height), time, Easing.SinIn);
            await Tetromino.FadeTo(0, time, Easing.CubicIn);

            Device.BeginInvokeOnMainThread(() =>
            {
                bdGrid.Children.Remove(Tetromino);
            });
        }

        private async void AnimateBackground()
        {
            Action<double> forward = input => bdGradient.AnchorY = input;
            Action<double> backward = input => bdGradient.AnchorY = input;

            while(true)
            {
                bdGradient.Animate(name: "forward", callback: forward, start: 0, end: 1.0, length: 10000, easing: Easing.CubicOut);
                await Task.Delay(12000);
                bdGradient.Animate(name: "backward", callback: backward, start: 1.0, end: 0, length: 10000, easing: Easing.CubicOut);
                await Task.Delay(12000);
            }
        }

        private void UpdateDailies()
        {
            Dictionary<int, Daily> dailies = JsonConvert.DeserializeObject<Dictionary<int, Daily>>(Preferences.Get("Dailies", JsonConvert.SerializeObject(new Dictionary<int, Daily>())));
            string myDate = DateTime.Today.ToString();
            Preferences.Set("DailiesDate", myDate);
            dailies.Clear();

            for (int i = 1; i < bdDaily.Children.Count; i++)
                bdDaily.Children.RemoveAt(i);

            for (int i = 0; i < 4; i++)
            {
                int chosen;
                do
                {
                    chosen = random.Next(0, 9);
                } while (dailies.ContainsKey(chosen));
                Daily daily;

                switch (chosen)
                {
                    case 0:
                        {
                            daily = new Daily("Complete 100 lines", 100, 30);
                            break;
                        }
                    case 1:
                        {
                            daily = new Daily("Complete 20 lines in a single game", 20, 50);
                            break;
                        }
                    case 2:
                        {
                            daily = new Daily("Score 3k Points", 3000, 50);
                            break;
                        }
                    case 3:
                        {
                            daily = new Daily("Finish a quick game", 1, 15);
                            break;
                        }
                    case 4:
                        {
                            daily = new Daily("Finish a normal game", 1, 15);
                            break;
                        }
                    case 5:
                        {
                            daily = new Daily("Complete 3 lines with 1 tetromino", 1, 50);
                            break;
                        }
                    case 6:
                        {
                            daily = new Daily("Complete 2 lines with 1 tetromino", 1, 30);
                            break;
                        }
                    case 7:
                        {
                            daily = new Daily("Complete 10 lines in a single game", 10, 30);
                            break;
                        }
                    case 8:
                        {
                            daily = new Daily("Complete 200 lines", 200, 50);
                            break;
                        }
                    default:
                        {
                            daily = null;
                            break;
                        }
                }
                dailies.Add(chosen, daily);

                /*
Grid grid = new Grid()
{
    RowDefinitions =
        {
            new RowDefinition { Height = new GridLength(2, GridUnitType.Auto) },
            new RowDefinition { Height = new GridLength(2, GridUnitType.Auto) },
            new RowDefinition { Height = new GridLength(2, GridUnitType.Auto) }
        },
    ColumnDefinitions =
        {
            new ColumnDefinition { Width = new GridLength(2, GridUnitType.Auto) },
            new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
            new ColumnDefinition { Width = new GridLength(2, GridUnitType.Auto) },
            new ColumnDefinition { Width = new GridLength(2, GridUnitType.Auto) },
        }
};
*/
            }

            Preferences.Set("Dailies", JsonConvert.SerializeObject(dailies));
            DisplayDailies();
        }

        private void DisplayDailies()
        {
            Dictionary<int, Daily> dailies = JsonConvert.DeserializeObject<Dictionary<int, Daily>>(Preferences.Get("Dailies", JsonConvert.SerializeObject(new Dictionary<int, Daily>())));
            foreach (KeyValuePair<int, Daily> daily in dailies)
            {
                Grid grid = new Grid()
                {
                    RowDefinitions =
                        {
                            new RowDefinition { Height = GridLength.Auto },
                            new RowDefinition { Height = GridLength.Auto },
                            new RowDefinition { Height = GridLength.Auto },
                        },
                    ColumnDefinitions =
                        {
                            new ColumnDefinition { Width = new GridLength(4, GridUnitType.Star) },
                            new ColumnDefinition { Width = new GridLength(15, GridUnitType.Star) },
                            new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
                            new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) }
                        },
                    //RowSpacing = 20,
                };

                BoxView boxView = new BoxView()
                {
                    BackgroundColor = Color.FromHex("#94c1ff"), //#00a9de
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill,
                    //HeightRequest = 1,
                    Opacity = 0.5
                };
                boxView.SetBinding(HeightRequestProperty, new Binding("Width", BindingMode.Default, new MathExpressionConverter(), converterParameter: "x*0.005", null, boxView));
                Grid.SetRow(boxView, 0);
                Grid.SetColumnSpan(boxView, 4);
                grid.Children.Add(boxView);

                Image image = new Image()
                {
                    BackgroundColor = Color.Transparent,
                    Aspect = Aspect.AspectFit,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    //Margin = new Thickness(0,0,-10,0),
                    Source = ImageSource.FromFile("daily_tetromino" + random.Next(1, 4) + ".png")
                };
                image.BindingContext = image;
                image.SetBinding(Image.HeightRequestProperty, "Width");
                Grid.SetRow(image, 1);
                Grid.SetRowSpan(image, 2);
                Grid.SetColumn(image, 0);
                grid.Children.Add(image);

                //StackLayout stackLayout = new StackLayout();
                Label lbl = new Label
                {
                    Text = daily.Value.description,
                    TextColor = Color.White,
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalTextAlignment = TextAlignment.Start,
                    VerticalTextAlignment = TextAlignment.Start,
                };
                lbl.BindingContext = lbl;
                lbl.SetBinding(Label.HeightRequestProperty, "Height");
                Grid.SetRow(lbl, 1);
                Grid.SetColumn(lbl, 1);
                grid.Children.Add(lbl);
                //stackLayout.BindingContext = lbl;
                //stackLayout.SetBinding(StackLayout.HeightRequestProperty, "Height");
                //stackLayout.Children.Add(lbl);
                //Grid.SetRow(stackLayout, 1);
                //Grid.SetColumn(stackLayout, 1);
                //grid.Children.Add(stackLayout);

                if (daily.Value.current >= daily.Value.required)
                {
                    lbl.Opacity = 0.5;

                    grid.Children.Add(new Label
                    {
                        Text = "Challenge Completed",
                        TextColor = Color.FromHex("#00A693"),
                        FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                        FontAttributes = FontAttributes.Bold,
                        HorizontalOptions = LayoutOptions.Start,
                        VerticalOptions = LayoutOptions.Center,
                        //HorizontalTextAlignment= TextAlignment.Start,
                        //VerticalTextAlignment= TextAlignment.Start,
                    }, 1, 2, 2, 3);
                }
                else
                {
                    ProgressBar pb = new ProgressBar
                    {
                        Progress = daily.Value.current / (double)daily.Value.required,
                        ProgressColor = Color.FromHex("#00A693"),
                        BackgroundColor = Color.FromHex("#7F000000"),
                        HeightRequest = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                        HorizontalOptions = LayoutOptions.Fill,
                        VerticalOptions = LayoutOptions.Center
                    };
                    CornerRadiusEffect.SetCornerRadius(pb, 30);
                    Grid.SetRow(pb, 2);
                    Grid.SetColumn(pb, 1);
                    grid.Children.Add(pb);

                    grid.Children.Add(new Label
                    {
                        Text = daily.Value.current + "/" + daily.Value.required,
                        TextColor = Color.White,
                        FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                        FontAttributes = FontAttributes.Bold,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        //HorizontalTextAlignment = TextAlignment.Center,
                        //VerticalTextAlignment = TextAlignment.Center,
                        Opacity = 0.9
                    }, 1, 2, 2, 3);
                }

                //grid1.Children.Add(lbl);
                //grid.Children.Add(stackLayout);

                Image image1 = new Image()
                {
                    BackgroundColor = Color.Transparent,
                    Aspect = Aspect.AspectFit,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Source = ImageSource.FromFile("exp.png")
                };
                image1.BindingContext = image1;
                image1.SetBinding(Image.HeightRequestProperty, "Width");
                Grid.SetRow(image1, 1);
                Grid.SetRowSpan(image1, 2);
                Grid.SetColumn(image1, 2);
                grid.Children.Add(image1);

                grid.Children.Add(new Label
                {
                    Text = daily.Value.reward.ToString(),
                    TextColor = Color.White,
                    FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                    FontAttributes = FontAttributes.None,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    //HorizontalTextAlignment = TextAlignment.Center,
                    //VerticalTextAlignment = TextAlignment.Center,
                }, 3, 4, 1, 3);

                bdDaily.Children.Add(grid);
            }
        }

        private void LoadUserData()
        {
            int level = Preferences.Get("Level", 0);
            lblLevel.Text = "Lv. " + level;

            int experience = Preferences.Get("Experience", 0);
            int required_experience = (int)Math.Pow(Math.Log((level + 1) * 50), 3);
            //double denominator = 100.0 / required_experience;
            //pbExperience.Progress = ((experience * denominator) / (required_experience * denominator)) / 100;
            pbExperience.Progress = experience / (double)required_experience;

            string dailies_date = Preferences.Get("DailiesDate", null);
            if (DateTime.Today.ToString() == dailies_date)
                DisplayDailies();

            // Update daily timer
            UpdateDailyTimer();
            Device.StartTimer(TimeSpan.FromMilliseconds(60000 - ((DateTime.Now.Second * 1000) + DateTime.Now.Millisecond)), () =>
            {
                UpdateDailyTimer();

                Device.StartTimer(TimeSpan.FromMinutes(1), () =>
                {
                    UpdateDailyTimer();
                    return true;
                });

                return false;
            });

            string username = Preferences.Get("Username", null);
            if (username == null)
                lblUsername.Text = "Unnamed";
            else lblUsername.Text = username;

            string avatar_path = Preferences.Get("AvatarPath", "avatar_image0.png");
            iBtnUser.Source = ImageSource.FromFile(avatar_path);

            if (username == null)
            {
                _ = Task.Run(async () =>
                {
                    string result = await DisplayPromptAsync("Welcome to Tetris!", "Please enter your username below", placeholder: "Your username...", maxLength: 25, keyboard: Keyboard.Text);
                    if (result != null && result != "")
                    {
                        Preferences.Set("Username", result);
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            lblUsername.Text = result;
                        });
                    }
                    else Preferences.Set("Username", "Unnamed");
                });
            }
        }

        private async void BtnPlay_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new GamePage());
        }

        private async void iBtnSettings_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage());
        }

        private async void iBtnAbout_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AboutPage());
        }

        private void TapGestureRecognizer_Profile(object sender, EventArgs e)
        {
            //await Navigation.PopAsync();
            //await Navigation.PushAsync(new ChallengesPage(), false);
            //Application.Current.MainPage = new ChallengesPage();
            Application.Current.MainPage = new ProfilePage();
        }

        private void TapGestureRecognizer_Challenges(object sender, EventArgs e)
        {
            Application.Current.MainPage = new ChallengesPage();
        }

        private async void TapGestureRecognizer_YouTube(object sender, EventArgs e)
        {
            try
            {
                await Browser.OpenAsync(new Uri("https://www.youtube.com/"), BrowserLaunchMode.SystemPreferred);
            }
            catch (Exception ex)
            {
                await DisplayAlert("System Error", "Unable to open youtube URL. Reason: "+ex.Message, "Close");
            }
        }
        private async void TapGestureRecognizer_Twitter(object sender, EventArgs e)
        {
            try
            {
                await Browser.OpenAsync(new Uri("https://www.twitter.com/"), BrowserLaunchMode.SystemPreferred);
            }
            catch (Exception ex)
            {
                await DisplayAlert("System Error", "Unable to open twitter URL. Reason: " + ex.Message, "Close");
            }
        }

        private async void TapGestureRecognizer_Facebook(object sender, EventArgs e)
        {
            try
            {
                await Browser.OpenAsync(new Uri("https://www.facebook.com/"), BrowserLaunchMode.SystemPreferred);
            }
            catch (Exception ex)
            {
                await DisplayAlert("System Error", "Unable to open facebook URL. Reason: " + ex.Message, "Close");
            }
        }

        private async void btnPlayTetris_Clicked(object sender, EventArgs e)
        {
            //((NavigationPage)Application.Current.MainPage).BarBackgroundColor = Color.Transparent;
            await Navigation.PushAsync(new PlayPage());
        }
    }
}
