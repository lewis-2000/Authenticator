using Microsoft.UI.Windowing;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Credentials.UI;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Authenticator
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private readonly HttpClient _httpClient;

        public MainWindow()
        {
            this.InitializeComponent();
            _httpClient = new HttpClient();


            // Set the initial size of the window and disable resizing
            var hwnd = WindowNative.GetWindowHandle(this);
            var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);
            var presenter = appWindow.Presenter as OverlappedPresenter;
            if (presenter != null)
            {
                presenter.IsResizable = false;
                presenter.IsMaximizable = false;
                appWindow.Resize(new Windows.Graphics.SizeInt32(400, 550));
            }

        }



        private async void OnAuthenticateButtonClick(object sender, RoutedEventArgs e)
        {
            var availabilty = await UserConsentVerifier.CheckAvailabilityAsync();

            if (availabilty == UserConsentVerifierAvailability.Available)
            {
                var result = await UserConsentVerifier.RequestVerificationAsync("Please Verify your identity");

                if (result == UserConsentVerificationResult.Verified)
                {
                    //Authentication Succesfull
                    AuthIcon.Fill = new SolidColorBrush(Microsoft.UI.Colors.Green);
                    await MakePostApiCall();

                }
                else
                {
                    //Authentication Failed
                    AuthIcon.Fill = new SolidColorBrush(Microsoft.UI.Colors.Red);

                }
            }
            else
            {
                AuthIcon.Fill = new SolidColorBrush(Microsoft.UI.Colors.Red);
            }
        }



        private async Task MakePostApiCall()
        {
            var requestBody = new StringContent("{\"authenticated\": true}", Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("https://your-api-endpoint.com/authenticate", requestBody);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                // Handle error
                System.Diagnostics.Debug.WriteLine($"Error making POST API call: {ex.Message}");
            }
        }

    }
}
