using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.CommunityToolkit.UI.Views;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Firebase.Storage;
using Firebase.Database;
using System.ComponentModel;
using Xamarin.Essentials;
using Firebase.Database.Query;
/// <summary>
/// 
/// </summary>
/// using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using System.IO;
using Xamarin.CommunityToolkit.UI.Views;

/**/
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
using System.Drawing.Imaging;
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
using Android.Content.Res;
using Math = System.Math;
using Android.Views.InputMethods;

namespace App1.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class CameraViewPage : ContentPage
    {

        public string ShowImage { get; set; }
        string[] placenum = { "place1", "place2", "place3" };
        string[] point = { "one", "two", "three" };
        int[] buf = { 0, 0, 0 };
        public CameraViewPage()
        {
            InitializeComponent();

            for (int i = 0; i < 3; i++)
            {
                buf[i] = Transpoter.place1Value[i][0] - '0';
            }
            getPhotoFromFirebase();
            flage_tem = false;
            getPhotoFromFirebase();

            
        }

        private void CaptureImage(object sender, EventArgs e)
        {
            xctCameraView.Shutter();

            //byte[] image = e.ImageData;
            //DependencyService.Get<ISaveService>().wirteFile("test.jpg", image);
        }

        private void Switch_Toggled(object sender, ToggledEventArgs e)//一開始是true
        {
            var state = e.Value;
            img.IsVisible = state;
            if (state == false) Mode.Text = "Off";
            else Mode.Text = "On";
        }
        //try to save the photo to firebase
        FirebaseClient firebaseClient = new Firebase.Database.FirebaseClient(FirebaseConfig.DatabaseUrl);
        FirebaseStorage firebaseStorage = new FirebaseStorage(FirebaseConfig.StorageBucket);
        public struct MyDatabaseRecord
        {
            public string userid;
            private string path;
            public System.String Path
            {
                get { return path; }//讀取屬性
                set { path = value; }//屬性指派值,執行
            }
            public MyDatabaseRecord(System.String id, System.String path1) { path = path1; userid = id; }
        }

        public struct user
        {
            public string userid;
        }
        public FileResult result { get; set; }
        public string A { get; set; }

        double temp;
        async private void MediaCaptured(object sender, MediaCapturedEventArgs e)
        {
            //顯示close 照片 部分
            /////imgView.Source = e.Image;//pixel
            //Show the image at the view 
            /////imgViewPanel.IsVisible = true;

            // 將 stream 寫入到臨時文件中
            Stream stream = new MemoryStream(e.ImageData);
            Stream stream_1 = new MemoryStream(e.ImageData);

            System.Boolean pass = await Template(stream);//暫刪
            if (pass == true)
            {
                DateTime dateToStore = DateTime.Now.AddDays(0);
                string DDate = dateToStore.ToString("yyyy-MM-dd");
                /////await DisplayAlert("成功", "成功擷取，相似閥值"+temp+"可回前一頁", "OK");

                string a = DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg";
                var task = new FirebaseStorage(FirebaseConfig.StorageBucket,//YOUR-PROJECT-ID.appspot.com gs://YOUR-PROJECT-ID.appspot.com
                 new FirebaseStorageOptions
                 {
                     ThrowOnCancel = true
                 })
                .Child("photos")
                .Child(a)
                .PutAsync(stream_1);

                //await DisplayAlert("111", "111，222", "22");
                //.PutAsync(stream);

                ////暫刪 Transpoter.stream1[Transpoter.step-1] = stream;//放進第幾個點//check
                //傳url到firebase cloud storage //到realtime not cloud
                var downloadlink = task.TargetUrl;
                if (downloadlink != null) {
                    string put = $"\"{Transpoter.place1Value[Transpoter.step - 1]},{a}\"";// (Transpoter.place1Value[Transpoter.step - 1] + ',' + a).ToString();
                    //Transpoter.photo[Transpoter.step - 1] = put;
                    Transpoter.pass[Transpoter.step - 1, 1] = true;
                    await firebaseClient.Child("daily_data").Child("controller").Child(DDate).Child(placenum[Transpoter.step - 1]).PutAsync(put.ToString());
                    await DisplayAlert("上傳", "已選擇上傳", "回前一頁");
                    await Navigation.PopAsync();//回前一頁
                }
            }
        }

        private Uri uri_tem;
        private System.Boolean flage_tem =true;
        private Mat templ = new Mat();
        private async Task<bool> Template(Stream stream)
        {
            AssetManager assets = Android.App.Application.Context.Assets;
            //Stream imagestream = assets.Open(".jpg");//
            //Stream templateStream = assets.Open("t1_1.jpg");// Transpoter.image_template[Transpoter.step-1]
            
            bool flag = true;
            //Mat img = Imgproc.imread("image.jpg", Imgproc.ImreadColor);
            //Mat templ = Imgproc.imread("template.jpg", Imgproc.ImreadColor);

            Mat img = ConvertStreamToMat(stream);
            //Mat templ = ConvertStreamToMat(templateStream);//
            //Bitmap rt=Bitmap.CreateBitmap(templ.Rows(), templ.Cols(),Bitmap.Config.Argb8888) ;
            //Utils.MatToBitmap(templ, rt);
            Mat dst = img.Clone();
            Imgproc.CvtColor(img, img, Imgproc.ColorBgr2gray);

            Imgproc.CvtColor(templ, templ, Imgproc.ColorBgr2gray);
            Imgproc.CvtColor(dst, dst, Imgproc.ColorBgr2gray);

            // Create the result matrix
            int resultCols = img.Cols() - templ.Cols() + 1;
            int resultRows = img.Rows() - templ.Rows() + 1;
            int imgc = img.Cols();
            int imgr = img.Rows();
            int tc = templ.Cols();
            int tr = templ.Rows();

            if (resultCols < 0 && resultRows < 0)//A比B大 就把圖調換
            {
                dst = templ.Clone();
                templ = img.Clone();
                img = dst.Clone();
                resultCols = img.Cols() - templ.Cols() + 1;
                resultRows = img.Rows() - templ.Rows() + 1;
            }
            else if (resultCols < 0 || resultRows < 0)//正方形跟長方形
            {
                //templ 旋轉90度//
                Mat rotateMat = Imgproc.GetRotationMatrix2D(new OpenCV.Core.Point(img.Rows() / 2, img.Cols() / 2), 90, 1);
                Imgproc.WarpAffine(img, img, rotateMat, img.Size());

                resultCols = img.Cols() - templ.Cols() + 1;
                resultRows = img.Rows() - templ.Rows() + 1;
                if (resultCols < 0 || resultRows < 0)  flag = false;//旋轉90度 沒救
            }


            if (flag == true)
            {
                Mat result = new Mat(resultRows, resultCols, CvType.Cv32f);//CV_32F

                Imgproc.MatchTemplate(img, templ, result, Imgproc.TmCcoeffNormed);
                Core.MinMaxLocResult mmr = Core.MinMaxLoc(result);
                OpenCV.Core.Point maxLoc = mmr.MaxLoc;
                OpenCV.Core.Point minLoc = mmr.MinLoc;
                double min = mmr.MinVal;
                double max = mmr.MaxVal;
                temp = max;//印
                double threshold = 0;////////
                if (Transpoter.place1Value[Transpoter.step - 1][0] == '1')//0.35,0.3,0.45
                {
                    threshold = 0.45;//
                }
                else if (Transpoter.place1Value[Transpoter.step - 1][0] == '2')//紅色
                {
                    threshold = 0.3;//0.3
                }
                else//二一
                {
                    threshold = 0.35;//0.35
                }
                if (max >= threshold)
                {  // Draw rectangle around the best match
                    OpenCV.Core.Rect rect = new OpenCV.Core.Rect(maxLoc, new Size(templ.Cols(), templ.Rows()));
                    Imgproc.Rectangle(dst, rect, new Scalar(255, 0, 0), 2);

                    Mat resultCv8uc4 = new Mat();
                    dst.ConvertTo(resultCv8uc4, CvType.Cv8uc4);//result

                    Bitmap.Config bitmapConfig = Bitmap.Config.Argb8888;
                    Bitmap resultBitmap = Bitmap.CreateBitmap(resultCv8uc4.Cols(), resultCv8uc4.Rows(), bitmapConfig);
                    Utils.MatToBitmap(resultCv8uc4, resultBitmap);

                    MemoryStream stream1 = new MemoryStream();

                    resultBitmap.Compress(Bitmap.CompressFormat.Png, 100, stream1);
                    stream1.Position = 0;
                    
                    stream1.Dispose();

                    await DisplayAlert("比對", "匹配" + decimal.Round((decimal)(max * 100), 2, MidpointRounding.AwayFromZero)+"% => 驗證成功", "OK");
                    //decimal.Round((decimal)(max * 100), 2, MidpointRounding.AwayFromZero) +
                    return flag;

                }
                else
                {
                   var res=await DisplayAlert("比對", "匹配"+ decimal.Round((decimal)(max * 100), 2, MidpointRounding.AwayFromZero) + "% => 驗證失敗，是否繼續", "OK","回主頁面");
                    if (res==false) {
                        //await Navigation.PushAsync(new Page1());//
                        await Navigation.PopAsync();//回前一頁
                    }
                    flag = false;
                    return flag;
                }
            }
            else
            {
                var res =  await DisplayAlert("比對", "出現錯誤，請重新驗證", "OK","回主頁面");
                if (res == false)
                {
                    //await Navigation.PushAsync(new Page1());
                    await Navigation.PopAsync();//回前一頁
                }
                return flag;
            }

            Mat ConvertStreamToMat(Stream stream2)
            {
                using (var memoryStream = new MemoryStream())
                {
                    stream2.CopyTo(memoryStream);
                    //memoryStream.ToArray();
                    Bitmap bitmap = BitmapFactory.DecodeByteArray(memoryStream.ToArray(), 0, memoryStream.ToArray().Length);

                    Mat mat = new Mat();
                    //mat = Imgcodecs.Imdecode(new MatOfByte(memoryStream.ToArray()), Imgcodecs.ImreadColor);
                    Utils.BitmapToMat(bitmap, mat);
                    return mat;
                }
                // byte[] buffer = new byte[stream.Length];
                //stream.Read(buffer, 0, buffer.Length);

            }
        }
        private void CloseImageView(object sender, EventArgs e)
        {
            //////imgViewPanel.IsVisible = false;
        }/* 暫刪*/

        private string imgUrl;
        public string ImgUrl
        {
            get { return imgUrl; }
            set
            {
                imgUrl = value;
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
        public void getPhotoFromFirebase()//改public
        {
            /*System.Uri uri;
            System.Uri.TryCreate(imageURI, UriKind.Absolute, out uri);
            Task<ImageSource> result = Task<ImageSource>.Factory.StartNew(() => ImageSource.FromUri(uri));
            _companyImage.Source = await result;*/
            if (flage_tem == false)//template 圖讀取
            {
               //firebaseStorage.Child("patrol_dataset").Child("one").Child("1_1.jpg").GetDownloadUrlAsync().ContinueWith( (task) =>//之後修
               firebaseStorage.Child("patrol_dataset").Child(point[buf[Transpoter.step - 1] - 1]).Child(Transpoter.place1Value[Transpoter.step - 1] + ".jpg").GetDownloadUrlAsync().ContinueWith((task) =>
               {
                    if (!task.IsFaulted && !task.IsCanceled)//修
                    {
                        //System.Uri uri;
                        System.Uri.TryCreate(task.Result.ToString(), UriKind.Absolute, out uri);
                        byte[] imageByte = DownloadImageAsync(uri.OriginalString).Result;
                        var t = imageByte.Length;
                        Bitmap bitmap = BitmapFactory.DecodeByteArray(imageByte, 0, imageByte.Length);
                        
                        Utils.BitmapToMat(bitmap, templ);
                        
                    }
                });

            }
            else//canny圖讀取
            {
                //firebaseStorage.Child("patrol_dataset").Child(Transpoter.point[Transpoter.step - 1]).Child(Transpoter.image_compare[Transpoter.step - 1]).GetDownloadUrlAsync().ContinueWith(async (task) =>//之後修
                firebaseStorage.Child("patrol_dataset").Child(point[buf[Transpoter.step - 1] - 1] + "c").Child(Transpoter.place1Value[Transpoter.step - 1] + ".jpg").GetDownloadUrlAsync().ContinueWith(async (task) => 
                {
                    if (!task.IsFaulted && !task.IsCanceled)
                    {
                    //System.Uri uri;
                        System.Uri.TryCreate(task.Result.ToString(), UriKind.Absolute, out uri);
                        img.Source = await Button_Clicked();

                    }
                });
            }
            
        }

        private async Task<ImageSource> Button_Clicked()
        {
            byte[] imageByte = await DownloadImageAsync(uri.OriginalString);//await //從uri拿圖片
            Bitmap bitmap = BitmapFactory.DecodeByteArray(imageByte, 0, imageByte.Length);
            Mat mat = new Mat();
            Utils.BitmapToMat(bitmap, mat);

            Size size = mat.Size();
            Imgproc.Resize(mat, mat, size);

            //Mat gray = new Mat(738, 554, CvType.Cv8uc3);//600 400
            Mat gray = new Mat(size, CvType.Cv8uc3);
            //Mat mIntermediateMat = new Mat(738, 554, CvType.Cv8uc3); ;//600 400
            Mat mIntermediateMat = new Mat(size, CvType.Cv8uc3);
            Mat blurred_img = new Mat();

            Imgproc.GaussianBlur(mat, mat, new Size(9, 9), 0);//3 3 0.8
            //Imgproc.Blur(mat, mat, new Size(5, 5));
            Imgproc.CvtColor(mat, gray, Imgproc.ColorBgr2gray);
            Imgproc.Canny(gray, mIntermediateMat, 50, 150);//80/130//50, 100

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
                    if (flage_tem == false) { 
                        response = await client.GetAsync(imageUrl);
                    }//////////////
                    else response = await client.GetAsync(imageUrl);

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
/*void ExportBitmapAsPNG(Bitmap imgView.Source)
        {
            var sdCardPath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            var filePath = System.IO.Path.Combine(sdCardPath, "test.png");
            var stream = new FileStream(filePath, FileMode.Create);
            bitmap.Compress(Bitmap.CompressFormat.Png, 100, stream);
            stream.Close();
        }*/

        /*MemoryStream stream = new MemoryStream();
image.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
System.IO.Path.Combine((string)Android.OS.Environment.ExternalStorageDirectory, "FolderPath");

var externalPath = global::Android.OS.Environment.ExternalStorageDirectory.Path + "/" +
global::Android.OS.Environment.DirectoryDownloads + "/" + fileName;
string a= DateTime.Now.ToString("yyyyMMddHHmmss" + ".jpg");
File.WriteAllBytes(a, e.ImageData);

var task = new FirebaseStorage(FirebaseConfig.StorageBucket,//YOUR-PROJECT-ID.appspot.com gs://YOUR-PROJECT-ID.appspot.com
new FirebaseStorageOptions
{
   ThrowOnCancel = true
})
.Child("photos")
.Child(a)
.PutAsync(imgView);

var downloadlink = task.GetDownloadUrlAsync();

//downloadLink.Text = downloadlink;
//傳url到firebase cloud storage //到realtime not cloud
if (downloadlink != null)
{
   const string id = "user1";
   MyDatabaseRecord t = new MyDatabaseRecord(id, downloadlink.Result.ToString());
   _ = firebaseClient.Child("URL").PostAsync(t);
   await DisplayAlert("上傳", "以選擇上傳", "OK");
}
}*/
/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearningXamarin.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CameraViewPage : ContentPage
    {
        public CameraViewPage()
        {
            InitializeComponent();
        }

        private void CaptureImage(object sender, EventArgs e)
        {
            xctCameraView.Shutter();
        }
        private void RecordVideo(object sender, EventArgs e)
        {
            xctCameraView.Shutter();
        }
        private void StopVideo(object sender, EventArgs e)
        {
            xctCameraView.Shutter();
        }

        private void Switch_Toggled(object sender, ToggledEventArgs e)
        {
            if (xctCameraView.CaptureMode == CameraCaptureMode.Photo)
            {
                captureMode.Text = "Video";
                xctCameraView.CaptureMode = CameraCaptureMode.Video;

                captureBtn.IsEnabled = false;
                btnrecordVideo.IsEnabled = true;
                btnstopVideo.IsEnabled = false;
            }
            else
            {
                captureMode.Text = "Photo";
                xctCameraView.CaptureMode = CameraCaptureMode.Photo;

                captureBtn.IsEnabled = true;
                btnrecordVideo.IsEnabled = false;
                btnstopVideo.IsEnabled = false;
            }
        }

        private void MediaCaptured(object sender, MediaCapturedEventArgs e)
        {

            imgView.Source = e.Image;
            imgViewPanel.IsVisible = true;
        }

        private void CloseImageView(object sender, EventArgs e)
        {
            imgViewPanel.IsVisible = false;
        }
    }
}*/