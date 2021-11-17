using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using client.Controller;
using client.Models;

namespace client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {   
            InitializeComponent();
            Run("127.0.0.1", 60606);
        }
        public MainWindow(NetworkStream stream)
        {   
            InitializeComponent();
            Stream = stream;
        }

        private TcpClient Client;

        private NetworkStream Stream;

        public void Run(string address, int port)
        {
            Client = new TcpClient();
            Client.Connect(address, port);
            Stream = Client.GetStream();
        }

        private void YesButton_OnClick(object sender, RoutedEventArgs e)
        {
            if(InputTextBoxLogin.Text == string.Empty ||InputTextBoxPassword.Password == string.Empty) return;

            var text = CommandsExtensions.GetString(Commands.SignIn) + InputTextBoxLogin.Text + Const.b + InputTextBoxPassword.Password;
            Packages.Send(Stream, text);
            var answer = Packages.Recv(Stream);
            var answerArray = new[] { answer.Substring(0, 1),  answer.Substring(1) };
            if (answerArray[0] == RoleExtensions.GetString(Role.Admin))
            {
                new AdminWindow(Stream, new Admin(answerArray[1])).Show();
                Hide();
            }
            else if (answerArray[0] == RoleExtensions.GetString(Role.User))
            {
                MessageBox.Show("hello user"); 
            }
            else
            {
                MessageBox.Show("Неверно введен логин или пароль"); 
                InputTextBoxPassword.BorderBrush = Brushes.Red;
                InputTextBoxLogin.BorderBrush = Brushes.Red;
            }
        }

        private void ToReg_OnClick(object sender, RoutedEventArgs e)
        {
            Hide();
            new SingUp(Stream).Show();
        }

        private void InputTextBoxLogin_OnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            InputTextBoxPassword.BorderBrush = Brushes.Black;
            InputTextBoxLogin.BorderBrush = Brushes.Black;
        }
    }
}