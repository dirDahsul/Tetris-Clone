using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using Plugin.SimpleAudioPlayer;
using Newtonsoft.Json;
using Syncfusion.XForms.Border;

namespace Tetris
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QuickGame : ContentPage
    {
        // Important notice !!!!!
        /* As different phones and other mobile devices come with different screen resolutions and sizes,
         * the game will be represented based on the FieldSize variables, calculated by the device's dimensions and/or
         * resolution, thus making it a similar experience for all devices. Basically, the sizes of the objects inside
         * the game would change depending on the device's resolution and dimensions, because they need to be fit in
         * the screen, but the gameplay mechanics would remain the same for all devices.
         * 
         * Also, the game would be represented by a simple x,y coordinate system with sizes, which are NOT
         * representative of the final size or position of the objects in the game. The objects will be upscaled based
         * on the FieldSize variables, while movement will be done by multiplying x and y in the coordinate system with
         * those upscaled sizes. Basically, if the nFieldSizeX is 10 and the nFieldSizeY is 20 (default), then the x
         * coordinate will be made to fit 10 tetromino pieces and the y coordinate will be made to fit 20 tetromino pieces.
         * Moving will be done by multiplying the position of the pieces with the corresponding FieldSize (nFieldSizeY or
         * nFieldSizeX). Example: Tetromino is at position x=1,y=2, and has a size 50, so then we will visualize
         * it at pos x=1*50, y=2*50.
         */


        // Core game settings        ===============================================================================================================================
        private int nFieldSizeX = Preferences.Get("nFieldWidth", 10); // Change for different field X size (DO NOT MAKE LESS THAN 4! -- bugs MAY be introduced)
        private int nFieldSizeY = Preferences.Get("nFieldHeight", 20) + 5; // Change for different field Y size (DO NOT MAKE LESS THAN 4! -- bugs MAY be introduced)
        private double nPieceSize = 0; // What is the ABSOLUTE size of a single tetromino piece (this gets determined based on the field size)

        private static readonly Random rnd = new Random(); // Random generator for choosing the tetromino and thus the color .... can be done other ways if wanted
        //private BoxView[,] nField = new BoxView[nFieldSizeY, nFieldSizeX]; // The visual representation of the field, filled with tetromino pieces
        private List<List<BoxView>> nField = new List<List<BoxView>>();
        private BoxView[] Tetromino = new BoxView[4]; // The visual representation of the tetromino based on the coordinates

        // Ghost piece
        private SfBorder[] GhostPiece = new SfBorder[4]; // The visual representation of the ghost piece based on the coordinates
        bool ghost_piece_enabled = Preferences.Get("GhostPiece", true);

        private struct Point // Just a basic point in the field
        { public int x, y; };

        private Point[] pCurTetromino = new Point[4]; // The coordinates of the tetromino and thus pretty much the tetromino itself
        private Point[] pLastTetromino = new Point[4];

        private int[,] nTetrominoFigures = new int[7, 4] // The different tetromino figures
        {
            { 1,3,5,7 }, // I
            { 3,5,7,6 }, // J  // 0 1
            { 2,3,5,7 }, // L  // 2 3
            { 2,3,4,5 }, // O  // 4 5
            { 3,5,4,6 }, // S  // 6 7
            { 3,5,4,7 }, // T
            { 2,4,5,7 }, // Z
        };

        private enum GameState // Possible game states
        {
            Paused = 0,
            Normal = 1,
            GameOver = 2
        }

        // Get dailies so they can be completed during gameplay
        Dictionary<int, Daily> dailies = JsonConvert.DeserializeObject<Dictionary<int, Daily>>(Preferences.Get("Dailies", JsonConvert.SerializeObject(new Dictionary<int, Daily>())));

        // Audio player for sounds
        ISimpleAudioPlayer big_clean = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
        ISimpleAudioPlayer biggest_clean = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
        ISimpleAudioPlayer fast_down = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
        ISimpleAudioPlayer little_clean = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
        ISimpleAudioPlayer move = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
        ISimpleAudioPlayer plain_down = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
        ISimpleAudioPlayer rotate = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
        bool sounds_enabled = Preferences.Get("Sounds", true);

        // Vibration
        bool vibration_enabled = Preferences.Get("Vibration", false);

        // Touch sensitivity
        double touch_sensitivity = Preferences.Get("nTouchSensitivity", 0);

        // Tick settings and controls (settings constantly used during the gameplay)       ==========================================================================
        GameState eGameState = GameState.GameOver; // The state of the game - start with game over
        int sMovedStepsX = 0; // How many steps the tetromino was moved (left or right) when dragging, based on its start-up position
        //int sMovedStepsY = 0; // How many steps the tetromino was moved (down) when dragging, based on its start-up position
        bool bMoveDown = false; // Move the piece down if conditions met
        int nCurrentFigure = 0; // If its always 0, then figure is not updated properly
        Color cTetrominoColor = Color.Black; // If its black, then color is not updated properly
        int nSpeed = 20; // That's the default speed (Increase for easier difficulty and vise-versa)
        int nDifficultyLevel = 1; // Display to the user the game's difficulty
        const int nSpeedMin = 5; // How fast can the game become (Increase for easier difficulty and vise-versa)
        int nSpeedCount = 0; // How many ticks have passed before forcing the piece down - if nSpeed ticks passed, then force down the piece
        bool bForceDown = false; // Whether the piece should be forced down
        int nTetrominoCount = 0; // How many tetrominoes have passed throughout the game
        int nLinesCount = 0; // How many lines have been cleared from the start of the game
        int nScore = 0; // Current score ... if more lines are removed at once... you get more score!!! ... yay
        List<int> lLines = new List<int>(); // The filled lines that need to be removed
        int MilisecondsPassed = 0; // If user wants to move the tetromino down .... and just fast releases the down movement (within half a second)
                                   // force the tetromino all the way down

        // Detect time remaining
        TimeSpan remaining = TimeSpan.FromMinutes(3);

        // Some other functions       ==========================================================================
        private bool DoesTetrominoMovementFit() // Bool function to check if the tetromino could be moved at current position Ho Ho Hoooo, merry crysis
        {
            for (int i = 0; i < 4; i++)
                if (pCurTetromino[i].x < 0 || pCurTetromino[i].x >= nFieldSizeX || pCurTetromino[i].y >= nFieldSizeY) return false;
                else if (nField[pCurTetromino[i].y][pCurTetromino[i].x] != null) return false;

            return true;
        }
        private bool DoesTetrominoRotationFit() // Bool function to check if the tetromino could be rotated at current position Ho Ho Hoooo, merry crysis
        {
            for (int i = 0; i < 4; i++) // Make sure board corners does not block rotation .... only field pieces
            {
                if (pCurTetromino[i].x < 0)
                {
                    for (int j = 0; j < 4; j++)
                        pCurTetromino[j].x++;

                    return DoesTetrominoRotationFit();
                }
                else if (pCurTetromino[i].x >= nFieldSizeX)
                {
                    for (int j = 0; j < 4; j++)
                        pCurTetromino[j].x--;

                    return DoesTetrominoRotationFit();
                }
                else if (pCurTetromino[i].y >= nFieldSizeY)
                {
                    for (int j = 0; j < 4; j++)
                        pCurTetromino[j].y--;

                    return DoesTetrominoRotationFit();
                }
                else if (nField[pCurTetromino[i].y][pCurTetromino[i].x] != null) return false; // out of index
            }

            return true;
        }
        private void UpdateVisualTetrominoPosition() // Update THE POSITION of the visual representation of the detromino, cuz some movement has been done
        {
            for (int i = 0; i < 4; i++)
            {
                Tetromino[i].TranslationX = (pCurTetromino[i].x) * nPieceSize /*Tetromino[i].WidthRequest*/;
                Tetromino[i].TranslationY = /*TitlePanel.Height + */(pCurTetromino[i].y - 5) * nPieceSize /*Tetromino[i].HeightRequest*/;
            }

            // Display ghost piece
            if (ghost_piece_enabled)
            {
                Point[] pSavedTetromino = new Point[4];
                for (int i = 0; i < 4; i++)
                    pSavedTetromino[i] = pCurTetromino[i];

                do
                {
                    // Save the non-colided tetromino and move the collide checking tetromino
                    for (int i = 0; i < 4; i++)
                        pCurTetromino[i].y++;
                } while (DoesTetrominoMovementFit());

                for (int i = 0; i < 4; i++)
                {
                    GhostPiece[i].TranslationX = (pCurTetromino[i].x) * nPieceSize /*Tetromino[i].WidthRequest*/;
                    GhostPiece[i].TranslationY = /*TitlePanel.Height + */(pCurTetromino[i].y - 6) * nPieceSize /*Tetromino[i].HeightRequest*/;
                }

                for (int i = 0; i < 4; i++)
                    pCurTetromino[i] = pSavedTetromino[i]; // reset it to the non-colided position
            }
        }

        private void PickTetromino() // Reset THE POSITION of the tetromino and change the COLOR of the visual representation of the tetromino
        {
            // Pick New Tetromino
            nCurrentFigure = rnd.Next(0, 7);

            // Switch tetromino color based on the picked figure
            switch (nCurrentFigure)
            {
                case 0:
                    {
                        cTetrominoColor = Color.DarkBlue;
                        break;
                    }
                case 1:
                    {
                        cTetrominoColor = Color.DarkGreen;
                        break;
                    }
                case 2:
                    {
                        cTetrominoColor = Color.DarkRed;
                        break;
                    }
                case 3:
                    {
                        cTetrominoColor = Color.DarkViolet;
                        break;
                    }
                case 4:
                    {
                        cTetrominoColor = Color.DarkOrange;
                        break;
                    }
                case 5:
                    {
                        cTetrominoColor = Color.DarkCyan;
                        break;
                    }
                case 6:
                    {
                        cTetrominoColor = Color.HotPink;
                        break;
                    }
                default:
                    {
                        cTetrominoColor = Color.Black;
                        break;
                    }
            }

            for (int i = 0; i < 4; i++) // Reset the tetromino's coordinates and the color of the visual representation of the tetromino
            {
                // Set tetromino coordinates (position)
                pCurTetromino[i].x = nTetrominoFigures[nCurrentFigure, i] % 2 + nFieldSizeX / 2 - 2;
                pCurTetromino[i].y = nTetrominoFigures[nCurrentFigure, i] / 2 + 2;

                // Update the color of the visual representation of the tetromino
                Tetromino[i].Color = cTetrominoColor; // C
            }
        }

        private void ResumeTimer()
        {
            Device.StartTimer(TimeSpan.FromSeconds(1), /*async*/ () =>
            {
                if (eGameState == GameState.Paused)
                    return false;

                remaining -= TimeSpan.FromSeconds(1);
                if(remaining.Seconds > 9) lblTimer.Text = remaining.Minutes.ToString() + ":" + remaining.Seconds.ToString();
                else lblTimer.Text = remaining.Minutes.ToString() + ":0" + remaining.Seconds.ToString();
                pbTimer.Progress = remaining.TotalSeconds/180.0;
                if (remaining.TotalMilliseconds <= 0)
                {
                    // Save user data about the game
                    SaveUserProgress();

                    //lblCurScore.IsVisible = false;
                    //ConditionLabel.Text = "GAME OVER\n\r" + ScoreLabel.Text + "\n\r\n\rDouble tap the screen to restart!";
                    //ConditionLabel.IsVisible = true;
                    eGameState = GameState.GameOver;
                    iBtnPause.IsEnabled = false;

                    LayoutRoot.BackgroundColor = Color.FromHex("#7F000000");
                    Frame frame = new Frame()
                    {
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        WidthRequest = App.screenWidth * 0.66,
                        HeightRequest = App.screenWidth * 0.66,
                        CornerRadius = (float)(App.screenWidth * 0.66),
                        BorderColor = Color.White,
                        BackgroundColor = Color.BlueViolet,
                    };
                    Label label = new Label()
                    {
                        Text = "Timed Out",
                        TextColor = Color.White,
                        FontSize = frame.HeightRequest * 0.15,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center,
                    };
                    frame.Content = label;
                    //Grid.SetRowSpan(frame, 4);
                    //Grid.SetColumnSpan(frame, 3);
                    bdGrid.Children.Add(frame);

                    //await Task.Delay(250);
                    Device.StartTimer(TimeSpan.FromSeconds(4), () =>
                    {
                        Application.Current.MainPage = new NavigationPage(new MainPage());

                        if (Preferences.Get("Music", true) == true)
                        {
                            App.player.Load("menu.mp3");
                            //App.player.Loop = true;
                            App.player.Volume = 75;
                            App.player.Play();
                        }
                        return false;
                    });

                    return false;
                }
                
                return true;
            });
        }

        public QuickGame()
        {
            InitializeComponent();

            // Initialize some start-up user data/settings
            lblHighScore.Text = Preferences.Get("QuickplayHighScore", 0).ToString();

            // Play music based on settings
            if (Preferences.Get("Music", true) == true)
            {
                //App.player.Dispose();
                App.player.Load("game.mp3");
                //App.player.Loop = true;
                App.player.Volume = 100;
                App.player.Play();
            }

            if (sounds_enabled)
            {
                big_clean.Load("big_clean.mp3");
                biggest_clean.Load("biggest_clean.mp3");
                fast_down.Load("fast_down.mp3");
                little_clean.Load("little_clean.mp3");
                move.Load("move.mp3");
                plain_down.Load("plain_down.mp3");
                rotate.Load("rotate.mp3");
            }

            // Some start-up settings        ===============================================================================================================================
            for (int fy = 0; fy < nFieldSizeY; fy++)
            {
                nField.Add(new List<BoxView>());
                for (int fx = 0; fx < nFieldSizeX; fx++)
                {
                    //nField.Add(new List<BoxView>());
                    nField.Last().Add(null); // Make the field hold null values
                }
            }

            double panel_width = App.screenWidth * 0.66; // Get desired panel width ... smaller than the width of the screen in my case
            double panel_height = App.screenHeight * 0.66; // Get desired panel height ... smaller than the width of the screen in my case
            nPieceSize = (panel_width / nFieldSizeX) > (panel_height / (nFieldSizeY - 5)) ? (panel_height / (nFieldSizeY - 5)) : (panel_width / nFieldSizeX); // Get the smaller panel size for calculating piece size
            //double piece_size = above;

            grTetrisPanel.WidthRequest = nPieceSize * nFieldSizeX; // Apply the width
            grTetrisPanel.HeightRequest = nPieceSize * (nFieldSizeY - 5); // Make sure that the panel height maintains square-ty

            for (int i = 0; i < 4; i++) // Create the tetromino (after that just update it)
            {
                Tetromino[i] = new BoxView() // Visual representation of the tetromino - BoxView
                {
                    WidthRequest = nPieceSize * 0.95,
                    HeightRequest = nPieceSize * 0.95,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Start,
                    CornerRadius = nPieceSize * 0.1,
                    //IsVisible = false,
                };

                if (ghost_piece_enabled)
                {
                    GhostPiece[i] = new SfBorder() // Visual representation of the ghost piece - SfBorder
                    {
                        WidthRequest = nPieceSize * 0.95,
                        HeightRequest = nPieceSize * 0.95,
                        HorizontalOptions = LayoutOptions.Start,
                        VerticalOptions = LayoutOptions.Start,
                        CornerRadius = nPieceSize * 0.1,
                        BackgroundColor = Color.Transparent,
                        BorderColor = Color.White,
                        BorderThickness = nPieceSize * 0.05
                        //IsVisible = false,
                    };

                    grTetrisPanel.Children.Add(GhostPiece[i]);
                }

                grTetrisPanel.Children.Add(Tetromino[i]); // Add BoxView tetromino to content pageeee so it can be displayed, blqqqqqqq
            }

            Start_Game();
        }

        private async void Start_Game()
        {
            //BoxView box = new BoxView()
            //{
            //    HorizontalOptions = LayoutOptions.Fill,
            //    VerticalOptions = LayoutOptions.Fill,
            //    Color = Color.FromHex("#7F000000"),
            //};
            //Grid.SetRowSpan(box, 4);
            //Grid.SetColumnSpan(box, 3);
            iBtnPause.IsEnabled = false;
            //LayoutRoot.BackgroundColor = Color.FromHex("#7F000000");

            BoxView boxView_background = new BoxView()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                BackgroundColor = Color.Black,
                Opacity = 0.0,
            };
            bdGrid.Children.Add(boxView_background);

            Action<double> forwardBackground = input => boxView_background.Opacity = input;
            boxView_background.Animate(name: "forwardBackground", callback: forwardBackground, start: 0.0, end: 0.5, length: 1000, easing: Easing.Linear, finished: (v, c) => boxView_background.Opacity = 0.5);
            //await Task.Delay(1500);

            Label label_info = new Label()
            {
                Text = "Get the best score\nyou can within 3m!",
                TextColor = Color.White,
                FontSize = grTetrisPanel.WidthRequest * 0.07,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
            };
            bdGrid.Children.Add(label_info);

            await Task.Delay(1500);
            await label_info.TranslateTo(x: label_info.TranslationX, y: label_info.TranslationY-(grTetrisPanel.WidthRequest*0.7), length: 1500, easing: Easing.Linear);

            BoxView boxView_outline = new BoxView()
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                CornerRadius = 999999999999999999,
                WidthRequest = 1,
                HeightRequest = 1,
                Background = new LinearGradientBrush(new GradientStopCollection() { new GradientStop() { Color = Color.FromHex("#6279FD"), Offset = (float)0.0 }, new GradientStop() { Color = Color.FromHex("#373EA6"), Offset = (float)1.0 }, }, new Xamarin.Forms.Point(0, 1), new Xamarin.Forms.Point(0, 1)),
                //HeightRequest = grTetrisPanel.WidthRequest * 0.9,
            };
            bdGrid.Children.Add(boxView_outline);

            //Action<double> forwardOutline1 = input => boxView_outline.WidthRequest = input;
            //Action<double> forwardOutline2 = input => boxView_outline.HeightRequest = input;
            //boxView_outline.Animate(name: "forwardOutline1", callback: forwardOutline1, start: 0.0, end: grTetrisPanel.Width * 0.95, length: 1000, easing: Easing.Linear);
            //boxView_outline.Animate(name: "forwardOutline2", callback: forwardOutline2, start: 0.0, end: grTetrisPanel.Width * 0.95, length: 1000, easing: Easing.Linear);
            _ = boxView_outline.ScaleTo(scale: grTetrisPanel.WidthRequest * 0.9, length: 1000, easing: Easing.Linear);

            BoxView boxView_inline = new BoxView()
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                CornerRadius = 999999999999999999,
                WidthRequest = 1,
                HeightRequest = 1,
                Background = new LinearGradientBrush(new GradientStopCollection() { new GradientStop() { Color = Color.FromHex("#181D3D"), Offset = (float)0.0 }, new GradientStop() { Color = Color.FromHex("#2D3775"), Offset = (float)1.0 }, }, new Xamarin.Forms.Point(0, 1), new Xamarin.Forms.Point(0, 1)),
                //HeightRequest = grTetrisPanel.WidthRequest * 0.7,
            };
            bdGrid.Children.Add(boxView_inline);

            //Action<double> forwardInline1 = input => boxView_inline.WidthRequest = input;
            //Action<double> forwardInline2 = input => boxView_inline.HeightRequest = input;
            //boxView_inline.Animate(name: "forwardInline1", callback: forwardInline1, start: 0.0, end: grTetrisPanel.Width * 0.75, length: 1000, easing: Easing.Linear);
            //boxView_inline.Animate(name: "forwardInline2", callback: forwardInline2, start: 0.0, end: grTetrisPanel.Width * 0.75, length: 1000, easing: Easing.Linear);
            await boxView_inline.ScaleTo(scale: grTetrisPanel.WidthRequest * 0.7, length: 1000, easing: Easing.Linear);

            Label label_timer = new Label()
            {
                Text = "3",
                TextColor = Color.White,
                FontSize = grTetrisPanel.WidthRequest * 0.4,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
            };
            bdGrid.Children.Add(label_timer);

            short t = 3;
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                t--;
                if (t <= 0)
                {
                    label_timer.Text = "GO!";
                    Action<double> backwardBackground = input => boxView_background.Opacity = input;
                    boxView_background.Animate(name: "backwardBackground", callback: backwardBackground, start: 0.5, end: 0, length: 1000, easing: Easing.Linear, finished: (v, c) => boxView_background.Opacity = 0);
                    Device.StartTimer(TimeSpan.FromSeconds(1), /*async*/ () =>
                    {
                        //var taskCompletionSource = new TaskCompletionSource<bool>();
                        //Action<double> backwardBackground = input => boxView_background.Opacity = input;
                        //boxView_background.Animate(name: "backwardBackground", callback: backwardBackground, start: 0.5, end: 0, length: 1000, easing: Easing.Linear, finished: (v, c) => taskCompletionSource.SetResult(c));
                        //await taskCompletionSource.Task;

                        //await Task.Delay(12000);

                        //bdGrid.Children.Remove(box);
                        //LayoutRoot.BackgroundColor = Color.Transparent;
                        //bdGrid.Children.Remove(frame);
                        bdGrid.Children.Remove(boxView_background);
                        bdGrid.Children.Remove(label_info);
                        bdGrid.Children.Remove(boxView_outline);
                        bdGrid.Children.Remove(boxView_inline);
                        bdGrid.Children.Remove(label_timer);
                        PickTetromino(); // Reset visual rep and positional tetromino to their start up values
                        UpdateVisualTetrominoPosition(); // Update visual representation tetromino's position ....
                        //eGameState = GameState.Normal;
                        iBtnPause.IsEnabled = true;
                        Resume_Game();
                        return false;
                    });

                    return false;
                }

                label_timer.Text = t.ToString();
                return true;
            });
        }

        private async void Resume_Game()
        {
            //if(eGameState == GameState.GameOver)
            eGameState = GameState.Normal;
            ResumeTimer();
            while (eGameState == GameState.Normal) // While the game state is unpaused
            {
                // Timing =======================
                //Thread.Sleep(50); // Small Step = 1 Game Tick ... crashes the emulator ... and ... this function is the only one that needs to stop (not everything) .. soo use asynchronous wait (this gets waited while everything else works normally)
                await Task.Delay(50); // Small Step = 1 Game Tick
                nSpeedCount++;
                bForceDown = (nSpeedCount == nSpeed);

                // Move down the tetromino (more like force the tetromino down for movement)
                if (bMoveDown == true)
                    bForceDown = true;

                // Force the tetromino down (time reached or moved down)
                if (bForceDown)
                {
                    // Update difficulty every (50 + nTetrominoCount/5) pieces
                    nSpeedCount = 0;
                    nTetrominoCount++;
                    if (nTetrominoCount % (50 + nTetrominoCount / 5) == 0)
                        if (nSpeed > nSpeedMin)
                        {
                            nSpeed--;
                            nDifficultyLevel++;
                            lblLevel.Text = "Level " + nDifficultyLevel;
                        }

                    // Save the non-colided tetromino and move the collide checking tetromino
                    for (int i = 0; i < 4; i++)
                    {
                        pLastTetromino[i] = pCurTetromino[i];
                        pCurTetromino[i].y++;
                    }

                    // Check if the moved down tetromino fits (doesn't collide)
                    if (DoesTetrominoMovementFit())
                        UpdateVisualTetrominoPosition(); // Update the visual representation of the tetromino
                    else // doesn't fit (collides)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            pCurTetromino[i] = pLastTetromino[i]; // reset it to the non-colided position

                            // Make the tetromino 'settings' temporary ones (cuz we dont want to attach references of the real tetromino to the field... cuz that way the field will move and act as the tetromino...)
                            double WidthR = Tetromino[i].Width;
                            double HeightR = Tetromino[i].Height;
                            LayoutOptions HOptions = Tetromino[i].HorizontalOptions;
                            LayoutOptions VOptions = Tetromino[i].VerticalOptions;
                            double TX = Tetromino[i].TranslationX; // Tetromino[i].X gives invalid data ..... that shit took me 1 day to fix ... i was looking for issues in places
                            double TY = Tetromino[i].TranslationY; // they didnt even exist ... omfg ... that shit retarded
                            Color C = Tetromino[i].Color;
                            CornerRadius CR = Tetromino[i].CornerRadius;

                            // Make the tetromino part of the field
                            nField[pCurTetromino[i].y][pCurTetromino[i].x] = new BoxView()
                            {
                                WidthRequest = WidthR,
                                HeightRequest = HeightR,
                                HorizontalOptions = HOptions,
                                VerticalOptions = VOptions,
                                TranslationX = TX,
                                TranslationY = TY,
                                Color = C,
                                CornerRadius = CR,
                                //Parent = P
                            };

                            grTetrisPanel.Children.Add(nField[pCurTetromino[i].y][pCurTetromino[i].x]); // Add that BoxView to the content, cuz it needs to be displayed somewhere in the content
                                                                                                        // And more specifically add it after the tetromino - after tetromino[i]'s
                        }

                        // Check for lines (don't need to check every line, just the ones closest to the tetromino)
                        for (int py = pCurTetromino[0].y - 3; py < pCurTetromino[0].y + 4; py++)
                            if (py < nFieldSizeY && py > -1)
                            {
                                bool bLine = true;
                                for (int px = 0; px < nFieldSizeX; px++)
                                    bLine &= (nField[py][px] != null);

                                if (bLine)
                                {
                                    // Remove Line, set to BLANK or smthng
                                    for (int px = 0; px < nFieldSizeX; px++)
                                        nField[py][px].Color = Color.Gray; // Change color of tetromino pieces on that line

                                    // need to make a line with boxview here .... if i have the time... thats easy... time needed thou
                                    /*nField[py, 0]
                                    BoxView lBV = new BoxView()
                                    {
                                        WidthRequest = TetrisPanel.Width,
                                        HeightRequest = 1,
                                        HorizontalOptions = ,
                                        VerticalOptions = VOptions,
                                        TranslationX = TX,
                                        TranslationY = TY,
                                        Color = C,
                                    };*/

                                    lLines.Add(py);
                                }
                            }

                        // Increase the score --------
                        nScore += (27*2);
                        if (lLines.Count() != 0)
                        {
                            // Increase the score even more if multiple lines are completed
                            nScore += (1 << lLines.Count()) * 112 * 2;
                            nLinesCount += lLines.Count();
                            lblLines.Text = "Lines " + nLinesCount;

                            // Check if challenge is part of the dailies ... and add progress to it ... if it is
                            if (lLines.Count() == 3 && dailies.ContainsKey(5) && dailies[5].current < dailies[5].required)
                            {
                                dailies[5].current = 1;
                                Preferences.Set("ChallengesCompleted", Preferences.Get("ChallengesCompleted", 0) + 1);
                            }

                            // Check if challenge is part of the dailies ... and add progress to it ... if it is
                            if (lLines.Count() == 2 && dailies.ContainsKey(6) && dailies[6].current < dailies[6].required)
                            {
                                dailies[6].current = 1;
                                Preferences.Set("ChallengesCompleted", Preferences.Get("ChallengesCompleted", 0) + 1);
                            }

                            // Play line completion sound .... if enabled
                            if (sounds_enabled)
                            {
                                switch (lLines.Count())
                                {
                                    case 1:
                                        {
                                            little_clean.Play();
                                            break;
                                        }
                                    case 2:
                                        {
                                            big_clean.Play();
                                            break;
                                        }
                                    default:
                                        {
                                            biggest_clean.Play();
                                            break;
                                        }
                                }

                                //player2.Loop = false;
                                //player2.Volume = 100;
                                //player2.Play();
                            }

                            // Animate Line Completion --------
                            // Display Frame (cheekily to draw lines)
                            //Thread.Sleep(400); // Delay a bit ... this doesnt work properly ... crashes the emulator, guess this functions needs to be the only one that stops... sooo we use await
                            await Task.Delay(600); // Delay a bit

                            // For every line in the list
                            foreach (int l in lLines)
                                for (int px = 0; px < nFieldSizeX; px++)
                                {
                                    grTetrisPanel.Children.Remove(nField[l][px]); // Remove the line
                                    for (int py = l; py > 0; py--)
                                    {
                                        nField[py][px] = nField[py - 1][px]; // Make the line the same as one above

                                        if (nField[py][px] != null) // Make the visual presentation of the field move down as well
                                            nField[py][px].TranslationY += nPieceSize /*Tetromino[0].HeightRequest*/;
                                    }

                                    if (nField[0][px] != null) // Clear the top layer cuz ... after everything moved down ... the top layer should be empty
                                    {
                                        grTetrisPanel.Children.Remove(nField[0][px]);
                                        nField[0][px] = null;
                                    }
                                }

                            lLines.Clear(); // All lines are removed ... sooo clear the list
                        }
                        else
                        {
                            // Play place sound if enabled
                            if (sounds_enabled)
                            {
                                //player2.Load("plain_down.mp3");
                                //player2.Loop = false;
                                //player2.Volume = 100;
                                plain_down.Play();
                            }
                        }

                        if (vibration_enabled)
                            Vibration.Vibrate();

                        // Show what is the upcoming tetromino ---- if i have time
                        //

                        lblCurScore.Text = nScore.ToString(); // Update the score shown on screen
                        PickTetromino(); // Pick new tetromino
                        UpdateVisualTetrominoPosition(); // Update visual representation's position of the tetromino ....

                        // If tetromino doesn't fit straight away, game over!
                        if (!DoesTetrominoMovementFit())
                        {
                            // Save user data about the game
                            SaveUserProgress();

                            //lblCurScore.IsVisible = false;
                            //ConditionLabel.Text = "GAME OVER\n\r" + ScoreLabel.Text + "\n\r\n\rDouble tap the screen to restart!";
                            //ConditionLabel.IsVisible = true;
                            eGameState = GameState.GameOver;
                            iBtnPause.IsEnabled = false;

                            LayoutRoot.BackgroundColor = Color.FromHex("#7F000000");
                            Frame frame = new Frame()
                            {
                                HorizontalOptions = LayoutOptions.Center,
                                VerticalOptions = LayoutOptions.Center,
                                WidthRequest = App.screenWidth * 0.66,
                                HeightRequest = App.screenWidth * 0.66,
                                CornerRadius = (float)(App.screenWidth * 0.66),
                                BorderColor = Color.White,
                                BackgroundColor = Color.BlueViolet,
                            };
                            Label label = new Label()
                            {
                                Text = "Game Over",
                                TextColor = Color.White,
                                FontSize = frame.HeightRequest * 0.15,
                                FontAttributes = FontAttributes.Bold,
                                HorizontalOptions = LayoutOptions.Center,
                                VerticalOptions = LayoutOptions.Center,
                                HorizontalTextAlignment = TextAlignment.Center,
                                VerticalTextAlignment = TextAlignment.Center,
                            };
                            frame.Content = label;
                            //Grid.SetRowSpan(frame, 4);
                            //Grid.SetColumnSpan(frame, 3);
                            bdGrid.Children.Add(frame);

                            await Task.Delay(2500);

                            Application.Current.MainPage = new NavigationPage(new MainPage());

                            if (Preferences.Get("Music", true) == true)
                            {
                                App.player.Load("menu.mp3");
                                //App.player.Loop = true;
                                App.player.Volume = 75;
                                App.player.Play();
                            }
                        }
                    }
                }
            }
        }

        private void SaveUserProgress()
        {
            //bool update_dailies = false;
            int challenges_completed = Preferences.Get("ChallengesCompleted", 0);
            // Check if challenge is part of the dailies ... and add progress to it ... if it is
            if (dailies.ContainsKey(0) && dailies[0].current < dailies[0].required)
            {
                dailies[0].current += nLinesCount;
                if (dailies[0].current >= dailies[0].required)
                {
                    dailies[0].current = dailies[0].required;
                    challenges_completed++;
                }

                //update_dailies = true;
            }

            // Check if challenge is part of the dailies ... and add progress to it ... if it is
            if (dailies.ContainsKey(1) && dailies[1].current < dailies[1].required)
            {
                dailies[1].current = nLinesCount;
                if (dailies[1].current >= dailies[1].required)
                {
                    dailies[1].current = dailies[1].required;
                    challenges_completed++;
                }

                //update_dailies = true;
            }

            // Check if challenge is part of the dailies ... and add progress to it ... if it is
            if (dailies.ContainsKey(2) && dailies[2].current < dailies[2].required)
            {
                dailies[2].current += nScore;
                if (dailies[2].current >= dailies[2].required)
                {
                    dailies[2].current = dailies[2].required;
                    challenges_completed++;
                }

                //update_dailies = true;
            }

            // Check if challenge is part of the dailies ... and add progress to it ... if it is
            if (dailies.ContainsKey(3) && dailies[3].current < dailies[3].required)
            {
                dailies[3].current = 1;
                //update_dailies = true;
                challenges_completed++;
            }

            // Check if challenge is part of the dailies ... and add progress to it ... if it is
            if (dailies.ContainsKey(7) && dailies[7].current < dailies[7].required)
            {
                dailies[7].current = nLinesCount;
                if (dailies[7].current >= dailies[7].required)
                {
                    dailies[7].current = dailies[7].required;
                    challenges_completed++;
                }

                //update_dailies = true;
            }

            // Check if challenge is part of the dailies ... and add progress to it ... if it is
            if (dailies.ContainsKey(8) && dailies[8].current < dailies[8].required)
            {
                dailies[8].current += nLinesCount;
                if (dailies[8].current > dailies[8].required)
                {
                    dailies[8].current = dailies[8].required;
                    challenges_completed++;
                }

                //update_dailies = true;
            }

            // Update some user statistics
            if (challenges_completed > Preferences.Get("ChallengesCompleted", 0))
                Preferences.Set("ChallengesCompleted", challenges_completed);

            // Update Dailies
            //if(update_dailies)
            Preferences.Set("Dailies", JsonConvert.SerializeObject(dailies));

            //int score = Preferences.Get("QuickplayHighScore", 0);
            if (nScore > Preferences.Get("QuickplayHighScore", 0))
                Preferences.Set("QuickplayHighScore", nScore);

            if (nLinesCount > 0)
                Preferences.Set("LinesCleared", Preferences.Get("LinesCleared", 0) + nLinesCount);

            RewardExperience(nScore / 150);
        }

        private void RewardExperience(int reward_experience)
        {
            int level = Preferences.Get("Level", 0);
            int experience = Preferences.Get("Experience", 0);
            int required_experience = (int)Math.Pow(Math.Log((level + 1) * 50), 3);

            if (experience + reward_experience > required_experience)
            {
                Preferences.Set("Level", level + 1);
                RewardExperience((experience + reward_experience) - required_experience);
                return;
            }
            Preferences.Set("Experience", experience+reward_experience);
        }

        //private async void TapGestureRecognizer_TappedTwice(object sender, EventArgs e)
        //{
        /*if (eGameState == GameState.GameOver) // Game state is game over
        {
            for (int fy = 0; fy < nFieldSizeY; fy++) // Reset the field
                for (int fx = 0; fx < nFieldSizeX; fx++)
                {
                    if (nField[fy, fx] != null)
                    {
                        grTetrisPanel.Children.Remove(nField[fy, fx]);
                        nField[fy, fx] = null;
                    }
                }

            bMoveDown = false; // Reset move down bool
            sMovedStepsX = 0; // Reset move x steps

            nScore = 0; // Reset the score
            nSpeed = 20; // Reset game speed to default value
            nTetrominoCount = 0; // Reset the number of tetrominos placed on the field
            ResetTetromino(); // Reset visual rep and positional tetromino to their start up values
            UpdateTetromino(); // Update visual representation tetromino's position ....

            ConditionLabel.IsVisible = false; // Hide condition label with the GAME OVER text
            ScoreLabel.Text = "Score: 0"; // Score label's text
            ScoreLabel.IsVisible = true; // Show score label with score
            eGameState = GameState.Unpaused; // Make game state to be unpaused
        }
        else if (eGameState == GameState.Unpaused) // Game state is unpaused
        {
            ConditionLabel.Text = "PAUSED\n\r\n\rDouble tap the screen to continue!"; // Change label text
            ConditionLabel.IsVisible = true; // Show condition label with the PAUSED text
            eGameState = GameState.Paused; // Make game state to be paused
        }
        else
        {
            ConditionLabel.IsVisible = false; // Hide label
            eGameState = GameState.Unpaused; // Make state to be unpaused
        }*/
        //}

        private void TapGestureRecognizer_TappedOnce(object sender, EventArgs e)
        {
            // Rotate upon tap if game is unpaused
            if (eGameState == GameState.Normal && nCurrentFigure != 3)
            {
                // Play rotate sound if enabled
                if (sounds_enabled)
                {
                    //player2.Load("rotate.mp3");
                    //player2.Loop = false;
                    //player2.Volume = 100;
                    rotate.Play();
                }

                if (vibration_enabled)
                    Vibration.Vibrate();

                // Save the non-colided tetromino and rotate the collide checking tetromino
                Point center = pCurTetromino[1]; //center of rotation
                for (int i = 0; i < 4; i++)
                {
                    pLastTetromino[i] = pCurTetromino[i];

                    int x = pCurTetromino[i].y - center.y;
                    int y = pCurTetromino[i].x - center.x;
                    pCurTetromino[i].x = center.x - x;
                    pCurTetromino[i].y = center.y + y;
                }

                // Check if the rotated tetromino doesn't fit (collides)
                if (!DoesTetrominoRotationFit())
                    for (int i = 0; i < 4; i++)
                        pCurTetromino[i] = pLastTetromino[i]; // reset it to the non-colided position
                else
                    UpdateVisualTetrominoPosition(); // Update the visual representation of the tetromino
            }
        }

        private void PanGestureRecognizer_PanUpdated(object sender, PanUpdatedEventArgs e)
        {
            if (eGameState == GameState.Normal) // Game state is unpaused
                switch (e.StatusType)
                {
                    case GestureStatus.Running:
                        {
                            if (e.TotalY >= /*(sMovedStepsY+1) **/3 * nPieceSize+((nPieceSize*0.01)*touch_sensitivity) /*Tetromino[0].HeightRequest*/) // Move down
                            {
                                // Play move sound if enabled
                                /*if (sounds_enabled)
                                {
                                    //player2.Load("move.mp3");
                                    //player2.Loop = false;
                                    //player2.Volume = 100;
                                    move.Play();
                                }*/

                                // Increase the number of moved steps
                                //sMovedStepsY++;

                                if (bMoveDown != true)
                                {
                                    bMoveDown = true;
                                    Device.StartTimer(TimeSpan.FromMilliseconds(1), () =>
                                    {
                                        //MilisecondsPassed++;

                                        if (bMoveDown == true)
                                        {
                                            MilisecondsPassed++;
                                            return true;
                                        }
                                        else
                                            return false;
                                    });
                                }
                            }

                            if (e.TotalX >= ((sMovedStepsX + 1) * nPieceSize + ((nPieceSize * 0.01) * touch_sensitivity) /*Tetromino[0].WidthRequest*/) /*+ Math.Log10(e.TotalY)*/) // Move right
                            {
                                // Play move sound if enabled
                                if (sounds_enabled)
                                {
                                    //player2.Load("move.mp3");
                                    //player2.Loop = false;
                                    //player2.Volume = 100;
                                    move.Play();
                                }

                                if (vibration_enabled)
                                    Vibration.Vibrate();

                                // Stop moving down
                                //sMovedStepsY=0;

                                // Increase the number of moved steps
                                sMovedStepsX++;

                                // Save the non-colided tetromino and move the collide checking tetromino
                                for (int i = 0; i < 4; i++)
                                {
                                    pLastTetromino[i] = pCurTetromino[i];
                                    pCurTetromino[i].x++; // pCurTetromino[i].x += 1;
                                }

                                // Check if the moved tetromino doesn't fit (collides)
                                if (!DoesTetrominoMovementFit())
                                    for (int i = 0; i < 4; i++)
                                        pCurTetromino[i] = pLastTetromino[i]; // reset it to the non-colided position
                                else
                                    UpdateVisualTetrominoPosition(); // Update the visual representation of the tetromino
                            }
                            else if (e.TotalX <= ((sMovedStepsX - 1) * nPieceSize + ((nPieceSize * 0.01) * touch_sensitivity) /*Tetromino[0].WidthRequest*/) /*+ Math.Log10(e.TotalY)*/) // Move left
                            {
                                // Play move sound if enabled
                                if (sounds_enabled)
                                {
                                    //player2.Load("move.mp3");
                                    //player2.Loop = false;
                                    //player2.Volume = 100;
                                    move.Play();
                                }

                                if (vibration_enabled)
                                    Vibration.Vibrate();

                                // Stop moving down
                                //sMovedStepsY=0;

                                // Decrease the number of moved steps
                                sMovedStepsX--;

                                // Save the non-colided tetromino and move the collide checking tetromino
                                for (int i = 0; i < 4; i++)
                                {
                                    pLastTetromino[i] = pCurTetromino[i];
                                    pCurTetromino[i].x--; // pCurTetromino[i].x -= 1;
                                }

                                // Check if the moved tetromino doesn't fit (collides)
                                if (!DoesTetrominoMovementFit())
                                    for (int i = 0; i < 4; i++)
                                        pCurTetromino[i] = pLastTetromino[i]; // reset it to the non-colided position
                                else
                                    UpdateVisualTetrominoPosition(); // Update the visual representation of the tetromino
                            }

                            break;
                        }
                    case GestureStatus.Completed:
                        {
                            if (bMoveDown == true)
                            {
                                bMoveDown = false;
                                nSpeedCount = 0;

                                if (MilisecondsPassed < 50)
                                {
                                    // Instantly make tetromino part of field
                                    //nSpeedCount = nSpeed;

                                    // Play place down sound if enabled
                                    if (sounds_enabled)
                                    {
                                        //player2.Load("fast_down.mp3");
                                        //player2.Loop = false;
                                        //player2.Volume = 100;
                                        fast_down.Play();
                                    }

                                    if (vibration_enabled)
                                        Vibration.Vibrate();

                                    do
                                    {
                                        // Save the non-colided tetromino and move the collide checking tetromino
                                        for (int i = 0; i < 4; i++)
                                        {
                                            pLastTetromino[i] = pCurTetromino[i];
                                            pCurTetromino[i].y++;
                                        }
                                    } while (DoesTetrominoMovementFit());

                                    for (int i = 0; i < 4; i++)
                                        pCurTetromino[i] = pLastTetromino[i]; // reset it to the non-colided position
                                    UpdateVisualTetrominoPosition(); // Update the visual representation of the tetromino
                                }

                                MilisecondsPassed = 0;
                            }
                            if (sMovedStepsX != 0) sMovedStepsX = 0;
                            //if (sMovedStepsY != 0) sMovedStepsY = 0;
                            break;
                        }
                }
        }

        private void iBtnPause_Clicked(object sender, EventArgs e)
        {
            //if (eGameState != GameState.Normal)
            //    return;

            iBtnPause.IsEnabled = false;
            eGameState = GameState.Paused;
            LayoutRoot.BackgroundColor = Color.FromHex("#7F000000");

            Frame frame = new Frame()
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = App.screenWidth * 0.7,
                HeightRequest = App.screenHeight * 0.3,
                CornerRadius = (float)(App.screenHeight * 0.3 * 0.1),
                BackgroundColor = Color.FromHex("#007DA9"),
                HasShadow = true,
            };

            Grid grid = new Grid()
            {
                RowDefinitions =
                {
                    new RowDefinition { Height = new GridLength(1.5, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                },
            };

            grid.Children.Add(new Label
            {
                Text = "PAUSED",
                TextColor = Color.White,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                //HorizontalTextAlignment= TextAlignment.Start,
                //VerticalTextAlignment= TextAlignment.Start,
            }, 0, 1, 0, 1);

            Button button1 = new Button()
            {
                Text = "QUIT",
                TextColor = Color.FromHex("#7FFFFFFF"),
                BackgroundColor = Color.FromHex("#2591B7"),
                BorderColor = Color.FromHex("#7FFFFFFF"),
                //HeightRequest = Device.GetNamedSize(NamedSize.Small, typeof(Label))*3,
                WidthRequest = App.screenWidth * 0.35,
                BorderWidth = 1.5,
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                //HorizontalTextAlignment= TextAlignment.Start,
                //VerticalTextAlignment= TextAlignment.Start,
            };
            button1.Clicked += (object snder, EventArgs evnt) =>
            {
                if (Preferences.Get("Music", true) == true)
                {
                    App.player.Load("menu.mp3");
                    //App.player.Loop = true;
                    App.player.Volume = 75;
                    App.player.Play();
                }

                // Save user data about the game
                SaveUserProgress();

                // handle the tap
                Application.Current.MainPage = new NavigationPage(new MainPage());
            };
            Grid.SetRow(button1, 1);
            grid.Children.Add(button1);

            Button button2 = new Button()
            {
                Text = "NEW GAME",
                TextColor = Color.FromHex("#7FFFFFFF"),
                BackgroundColor = Color.FromHex("#2591B7"),
                BorderColor = Color.FromHex("#7FFFFFFF"),
                //HeightRequest = Device.GetNamedSize(NamedSize.Small, typeof(Label))*3,
                WidthRequest = App.screenWidth * 0.35,
                BorderWidth = 1.5,
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                //HorizontalTextAlignment= TextAlignment.Start,
                //VerticalTextAlignment= TextAlignment.Start,
            };
            button2.Clicked += (object snder, EventArgs evnt) =>
            {
                // handle the tap
                //Application.Current.MainPage = new NavigationPage(new MainPage());

                for (int fy = 0; fy < nFieldSizeY; fy++) // Reset the field
                    for (int fx = 0; fx < nFieldSizeX; fx++)
                    {
                        if (nField[fy][fx] != null)
                        {
                            grTetrisPanel.Children.Remove(nField[fy][fx]);
                            nField[fy][fx] = null;
                        }
                    }

                bMoveDown = false; // Reset move down bool
                sMovedStepsX = 0; // Reset move x steps

                nScore = 0; // Reset the score
                nSpeed = 20; // Reset game speed to default value
                nSpeedCount = 0;
                nTetrominoCount = 0; // Reset the number of tetrominos placed on the field
                nLinesCount = 0;  // How many lines have been cleared from the start of the game
                nDifficultyLevel = 1; // Display to the user the game's difficulty
                MilisecondsPassed = 0; // If user wants to move the tetromino down .... and just fast releases the down movement (within half a second)
                lblLines.Text = "Lines 0";
                lblLevel.Text = "Level 1";
                lblCurScore.Text = "0";

                // handle the tap
                iBtnPause.IsEnabled = true;
                LayoutRoot.BackgroundColor = Color.Transparent;

                bdGrid.Children.Remove(frame);
                //eGameState = GameState.Normal;

                PickTetromino(); // Reset visual rep and positional tetromino to their start up values
                UpdateVisualTetrominoPosition(); // Update visual representation tetromino's position ....
                Resume_Game();
            };
            Grid.SetRow(button2, 2);
            grid.Children.Add(button2);

            Button button3 = new Button()
            {
                Text = "CONTINUE",
                TextColor = Color.White,
                BackgroundColor = Color.DarkOrange,
                BorderColor = Color.FromHex("#7FFFFFFF"),
                //HeightRequest = Device.GetNamedSize(NamedSize.Small, typeof(Label))*3,
                WidthRequest = App.screenWidth * 0.35,
                BorderWidth = 1.5,
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                //HorizontalTextAlignment= TextAlignment.Start,
                //VerticalTextAlignment= TextAlignment.Start,
            };
            button3.Clicked += (object snder, EventArgs evnt) =>
            {
                // handle the tap
                iBtnPause.IsEnabled = true;
                LayoutRoot.BackgroundColor = Color.Transparent;

                bdGrid.Children.Remove(frame);
                nSpeedCount = 0;
                //eGameState = GameState.Normal;
                Resume_Game();
            };
            Grid.SetRow(button3, 3);
            grid.Children.Add(button3);

            frame.Content = grid;
            bdGrid.Children.Add(frame);
        }
    }
}