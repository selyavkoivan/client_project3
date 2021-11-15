using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using client.Controller;
using client.Models;

namespace client
{
    public partial class AdminWindow : Window
    {
        private NetworkStream Stream;
        private int admin;
        public AdminWindow(NetworkStream stream, Admin admin)
        {
            InitializeComponent();
            Stream = stream;
            this.admin = admin.id;
            fillLabel(admin);
        }

        private void fillLabel(Admin admin)
        {
            Name.Text = admin.user.name;
            Login.Text = admin.user.login;
            Position.Text = admin.position;
        }

        private void ShowUsers_OnClick(object sender, RoutedEventArgs e)
        {
            Packages.Send(Stream, CommandsExtensions.GetString(Commands.ShowUsers));
            var answer = Packages.Recv(Stream);
        }

        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Packages.Send(Stream, CommandsExtensions.GetString(Commands.ShowUsers));
            var answer = Packages.Recv(Stream);
            var users = ListConvertor.GetUsers(answer);
            UGrid.ItemsSource = users;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var data = Commands.EditAdmin.GetString() + new Admin
            {
                id = admin, position = Position.Text, user = new User
                {
                    name = Name.Text,
                    login = Login.Text
                }
            };
            Packages.Send(Stream, data);
            if (Packages.Recv(Stream) == "1")
            {
                MessageBox.Show("Данные успешно изменены");
            }
            else
            {
                MessageBox.Show("Логин уже занят");
                Login.BorderBrush = Brushes.Red;
            }
        }

        private void Login_OnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            Login.BorderBrush = Brushes.Black;
        }


        private void Account_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Packages.Send(Stream, Commands.ShowAdmin.GetString() + admin);
            fillLabel(new Admin(Packages.Recv(Stream)));
        }
    }
}