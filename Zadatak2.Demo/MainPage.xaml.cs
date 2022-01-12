using ClassLibrary11;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Zadatak2.Demo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 

    public sealed partial class MainPage : Page
    {
        MyImageClass MyObject;
        public MainPage()
        {
            this.InitializeComponent();
            MyObject = new MyImageClass();
        }

        public object Arguments { get; private set; }
        public string FileName { get; private set; }


        private async void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            //picker.FileTypeFilter.Add(".jpeg");
            //picker.FileTypeFilter.Add(".png");
            var files = await picker.PickMultipleFilesAsync();

            StringBuilder output = new StringBuilder("Picked files:\n");
            if (files.Count > 0)
            {
                // Application now has read/write access to the picked file(s)
                foreach (StorageFile file in files)
                {
                    output.Append(file.Path.ToString() + "\n");
                }
            }
            FolderPath.Text = output.ToString();
            MyObject.LoadingImages(files);
          
            MyObject.CreateNewMainThreads();
        }

        private async void DownloadFile_Click(object sender, RoutedEventArgs e)
        {
            var savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("JPG Image", new List<string>() { ".jpg" });
            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = "New Image";

            /* var picker = new FolderPicker();
             var pfolder = await picker.PickSingleFolderAsync();
             StorageApplicationPermissions.FutureAccessList.Add(pfolder);

             var folder = await StorageFolder.GetFolderFromPathAsync(@"C:\Users\admin\Pictures\Saved Pictures");
             var file = await folder.CreateFileAsync("text.txt");
             using (var writer = await file.OpenStreamForWriteAsync())
             {
                 await writer.WriteAsync(new byte[100], 0, 0);
             }*/


        }
    }
}
