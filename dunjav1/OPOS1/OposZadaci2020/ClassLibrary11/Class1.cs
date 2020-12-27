using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace ClassLibrary11
{
    public class MyImageClass
    {
        //public static List<Tuple<WriteableBitmap, int>> LoadedImagesList;
        public List<Task> MainTaskList;
        public List<Tuple<Color[][], StorageFile>> imageMatrixList;
        public List<StorageFile> GrayScaleImages;

        public MyImageClass()
        {
            imageMatrixList = new List<Tuple<Color[][], StorageFile>>();
            GrayScaleImages = new List<StorageFile>();
            //LoadedImagesList = new List<Tuple<WriteableBitmap, int>>();
            MainTaskList = new List<Task>();

        }
        //public StorageFile CopyFromList()
        //{
        //    StorageFile copy = GrayScaleImages[0];
        //    GrayScaleImages.RemoveAt(0);
        //    return copy;
        //}

        public Color GetPixel(byte[] pixels, int x, int y, uint height)
        {
            int i = x;
            int j = y;
            int k = (i * (int)height + j) * 3;
            var r = pixels[k + 0];
            var g = pixels[k + 1];
            var b = pixels[k + 2];

            return Color.FromArgb(0, r, g, b);
        }

        public async void LoadingImages(IReadOnlyList<StorageFile> files)
        {
            foreach (StorageFile file in files)
            {
                Stream imgstream = await file.OpenStreamForReadAsync();
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(imgstream.AsRandomAccessStream());
                Color[][] matrix = new Color[decoder.PixelWidth][];

                var data = await decoder.GetPixelDataAsync();
                var bytes = data.DetachPixelData();
                for (int i = 0; i < decoder.PixelWidth; i++)
                {
                    matrix[i] = new Color[decoder.PixelHeight];
                    for (int j = 0; j < decoder.PixelHeight; j++)
                    {
                        matrix[i][j] = GetPixel(bytes, i, j, decoder.PixelHeight);
                    }
                }
                imageMatrixList.Add(new Tuple<Color[][], StorageFile>(matrix, file));

            }
        }
        public void CreateNewMainThreads()
        {
            while (imageMatrixList.Count != 0)
            {
                Tuple<Color[][], StorageFile> Element = imageMatrixList[0];
                imageMatrixList.RemoveAt(0);
                MainTaskList.Add(Task.Factory.StartNew(new Action(() => { ImageProcessing(Element.Item1, Element.Item2, 4); })));
            }
        }
        public void ImageProcessing(Color[][] imageMatrix, StorageFile file, int numberOfThreads)
        {
            int IsImageGrayScale = 1;
            int imageWidth = imageMatrix.Length;
            int imageHeight = imageMatrix[0].Length;

            List<Color[][]> ListOfImageParts = new List<Color[][]>();

            //rastavljanje slike u manje dijelove radili paralelizacije
            int partition = Convert.ToInt32(Math.Ceiling(imageWidth / Convert.ToDouble(numberOfThreads)));

            for (int i = 0; i < numberOfThreads - 1; i++)
            {
                Color[][] ImagePart = new Color[partition][];
                for (int j = 0; j < partition; j++)
                {
                    ImagePart[j] = new Color[imageHeight];
                    for (int k = 0; k < imageHeight; k++)
                        ImagePart[j][k] = imageMatrix[i * partition + j][k];
                }
                ListOfImageParts.Add(ImagePart);
            }
            //posljednji. nepotpuni dio slike
            {
                Color[][] ImagePart = new Color[imageWidth - partition * (numberOfThreads - 1)][];
                for (int j = 0; j < imageWidth - partition * (numberOfThreads - 1); j++)
                {
                    ImagePart[j] = new Color[imageHeight];
                    for (int k = 0; k < imageHeight; k++)
                        ImagePart[j][k] = imageMatrix[partition * (numberOfThreads - 1) + j][k];
                }
                ListOfImageParts.Add(ImagePart);
            }
            List<Task> TaskSublist = new List<Task>();
            foreach (Color[][] temp in ListOfImageParts)
                TaskSublist.Add(Task.Factory.StartNew(new Action(() => { ParalellRun(temp, ref IsImageGrayScale); })));

            Task.WaitAll(TaskSublist.ToArray());
            GrayScaleImages.Add(file);

            if (IsImageGrayScale == 1)
            {
            }
        }

        public string getGray()
        {
            return GrayScaleImages.Count.ToString();
        }
        public static void ParalellRun(Color[][] ImagePartMatrix, ref int IsImageGrayScale)
        {
            int imageWidth = ImagePartMatrix.Length;
            int imageHeight = ImagePartMatrix[0].Length;
            for (int i = 0; i < imageWidth; i++)
                for (int j = 0; j < imageHeight; j++)
                {
                    if (ImagePartMatrix[i][j].R != ImagePartMatrix[i][j].G || ImagePartMatrix[i][j].R != ImagePartMatrix[i][j].B)
                        Interlocked.Increment(ref IsImageGrayScale);
                }
        }


    }
}
