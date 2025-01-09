using System.Text.Json.Serialization;

namespace GithubSearchAPI.DTOs
{
    public class RepositoryDTO
    {
        public string Name { get; set; }

        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; set; }
        public string Description { get; set; }
    }
}
