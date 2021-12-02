namespace client.Controller.Const
{
    public enum Answer
    {
        Success,
        Erroe
    }
    public static class AnswerExtensions
    {
        public static string GetString(this Answer role)
        {
            switch (role)
            {
                case Answer.Success: return "s";
                case Answer.Erroe: return "e";
                default: return "e";
            }
        }
    }
}