namespace GithubSearchAPI.DTOs
{
    public class GitHubSearchResponse
    {
        //public int TotalCount { get; set; }
        //public bool IncompleteResults { get; set; }
        public IEnumerable<RepositoryDTO> Items { get; set; }
    }
}
