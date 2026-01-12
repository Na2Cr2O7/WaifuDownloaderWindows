using ABI.System;
using WaifuDownloaderWindows;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using WinRT.Interop;
using System.Security.Cryptography;
using System.Collections.ObjectModel;
using WaifuDownloader;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WaifuDownloaderWindows
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        mainpage page;
        WaifuDownloaderAPI waifuDownloader;
        Option option;
        ObservableCollection<string> NSFWOptions;
        nint hwnd;
        public MainWindow()
        {
            waifuDownloader = new();
            option = new();
            NSFWOptions = [NSFWOption.SHOW_EVERYTHING.ToString(), NSFWOption.ONLY_NSFW.ToString(),NSFWOption.BLOCK_NSFW.ToString()];
            hwnd = WindowNative.GetWindowHandle(this);
            page = new mainpage(waifuDownloader:ref waifuDownloader,option:ref option,ref hwnd);
            
            InitializeComponent();
            ExtendsContentIntoTitleBar = true;
            rootFrame.Content =page ;
            SelectNSFW.SelectedItem = option.Items!.Nsfw_mode.ToString();
            AutoReloadInterval.Value = option.Items!.Auto_reload_interval;

        }

        private async void Website_Tapped(object sender, TappedRoutedEventArgs e)
        { 
        }

        private async void Credits_Tapped(object sender, TappedRoutedEventArgs e)
        {

            await ShowDialog("Art work by", waifuDownloader.GetTheArtist() ?? "None");



            //await dialog.ShowAsync();

        }

        private void SelectNSFW_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           string g = NSFWOption.SHOW_EVERYTHING.ToString();
            string selected=(string)e.AddedItems[0];
            if (selected.Length < 0)
            {
                return;
            }
            if (selected == NSFWOption.SHOW_EVERYTHING.ToString())
            {
                option.Items!.Nsfw_mode = NSFWOption.SHOW_EVERYTHING;
            }
            else if (selected == NSFWOption.BLOCK_NSFW.ToString())
            {
                option.Items!.Nsfw_mode = NSFWOption.BLOCK_NSFW;
            }
            else if (selected == NSFWOption.ONLY_NSFW.ToString()) 
            { 
                option.Items!.Nsfw_mode = NSFWOption.ONLY_NSFW;
            }
            option.Save();
        }

        private async void Credits_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            await ShowDialog("Coded by","Na2Cr2O7");
        }

        private async System.Threading.Tasks.Task ShowDialog(string Title,string Content)
        {
            ContentDialog dialog = new()
            {
                // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
                XamlRoot = this.root.XamlRoot,
                Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                Title = Title,
                PrimaryButtonText = "OK",
                SecondaryButtonText = null,
                CloseButtonText = null,
                DefaultButton = ContentDialogButton.Primary,
                Content = Content
            };
            //dialog.Content = new ContentDialog();



            await dialog.ShowAsync();
        }

        private async void Legal_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await ShowDialog("Legal", "© 2024 SilverOS Na2Cr2O7");
        }


        private void AutoReloadInterval_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            option.Items!.Auto_reload_interval=(int)AutoReloadInterval.Value;
            option.Save();
        }

        private async void NavigationViewItemHeader_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await ShowDialog("Catgirl Downloader", "0.1.0");

        }
    }
}
    