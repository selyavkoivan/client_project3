using System;
using System.ComponentModel;
using System.IO;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using client.Controller;
using client.Controller.Const;
using client.Models;

namespace client
{
    public partial class SingUp
    {
        private NetworkStream Stream { get; }

        public SingUp(NetworkStream stream)
        {
            Stream = stream;

            InitializeComponent();
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private Byte[] Data;

        private void YesButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (InputTextBoxLogin.Text == string.Empty || InputTextBoxName.Text == string.Empty ||
                InputTextBoxPassword.Password == string.Empty ||
                InputTextBoxRepeatPassword.Password == string.Empty) return;

            if (InputTextBoxPassword.Password != InputTextBoxRepeatPassword.Password)
            {
                InputTextBoxPassword.BorderBrush = Brushes.Red;
                InputTextBoxRepeatPassword.BorderBrush = Brushes.Red;
                MessageBox.Show("Подтвердите пароль");
                return;
            }

            var message = Commands.SignUp.GetString() + new User
            {
                login = InputTextBoxLogin.Text,
                password = InputTextBoxPassword.Password,
                name = InputTextBoxName.Text
            };
            Packages.Send(Stream, message);
            if (Packages.Recv(Stream) == "0")
            {
                InputTextBoxLogin.BorderBrush = Brushes.Red;
                MessageBox.Show("Введенный логин уже занят");
            }
            else
            {
                MessageBox.Show("Регистрация прошла успешно");
                Close();
                new MainWindow(Stream).Show();
            }
        }

        private void InputTextBoxRepeatePassword_OnTextInput(object sender,
            KeyboardFocusChangedEventArgs keyboardFocusChangedEventArgs)
        {
            InputTextBoxRepeatPassword.BorderBrush = new TextBox().BorderBrush;
            InputTextBoxPassword.BorderBrush = new TextBox().BorderBrush;
        }

        private void InputTextBoxLogin_OnTextInput(object sender,
            KeyboardFocusChangedEventArgs keyboardFocusChangedEventArgs)
        {
            InputTextBoxLogin.BorderBrush = Brushes.Black;
        }

        private void ToSignIn_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
            new MainWindow(Stream).Show();
        }
    }
}