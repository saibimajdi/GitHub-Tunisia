using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace GitHubTunisia.Models
{
    public class HttpClient
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

                            usersInformation.Add(user.Information);
                        }
                    }
                }
            }

            return usersInformation;
        }
    }
}