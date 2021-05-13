using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.MobCAT;
using Xamarin.Essentials;
using Xamarin.Forms;
using XamCast.Models;

namespace XamCast
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<MediaInfo> MediaSourcesList = new ObservableCollection<MediaInfo>();
        IChromecastService _castHelper = ServiceContainer.Resolve<IChromecastService>();

        public MainPage()
        {
            InitializeComponent();
            CreateListOfThings();
            MediaSourceCollectionView.ItemsSource = MediaSourcesList;
        }

        void CreateListOfThings()
        {
            MediaSourcesList.Add(new MediaInfo
            {
                DisplayName = "Hello World Friday May 7",
                SourceURL = "https://sec.ch9.ms/ch9/bef2/b7873fe6-f47c-4597-adc8-c2b14552bef2/hw_2021-05-07_mid.mp4"
            });

            MediaSourcesList.Add(new MediaInfo
            {
                DisplayName = "Hello World Friday April 30",
                SourceURL = "https://sec.ch9.ms/ch9/c6fa/3342bac3-e6ff-4984-a454-6240f275c6fa/hw20210430_mid.mp4"
            });

            MediaSourcesList.Add(new MediaInfo
            {
                DisplayName = "Hello World Friday April 23",
                SourceURL = "https://sec.ch9.ms/ch9/ae3a/b5bd3813-fac7-448b-a330-d17131e3ae3a/hw20210423_mid.mp4"
            });

            MediaSourcesList.Add(new MediaInfo
            {
                DisplayName = "Hello World Friday April 16",
                SourceURL = "https://sec.ch9.ms/ch9/16c1/3bafea31-aee3-41e1-a06b-13154b3116c1/helloword20210416_mid.mp4"
            });

            MediaSourcesList.Add(new MediaInfo
            {
                DisplayName = "Hello World Friday April 9",
                SourceURL = "https://sec.ch9.ms/ch9/1c43/c1ce355b-77ec-4773-9793-ffed8f641c43/hw2021-04-09_mid.mp4"
            });

            MediaSourcesList.Add(new MediaInfo
            {
                DisplayName = "Hello World Friday May 7",
                SourceURL = "https://sec.ch9.ms/ch9/bef2/b7873fe6-f47c-4597-adc8-c2b14552bef2/hw_2021-05-07_mid.mp4"
            });

            MediaSourcesList.Add(new MediaInfo
            {
                DisplayName = "Hello World Friday April 30",
                SourceURL = "https://sec.ch9.ms/ch9/c6fa/3342bac3-e6ff-4984-a454-6240f275c6fa/hw20210430_mid.mp4"
            });

            MediaSourcesList.Add(new MediaInfo
            {
                DisplayName = "Hello World Friday April 23",
                SourceURL = "https://sec.ch9.ms/ch9/ae3a/b5bd3813-fac7-448b-a330-d17131e3ae3a/hw20210423_mid.mp4"
            });

            MediaSourcesList.Add(new MediaInfo
            {
                DisplayName = "Hello World Friday April 16",
                SourceURL = "https://sec.ch9.ms/ch9/16c1/3bafea31-aee3-41e1-a06b-13154b3116c1/helloword20210416_mid.mp4"
            });

            MediaSourcesList.Add(new MediaInfo
            {
                DisplayName = "Hello World Friday April 9",
                SourceURL = "https://sec.ch9.ms/ch9/1c43/c1ce355b-77ec-4773-9793-ffed8f641c43/hw2021-04-09_mid.mp4"
            });
        }

        async void MediaSourceCollectionView_SelectionChanged(System.Object sender, Xamarin.Forms.SelectionChangedEventArgs e)
        {
            MediaInfo previousSelection = e.PreviousSelection.FirstOrDefault() as MediaInfo;
            MediaInfo currentSelection = e.CurrentSelection.FirstOrDefault() as MediaInfo;

            if (DeviceInfo.Platform == DevicePlatform.iOS)
                await Navigation.PushAsync(new PlayerPage(currentSelection));
            else
                _castHelper.OpenPlayerPage(currentSelection);
        }
    }
}
