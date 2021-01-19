using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Interop;
using System.Windows;

namespace Wpf_KG_5
{
    static class GistogramColor
    {
        public static MainWindow mainWindow;
        public static void showGistagrams(BitmapImage image)
        {
            System.Drawing.Image img = CalculateBarChart(BitmapImage2Bitmap(image));
            mainWindow.gistogramDisplay.Source = Bitmap2BitmapImage(new Bitmap(img));

        }

        public static void showGistagrams(Bitmap image)
        {
            System.Drawing.Image img = CalculateBarChart(image);
            mainWindow.gistogramDisplay.Source = Bitmap2BitmapImage(new Bitmap(img));
        }

            //Построение гистаграммы
        public static Image CalculateBarChart(Image image)
        {
            Bitmap barChart = null;
            if (image != null)
            {
                // определяем размеры гистограммы. В идеале, ширина должны быть кратна 768 - 
                // по пикселю на каждый столбик каждого из каналов


                int width = 768;
                int height = 150;
                
                
                // получаем битмап из изображения
                Bitmap bmp = new Bitmap(image);
                // создаем саму гистограмму
                barChart = new Bitmap(width, height);
                // создаем массивы, в котором будут содержаться количества повторений для каждого из значений каналов.
                // индекс соответствует значению канала
                int[] R = new int[256];
                int[] G = new int[256];
                int[] B = new int[256];
                int i, j;
                System.Drawing.Color color;
                // собираем статистику для изображения
                for (i = 0; i < bmp.Width; i = i + 10)
                    for (j = 0; j < bmp.Height; j = j + 10)
                    {
                        color = bmp.GetPixel(i, j);
                        ++R[color.R];
                        ++G[color.G];
                        ++B[color.B];
                    }
                // находим самый высокий столбец, чтобы корректно масштабировать гистограмму по высоте
                int max = 0;
                for (i = 0; i < 256; ++i)
                {
                    if (R[i] > max)
                        max = R[i];
                    if (G[i] > max)
                        max = G[i];
                    if (B[i] > max)
                        max = B[i];
                }
                // определяем коэффициент масштабирования по высоте
                double point = (double)max / height;
                // отрисовываем столбец за столбцом нашу гистограмму с учетом масштаба
                for (i = 0; i < width - 3; ++i)
                {
                    for (j = height - 1; j > height - R[i / 3] / point; --j)
                    {
                        barChart.SetPixel(i, j, System.Drawing.Color.Red);
                    }
                    ++i;
                    for (j = height - 1; j > height - G[i / 3] / point; --j)
                    {
                        barChart.SetPixel(i, j, System.Drawing.Color.Green);
                    }
                    ++i;
                    for (j = height - 1; j > height - B[i / 3] / point; --j)
                    {
                        barChart.SetPixel(i, j, System.Drawing.Color.Blue);
                    }
                }
            }
            else
                barChart = new Bitmap(1, 1);
            return barChart;
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
        private static Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
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
