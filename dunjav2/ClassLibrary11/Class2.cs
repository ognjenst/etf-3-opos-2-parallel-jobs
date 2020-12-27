using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;

namespace ClassLibrary11
{
    public class Image
    {
        public StorageFile ImageStorageFile;
        public int NumberOfThreads;
        public int IsGrayScale;

        public Color[][] ImageMatrix;
        public List<Color[][]> ListOfImagePartsMatrix;

        private Stream ImageStream;
        private BitmapDecoder ImageDecoder;

        private Image(StorageFile file)
        {
            IsGrayScale = 1;
            Random rnd = new Random();
            NumberOfThreads = rnd.Next(3, 6);

            ImageStorageFile = file;
            ListOfImagePartsMatrix = new List<Color[][]>();
        }
        public static async Task<Image> CreateNewImage(StorageFile file)
        {
            Image NewImage = new Image(file);
            await NewImage.InitNewImage();
            return NewImage;
        }
        private async Task InitNewImage()
        {
            await Task.Factory.StartNew(new Action(async () => 
            {
                ImageStream = await ImageStorageFile.OpenStreamForReadAsync();
                ImageDecoder = await BitmapDecoder.CreateAsync(ImageStream.AsRandomAccessStream());
                ImageMatrix = new Color[ImageDecoder.PixelWidth][];

                var data = await ImageDecoder.GetPixelDataAsync();
                var bytes = data.DetachPixelData();
                for (int i = 0; i < ImageDecoder.PixelWidth; i++)
                {
                    ImageMatrix[i] = new Color[ImageDecoder.PixelHeight];
                    for (int j = 0; j < ImageDecoder.PixelHeight; j++)
                    {
                        ImageMatrix[i][j] = GetPixel(bytes, i, j, ImageDecoder.PixelHeight);
                    }
                }
                //imageMatrixList.Add(new Tuple<Color[][], StorageFile>(matrix, ImageStorageFile));
            }));
        }
        private Color GetPixel(byte[] pixels, int x, int y, uint height)
        {
            int i = x;
            int j = y;
            int k = (i * (int)height + j) * 3;
            var r = pixels[k + 0];
            var g = pixels[k + 1];
            var b = pixels[k + 2];

            return Color.FromArgb(0, r, g, b);
        }
    }

}
