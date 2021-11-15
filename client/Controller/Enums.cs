namespace client.Controller
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
    public enum Commands
    {
        SignUp,
        SignIn,
        ShowUsers,
        EditAdmin,
        ShowAdmin
        
            
    }
    public static class CommandsExtensions
    {
        public static string GetString(this Commands command)
        {
            switch (command)
            {
                case Commands.SignUp: return "reg";
                case Commands.SignIn: return "sgn";
                case Commands.ShowUsers: return "sus";
                case Commands.EditAdmin: return "adt";
                case Commands.ShowAdmin: return "sad";
                default: return "err";
            }
        }
    }
}