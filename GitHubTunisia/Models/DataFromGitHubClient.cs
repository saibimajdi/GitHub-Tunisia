using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Octokit;
using System.Threading.Tasks;

namespace GitHubTunisia.Models
{
    public class DataFromGitHubClient
    {
        private Octokit.GitHubClient _client;

        public DataFromGitHubClient()
        {
            _client = new GitHubClient(new Octokit.ProductHeaderValue("ee"));
            _client.Credentials = new Octokit.Credentials(Environment.GetEnvironmentVariable("GITHUB_TOKEN"));
        }
        
        

        public async Task<List<GithubUser>> GetUsers(int page = 1, int perPage = 100)
        {
            SearchUsersRequest request = new SearchUsersRequest("location:tunisia")
            {
                Location = "tunisia",
                AccountType = AccountSearchType.User
            };

            var result = await _client.Search.SearchUsers(request);

            List<GithubUser> users = new List<GithubUser>();
            
            foreach(var user in result.Items)
            {
                var githubUser = new GithubUser()
                {
                    UserInfo = await GetFullUserInformation(user.Login),
                    EventsNumber = await GetUserEventsNumber(user.Login)
                };

                githubUser.Score = GetUserScore(user.Followers, user.PublicRepos, githubUser.EventsNumber);
                    
                users.Add(githubUser);
            }

            return users;
        }

        public async Task<int> GetUserEventsNumber(string login)
        {
            ApiOptions apiOptions = new ApiOptions()
            {
                PageCount = 100,
                PageSize = 100,
                StartPage = 1
            };

            var activities = await _client.Activity.Events.GetAllUserPerformed(login, apiOptions);

            return activities.Count;
        }

        public async Task<Octokit.User> GetFullUserInformation(string login) => await _client.User.Get(login);

        private int GetUserScore(int followers, int publicRepos, int events) => (int)(0.1 * followers + 0.1 * publicRepos + 0.8 * events) * 10;
    }

    public class GithubUser 
    {
        public Octokit.User UserInfo { get; set; }
        public int EventsNumber { get; set; }
        public int Score { get; set; }
        
    }
}