using System.Net.Sockets;
using System.Windows;
using System.Windows.Input;
using client.Controller;
using client.Controller.Const;
using client.Models;

namespace client
{
    public partial class EditPassword : Window
    {
        private User user;
        private NetworkStream Stream;
        public EditPassword(User user, NetworkStream stream)
        {
            this.user = user;
            this.Stream = stream;
            InitializeComponent();
        }

        private void EditPassword_OnClick(object sender, RoutedEventArgs e)
        {
            if (InputTextBoxPassword.Password != user.password) MessageBox.Show("Неверно введен старый пароль");
            else if (InputTextBoxNewPassword.Password != InputTextBoxRepeatNewPassword.Password)
                MessageBox.Show("Пароли не совпадают");
            else
            {
                user.password = InputTextBoxNewPassword.Password;
                Packages.Send(Stream, Commands.EditUserPassword.GetString() + user);
                Close();
            }
        }
        
    }
}