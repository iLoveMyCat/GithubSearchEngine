namespace GithubSearchAPI.Models
{
    public class User
    {
        public int Id { get; set; } //could make it UID
        public string Username { get; set; }
        public string hashedPassowrd { get; set; } 
    }
}
