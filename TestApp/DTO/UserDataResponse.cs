namespace TestApp.DTO
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Avatar { get; set; }
    }

    public class UserDataResponse
    {
        public List<User> Data { get; set; }
        public int Page { get; set; }
        public int Total_Pages { get; set; }
    }

    public class SingleUserResponse
    {
        public User Data { get; set; }
    }

}
