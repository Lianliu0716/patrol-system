using System;
using static App1.Page1;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using App1;
using App1.Droid;

[assembly: ExportRenderer(typeof(Button), typeof(CustomButton))]
namespace App1.Droid
{
	public class CustomButton : ButtonRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
		{
			base.OnElementChanged(e);

			if (Control != null)
			{

			}
		}
	}
}