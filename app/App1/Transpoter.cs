using Java.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Firebase.Database.Query;
using Firebase.Database;
using System.Linq;
namespace App1
{
    class Transpoter
    {
        FirebaseClient firebaseClient = new FirebaseClient(FirebaseConfig.DatabaseUrl);
        public static bool show_once=true;
        public static int step=1;
        public static string []photo = { "", "", "" };//存照片

        public static string[] place1Value = { "abc", "def", "ghi" };//1_1...
        string[] placenum = { "place1", "place2", "place3" };

        public static string[] point = { "onec","twoc","threec"};//
        public static string[] image_compare = { "1_1.jpg", "2_1.jpg", "3_1.jpg" ,"",""};//
        public static string[] image_template = { "t1_1.jpg", "t2_1.jpg", "t3_1.jpg" ,"",""};//
        public static double[,] place = new double[5,2];//每個的location


        public static bool[,] pass = new bool[5,2];
        public static bool[] pass_each = { false, false};//每頁的兩個func
        public static string[] url = new string[5];//url 樣本圖
        public static string[,] url_user = new string[5, 3];//資料

        public static Stream[] stream1 = new Stream[5];
        //[][] url = new string[5][];//點加url

        /* 24.9702637,121.2633297 門口
         * 24.9703242,121.2664810 一館
         * 24.96979,121.26709 紅
         * 24.96924,121.26749 二一石
         */
        public static void Get_from_db()
        {//從firebase 取值 -Main page
            //再修 座標要依照動態規劃的去取//
            double[] x = { 24.9702637, 121.2633297, 24.96979, 121.26709, 24.96924, 121.26749 };
            for (int i = 0,j=0; i < 6; i+=2,j++)
            {
                place[j, 0] = x[i];
                place[j, 1] = x[i + 1];
            }
        }
        public static void next_page() {
            //存gps photo 有無過
            pass[step - 1, 0] = pass_each[0];
            pass[step - 1, 1] = pass_each[1];
            pass_each[0] = false;
            pass_each[1] = false;
        }
        //public static Map a;//各點

        public void Get_place()
        {

            DateTime dateToStore = DateTime.Now.AddDays(0);
            string Date = dateToStore.ToString("yyyy-MM-dd");
            int times = 1;
            for (; times <= 3; times++)
            {
                var place1Ref = firebaseClient.Child("daily_data").Child("user").Child(Date).Child(placenum[times - 1]);//Child(placenum[times - 1]);

                place1Ref.OnceSingleAsync<string>().ContinueWith(async (task) =>
                {
                    if (!task.IsFaulted && !task.IsCanceled)
                    {
                        place1Value[times - 1] = task.Result;
                        string temp = task.Result;
                        
                    }
                }).Wait();
            }
        }
        public async void Get_place_pt()
        {

            DateTime dateToStore = DateTime.Now.AddDays(0);
            string Date = dateToStore.ToString("yyyy-MM-dd");
            var temp = firebaseClient
       .Child("daily_data").Child("user").Child(Date).Child("place1");

            if (temp.OnceSingleAsync<string>().Result == null)
            {
                string a = null;
                string b = null;
                string c = null;

                string[] temp111 = { a, b, c };

                int finish = 0;

                System.Random random = new System.Random();
                int[] numbers = Enumerable.Range(1, 3).ToArray();



                for (int i = 0; i < 3; i++)
                {
                    int swapIndex = random.Next(i, 3);
                    int tempt = numbers[i];
                    numbers[i] = numbers[swapIndex];
                    numbers[swapIndex] = tempt;
                }
                numbers.Take(3).ToArray();
                //DisplayAlert( Convert.ToString(numbers[0]), Convert.ToString(numbers[1]), Convert.ToString(numbers[2]));

                for (int i = 0; i < 3; i++)
                {
                    temp111[i] = Convert.ToString(numbers[i]);
                    int which = random.Next(1, 4);
                    temp111[i] += '_';
                    temp111[i] += Convert.ToString(which);
                }

                //DisplayAlert(temp111[0], temp111[1], temp111[2]);


                var data = new
                {
                    place1 = temp111[0],
                    place2 = temp111[1],
                    place3 = temp111[2]
                };
                place1Value[0] = temp111[0];
                place1Value[1] = temp111[1];
                place1Value[2] = temp111[2];
            }
            else
            {
                Get_place();
                int i = 0;
            
            }

            // Get_place();

        }
    }
}
