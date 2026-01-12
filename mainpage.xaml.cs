using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.VisualBasic.FileIO;
using Microsoft.Windows.Storage.Pickers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using WaifuDownloader;
using WaifuDownloaderWindows;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Protection.PlayReady;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using WinRT.Interop;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WaifuDownloaderWindows
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class mainpage : Page
    {
        Option option;
        public WaifuDownloaderAPI waifuDownloader;
        private BitmapImage bitmapImage;
        private byte[] imageBytes;
        private DispatcherTimer autoReloadTimer;
        nint hwnd;
        public mainpage(ref WaifuDownloaderAPI waifuDownloader,ref Option option,ref nint hwnd)
        {
            InitializeComponent();
            this.waifuDownloader = waifuDownloader;
            this.option = option;
            this.hwnd=hwnd;
           
           

            //DownloadButton.Content = url;



            NewImage();
            AutoReloadSwitch.IsOn = option.Items!.Auto_reload_enabled;
            autoReloadTimer = new()
            {
                Interval = System.TimeSpan.FromSeconds(option.Items!.Auto_reload_interval),

            };
            autoReloadTimer.Tick += AutoReloadTimer_Tick;
            if (option.Items!.Auto_reload_enabled)
            {
                autoReloadTimer.Start();
            }
        }
        private void AutoReloadTimer_Tick(object? sender, object e)
        {
            NewImage();
        }

        private async void NewImage()
        {
            Progress.IsActive = true;
            bool nsfw;
            if (option.Items!.Nsfw_mode == NSFWOption.ONLY_NSFW)
            {
                nsfw = true;
            }
            else if (option.Items!.Nsfw_mode == NSFWOption.BLOCK_NSFW)
            {
                nsfw = false;
            }
            else
            {
                var rd = new Random();
                if(rd.Next(1,2)==1)
                {
                    nsfw = true;
                }
                else
                {
                    nsfw = false;
                }
            }
            string? url= await waifuDownloader.GetAImageURLAsync(nsfw);
            if (url is null)
            {
                if (AutoReloadSwitch.IsOn)
                {
                    autoReloadTimer.Interval = System.TimeSpan.FromSeconds(option.Items!.Auto_reload_interval);
                    autoReloadTimer.Start();
                }
                else
                {
                    autoReloadTimer.Stop();
                }
                DispatcherTimer retry = new()
                {
                    Interval = TimeSpan.FromSeconds(1)
                };
                retry.Tick += (e, e2) => { retry.Stop(); NewImage(); };
                retry.Start();
                return;
            }
            //BitmapImage bitmapImage = new()
            //{
            //    UriSource = new System.Uri(Display.BaseUri, url)
            //};
            //Display.Source = bitmapImage;
            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = true,
                MaxAutomaticRedirections = 10 
            };
            using var httpClient = new HttpClient(handler);

            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
            imageBytes = await httpClient.GetByteArrayAsync(url);
           
         

            using (var stream = new MemoryStream(imageBytes))
            {
                bitmapImage = new BitmapImage();
                await bitmapImage.SetSourceAsync(stream.AsRandomAccessStream());
            }
            Display.Source = bitmapImage;
            Progress.IsActive = false;
            if (AutoReloadSwitch.IsOn)
            {
                autoReloadTimer.Interval= System.TimeSpan.FromSeconds(option.Items!.Auto_reload_interval);
                autoReloadTimer.Start();
            }
            else
            {
                autoReloadTimer.Stop();
            }
        }
        private static string ComputeMd5(byte[] data)
        {
            var hash = MD5.HashData(data);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            autoReloadTimer.Stop();

            NewImage();
        }

        private async void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            var savePicker = new Windows.Storage.Pickers.FileSavePicker();
            if (imageBytes is null)
            {
                return;
            }
            
            InitializeWithWindow.Initialize(savePicker, hwnd);

            // Configure picker
            savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            savePicker.SuggestedFileName = ComputeMd5(imageBytes);
            string Extension = waifuDownloader.GetTheExtension();
            savePicker.FileTypeChoices.Add("Image", new List<string> { Extension });
            savePicker.DefaultFileExtension = Extension;

            // Show save picker
            var file = await savePicker.PickSaveFileAsync();
            if (file == null)
                return; // User canceled

            try
            {
                using var fileStream = await file.OpenAsync(FileAccessMode.ReadWrite);
                await fileStream.WriteAsync(imageBytes.AsBuffer());
                await fileStream.FlushAsync();
            }
            catch 
            {
            }
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Display_ImageOpened(object sender, RoutedEventArgs e)
        {
            Progress.IsActive = false;
            if (AutoReloadSwitch.IsOn)
            {
                autoReloadTimer.Start();
            }
            else
            {
                autoReloadTimer.Stop();
            }
        }


        private void AutoReloadSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            option.Items!.Auto_reload_enabled = AutoReloadSwitch.IsOn;
            option.Save();
            if (autoReloadTimer is null)
            {
                return;
            }
            if (AutoReloadSwitch.IsOn)
            {
                autoReloadTimer.Start();
            }
            else
            {
                autoReloadTimer.Stop();
            }
        }
    }
}
