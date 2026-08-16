/*using Syncfusion.XForms.ProgressBar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App1
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Page1 : ContentPage
    {
        public Page1()
        {
            InitializeComponent();
            SfStepProgressBar stepProgressBar = new SfStepProgressBar();
            stepProgressBar.VerticalOptions = LayoutOptions.Center;
            stepProgressBar.HorizontalOptions = LayoutOptions.Center;

            stepProgressBar.Children.Add(new StepView() { PrimaryText = "Step 1" });
            stepProgressBar.Children.Add(new StepView() { PrimaryText = "Step 2" });
            stepProgressBar.Children.Add(new StepView() { PrimaryText = "Step 3" });
            stepProgressBar.Children.Add(new StepView() { PrimaryText = "Step 4", Status = StepStatus.InProgress });
            stepProgressBar.Children.Add(new StepView() { PrimaryText = "Step 5" });
        }
    }


}
*/ using Android.OS;
using App1.Views;
using Firebase.Database;
using Firebase.Storage;
using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace App1
{
    public partial class Page1 : ContentPage
    {
        string[] placenum = { "place1", "place2", "place3" };
        string[] point = { "one", "two", "three" };
        int[] buf = { 0, 0, 0 };
        public Page1()
        {
            InitializeComponent();
            for (int i = 0; i < 3; i++)
            {
                buf[i] = Transpoter.place1Value[i][0] - '0';
            }
            if (Transpoter.show_once) {
                DisplayAlert("巡邏開始", "請點第一個點(上方)，並按照順序進行", "ok");
                Transpoter.show_once = false;
            }
        }

        //private bool flag_gps, flage_photo = false;//判斷兩個都通顧
            FirebaseClient firebaseClient = new Firebase.Database.FirebaseClient(FirebaseConfig.DatabaseUrl);
            FirebaseStorage firebaseStorage = new FirebaseStorage(FirebaseConfig.StorageBucket);


        private void get_pic() {
            ///顯示樣本圖片
            Uri uri;//從firebase拿的
                    //string[] point = { "one" };//巡邏點

            //DisplayAlert("a", Transpoter.step.ToString() , "ok");

            //firebaseStorage.Child("patrol_dataset").Child(Transpoter.point[Transpoter.step - 1]).Child(Transpoter.image_compare[Transpoter.step - 1]).GetDownloadUrlAsync().ContinueWith(async (task) =>
            firebaseStorage.Child("patrol_dataset").Child(point[buf[Transpoter.step - 1] - 1] + "c").Child(Transpoter.place1Value[Transpoter.step - 1] + ".jpg").GetDownloadUrlAsync().ContinueWith(async (task) => {
                if (!task.IsFaulted && !task.IsCanceled)
                {
                    //System.Uri uri;
                    System.Uri.TryCreate(task.Result.ToString(), UriKind.Absolute, out uri);
                    //Image a = new Image { Source = uri };
                    //this.image = a;
                    this.image.Source = ImageSource.FromUri(uri);
                    Transpoter.url[Transpoter.step - 1] = uri.ToString();//樣本url
                    
                }
            });
           
        }
        int GoalProgress { get; set; }
        int step { get; set; }
        private async void stepBar_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

            sender.ToString();
            string a=e.PropertyName;
            if (a == "StepSelected")
            {
                if (stepBar.StepSelected == 4)//跑到finish
                {
                    await Navigation.PushAsync(new Finish());
                }
                else
                {
                    //Transpoter.next_page();//紀錄儲存
                    Transpoter.step = stepBar.StepSelected;//點第幾個step
                    get_pic();

                }
            }
        }
        /*拍照頁面 1*/
        async private  void takephoto(object sender, EventArgs e)
        {
             await Navigation.PushAsync(new CameraViewPage());
        }

        /*gps計算頁面 0*/
        async private void gps(object sender, EventArgs e)//得到使用者位置
        {
            var position = await Xamarin.Essentials.Geolocation.GetLocationAsync();
            int a=Transpoter.place1Value[Transpoter.step - 1][0]-'0';
            Transpoter.pass[Transpoter.step-1,0] = calculate_location(position, Transpoter.place[a-1, 0], Transpoter.place[a-1, 1]);//再修//
            //await Map.OpenAsync(47.6529, -122.1422);//開啟map看目前位置
        }

        private bool calculate_location(Location location,double a, double b) {//計算兩點距離
            //Location place1 = new Location(42.358056, -71.063611);//緯度與經度
            Location place1 = new Location(a,b);//緯度與經度
            double km = Location.CalculateDistance(location, place1, DistanceUnits.Kilometers);
            km *= 1000;//變m
            if (km > 50)
            { //範圍500m?
                DisplayAlert("未通過",  "巡邏點距"+ decimal.Round((decimal)km, 2, MidpointRounding.AwayFromZero)+"公尺。未在範圍內，請重新嘗試", "cancel");
                //未通過
                return false;
            }
            else { //通過
                DisplayAlert("通過", "巡邏點距" + decimal.Round((decimal)km, 2, MidpointRounding.AwayFromZero) + "公尺。於範圍內", "ok");
                return true;
            }
        }


    }
}

