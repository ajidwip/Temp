using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;

[assembly: OwinStartup(typeof(SignalRRealTimeSQL.Startup))]

namespace SignalRRealTimeSQL
{
    public class Startup
    {
        int humidity;
        int temparature, temparaturebefore;
        DateTime date;
        public void Configuration(IAppBuilder app)
        {
            GetData();
        }
        public void GetData()
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DataBase"].ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(@"SELECT [ID],[humidity],[temparature],[date] FROM [dbo].[TableTemp] ORDER BY ID DESC", connection))
                {
                    // Make sure the command object does not already have
                    // a notification object associated with it.
                    command.Notification = null;
                    SqlDependency.Start(ConfigurationManager.ConnectionStrings["DataBase"].ConnectionString);
                    SqlDependency dependency = new SqlDependency(command);
                    dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);

                    if (connection.State == System.Data.ConnectionState.Closed)
                        connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    var listData = reader.Cast<IDataRecord>()
                          .Select(x => new Temparature()
                          {
                              ID = x.GetInt32(0),
                              humidity = x.GetInt32(1),
                              temparature = x.GetInt32(2),
                              date = x.GetDateTime(3)
                          }).ToList();
                    humidity = listData[0].humidity;
                    temparature = listData[0].temparature;
                    temparaturebefore = listData[1].temparature;
                    date = listData[0].date;
                }
            }
        }
        private void dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            GetData();
            if (temparature > 24 && temparature != temparaturebefore)
            {
                var appid = "AAAALtOumZU:APA91bEa--eJdP0bEyipzqRHyrC5zKUotImjLh5PeMyeOJmgBnUnipwioVh56wHoRuAeHy5X2OGiX7I_zfBMj2yYouasL6zlarL63xipDctdVhq-DNd2YVXtuBfd7HYDteWTH9CettyO";
                var senderid = "201119930773";
                net.azurewebsites.customerpointwsbackup.BasicHttpBinding_IService1 notif = new net.azurewebsites.customerpointwsbackup.BasicHttpBinding_IService1();

                notif.PushNotification(appid, senderid, "/topics/all", "Sistem Peringatan Ruang Server", "Temperature : "+ temparature + "℃ , Humidity :" + humidity + "%");
            }
        }
    }
}
