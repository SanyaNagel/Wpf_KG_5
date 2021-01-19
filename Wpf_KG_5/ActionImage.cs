using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Interop;
using System.Windows;
using System.Threading;

namespace Wpf_KG_5
{
    static class ActionImage
    {
        public static MainWindow mainWindow;
        public static Bitmap bmpImg;
        public static int prevSlider1 = 0;
        public static int prevSlider2 = 0;
        public static int prevSlider3 = 0;
        public static BitmapImage Standard;

        private static int R = 0;
        private static int G = 0;
        private static int B = 0;

        public static void loadPanel(string name)
        {
            switch (name)
            {
                case "Чёрно-белый":
                    mainWindow.R.Content = "B";
                    mainWindow.Slider1.Maximum = 255;
                    mainWindow.Slider1.Value = 128;

                    //Скрываем два последних
                    mainWindow.Slider2.Visibility = Visibility.Hidden;
                    mainWindow.Slider3.Visibility = Visibility.Hidden;
                    //Скрываем их название
                    mainWindow.G.Visibility = Visibility.Hidden;
                    mainWindow.B.Visibility = Visibility.Hidden;
                    break;

                case "Стандарт":
                    mainWindow.imageDisplay.Source = Standard;
                    loadBrightness();
                    mainWindow.R.Content = "R";
                    mainWindow.Slider1.Maximum = 255;
                    mainWindow.Slider2.Maximum = 255;
                    mainWindow.Slider3.Maximum = 255;
                    //Открываем два последних
                    mainWindow.Slider2.Visibility = Visibility.Visible;
                    mainWindow.Slider3.Visibility = Visibility.Visible;
                    //Открываем их название
                    mainWindow.G.Visibility = Visibility.Visible;
                    mainWindow.B.Visibility = Visibility.Visible;
                    break;

                case "Яркость":
                    mainWindow.Slider1.Maximum = 255;
                    mainWindow.Slider1.Value = 0;
                    mainWindow.R.Content = "A";

                    //Скрываем два последних
                    mainWindow.Slider2.Visibility = Visibility.Hidden;
                    mainWindow.Slider3.Visibility = Visibility.Hidden;
                    //Скрываем их название
                    mainWindow.G.Visibility = Visibility.Hidden;
                    mainWindow.B.Visibility = Visibility.Hidden;
                    break;

                case "Контраст":
                    var thread = new Thread(calceMidleCenelRGB) { IsBackground = true };
                    thread.Start();
                    mainWindow.Slider1.Maximum = 4;
                    mainWindow.Slider1.Value = mainWindow.Slider1.Maximum/2;

                    mainWindow.R.Content = "K";

                    //Скрываем два последних
                    mainWindow.Slider2.Visibility = Visibility.Hidden;
                    mainWindow.Slider3.Visibility = Visibility.Hidden;
                    //Скрываем их название
                    mainWindow.G.Visibility = Visibility.Hidden;
                    mainWindow.B.Visibility = Visibility.Hidden;
                    break;

                default: break;
            }
        }

        public static void Negative()
        {
            Bitmap result = new Bitmap(bmpImg.Width, bmpImg.Height);
            Color cr = new Color();

            for (int stPix = 0; stPix < 5; stPix++)
            {
                for (int i = 0; i < bmpImg.Width; ++i)
                {
                    for (int j = stPix; j < bmpImg.Height; j += 5)
                    {
                        cr = bmpImg.GetPixel(i, j);

                        result.SetPixel(i, j, Color.FromArgb(255-cr.R, 255 - cr.G, 255 - cr.B));
                    }

                }
                mainWindow.Dispatcher.Invoke(() =>
                {
                    mainWindow.imageDisplay.Source = Bitmap2BitmapImage(result);
                });

            }
            mainWindow.Dispatcher.Invoke(() =>
            {
                GistogramColor.showGistagrams(result);
            });
        }

        public static void GrayShade()
        {
            Bitmap result = new Bitmap(bmpImg.Width, bmpImg.Height);
            Color cr = new Color();
            
            for (int stPix = 0; stPix < 5; stPix++)
            {
                for (int i = 0; i < bmpImg.Width; ++i)
                {
                    for (int j = stPix; j < bmpImg.Height; j += 5)
                    {
                        cr = bmpImg.GetPixel(i, j);


                        result.SetPixel(i, j, Color.FromArgb(cr.R, cr.R, cr.R));
                    }

                }
                mainWindow.Dispatcher.Invoke(() =>
                {
                    mainWindow.imageDisplay.Source = Bitmap2BitmapImage(result);
                });

            }
            mainWindow.Dispatcher.Invoke(() =>
            {
                GistogramColor.showGistagrams(result);
            });
        }

        public static void Contrast()
        {
            Bitmap result = new Bitmap(bmpImg.Width, bmpImg.Height);
            Color cr = new Color();
            double newValueSlider1 = 0;

            mainWindow.Dispatcher.Invoke(() =>
            {
                newValueSlider1 = mainWindow.Slider1.Value;
            });

            for (int stPix = 0; stPix < 5; stPix++)
            {
                for (int i = 0; i < bmpImg.Width; ++i)
                {
                    for (int j = stPix; j < bmpImg.Height; j += 5)
                    {
                        cr = bmpImg.GetPixel(i, j);

                        int red = cr.R;
                        red = (int)(newValueSlider1 * (red - R) + R);

                        if (red > 255)
                        {
                            red = 255;
                        }
                        else if (red < 0)
                        {
                            red = 0;
                        }

                        int green = cr.G;
                        green = (int)(newValueSlider1 * (green - G) + G);

                        if (green > 255)
                        {
                            green = 255;
                        }
                        else if (green < 0)
                        {
                            green = 0;
                        }

                        int blue = cr.B;
                        blue = (int)(newValueSlider1 * (blue - B) + B);

                        if (blue > 255)
                        {
                            blue = 255;
                        }
                        else if (blue < 0)
                        {
                            blue = 0;
                        }
                        result.SetPixel(i, j, Color.FromArgb(red, green, blue));
                    }

                }
                mainWindow.Dispatcher.Invoke(() =>
                {
                    mainWindow.imageDisplay.Source = Bitmap2BitmapImage(result);
                });

            }
            mainWindow.Dispatcher.Invoke(() =>
            {
                GistogramColor.showGistagrams(result);
            });

        }

        public static void calceMidleCenelRGB()
        {
            int h = bmpImg.Height;
            int w = bmpImg.Width;
            Color color;
            for (int i = 0; i < w; ++i)
            {
                for (int j = 0; j < h; ++j)
                {
                    color = bmpImg.GetPixel(i, j);
                    R += color.R;
                    G += color.G;
                    B += color.B;
                }
            }

            R /= (w * h);
            G /= (w * h);
            B /= (w * h);
        }

        public static void loadBrightness()
        {
            calceMidleCenelRGB();

            mainWindow.Dispatcher.Invoke(() =>
            {
                mainWindow.Slider1.Value = R;
                mainWindow.Slider2.Value = G;
                mainWindow.Slider3.Value = B;
                GistogramColor.showGistagrams(bmpImg);
            });
            prevSlider1 = R;
            prevSlider2 = G;
            prevSlider3 = B;

        }

        public static void Brightness()
        {

            Bitmap result = new Bitmap(bmpImg.Width, bmpImg.Height);
            Color cr = new Color();
            int newValueSlider1 = 0;

            mainWindow.Dispatcher.Invoke(() =>
            {
                newValueSlider1 = (int)mainWindow.Slider1.Value;

            });

            for (int stPix = 0; stPix < 5; stPix++)
            {

                for (int i = 0; i < bmpImg.Width; ++i)
                {
                    for (int j = stPix; j < bmpImg.Height; j += 5)
                    {
                        cr = bmpImg.GetPixel(i, j);

                        int red = cr.R;
                        red += newValueSlider1;

                        if (red > 255)
                        {
                            red = 255;
                        }
                        else if (red < 0)
                        {
                            red = 0;
                        }

                        int green = cr.G;
                        green += newValueSlider1;

                        if (green > 255)
                        {
                            green = 255;
                        }
                        else if (green < 0)
                        {
                            green = 0;
                        }

                        int blue = cr.B;
                        blue += newValueSlider1;

                        if (blue > 255)
                        {
                            blue = 255;
                        }
                        else if (blue < 0)
                        {
                            blue = 0;
                        }
                        result.SetPixel(i, j, Color.FromArgb(red, green, blue));
                    }

                }
                mainWindow.Dispatcher.Invoke(() =>
                {
                    mainWindow.imageDisplay.Source = Bitmap2BitmapImage(result);
                });

            }
            mainWindow.Dispatcher.Invoke(() =>
            {
                GistogramColor.showGistagrams(result);
            });

        }


        public static void ColorEqualizer()
        {

            Bitmap result = new Bitmap(bmpImg.Width, bmpImg.Height);
            Color cr = new Color();
            int newValueSlider1 = 0;
            int newValueSlider2 = 0;
            int newValueSlider3 = 0;

            mainWindow.Dispatcher.Invoke(() =>
            {
                newValueSlider1 = (int)mainWindow.Slider1.Value;
                newValueSlider2 = (int)mainWindow.Slider2.Value;
                newValueSlider3 = (int)mainWindow.Slider3.Value;

            });

            for (int stPix = 0; stPix < 5; stPix++)
            {

                for (int i = 0; i < bmpImg.Width; ++i)
                {
                    for (int j = stPix; j < bmpImg.Height; j+=5)
                    {
                        cr = bmpImg.GetPixel(i, j);

                        int red = cr.R;
                        red += newValueSlider1 - prevSlider1;

                        if (red > 255) {
                            red = 255;
                        }
                        else if (red < 0)
                        {
                            red = 0;
                        }

                        int green = cr.G;
                        green += newValueSlider2 - prevSlider2;
                    
                        if (green > 255)
                        {
                            green = 255;
                        }
                        else if(green < 0)
                        {
                            green = 0;
                        }

                        int blue = cr.B;
                        blue += newValueSlider3 - prevSlider3;
                    
                        if (blue > 255)
                        {
                            blue = 255;
                        }
                        else if (blue < 0)
                        {
                            blue = 0;
                        }
                        result.SetPixel(i, j, Color.FromArgb(red, green, blue));
                    }

                }
                mainWindow.Dispatcher.Invoke(() =>
                {
                    mainWindow.imageDisplay.Source = Bitmap2BitmapImage(result);
                });

            }
            mainWindow.Dispatcher.Invoke(() =>
            {
                GistogramColor.showGistagrams(result);
            });

        }

        public static void BlachAndWhite()
        {
            int P = 0;
            mainWindow.Dispatcher.Invoke(() =>
            {
                P = (int)mainWindow.Slider1.Value;
            });
            Bitmap result = new Bitmap(bmpImg.Width, bmpImg.Height);
            Color color = new Color();
            for (int stPix = 0; stPix < 5; stPix++)
            {
                for (int j = 0; j < bmpImg.Height; j++)
                {
                    for (int i = stPix; i < bmpImg.Width; i += 5)
                    {
                        color = bmpImg.GetPixel(i, j);
                        int K = (color.R + color.G + color.B) / 3;
                        result.SetPixel(i, j, K <= P ? Color.Black : Color.White);
                        
                    }
                }

                mainWindow.Dispatcher.Invoke(() =>
                {
                    mainWindow.imageDisplay.Source = Bitmap2BitmapImage(result);
                });
                
            }
            mainWindow.Dispatcher.Invoke(() =>
            {
                GistogramColor.showGistagrams(result);
            });

        }


        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
        private static BitmapSource Bitmap2BitmapImage(Bitmap bitmap)
        {
            IntPtr hBitmap = bitmap.GetHbitmap();
            BitmapSource retval;

            try
            {
                retval = Imaging.CreateBitmapSourceFromHBitmap(
                             hBitmap,
                             IntPtr.Zero,
                             Int32Rect.Empty,
                             BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(hBitmap);
            }

            return retval;
        }
        public static Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }
    }
}
