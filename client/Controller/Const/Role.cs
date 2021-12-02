namespace client.Controller.Const
{
    public enum Role
    {
        Admin,
        User,
        Error
    }

    public static class RoleExtensions
    {
        public static string GetString(this Role role)
        {
            switch (role)
            {
                case Role.Admin: return "a";
                case Role.User: return "u";
                case Role.Error: return "e";
                default: return "e";
            }
        }
    }
}