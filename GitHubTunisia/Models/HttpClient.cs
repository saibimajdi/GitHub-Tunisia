using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace GitHubTunisia.Models
{
    public class Client
    {
        private System.Net.Http.HttpClient _client { get; set; }

        private readonly string _baseUrlForTunisianCoders = "https://api.github.com/search/users?q=+location:tunisia";

        // get the token from the Environment Variable
        private readonly string _token = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
        
        public async Task<List<Models.UserInformation>> GetUsersInformation(int page = 1, int perPage = 100)
        {
            List<Models.UserInformation> usersInformation = new List<UserInformation>();

            _client = new System.Net.Http.HttpClient();

            using (HttpRequestMessage request = new HttpRequestMessage()
                    {
                        Method = HttpMethod.Get,
                        RequestUri = new Uri($"{_baseUrlForTunisianCoders}&per_page={perPage}&page={page}")
                    })
            {
                // add the required 'User-Agent' required header
                request.Headers.Add("User-Agent", "github-tunisia");

                // add the token to the request headers
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("token", _token);

                // send the http request and get the result
                var response = await _client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    // get the data from the responses
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    
                    // deserialize the response data
                    Models.GitHubUsersResponse githubResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<GitHubUsersResponse>(jsonContent);

                    if(githubResponse != null && githubResponse.items != null)
                    {
                        // foreach user we need to get all his information, so we should send an http request for each user!
                        foreach(var user in githubResponse.items)
                        {
                            var userRequest = new HttpRequestMessage()
                            {
                                RequestUri = new Uri(user.url),
                                Method = HttpMethod.Get
                            };

                            userRequest.Headers.Add("User-Agent", "github-tunisia");
                            userRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("token", _token);

                            var userResponse = await _client.SendAsync(userRequest);
                            var userJsonContent = await userResponse.Content.ReadAsStringAsync();

                            user.Information = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.UserInformation>(userJsonContent);
                            user.Information.events = await GetUserEvents(user.login);
                            user.Information.score = GetUserScore(user.Information.public_gists,
                                                                  user.Information.public_repos,
                                                                  user.Information.followers,
                                                                  user.Information.following,
                                                                  user.Information.events);

                            usersInformation.Add(user.Information);
                        }
                    }
                }
            }

            return usersInformation;
        }
        
        public async Task<int> GetUserEvents(string login)
        {
            _client = new HttpClient();

            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new Uri($"https://api.github.com/users/{login}/events?per_page=10000")))
            {
                // add the required 'User-Agent' required header
                request.Headers.Add("User-Agent", "github-tunisia");

                // add the token to the request headers
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("token", _token);

                // send the http request and get the result
                var response = await _client.SendAsync(request);

                if(response.IsSuccessStatusCode)
                {
                    var jsonObjectAsString = await response.Content.ReadAsStringAsync();

                    dynamic jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonObjectAsString);  // JObject.Parse(jsonObjectAsString);

                    return jsonObject.Count;
                }
            }

            return 0;
        }

        private int GetUserScore(int? public_gists, int? public_repos, int? followers, int? following, int events)
        {
            return (int) ((0.1 * (public_repos + public_gists) + 0.2 * (followers + following) + 0.7 * events) * 10);
        }
    }
}