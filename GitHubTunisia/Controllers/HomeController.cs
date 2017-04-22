using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Octokit;
using System.Configuration;
using System.Threading.Tasks;
using System.Net.Http;

namespace GitHubTunisia.Controllers
{
    public class HomeController : Controller
    {
        [OutputCache(Duration = 1800, VaryByParam = "none", Location = System.Web.UI.OutputCacheLocation.ServerAndClient)]
        public async Task<ActionResult> Index()
        {
            var client = new GitHubClient(new ProductHeaderValue("github-tunisia"));
            client.Credentials = new Credentials(ConfigurationManager.AppSettings["GitHubToken"]);

            var searchUserRequest = new SearchUsersRequest("location:tunisia")
            {
                Location = "Tunisia",
                SortField = UsersSearchSort.Followers
            };

            var searchResult = await client.Search.SearchUsers(searchUserRequest);

            List<Octokit.User> users = new List<Octokit.User>();

            foreach (var item in searchResult.Items)
                users.Add(await client.User.Get(item.Login));

            return View(users.Take(10).ToList());
        }

        [OutputCache(Duration = 1800, VaryByParam = "none", Location = System.Web.UI.OutputCacheLocation.ServerAndClient)]
        public async Task<ActionResult> Repos()
        {
            var client = new GitHubClient(new ProductHeaderValue("github-tunisia"));
            client.Credentials = new Credentials(ConfigurationManager.AppSettings["GitHubToken"]);

            var searchReposRequest = new SearchRepositoriesRequest("location:tunisia")
            {
                SortField = RepoSearchSort.Stars,
            };

            var searchResult = await client.Search.SearchRepo(searchReposRequest);

            List<Octokit.Repository> repos = new List<Repository>();

            foreach (var repo in searchResult.Items)
                repos.Add(await client.Repository.Get(repo.Id));

            return View(repos);
        }

        [OutputCache(Duration = 1800, VaryByParam = "none", Location = System.Web.UI.OutputCacheLocation.ServerAndClient)]
        public ActionResult Coders()
        {
            var client = new GitHubClient(new ProductHeaderValue("github-tunisia"));
            client.Credentials = new Credentials(ConfigurationManager.AppSettings["GitHubToken"]);

            var searchUserRequest = new SearchUsersRequest("location:tunisia")
            {
                Location = "Tunisia",
                SortField = UsersSearchSort.Followers
            };

            var searchResult = client.Search.SearchUsers(searchUserRequest).Result;

            List<Octokit.User> users = new List<Octokit.User>();

            foreach (var item in searchResult.Items)
                users.Add(client.User.Get(item.Login).Result);

            return View(users);
        }

        // only for test!!!!
        public async Task<ActionResult> q()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://api.github.com");

            var response = await client.GetAsync("/search/users?q=+location:tunisia&per_page=99");

            var jsonContent = await response.Content.ReadAsStringAsync();

            List<Models.User> users = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Models.User>>(jsonContent);

            

            return View();
        }
    }
}