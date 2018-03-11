using System;
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

using Newtonsoft.Json;

using TALKiWAYz.Models;
using TALKiWAYz.Utils;
using UWPSpeechToText;
using Button = Windows.UI.Xaml.Controls.Button;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace TALKiWAYz.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ConversationPage : Page
    {
        private CortanaTalk Cortana = new CortanaTalk();
        private STT SpeechToText = new STT();

        private ConversationDetails ConversationDetails;
        private int Score = 0;

        private int currentPage = 0;

        public ConversationPage()
        {
            this.InitializeComponent();

            ScoreText.Text = "Score: 0";

            ConversationDetails =
                JsonConvert.DeserializeObject<ConversationDetails>(File.ReadAllText("./Assets/Conversation.json"));

            DrawPage(currentPage);
        }

        public async void DrawPage(int statementNum)
        {
            ResponseCanvas.Children.Clear();

            var statement = ConversationDetails.Conversations[0].Statements[statementNum];

            QuestionText.Text = statement.SpeechText;
            for (var i = 0; i < statement.Responses.Count; i++)
            {
                var responseTextBlock = new Button();
                responseTextBlock.Name = $"response_{i}";
                responseTextBlock.FontSize = 20d;
                responseTextBlock.Content = statement.Responses[i].Text;
                responseTextBlock.Click += onResponseClick;

                Canvas.SetLeft(responseTextBlock, 500);
                Canvas.SetTop(responseTextBlock, 150 + (i * 50));
                ResponseCanvas.Children.Add(responseTextBlock);
            }

            var talkMethod = await Cortana.Talk(QuestionText.Text);

            var detectedText = await SpeechToText.Listen();
        }

        private void onResponseClick(object sender, RoutedEventArgs e)
        {
            var button = (Button) sender;
            var responseId = int.Parse(button.Name.Split('_')[1]);

            var statement = ConversationDetails.Conversations[0].Statements[currentPage];
            if (statement.Responses[responseId].Correct)
            {
                Score++;
            }

            ScoreText.Text = $"Score: {Score}";

            currentPage++;
            DrawPage(currentPage);
        }
    }
}
