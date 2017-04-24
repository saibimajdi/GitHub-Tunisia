using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace GitHubTunisia
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected async void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //await InitDataAsync();
        }

        public static async Task InitDataAsync()
        {
            if (Models.Data.UsersInformation != null)
                return;

            Models.Client client = new Models.Client();
            Models.Data.UsersInformation = await client.GetAllUsersInformation();
            Models.Data.UsersInformation.Sort((user1, user2) => user2.score - user1.score);
        }
    }
}
