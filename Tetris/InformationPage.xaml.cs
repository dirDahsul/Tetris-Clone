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
    public partial class InformationPage : ContentPage
    {
        public InformationPage()
        {
            InitializeComponent();

            // Тук ще добавим кода за извличане на информация за вида на дисплея
            // Get Metrics
            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
            // Orientation (Landscape, Portrait, Square, Unknown)
            var orientation = mainDisplayInfo.Orientation;
            // Rotation (0, 90, 180, 270)
            var rotation = mainDisplayInfo.Rotation;
            // Width (in pixels)
            var width = mainDisplayInfo.Width;
            // Height (in pixels)
            var height = mainDisplayInfo.Height;
            // Screen density
            var density = mainDisplayInfo.Density;

            // Обобщаваме информацията за дисплея
            lblDisplayInfo.Text = "Main display ifo: " + mainDisplayInfo + "\r\n" +
                "Orientation: " + orientation + "\r\n" +
                "Screen density: " + density + "\r\n" +
                "Rotation: " + rotation + "\r\n" +
                "Width x Height: " + width + "x" + height;

            // Тук ще добавим кода за извличане на информация за вида на устройството
            // Device Model (SMG-950U, iPhone10,6) -
            var device = DeviceInfo.Model;
            // Manufacturer (Samsung) -
            var manufacturer = DeviceInfo.Manufacturer;
            // Device Name (Motz's iPhone) -
            var deviceName = DeviceInfo.Name;
            // Operating System Version Number (7.0) -
            var version = DeviceInfo.VersionString;
            // Platform (Android) -
            var platform = DeviceInfo.Platform;
            // Idiom (Phone) -
            var idiom = DeviceInfo.Idiom;
            // Device Type (Physical) -
            var deviceType = DeviceInfo.DeviceType;

            // Обобщаваме информацията за вида на устройството
            lblDeviceInfo.Text = manufacturer + " " + device + " " + version + "\r\n" +
                "Device name: " + deviceName + "\r\n" +
                "Platform: " + platform + " for " + idiom + " - " + deviceType;
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