using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Octokit;
using System.Configuration;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

namespace GitHubTunisia.Controllers
{
    public class HomeController : Controller
    {
        [OutputCache(Duration = 1800, VaryByParam = "none", Location = System.Web.UI.OutputCacheLocation.Server)]
        public async Task<ActionResult> Index() 
        {
            var customClient = new Models.HttpClient();

            var usersInformation = await customClient.GetUsersInformation(perPage: 10);

            return View(usersInformation);
        }

        [OutputCache(Duration = 1800, VaryByParam = "none", Location = System.Web.UI.OutputCacheLocation.Server)]
        public async Task<ActionResult> Repos()
        {
            var client = new GitHubClient(new Octokit.ProductHeaderValue("github-tunisia"));

            var token = Environment.GetEnvironmentVariable("GITHUB_TOKEN");

            if (token == null)
                throw new Exception("Invalid GitHub access token, please set an environment variable called GITHUB_TOKEN with containing a valid token!");
            
            //client.Credentials = new Credentials(ConfigurationManager.AppSettings["GitHubToken"]);

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

        //[OutputCache(Duration = 1800, VaryByParam = "none", Location = System.Web.UI.OutputCacheLocation.Server)]
        public async Task<ActionResult> Coders(int p = 1)// p = page
        {
            var customClient = new Models.HttpClient();

            p = (p < 1 || p > 1500) ? 1 : p;

            ViewBag.page = p;

            var usersInformation = await customClient.GetUsersInformation(p, 100);
            
            return View(usersInformation);
        }

        // only for test!!!!
        public async Task<ActionResult> q()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://api.github.com");

            var token = Environment.GetEnvironmentVariable("GITHUB_TOKEN");

            if (token == null)
                throw new Exception("Invalid GitHub access token, please set an environment variable called GITHUB_TOKEN with containing a valid token!");

            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"http://api.github.com/search/users?q=+location:tunisia&per_page=200"),
                Method = HttpMethod.Get,
            };
            
            request.Headers.Add("User-Agent", "github-tunisia");

            request.Headers.Authorization = new AuthenticationHeaderValue("token", token);

            var response = await client.SendAsync(request);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                return Json("not found", JsonRequestBehavior.AllowGet);

            var jsonContent = await response.Content.ReadAsStringAsync();

            Models.GitHubUsersResponse gitHubResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.GitHubUsersResponse>(jsonContent);
            //List<Models.User> users = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Models.User>>(jsonContent);
            
            foreach(var user in gitHubResponse.items)
            {
                var userRequest = new HttpRequestMessage()
                {
                    RequestUri = new Uri(user.url),
                    Method = HttpMethod.Get
                };

                userRequest.Headers.Add("User-Agent", "github-tunisia");
                userRequest.Headers.Authorization = new AuthenticationHeaderValue("token", token);

                var userResponse = await client.SendAsync(userRequest);
                var userJsonContent = await userResponse.Content.ReadAsStringAsync();
                user.Information = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.UserInformation>(userJsonContent);
            }

            return Json(gitHubResponse.items, JsonRequestBehavior.AllowGet);
        }
    }
}