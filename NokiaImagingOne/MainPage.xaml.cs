using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using NokiaImagingOne.Resources;
using Microsoft.Phone.Tasks;
using Nokia.Graphics.Imaging;
using Nokia.Graphics;
using Windows.Phone.Media.Capture;
using System.IO;
using System.Windows.Media.Imaging;
using Microsoft.Xna.Framework.Media;
using Windows.Storage.Streams;

namespace NokiaImagingOne
{
    public partial class MainPage : PhoneApplicationPage
    {
       
        private EditingSession session;
        // Constructor
        public MainPage()
        {
            InitializeComponent();
           
            
            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        private async void task_Completed(object sender, PhotoResult e)
        {
            /* byte[] imageBits = new byte[(int)e.ChosenPhoto.Length];
            e.ChosenPhoto.Read(imageBits, 0, imageBits.Length);
            e.ChosenPhoto.Seek(0, System.IO.SeekOrigin.Begin);
            MemoryStream screenshot = new MemoryStream(imageBits);
            BitmapImage imageFromStream = new BitmapImage();
            imageFromStream.SetSource(screenshot); */
            Stream imgstream = e.ChosenPhoto;
            //System.Windows.Media.Imaging.BitmapImage bmp = new System.Windows.Media.Imaging.BitmapImage();
            //bmp.SetSource(e.ChosenPhoto);
            //img.Source = bmp;
            ApplyButton.Visibility = Visibility.Visible;
            session = await EditingSessionFactory.CreateEditingSessionAsync(imgstream);
            try
            {
                session.AddFilter(FilterFactory.CreateCartoonFilter(true));

                // Save the image as a jpeg to the camera roll
                using (MemoryStream stream = new MemoryStream())
                {
                    WriteableBitmap bitmap = new WriteableBitmap(3552, 2448);
                    await session.RenderToWriteableBitmapAsync(bitmap);
                    bitmap.SaveJpeg(stream, bitmap.PixelWidth, bitmap.PixelHeight, 0, 100);
                    img.Source = bitmap;
                }
                //Force the framework to redraw the Image.
            }
            catch (Exception exception)
            {
                MessageBox.Show("Exception:" + exception.Message);
            }
            
        }

        private async void Save_Clicked(object sender, RoutedEventArgs e)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                WriteableBitmap bitmap = new WriteableBitmap(3552, 2448);
                await session.RenderToWriteableBitmapAsync(bitmap);
                bitmap.SaveJpeg(stream, bitmap.PixelWidth, bitmap.PixelHeight, 0, 100);
                stream.Seek(0, SeekOrigin.Begin);
                using (MediaLibrary mediaLibrary = new MediaLibrary())
                    mediaLibrary.SavePictureToCameraRoll("Picture.jpg", stream);
                MessageBox.Show("Image saved!");
            }
        }

        private void Browse_Clicked(object sender, RoutedEventArgs e)
        {
            PhotoChooserTask task = new PhotoChooserTask();
            task.Show();
            task.Completed += task_Completed;
        }

        private void Apply_Clicked(object sender, RoutedEventArgs e)
        {
            ApplyButton.Visibility = Visibility.Collapsed;
            SaveButton.Visibility = Visibility.Visible;
            MessageBox.Show("Applied");
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}

    }
}