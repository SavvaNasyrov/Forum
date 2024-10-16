namespace InternalAuthService.Models
{
    public class UserPublics
    {
        public required string Login {  get; set; }

        public required Role Role { get; set; }

        private UserPublics() { }

        public static UserPublics FromUser(User user)
        {
            return new UserPublics() { Login = user.Login, Role = user.Role };
        }
    }
}
