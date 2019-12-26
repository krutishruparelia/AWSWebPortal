using System;
using System.IO;
using System.Timers;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace AWS
{

    public class MvcApplication : HttpApplication
    {
        public static System.Timers.Timer aTimer;
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            DevExtremeBundleConfig.RegisterBundles(BundleTable.Bundles);
            int period = 60000; // 10 minutes
            aTimer = new System.Timers.Timer(period);//20 min
            aTimer.Elapsed += new ElapsedEventHandler(RunThis);
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }
        private static void RunThis(object source, ElapsedEventArgs e)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to. 
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine("Called");
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine("Called 1");
                }
            }
        }
        private void TimerCallback(object state)
        {

            // Do your stuff here
        }
    }
}
