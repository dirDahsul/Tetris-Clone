using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.CommunityToolkit.Converters;
using System.Globalization;

namespace Tetris
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfilePage : ContentPage
    {
        public int ScreenHeight
        {
            get { return App.screenHeight; }
        }
        //public int ScreenWidth
        //{
        //    get { return App.screenWidth; }
        //}

        //public Thickness Margin
        //{
        //    get { return new Thickness(ScreenWidth*0.05, ScreenHeight*0.05, ScreenWidth * 0.05, 0); }
        //}

        private ImageButton CurAvatar;
        //private int SelectedAvatar = Preferences.Get("SelectedAvatar", 0);
        private string AvatarPath = Preferences.Get("AvatarPath", null);

        public ProfilePage()
        {
            InitializeComponent();

            LoadUserData();
            LoadAndDisplayAvatars();

            Task.Run(AnimateBackground);
        }

        /*public class MultiplicationConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return ((double)parameter)*((double)value);
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return (double)value;
            }
        }*/

        private void LoadUserData()
        {
            int level = Preferences.Get("Level", 0);
            lblLevel.Text = "Lv. " + level;

            //int experience = Preferences.Get("Experience", 0);
            //int required_experience = 100 + (level * 50);
            //double denominator = 100.0 / required_experience;
            //pbExperience.Progress = ((experience * denominator) / (required_experience * denominator)) / 100;

            int experience = Preferences.Get("Experience", 0);
            int required_experience = (int)Math.Pow(Math.Log((level + 1) * 50), 3);
            //double denominator = 100.0 / required_experience;
            //pbExperience.Progress = ((experience * denominator) / (required_experience * denominator)) / 100;
            pbExperience.Progress = experience / (double)required_experience;

            string username = Preferences.Get("Username", null);
            if (username == null)
                lblUsername.Text = "Unnamed";
            else lblUsername.Text = username;

            int lines_cleared = Preferences.Get("LinesCleared", 0);
            lblLinesCleared.Text = lines_cleared.ToString();

            int marathon_high_score = Preferences.Get("MarathonHighScore", 0);
            lblMarathonHighScore.Text = marathon_high_score.ToString();

            int quickplay_high_score = Preferences.Get("QuickplayHighScore", 0);
            lblQuickplayHighScore.Text = quickplay_high_score.ToString();

            string join_date = Preferences.Get("JoinDate", DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year);
            lblJoinDate.Text = join_date;

            int login_streak = Preferences.Get("LoginStreak", 0);
            lblConsecutiveLoginStreak.Text = login_streak.ToString();

            int challenges_completed = Preferences.Get("ChallengesCompleted", 0);
            lblChallengesCompleted.Text = challenges_completed.ToString();

            //int avatars_unlocked = Preferences.Get("AvatarsUnlocked", 0);
            //lblAvatarsUnlocked.Text = avatars_unlocked.ToString();
        }

        private void LoadAndDisplayAvatars()
        {
            //int number_of_avatars = 10;
            List<Avatar> avatars = new List<Avatar>();
            for(int i = 0; i < 11; i++)
            {
                avatars.Add(new Avatar(i, "avatar_image" + i + ".png"));
            }
            avatars.Add(new Avatar(15, "avatar_image_veteran.png"));
            avatars.Add(new Avatar(20, "avatar_image_overkill.png"));
            avatars.Add(new Avatar(25, "avatar_image_mayhem.png"));
            avatars.Add(new Avatar(35, "avatar_image_deathwish.png"));
            avatars.Add(new Avatar(50, "avatar_image_deathsentence.png"));
            avatars.Add(new Avatar(100, "avatar_image_HELL_ON_EARTH.png"));

            int level = Preferences.Get("Level", 0);
            int avatars_unlocked = 0;
            for (int i = 0; i < (double)avatars.Count / 4; i++)
            {
                Grid grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });

                /*Grid grid = new Grid()
                {
                    RowDefinitions =
                        {
                            new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) },
                        },
                    ColumnDefinitions =
                        {
                            new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
                            new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
                            new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
                            new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
                        }
                };*/

                for (int j = 0; j < 4 && (i*4)+j < avatars.Count; j++)
                {
                    Grid grid1 = new Grid()
                    {
                        ColumnDefinitions =
                        {
                            new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                        },
                        RowDefinitions =
                        {
                            new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                            //new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) },
                        }
                    };
                    grid1.BindingContext = grid1;
                    grid1.SetBinding(Grid.HeightRequestProperty, "Width");

                    Grid.SetRow(grid1, 0);
                    Grid.SetColumn(grid1, j);

                    ImageButton imageButton = new ImageButton()
                    {
                        Source = ImageSource.FromFile(avatars[(i*4)+j].path),
                        Aspect = Aspect.Fill,
                        VerticalOptions = LayoutOptions.Fill,
                        HorizontalOptions = LayoutOptions.Fill,
                        //HeightRequest = 55,
                        //WidthRequest = 55,
                        CornerRadius = 10,
                        //TabIndex = (i*4)+j
                    };
                    if(AvatarPath == avatars[(i*4)+j].path || (i*4)+j == 0 && AvatarPath == null)
                    {
                        imageButton.BorderColor = Color.DodgerBlue;
                        imageButton.BorderWidth = 3;
                        CurAvatar = imageButton;
                        AvatarPath = avatars[(i*4)+j].path;
                        imgAvatar.Source = ImageSource.FromFile(AvatarPath);
                    }
                    if (level >= avatars[(i * 4) + j].required)
                    {
                        imageButton.Clicked += ImageButton_Clicked;
                        avatars_unlocked++;
                    }
                    Grid.SetRow(imageButton, 0);
                    grid1.Children.Add(imageButton);

                    /*Label label = new Label()
                    {
                        Margin = new Thickness(0, -10, 0, 0),
                        Text = "UNLOCKED",
                        FontSize = 12,
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Color.White,
                        VerticalOptions = LayoutOptions.Start,
                        HorizontalOptions = LayoutOptions.Center,
                        HorizontalTextAlignment = TextAlignment.Center
                    };
                    Grid.SetRow(label, 1);*/

                    if (level < avatars[(i*4)+j].required)
                    {
                        //label.Text = "Lv. " + avatars[(i*4)+j].required + "required";

                        BoxView boxView = new BoxView()
                        {
                            BackgroundColor = Color.Black,
                            Opacity = 0.5,
                            VerticalOptions = LayoutOptions.Fill,
                            HorizontalOptions = LayoutOptions.Fill,
                            //HeightRequest = 55,
                            //WidthRequest = 55,
                            CornerRadius = 10
                        };
                        Grid.SetRow(boxView, 0);
                        grid1.Children.Add(boxView);

                        Grid grid2 = new Grid()
                        {
                            RowDefinitions =
                            {
                                new RowDefinition { Height = new GridLength(6, GridUnitType.Star) },
                                new RowDefinition { Height = new GridLength(3, GridUnitType.Star) },
                            }
                        };
                        //grid2.BindingContext = grid1;
                        grid2.SetBinding(HeightRequestProperty, new Binding("Height", BindingMode.Default, new MathExpressionConverter(), converterParameter: "x*0.75", null, grid1));
                        //grid2.HeightRequest = grid1.Height * 0.75;
                        //grid2.WidthRequest = grid1.Width - (grid1.Height * 0.25);
                        grid2.SetBinding(WidthRequestProperty, new Binding("Width", BindingMode.Default, new MathExpressionConverter(), converterParameter: "x*0.75", null, grid1));
                        grid2.HorizontalOptions = LayoutOptions.Center;
                        grid2.VerticalOptions = LayoutOptions.Center;

                        grid2.Children.Add(new Image
                        {
                            //Margin = new Thickness(20,20,20,20),
                            BackgroundColor = Color.Transparent,
                            //HeightRequest = 35,
                            //WidthRequest = 35,
                            Aspect = Aspect.AspectFit,
                            HorizontalOptions = LayoutOptions.Center,
                            VerticalOptions = LayoutOptions.End,
                            Source = ImageSource.FromFile("lock.png")
                        }, 0, 1, 0, 1);
                        grid2.Children.Add(new Label
                        {
                            //Margin = new Thickness(0, -5, 0, 0),
                            Text = "Lv. " + avatars[(i * 4) + j].required,
                            FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                            FontAttributes = FontAttributes.Bold,
                            TextColor = Color.White,
                            VerticalOptions = LayoutOptions.Center,
                            HorizontalOptions = LayoutOptions.Center,
                            HorizontalTextAlignment = TextAlignment.Center,
                            VerticalTextAlignment = TextAlignment.Center,
                        }, 0, 1, 1, 2);
                        /*
                        Label label = new Label()
                        {
                            //Margin = new Thickness(0, -5, 0, 0),
                            Text = "Lv. " + avatars[(i * 4) + j].required,
                            FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                            FontAttributes = FontAttributes.Bold,
                            TextColor = Color.White,
                            VerticalOptions = LayoutOptions.Center,
                            HorizontalOptions = LayoutOptions.Center,
                            HorizontalTextAlignment = TextAlignment.Center,
                            VerticalTextAlignment = TextAlignment.Center,
                        };
                        label.SetBinding(FontSi)
                                                    grid2.SetBinding(HeightRequestProperty, new Binding("Height", BindingMode.Default, new MultiplicationConverter(), 0.75, null, grid1));
                        
                        Grid.SetRow(label, 1);
                        grid2.Children.Add(label);*/

                        Grid.SetRow(grid2, 0);
                        grid1.Children.Add(grid2);
                    }

                    grid.Children.Add(grid1);
                }

                bdAvatars.Children.Add(grid);
                //grid.RowDefinitions.First().Height = grid.ColumnDefinitions.FirstOrDefault().Width;
            }

            lblAvatarsUnlocked.Text = avatars_unlocked.ToString();
        }

        private void ImageButton_Clicked(object sender, EventArgs e)
        {
            if (CurAvatar == ((ImageButton)sender))
                return;

            CurAvatar.BorderWidth = 0;
            CurAvatar = ((ImageButton)sender);
            CurAvatar.BorderWidth = 3;
            CurAvatar.BorderColor = Color.DodgerBlue;
            //SelectedAvatar = CurAvatar.TabIndex;
            //Preferences.Set("SelectedAvatar", SelectedAvatar);
            //AvatarPath = CurAvatar.Source.
            AvatarPath = ((FileImageSource)CurAvatar.Source).File;
            Preferences.Set("AvatarPath", AvatarPath);
            imgAvatar.Source = ImageSource.FromFile(AvatarPath);
            //imgAvatar.Source = CurAvatar.Source;
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

        private void TapGestureRecognizer_Home(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new MainPage());
            ((NavigationPage)Application.Current.MainPage).BarBackgroundColor = Color.FromHex("#20a6c9");
        }

        private void TapGestureRecognizer_Challenges(object sender, EventArgs e)
        {
            Application.Current.MainPage = new ChallengesPage();
        }

        private async void TapGestureRecognizer_Username(object sender, EventArgs e)
        {
            string username = Preferences.Get("Username", null);
            string result = await DisplayPromptAsync("Change Username", "Please enter your desired username below", placeholder: "Your username...", initialValue: username, maxLength: 25, keyboard: Keyboard.Text);
            if (result != null && result != "")
            {
                Preferences.Set("Username", result);
                //Device.BeginInvokeOnMainThread(() =>
                //{
                    lblUsername.Text = result;
                //});
            }
            //else Preferences.Set("Username", "Unnamed");
        }
    }
}