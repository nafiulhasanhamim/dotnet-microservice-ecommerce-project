namespace UserAPI.DTOs
{
    public class AdminDto
    {
        public string UserId { get; set; }
    }
    public class UserDto
    {
        public string UserId {get; set;}
        public string Email {get; set;}
        public string UserName {get; set;}
        public IList<string> Roles {get; set;}
    }
}