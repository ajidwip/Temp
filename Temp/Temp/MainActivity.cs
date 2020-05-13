using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Firebase.Messaging;
using Firebase.Iid;
using Android.Gms.Common;
using Android.Util;
using Android.Support.V4.Content;
using System;
using Android.Support.V4.App;
using Android.Content.PM;
using System.Data;
using System.Timers;

namespace Temp
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        int requestPermissions;
        string locationPermission = Android.Manifest.Permission.AccessCoarseLocation;
        string locationfinePermission = Android.Manifest.Permission.AccessFineLocation;
        string wifiPermission = Android.Manifest.Permission.AccessWifiState;
        string changewifiPermission = Android.Manifest.Permission.ChangeWifiState;
        string internetPermission = Android.Manifest.Permission.Internet;
        static TextView temp, humidity, update;
        static LinearLayout background;
        static Timer timer;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            temp = FindViewById<TextView>(Resource.Id.temp);
            humidity = FindViewById<TextView>(Resource.Id.humidity);
            update = FindViewById<TextView>(Resource.Id.update);
            background = FindViewById<LinearLayout>(Resource.Id.background);
            if (!(ContextCompat.CheckSelfPermission(this, locationPermission) == (int)Permission.Granted))
            {
                ActivityCompat.RequestPermissions(this, new String[] { locationPermission, locationfinePermission, wifiPermission, changewifiPermission, internetPermission }, requestPermissions);
            }
            FirebaseMessaging.Instance.SubscribeToTopic("all");
            IsPlayServicesAvailable();
            timer = new System.Timers.Timer();
            timer.Interval = 3000;
            timer.Enabled = true;
            timer.Elapsed += (sender, args) =>
            {
                RunOnUiThread(() =>
                {
                    GetData();
                });

            };
            GetData();
        }
        public bool IsPlayServicesAvailable()
        {
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (resultCode != ConnectionResult.Success)
            {

                if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
                {
                    //msgText.Text = GoogleApiAvailability.Instance.GetErrorString(resultCode);
                }

                else
                {
                    // msgText.Text = "This device is not supported";
                    Finish();
                }
                return false;
            }
            else
            {
                // do whatever if play service is not available
                //msgText.Text = "Google Play Services is available.";
                return true;
            }
        }
        private static void GetData()
        {
            DataTable dttemp = new DataTable();
            WebReference.BasicHttpsBinding_IService1 MyClient = new WebReference.BasicHttpsBinding_IService1();
            WebReference.Temp tempdata = new WebReference.Temp();
            tempdata = MyClient.GetTemp();
            dttemp = tempdata.Temptable;

            if (dttemp.Rows[0][0].ToString() != "")
            {
                temp.Text = dttemp.Rows[0][2].ToString();
                humidity.Text = dttemp.Rows[0][1].ToString();
                update.Text = "Update on " + dttemp.Rows[0][3].ToString();
                int suhu = Int32.Parse(dttemp.Rows[0][2].ToString());
                if (suhu > 23)
                {
                    background.SetBackgroundResource(Resource.Drawable.orange);
                }
                else
                {
                    background.SetBackgroundResource(Resource.Drawable.green);
                }
            }
        }
    }
}