using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Interop;
using System.Threading;

namespace Wpf_KG_5
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BitmapImage image;
        public MainWindow()
        {
            InitializeComponent();
            GistogramColor.mainWindow = this;
            ActionImage.mainWindow = this;
            image = showImage();    //Отображаем картинку
            ActionImage.bmpImg = ActionImage.BitmapImage2Bitmap(image);
            ActionImage.Standard = image;
            ActionImage.loadPanel("Стандарт");
            
        }
        
        
        

        //Отобразить изображение на форме
        public BitmapImage showImage()
        {
            BitmapImage img = new BitmapImage();
            img.BeginInit();
            img.UriSource = new Uri("ram.jpg", UriKind.Relative);
            img.CacheOption = BitmapCacheOption.OnLoad;
            img.EndInit();
            GistogramColor.showGistagrams(img);
            imageDisplay.Width = 1500;
            imageDisplay.Source = img;
            return img;
        }

        private bool flagInit = false;
        private void listAction_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (flagInit == true)
            {
                ActionImage.loadPanel((listAction.SelectedItem as TextBlock).Text);

            }
            else
            {
                flagInit = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Thread thread;
            string current = (listAction.SelectedItem as TextBlock).Text;
            switch (current)
            {
                case "Чёрно-белый":
                    ActionImage.bmpImg = ActionImage.BitmapImage2Bitmap(image);
                    thread = new Thread(ActionImage.BlachAndWhite) { IsBackground = true };
                    thread.Start();
                    break;

                case "Стандарт":
                    ActionImage.bmpImg = ActionImage.BitmapImage2Bitmap(image);
                    thread = new Thread(ActionImage.ColorEqualizer) { IsBackground = true };
                    thread.Start();
                    break;

                case "Негатив":
                    ActionImage.bmpImg = ActionImage.BitmapImage2Bitmap(image);
                    thread = new Thread(ActionImage.Negative) { IsBackground = true };
                    thread.Start();
                    break;

                case "Оттенки серого":
                    ActionImage.bmpImg = ActionImage.BitmapImage2Bitmap(image);
                    thread = new Thread(ActionImage.GrayShade) { IsBackground = true };
                    thread.Start();
                    break;

                case "Яркость":
                    ActionImage.bmpImg = ActionImage.BitmapImage2Bitmap(image);
                    thread = new Thread(ActionImage.Brightness) { IsBackground = true };
                    thread.Start();
                    break;

                case "Контраст":
                    ActionImage.bmpImg = ActionImage.BitmapImage2Bitmap(image);
                    thread = new Thread(ActionImage.Contrast) { IsBackground = true };
                    thread.Start();
                    break;
            }
        }
    }
}
