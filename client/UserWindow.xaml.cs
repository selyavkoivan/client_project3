using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using client.Controller;
using client.Controller.Const;
using client.Models;
using Size = client.Models.Size;

namespace client
{
    public partial class UserWindow : Window
    {
        private NetworkStream Stream;
        private User user;
        public UserWindow(NetworkStream stream, User user)
        {
            InitializeComponent();
            Stream = stream;
            this.user = user;
            Packages.Send(Stream, Commands.ShowUserOrders.GetString() + new User(user));
            FillOrderTable(JsonSerializer.Deserialize<List<Order>>(Packages.Recv(Stream)));
            FillUserData(user);
        }

        private void FillUserData(User user)
        {
            Name.Text = user.name;
            Login.Text = user.login;
        }

        private void FillOrderTable(List<Order> orders)
        {
            UserOrderGrid.ItemsSource = orders;
        }

        private void Account_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Packages.Send(Stream, Commands.ShowUser.GetString() + user);
            FillUserData(JsonSerializer.Deserialize<User>(Packages.Recv(Stream)));
            Packages.Send(Stream, Commands.ShowUserOrders.GetString() + user);
            FillOrderTable(JsonSerializer.Deserialize<List<Order>>(Packages.Recv(Stream)));
        }

        private void Login_OnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            Login.BorderBrush = Brushes.Black;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var newUser = new User
            {
                name = Name.Text, login = Login.Text, password = user.password, userId = user.userId, card = user.card
            };
            var data = Commands.EditUser.GetString() + newUser;
            Packages.Send(Stream, data);
            if (Packages.Recv(Stream) == Answer.Success.GetString())
            {
                user = newUser;
                MessageBox.Show("Данные успешно изменены");
            }
            else
            {
                MessageBox.Show("Логин уже занят");
                Login.BorderBrush = Brushes.Red;
            }
        }

        private void ShowCard_OnClick(object sender, RoutedEventArgs e)
        {
            Card card = new Card(user, Stream);
            card.ShowDialog();
            Packages.Send(Stream, Commands.ShowUser.GetString() + user);
            user = JsonSerializer.Deserialize<User>(Packages.Recv(Stream));
            FillUserData(user);
        }

        private void Goods_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FillGoodsTable();
        }

        private void FillGoodsTable()
        {
            Packages.Send(Stream, Commands.ShowGoods.GetString());
            
            var goods = JsonSerializer.Deserialize<List<CalculatedProduct>>(Packages.Recv(Stream));
            goods.ForEach(p => p.SetCount());
            GGrid.ItemsSource = goods;
        }

        private void GGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var product = (Product)GGrid.SelectedItem;
            SelectProductTabControl(product);
        }

        private void SelectProductTabControl(Product product)
        {
            Dispatcher.BeginInvoke((Action)(() => UserTabControl.SelectedItem = Product));
            FillProductDataGrid(product);
        }

        private void FillProductDataGrid(Product product)
        {
            PrDataGrid.Items.Clear();
            PrDataGrid.Items.Add(product);
            SizesDataGrid.ItemsSource = product.sizes;
            NameProductBlock.Text = product.name;
            DescriptionProductBlock.Text = product.description;
        }

        private void CreateOrder_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (user.card == null || user.card.PaymentCardId == 0) new Card(user, Stream).ShowDialog();
                var count = int.Parse(CountOfProduct.Text);
                if (count <= 0) throw new Exception();
                var product = (Product)GGrid.SelectedItem;
                product.sizes = new List<Size> { (Size)SizesDataGrid.SelectedItem };
                if (product.sizes[0] == null) throw new Exception();
                var order = DeliveryAddress.Text == string.Empty ? 
                    new Order(user, product, count, DateTime.Now) : 
                    new Order(user, product, count, DateTime.Now, DeliveryAddress.Text);
                
                Packages.Send(Stream, Commands.AddOrder.GetString() + order);
                if (Packages.Recv(Stream) == Answer.Success.GetString())
                {
                    MessageBox.Show("Заказ успешно создан");
                    Packages.Send(Stream, Commands.ShowProduct.GetString() + product);
                    FillProductDataGrid(JsonSerializer.Deserialize<Product>(Packages.Recv(Stream)));
                }
                else MessageBox.Show("Ошибка при составлении заказа");
            }
            catch (Exception)
            {
                MessageBox.Show("Неверный ввод количества или не выбран размер");
            }
        }
    }
}