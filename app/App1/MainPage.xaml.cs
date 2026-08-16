using Firebase.Storage;
using Firebase.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
using Firebase.Database.Query;

namespace App1
{

    public partial class MainPage : ContentPage//ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            Transpoter.Get_from_db();///
            var mc = new Transpoter();
            mc.Get_place_pt();
        }
        /*
        private void XamarinCommunityToolkits(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new CommunityToolkitPage());
        }

        private void XamarinEssentials(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new XamarinEssentialsPage());
        }
        */
        private void Page1(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Page1());//
        }
        /*
FirebaseClient firebaseClient = new Firebase.Database.FirebaseClient(FirebaseConfig.DatabaseUrl);
public struct MyDatabaseRecord
{
public string userid;
private string path;
public String Path
{
get { return path; }//讀取屬性
set { path = value; }//屬性指派值,執行
}
public MyDatabaseRecord(String id, String path1) { path = path1; userid = id; }
}

public struct user
{
public string userid;
}
public FileResult result { get; set; }
public string A { get; set; }
async private void TakePhoto_Clicked(object sender, EventArgs e)
{
result = await MediaPicker.CapturePhotoAsync();
if (result != null)
{
var stream = await result.OpenReadAsync();//實際路徑
image.Source = ImageSource.FromStream(() => stream);

}
}

async private void UploadPhoto_Clicked(object sender, EventArgs e)
{
if (result == null)
return;

var task = new FirebaseStorage(FirebaseConfig.StorageBucket,//YOUR-PROJECT-ID.appspot.com gs://YOUR-PROJECT-ID.appspot.com
new FirebaseStorageOptions
{
ThrowOnCancel = true
})
.Child("photos")
.Child(result.FileName)
.PutAsync(await result.OpenReadAsync());

var downloadlink = await task;

//downloadLink.Text = downloadlink;
//傳url到firebase cloud storage //到realtime not cloud
if (downloadlink != "")
{
const string id = "user1";
MyDatabaseRecord t = new MyDatabaseRecord(id, downloadlink);
_ = firebaseClient.Child("URL").PostAsync(t);
await DisplayAlert("上傳", "以選擇上傳", "OK");
}
}

async private void PickPhoto_Clicked(object sender, EventArgs e)
{
result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
{
Title = "Please pick a photo"
});

if (result != null)
{
var stream = await result.OpenReadAsync();
image.Source = ImageSource.FromStream(() => stream);

}
}*/

    }
}
