using System.Collections.Generic;

namespace GitHubTunisia.Models
{
    public class GitHubUsersResponse
    {
        public int? total_count { get; set; }
        public bool? incomplete_results { get; set; }
        public List<User> items { get; set; }
    }
}