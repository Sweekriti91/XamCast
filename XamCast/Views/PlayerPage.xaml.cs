using System;
using System.Collections.Generic;
using Xamarin.CommunityToolkit.Core;
using Xamarin.Forms;
using XamCast.Models;

namespace XamCast
{
    public partial class PlayerPage : ContentPage
    {
        public PlayerPage(MediaInfo media)
        {
            InitializeComponent();
            PlayerControl.Source = MediaSource.FromUri(media.SourceURL);
            PlayerVideName.Text = media.DisplayName;
        }


        void OnMediaOpened(object sender, EventArgs e)
        {
            Console.WriteLine("Media opened.");
        }

        void OnMediaFailed(object sender, EventArgs e)
        {
            Console.WriteLine("Media failed.");
        }

        void OnMediaEnded(object sender, EventArgs e)
        {
            Console.WriteLine("Media ended.");
        }

        void OnSeekCompleted(object sender, EventArgs e)
        {
            Console.WriteLine("Seek completed.");
        }
    }
}
