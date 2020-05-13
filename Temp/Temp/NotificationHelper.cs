using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Temp;
using Temp.Droid;
using Xamarin.Forms;

namespace Temp.Droid
{
    class NotificationHelper : INotification
    {
        private NotificationCompat.Builder mBuilder;
        public static String NOTIFICATION_CHANNEL_ID = "10023";

        public void CreateNotification(String title, String message)
        {
            try
            {
                var intent = new Intent(global::Android.App.Application.Context, typeof(MainActivity));
                intent.AddFlags(ActivityFlags.ClearTop);
                intent.PutExtra("title", message);
                var pendingIntent = PendingIntent.GetActivity(global::Android.App.Application.Context, 0, intent, PendingIntentFlags.OneShot);

                var sound = global::Android.Net.Uri.Parse(ContentResolver.SchemeAndroidResource + "://" + global::Android.App.Application.Context.PackageName + "/" + Resource.Raw.alert);
                // Creating an Audio Attribute
                var alarmAttributes = new AudioAttributes.Builder()
                    .SetContentType(AudioContentType.Sonification)
                    .SetUsage(AudioUsageKind.Notification).Build();

                mBuilder = new NotificationCompat.Builder(global::Android.App.Application.Context);
                mBuilder.SetSmallIcon(Resource.Drawable.abc);
                mBuilder.SetContentTitle(title)
                        .SetSound(sound)
                        .SetAutoCancel(true)
                        .SetContentTitle(title)
                        .SetContentText(message)
                        .SetChannelId(NOTIFICATION_CHANNEL_ID)
                        .SetPriority((int)NotificationPriority.High)
                        .SetVibrate(new long[0])
                        .SetDefaults((int)NotificationDefaults.Sound | (int)NotificationDefaults.Vibrate)
                        .SetVisibility((int)NotificationVisibility.Public)
                        .SetSmallIcon(Resource.Drawable.abc)
                        .SetContentIntent(pendingIntent);



                NotificationManager notificationManager = global::Android.App.Application.Context.GetSystemService(Context.NotificationService) as NotificationManager;
 
                if (global::Android.OS.Build.VERSION.SdkInt >= global::Android.OS.BuildVersionCodes.O)
                {
                    NotificationImportance importance = global::Android.App.NotificationImportance.High;

                    NotificationChannel notificationChannel = new NotificationChannel(NOTIFICATION_CHANNEL_ID, title, importance);
                    notificationChannel.EnableLights(true);
                    notificationChannel.EnableVibration(true);
                    notificationChannel.SetSound(sound, alarmAttributes);
                    notificationChannel.SetShowBadge(true);
                    notificationChannel.Importance = NotificationImportance.High;
                    notificationChannel.SetVibrationPattern(new long[] { 100, 200, 300, 400, 500, 400, 300, 200, 400 });

                    if (notificationManager != null)
                    {
                        mBuilder.SetChannelId(NOTIFICATION_CHANNEL_ID);
                        notificationManager.CreateNotificationChannel(notificationChannel);
                    }
                }

                notificationManager.Notify(0, mBuilder.Build());
            }
            catch (Exception ex)
            {
                //
            }
        }
    }
}