using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Hangfire;
using LechTyper.Repository;
using LechTyper.Models;
using LechTyper.Controllers;


[assembly: OwinStartup(typeof(LechTyper.App_Start.OWINStartup))]

namespace LechTyper.App_Start
{
    public class OWINStartup
    {
        private HomeController _homecontroller;
        public void Configuration(IAppBuilder app)
        {
            GlobalConfiguration.Configuration.UseSqlServerStorage("DefaultConnection");
            

            app.UseHangfireDashboard();
            app.UseHangfireServer();
            _homecontroller = new HomeController();
            RecurringJob.AddOrUpdate("LechTyperPlay", () => _homecontroller.Play(), Cron.Daily(0, 5));
        }
    }
}
