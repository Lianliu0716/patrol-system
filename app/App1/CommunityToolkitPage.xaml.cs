using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using App1.Views;

namespace App1
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CommunityToolkitPage : ContentPage
    {
        public int Step{get;}
        public CommunityToolkitPage()//int step
        {
            InitializeComponent();
            //Step = step;
        }

        private void NavigateCameraViewPage(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new CameraViewPage());
        }
    }
}