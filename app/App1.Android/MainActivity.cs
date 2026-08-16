
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using OpenCV.Android;
using Android.Util;
using Android.Content;

namespace App1.Droid
{//Main
    [Activity(Label = "App1", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public const string TAG = "OCVSample::Activity";
        private BaseLoaderCallback mLoaderCallback;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            //
            mLoaderCallback = new Callback(this, this);
            if (!OpenCVLoader.InitDebug())
            {
                Log.Debug(TAG, "Internal OpenCV library not found. Using OpenCV Manager for initialization");
                OpenCVLoader.InitAsync(OpenCVLoader.OpencvVersion300, this, mLoaderCallback);
            }
            else
            {
                Log.Debug(TAG, "OpenCV library found inside package. Using it!");
                mLoaderCallback.OnManagerConnected(LoaderCallbackInterface.Success);
            }
            //
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

            
        }
        class Callback : BaseLoaderCallback
        {
            private readonly MainActivity _activity;
            public Callback(Context context, MainActivity activity)
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
                            Log.Info(MainActivity.TAG, "OpenCV loaded successfully");

                            // Load native library after(!) OpenCV initialization
                            //JavaSystem.LoadLibrary("mixed_sample");

                            //_activity.mOpenCvCameraView.EnableView();
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
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}