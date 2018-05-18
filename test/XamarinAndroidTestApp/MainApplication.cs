using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using Plugin.CurrentActivity;

namespace XamarinAndroidTestApp
{
    [Application]
    public class MainApplication : Application
    {
        public MainApplication(IntPtr handle, JniHandleOwnership transer)
          : base(handle, transer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
            CrossCurrentActivity.Current.Init(this);
        }
    }
}