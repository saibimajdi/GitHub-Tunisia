using System;
using System.Linq;
using System.Web;

namespace GitHubTunisia.Models
{
    public class UserInformation
    {
        public string login { get; set; }
        public int? id { get; set; }
        public string avatar_url { get; set; }
        public string gravatar_id { get; set; }
        public string url { get; set; }
        public string html_url { get; set; }
        public string followers_url { get; set; }
        public string following_url { get; set; }
        public string gists_url { get; set; }
        public string starred_url { get; set; }
        public string subscriptions_url { get; set; }
        public string organizations_url { get; set; }
        public string repos_url { get; set; }
        public string events_url { get; set; }
        public string received_events_url { get; set; }
        public string type { get; set; }
        public bool? site_admin { get; set; }
        public string name { get; set; }
        public string company { get; set; }
        public string blog { get; set; }
        public string location { get; set; }
        public string email { get; set; }
        public bool? hireable { get; set; }
        public string bio { get; set; }
        public int? public_repos { get; set; }
        public int? public_gists { get; set; }
        public int? followers { get; set; }
        public int? following { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public int events { get; set; }

        public int score { get; set; }
    }
}

/*
 20% : Gist + Repos = 24
 30% : Followers + Followings = 30
 50% : Events (only last 90 days) = 54 

    

     */