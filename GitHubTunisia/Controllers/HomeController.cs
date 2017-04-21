using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Octokit;
using System.Configuration;
using System.Threading.Tasks;

namespace GitHubTunisia.Controllers
{
    public class HomeController : Controller
    {
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
    }
}