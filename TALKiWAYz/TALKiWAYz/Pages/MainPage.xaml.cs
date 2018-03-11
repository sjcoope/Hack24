﻿using System;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using TALKiWAYz.Utils;
using UWPTextToSpeech;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TALKiWAYz.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private CortanaTalk Cortana;

        public MainPage()
        {
            Cortana = new CortanaTalk();

            this.InitializeComponent();
        }

        private async void StartGameButton_Click(object sender, RoutedEventArgs e)
        {
            Task<bool> talkTask = Cortana.Talk("Ok, let's play");

            var success = await talkTask;

            await Task.Run(async () => { await Task.Delay(TimeSpan.FromSeconds(5)); });
            if (success)
            {
                var rootFrame = Window.Current.Content as Frame;

                rootFrame.Navigate(typeof(ConversationPage));
            }
        }
    }
}
