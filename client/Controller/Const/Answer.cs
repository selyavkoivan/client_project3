namespace client.Controller.Const
{
    public enum Answer
    {
        Success,
        Error
    }
    public static class AnswerExtensions
    {
        public static string GetString(this Answer role)
        {
            switch (role)
            {
                case Answer.Success: return "s";
                case Answer.Error: return "e";
                default: return "e";
            }
        }
    }
}