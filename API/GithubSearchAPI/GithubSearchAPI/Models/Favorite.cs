namespace GithubSearchAPI.Models
{
    public class Favorite
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string RepositoryName { get; set; }
        public string RepositoryURL { get; set; }
    }
}
