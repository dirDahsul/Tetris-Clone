using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using Newtonsoft.Json;
using Xamarin.CommunityToolkit.Effects;
using Xamarin.CommunityToolkit.Converters;

namespace Tetris
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChallengesPage : ContentPage
    {
        private readonly Random random = new Random();

        public ChallengesPage()
        {
            //Preferences.Clear();
            InitializeComponent();

            LoadUserData();
            Task.Run(AnimateBackground);

            // Update daily timer
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
        }

        private void UpdateDailyTimer()
        {
            // You could use TimeSpan.FromDays(1) as well
            var remaining = TimeSpan.FromHours(24) - DateTime.Now.TimeOfDay;
            lblDailyTimeChallenges.Text = remaining.Hours + "h " + (remaining.Minutes+1) + "m";

            string dailies_date = Preferences.Get("DailiesDate", null);
            if (DateTime.Today.ToString() != dailies_date)
                UpdateDailies();
        }

        private async void AnimateBackground()
        {
            Action<double> forward = input => bdGradient.AnchorY = input;
            Action<double> backward = input => bdGradient.AnchorY = input;

            while (true)
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

            //int experience = Preferences.Get("Experience", 0);
            //int required_experience = 100 + (level * 50);
            //double denominator = 100.0 / required_experience;
            //pbExperience.Progress = ((experience * denominator) / (required_experience * denominator)) / 100;
            //lblExperience.Text = experience + "/" + required_experience;

            int experience = Preferences.Get("Experience", 0);
            int required_experience = (int)Math.Pow(Math.Log((level+1) * 50), 3);
            //double denominator = 100.0 / required_experience;
            //pbExperience.Progress = ((experience * denominator) / (required_experience * denominator)) / 100;
            pbExperience.Progress = experience / (double)required_experience;
            lblExperience.Text = experience + "/" + required_experience;

            string dailies_date = Preferences.Get("DailiesDate", null);
            if (DateTime.Today.ToString() == dailies_date)
                DisplayDailies();
            UpdateDailyTimer();
        }

        private void TapGestureRecognizer_Profile(object sender, EventArgs e)
        {
            Application.Current.MainPage = new ProfilePage();
        }

        private void TapGestureRecognizer_Home(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new MainPage());
            ((NavigationPage)Application.Current.MainPage).BarBackgroundColor = Color.FromHex("#20a6c9");
        }
    }
}