using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using System.IO;
using Xamarin.CommunityToolkit.UI.Views;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Firebase.Storage;
using Firebase.Database;
using System.ComponentModel;
using Xamarin.Essentials;
using Firebase.Database.Query;

using FFImageLoading;
using FFImageLoading.Work;

using System.Net;
using System.Drawing;
using System.Windows.Media.Imaging;
//using Windows.UI.Xaml.Media.Imaging;
//using Windows.Storage.Streams;
using Android.Graphics;
//using OpenCvSharp;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
//using OpenCV.Android;
//using OpenCV.Core;
using Android.Util;
//using OpenCV.ImgProc;
//using Size = OpenCV.Core.Size;
using Java.Util;


using OpenCV.Android;
using OpenCV.Core;
using OpenCV.ImgProc;
using Size = OpenCV.Core.Size;
using Bitmap = Android.Graphics.Bitmap;
using Mat = OpenCV.Core.Mat;
using ImageSource = Xamarin.Forms.ImageSource;
using Image = Xamarin.Forms.Image;
using System.Runtime.InteropServices;
using Java.Lang;
using Java.IO;
using System.Net.Http;
using FFImageLoading.Decoders;

namespace App1
{
    //[XamlCompilation(XamlCompilationOptions.Compile)] 
    [Activity(Label = "XamarinEssentialsPage")]
    public partial class XamarinEssentialsPage : ContentPage
    {

        public XamarinEssentialsPage()
        {
            InitializeComponent();

            Content = new Label
            {
                Text = "All done! press the button to upload",
                HorizontalTextAlignment = Xamarin.Forms.TextAlignment.Center,
                VerticalTextAlignment = Xamarin.Forms.TextAlignment.Center,
                BackgroundColor = System.Drawing.Color.Yellow,
                TextColor = System.Drawing.Color.Blue
            };
            //BindingContext = new Mybindingobj();
            //getPhotoFromFirebase();
        }
        FirebaseClient firebaseClient = new Firebase.Database.FirebaseClient(FirebaseConfig.DatabaseUrl);
        FirebaseStorage firebaseStorage = new FirebaseStorage(FirebaseConfig.StorageBucket);
        private string imgUrl;
        public string ImgUrl
        {
            get { return imgUrl; }
            set
            {
                imgUrl = value;
                //OnPropertyChanged("ImgUrl");
            }
        }

        public const string TAG = "OCVSample::Activity";
        private BaseLoaderCallback mLoaderCallback;
        private static Activity Context;
        private ILoaderCallbackInterface mOpenCVCallBack;

        class Callback : BaseLoaderCallback
        {
            private readonly XamarinEssentialsPage _activity;
            public Callback(Context context, XamarinEssentialsPage activity)
                : base(context)
            {
                _activity = activity;
            }
            public override void OnManagerConnected(int status)
            {
                switch (status)
                {
                    case LoaderCallbackInterface.Success:
                        {
                            Log.Info(XamarinEssentialsPage.TAG, "OpenCV loaded successfully");
                        }
                        break;
                    default:
                        {
                            base.OnManagerConnected(status);
                        }
                        break;
                }
            }
        }
        private Uri uri;//從firebase拿的

        public void upload() {//all upload!!!!!!

            /*string a = DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg";

            var task = new FirebaseStorage(FirebaseConfig.StorageBucket,//YOUR-PROJECT-ID.appspot.com gs://YOUR-PROJECT-ID.appspot.com
            new FirebaseStorageOptions
            {
                ThrowOnCancel = true
            })
            .Child("photos")
            .Child(a)
            .PutAsync(stream);

            //傳url到firebase cloud storage //到realtime not cloud
            var downloadlink = task.TargetUrl;
            const string id = "user1";
            MyDatabaseRecord t = new MyDatabaseRecord(id, downloadlink.ToString());
            _ = firebaseClient.Child("URL").PostAsync(t);
                await DisplayAlert("上傳", "以選擇上傳", "OK");*/

        }
        public void getPhotoFromFirebase()//改public// ban!!!!!
        {
            /*System.Uri uri;
            System.Uri.TryCreate(imageURI, UriKind.Absolute, out uri);
            Task<ImageSource> result = Task<ImageSource>.Factory.StartNew(() => ImageSource.FromUri(uri));
            _companyImage.Source = await result;*/
            string[] point = { "one" };//巡邏點
            firebaseStorage.Child("patrol_dataset").Child(point[0]).Child("1_1.jpg").GetDownloadUrlAsync().ContinueWith(async (task) =>
            {
                if (!task.IsFaulted && !task.IsCanceled)
                {
                    //System.Uri uri;
                    System.Uri.TryCreate(task.Result.ToString(), UriKind.Absolute, out uri);
                    //img.Source = await Button_Clicked();

                }
                task.Wait();
            });


        }

        private async Task<ImageSource> Button_Clicked()
        {
            byte[] imageByte = await DownloadImageAsync(uri.OriginalString);//await //從uri拿圖片
            Bitmap bitmap = BitmapFactory.DecodeByteArray(imageByte, 0, imageByte.Length);
            Mat mat = new Mat();
            Utils.BitmapToMat(bitmap, mat);

            Mat gray = new Mat(600, 400, CvType.Cv8uc3);
            Mat mIntermediateMat = new Mat(600, 400, CvType.Cv8uc3); ;
            Mat blurred_img = new Mat();
            Imgproc.Blur(mat, mat, new Size(5,5));
            Imgproc.CvtColor(mat, gray, Imgproc.ColorBgr2gray);
            Imgproc.Canny(gray, mIntermediateMat, 80, 130);

            Bitmap resultBitmap = Bitmap.CreateBitmap(mIntermediateMat.Cols(), mIntermediateMat.Rows(), Bitmap.Config.Argb8888); ;//mIntermediateMat
            Utils.MatToBitmap(mIntermediateMat, resultBitmap);//mIntermediateMat

            MemoryStream MyMemoryStream = new MemoryStream();
            resultBitmap.Compress(Bitmap.CompressFormat.Png, 100, MyMemoryStream);
            //var PhotoStream = Xamarin.Forms.ImageSource.FromStream(() => MyMemoryStream);//ImageSource
            ImageSource Source = ImageSource.FromStream(() => new MemoryStream(MyMemoryStream.ToArray()));
            return Source;
        }

        private async Task<byte[]> DownloadImageAsync(string imageUrl)
        {
            byte[] imageByte;
            using (HttpClientHandler handler = new HttpClientHandler())
            using (HttpClient client = new HttpClient(handler))
            {
                try
                {
#region 呼叫遠端 Web API
                    HttpResponseMessage response = null;

                    // 設定相關網址內容
                    response = await client.GetAsync(imageUrl);

#endregion

#region 處理呼叫完成 Web API 之後的回報結果
                    if (response != null)
                    {
                        if (response.IsSuccessStatusCode == true)
                        {
                            // 重點：Content 利用 ReadAsByteArrayAsync() 取回圖檔 byte
                            return await response.Content.ReadAsByteArrayAsync();
                        }
                        else
                        {
                            imageByte = null;
                        }
                    }
                    else
                    {
                        imageByte = null;
                    }
#endregion
                }
                catch
                {
                    imageByte = null;
                }
            }

            return imageByte;
        }
    }

}

////img.Source = ImageSource.FromStream(() => new MemoryStream(temp.ToArray()));
//img.Source = ImageSource.FromStream(() => new MemoryStream(imageByte));
/*ImgUrl = uri.ToString().Replace("&", "&amp;");

var webClient = new WebClient();
byte[] imageBytes = webClient.DownloadData(ImgUrl);//轉成byte
img.Source = ImageSource.FromStream(() => new MemoryStream(imageBytes));*/

/*var tempFilePath = System.IO.Path.GetTempFileName();//IO
var tempFileStream = File.OpenWrite(tempFilePath);//IO
Stream stream = new MemoryStream(imageBytes);//放入stream
stream.CopyToAsync(tempFileStream);   不確定*/

/*if (!OpenCVLoader.InitDebug())
{
    System.Console.WriteLine("Failed to INIT \n OpenCV Failure");
}
else
{
    System.Console.WriteLine("OpenCV INIT Succes");
}

Mat mat = new Mat(433, 577, CvType.Cv8uc3);
Mat gray = new Mat(433, 577, CvType.Cv8uc3);
Mat mIntermediateMat = new Mat(433, 577, CvType.Cv8uc3); ;//新加
mat.Put(0, 0, imageBytes);
//mRgba = inputFrame.Rgba();
Imgproc.CvtColor(mat, gray, Imgproc.ColorBgr2gray);
Imgproc.Canny(gray, mIntermediateMat, 80, 100);

Bitmap resultBitmap = Bitmap.CreateBitmap(mIntermediateMat.Cols(), mIntermediateMat.Rows(), Bitmap.Config.Argb8888); ;
Utils.MatToBitmap(mIntermediateMat, resultBitmap);

MemoryStream MyMemoryStream = new MemoryStream();
resultBitmap.Compress(Bitmap.CompressFormat.Png, 100, MyMemoryStream);
var PhotoStream = Xamarin.Forms.ImageSource.FromStream(() => MyMemoryStream);//ImageSource
this.BindingContext = new Mybindingobj { ImageSourceTemp = PhotoStream };
resultBitmap.Dispose();
MyMemoryStream.Dispose();*/



/*var tempFilePath = System.IO.Path.GetTempFileName();//IO
var tempFileStream = File.OpenWrite(tempFilePath);//IO
Stream stream = new MemoryStream(imageBytes);//放入stream
stream.CopyToAsync(tempFileStream);   不確定*/

//ImageSource Source1 = ImageSource.FromUri(new Uri(ImgUrl));
///Image image = new Image { Source = ImageSource.FromUri(new Uri(ImgUrl)) };
/*if (!OpenCVLoader.InitAsync(OpenCVLoader.OpencvVersion242,
(Activity)XamarinEssentialsPage.Context, mOpenCVCallBack))
{
    Log.Info("TEST", "Cannot connect to OpenCV Manager");
}

if (!OpenCVLoader.InitDebug())
{
    Log.Debug(TAG, "Internal OpenCV library not found. Using OpenCV Manager for initialization");
    OpenCVLoader.InitAsync(OpenCVLoader.OpencvVersion300, this, mLoaderCallback);
}
else
{
    Log.Debug(TAG, "OpenCV library found inside package. Using it!");
    mLoaderCallback.OnManagerConnected(LoaderCallbackInterface.Success);
}*/

/*opencv
 * Mat rgba = inputFrame.Rgba();
Mat rgbaInnerWindow;
Size sizeRgba = rgba.Size();
int rows = (int)sizeRgba.Height;
int cols = (int)sizeRgba.Width;

int left = cols / 8;
int top = rows / 8;

int width = cols * 3 / 4;
int height = rows * 3 / 4;
rgbaInnerWindow = rgba.Submat(top, top + height, left, left + width);
Imgproc.Canny(rgbaInnerWindow, mIntermediateMat, 80, 90);
Imgproc.CvtColor(mIntermediateMat, rgbaInnerWindow, Imgproc.ColorGray2bgra, 4);
rgbaInnerWindow.Release();*/

/*
Mat mat = CvInvoke.Imread(tempFilePath, ImreadModes.Color);
Mat grayImage = new Mat();
CvInvoke.CvtColor(mat, grayImage, ColorConversion.Bgr2Gray);
// 使用 Canny 演算法處理圖像
var cannyMat = new Mat();
CvInvoke.Canny(mat, cannyMat, 100, 200);//mat// 使用 Emgu CV 讀取臨時文件

System.Drawing.Bitmap bitmapTest = cannyMat.ToBitmap();//ver3
*/
//中間是現在在修改的

/*Android.Media.Image MySystemImage = resultBitmap;
MySystemImage.Save(MyMemoryStream, ImageFormat.Jpeg);
var PhotoStream = Xamarin.Forms.ImageSource.FromStream(() => MyMemoryStream);//ImageSource
this.BindingContext = new Mybindingobj { ImageSourceTemp = PhotoStream };
a.Dispose();
MyMemoryStream.Dispose();*/

/*Bitmap a = new Bitmap(mIntermediateMat.Cols, cannyMat.Rows, (int)cannyMat.Step, PixelFormat.Format24bppRgb, cannyMat.DataPointer);
MemoryStream MyMemoryStream = new MemoryStream();
Android.Media.Image MySystemImage = a;
MySystemImage.Save(MyMemoryStream, ImageFormat.Jpeg);
var PhotoStream = Xamarin.Forms.ImageSource.FromStream(() => MyMemoryStream);//ImageSource
this.BindingContext = new Mybindingobj { ImageSourceTemp = PhotoStream };
a.Dispose();
MyMemoryStream.Dispose();*/


/*ver2
Bitmap a = new Bitmap(cannyMat.Cols, cannyMat.Rows, (int)cannyMat.Step, PixelFormat.Format24bppRgb, cannyMat.DataPointer);
MemoryStream MyMemoryStream = new MemoryStream();
Android.Media.Image MySystemImage = a;
MySystemImage.Save(MyMemoryStream, ImageFormat.Jpeg);
var PhotoStream = Xamarin.Forms.ImageSource.FromStream(() => MyMemoryStream);//ImageSource
this.BindingContext = new Mybindingobj{ ImageSourceTemp = PhotoStream };
a.Dispose();
MyMemoryStream.Dispose();
 */

/*ver1
System.Drawing.Image img = a;

BitmapImage bitmapImage = new BitmapImage();


using (MemoryStream stream1 = new MemoryStream())
{
    a.Save(stream1, System.Drawing.Imaging.ImageFormat.Png);//memorystream
    stream1.Position = 0;
    bitmapImage.SetSource((IRandomAccessStream)stream1);//bitmapimage
    Stream stream2 = stream1;//stream
    var PhotoStream = Xamarin.Forms.ImageSource.FromStream(() => stream1);//ImageSource
    View.Source = PhotoStream;
    stream2.Dispose();
    stream1.Dispose();

}
// 刪除臨時文件
File.Delete(tempFilePath);/**/

//BitmapImage bitmap = new BitmapImage(new Uri(uri.ToString(), UriKind.Absolute));
//View.Source = bitmap;
/*View.Source = new UriImageSource
    {
        Uri = uri,
        CachingEnabled = true,
        CacheValidity = new TimeSpan(3, 0, 0, 0)
    };*/
/*ImageService.Instance.LoadUrl(temp.Result)
    .Retry(3, 200)
    .DownSample(500, 500)
    .InvalidateLayout(true);*/

/*private BitmapImage BitmapToBitmapImage(System.Drawing.Bitmap bitmap)
{
    BitmapImage bitmapImage = new BitmapImage();
    using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
    {
        bitmap.Save(ms, bitmap.RawFormat);
        bitmapImage.BeginInit();
        bitmapImage.StreamSource = ms;
        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        bitmapImage.EndInit();
        bitmapImage.Freeze();
    }
    return bitmapImage;
}
private Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
{

    using (MemoryStream outStream = new MemoryStream())
    {
        BitmapEncoder enc = new BmpBitmapEncoder();
        enc.Frames.Add(BitmapFrame.Create(bitmapImage));
        enc.Save(outStream);
        Bitmap bitmap = new Bitmap(outStream);
        outStream.Close();

        return new Bitmap(bitmap);
    }
}*/
