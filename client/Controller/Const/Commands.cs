﻿namespace client.Controller.Const
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
        AddCard,
        EditCard,
        DeleteCard,
        EditUser,
        Error,
        EditUserStatus,
        ShowUser,
        FilterUserOrders,
        FilterGoods,
        FIlterUsers,
        EditDeliveryStatus,
        SetRate,
        GetRates,
        GetProductRates,
        EditUserPassword,
        AddMessage,
        GetOrderMessages,
        FilterOrders
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
                case Commands.AddCard: return "acd";
                case Commands.EditCard: return "ecd";
                case Commands.DeleteCard: return "dcd";
                case Commands.EditUser: return "udt";
                case Commands.EditUserStatus: return "ust";
                case Commands.ShowUser: return "sur";
                case Commands.FilterGoods: return "fgs";
                case Commands.FilterUserOrders: return "fuo";
                case Commands.FIlterUsers: return "fus";
                case Commands.EditDeliveryStatus: return "eds";
                case Commands.SetRate: return "srt";
                case Commands.GetRates: return "grs";
                case Commands.GetProductRates: return "gpr";
                case Commands.EditUserPassword: return "eup";
                case Commands.AddMessage: return "ams";
                case Commands.GetOrderMessages: return "gom";
                case Commands.FilterOrders: return "fod";
                default: return "err";
            }
        }
    }
}