using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Timers;
using Microsoft.Web.Administration;

namespace WebAPI_QM
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        private static System.Timers.Timer aTimer;

        private static void SetTimer()
        {
            // Create a timer with a two seconds interval.
            aTimer = new System.Timers.Timer(2000);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            DateTime RefuseTime = DateTime.Parse(ConfigurationManager.AppSettings["RefuseTime"]);

            if (DateTime.Now >= RefuseTime)
            {
                string sql = "select Requesting from RequestCounter";
                int Requsting = (int)Common.SQLHelper.ExecuteScalarToObject(Common.SQLHelper.Asset_strConn, CommandType.Text, sql, null);

                if (Requsting == 0)
                {
                    var server = new ServerManager();
                    var site = server.Sites.FirstOrDefault(s => s.Name == "GageManagerServer");
                    if (site != null)
                    {
                        //stop the site...
                        site.Stop();
                        //Environment.Exit(0);
                    }
                }
            }
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            //SetTimer();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);



            string sql = @"update RequestCounter set Requesting = 0";
            int a = Common.SQLHelper.ExecuteNonQuery(Common.SQLHelper.Asset_strConn, CommandType.Text, sql);
        }


        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            DateTime RefuseTime = DateTime.Parse(ConfigurationManager.AppSettings["RefuseTime"]);

            if (DateTime.Now < RefuseTime)
            {
                string sql = @"update RequestCounter set Requesting += 1";
                Common.SQLHelper.ExecuteNonQuery(Common.SQLHelper.Asset_strConn, CommandType.Text, sql);

            }
            else
                throw new HttpResponseException(HttpStatusCode.BadRequest);
        }


        protected void Application_EndRequest(object sender, EventArgs e)
        {
            if (HttpContext.Current.Request.HttpMethod == "OPTIONS")
            {
                HttpContext.Current.Response.StatusCode = 200;
            }

            string sql = @"update RequestCounter set [Requesting] -= 1  where [Requesting] > 0";
            Common.SQLHelper.ExecuteNonQuery(Common.SQLHelper.Asset_strConn, CommandType.Text, sql);
        }
    }
}
