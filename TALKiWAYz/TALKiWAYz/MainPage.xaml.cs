using System;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using UWPTextToSpeech;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TALKiWAYz
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public Authentication auth;
        public string accessToken;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void HelloWorldButton_Click(object sender, RoutedEventArgs e)
        {
            //Console.WriteLine("Starting Authtentication");
            
            var file = File.ReadAllText("./Assets/Env.txt");
            auth = new Authentication(file);

            try
            {
                accessToken = auth.GetAccessToken();
                //Console.WriteLine("Token: {0}\n", accessToken);
            }
            catch (Exception ex)
            {
                //Console.WriteLine("Failed authentication.");
                //Console.WriteLine(ex.ToString());
                //Console.WriteLine(ex.Message);
                return;
            }

            //Console.WriteLine("Starting TTSSample request code execution.");

            string requestUri = "https://speech.platform.bing.com/synthesize";

            var cortana = new Synthesize(new Synthesize.InputOptions()
            {
                RequestUri = new Uri(requestUri),
                // Text to be spoken.
                Text = "Hello World. Would you like to play a game?",
                VoiceType = Gender.Female,
                // Refer to the documentation for complete list of supported locales.
                Locale = "en-US",
                // You can also customize the output voice. Refer to the documentation to view the different
                // voices that the TTS service can output.
                VoiceName = "Microsoft Server Speech Text to Speech Voice (en-US, ZiraRUS)",
                // Service can return audio in different output format. 
                OutputFormat = AudioOutputFormat.Riff16Khz16BitMonoPcm,
                AuthorizationToken = "Bearer " + accessToken,
            });

            cortana.OnAudioAvailable += TTSUtils.StoreAudio;
            cortana.OnError += TTSUtils.ErrorHandler;
            cortana.Speak(CancellationToken.None).Wait();
        }
    }
}
