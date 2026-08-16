using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static App1.Views.CameraViewPage;

namespace App1
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class Finish : ContentPage
    {
        public Finish()
        {
            InitializeComponent();
            //upload();一次上傳
            Checked();
        }
        FirebaseClient firebaseClient = new Firebase.Database.FirebaseClient(FirebaseConfig.DatabaseUrl);
        FirebaseStorage firebaseStorage = new FirebaseStorage(FirebaseConfig.StorageBucket);

        private void Checked() {
            //確認每個點是否執行完畢
            string a = "";
            
            for (int i = 0; i < 3; i++) {//修
                if (Transpoter.pass[i, 0] == false || Transpoter.pass[i, 1] == false) {
                        a += "第" + (i + 1) + "巡邏點";
                    if (Transpoter.pass[i, 0] == false)
                        a += "GPS未檢驗";
                    if (Transpoter.pass[i, 0] == false && Transpoter.pass[i, 0] == false)
                        a += " ; ";
                    if (Transpoter.pass[i, 1] == false)
                        a += "照片未拍";
                    a += "\n";
                }
            }
            if (a == "") a = "未漏掉任一步驟，成功上傳";
        
            show.Text = a;
        }

        public string ShowImage { get; set; }
        string[] placenum = { "place1", "place2", "place3" };
        string[] point = { "one", "two", "three" };
        int[] buf = { 0, 0, 0 };
        private async void upload() {
            DateTime dateToStore = DateTime.Now.AddDays(0);
            string DDate = dateToStore.ToString("yyyy-MM-dd");
            string a = DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg";

            //string put = $"\"{Transpoter.place1Value[Transpoter.step - 1]},{a}\"";// (Transpoter.place1Value[Transpoter.step - 1] + ',' + a).ToString();
            /*一次上傳
            string temp = "{";
            for (int i = 0; i < 3; i++) {
                if (Transpoter.photo[i] != "") {
                    temp += Transpoter.photo[i];//"\"" + placenum[i] + "\":" + Transpoter.photo[i];
                    
                    if (Transpoter.pass[i, 1] && Transpoter.pass[i+1,1]) { temp += ","; }
                   
                }
            }
            temp += "}";

            await firebaseClient.Child("daily_data").Child("controller").Child(DDate).Child(placenum[Transpoter.step - 1]).PutAsync((temp).ToString());
            var res = await DisplayAlert("上傳", "已選擇上傳", "OK", "Cancel");
            */
            /*var res =  await DisplayAlert("上傳", "已選擇上傳", "OK", "Cancel");
            if (res == true)
            {
                Transpoter.pass[Transpoter.step - 1, 1] = true;
                //                       await Navigation.PushModalAsync(new Page1());
                //await Navigation.PopAsync();
                //await this.Navigation.PopAsync();
                //Navigation.InsertPageBefore(new Page1(), this);
            }*/
        }
        private void back(object sender, EventArgs e)
        {
            //Navigation.PushModalAsync(new MainPage());//回主畫面
            Navigation.PopToRootAsync();

            /*string a = DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg";
            var task = new FirebaseStorage(FirebaseConfig.StorageBucket,//YOUR-PROJECT-ID.appspot.com gs://YOUR-PROJECT-ID.appspot.com
            new FirebaseStorageOptions
            {
                ThrowOnCancel = true
            })
            .Child("photos")
            .Child(a)
            .PutAsync(stream);

            var downloadlink = task.TargetUrl;
            if (downloadlink != null)
            {
                //在結束時才上傳
                const string id = "user1";
                MyDatabaseRecord t = new MyDatabaseRecord(id, downloadlink.ToString());
                _ = firebaseClient.Child("URL").PostAsync(t);
                await DisplayAlert("上傳", "以選擇上傳", "OK");
            }*/

        }
    }
}