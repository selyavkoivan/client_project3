namespace client.Controller.Const
{
    public enum Commands
    {
        SignUp,
        SignIn,
        ShowUsers,
        ShowGoods,
        ShowAdmin,
        ShowOrders,
        ShowProduct,
        ShowUserOrders,
        EditAdmin,
        EditProduct,
        SetNewAdmin,
        AddProduct,
        AddOrder,
        DeleteProduct,
        DeleteOrder,
        Error
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
                case Commands.ShowGoods: return "sgs";
                case Commands.ShowAdmin: return "sad";
                case Commands.ShowOrders: return "sds";
                case Commands.ShowUserOrders: return "sud";
                case Commands.EditAdmin: return "adt";
                case Commands.EditProduct: return "pdt";
                case Commands.SetNewAdmin: return "nad";
                case Commands.AddProduct: return "apr";
                case Commands.AddOrder: return "adr";
                case Commands.DeleteProduct: return "dpr";
                case Commands.DeleteOrder: return "ddr";
                case Commands.ShowProduct: return "spd";
                case Commands.Error: return "err";
                default: return "err";
            }
        }
    }
}