﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using client.Controller;
using client.Controller.Const;
using client.Models;
using Size = client.Models.Size;

namespace client
{
    public partial class AdminWindow : Window
    {
        private NetworkStream Stream;
        private Admin admin;

        public AdminWindow(NetworkStream stream, Admin admin)
        {
            InitializeComponent();
            Stream = stream;
            this.admin = admin;
            Packages.Send(Stream, Commands.ShowUserOrders.GetString() + new User(admin));
            FillOrderTable(JsonSerializer.Deserialize<List<Order>>(Packages.Recv(Stream)));
            FillAdminData(admin);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            new MainWindow(Stream).Show();
        }

        private void FillAdminData(Admin admin)
        {
            Name.Text = admin.name;
            Login.Text = admin.login;
            Position.Text = admin.position;
        }

        private void FillOrderTable(List<Order> orders)
        {
            UserOrderGrid.ItemsSource = orders;
        }


        private void Users_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FillUsersPage();
        }

        private void FillUsersPage()
        {
            Packages.Send(Stream, Commands.ShowUsers.GetString());
            var answer = Packages.Recv(Stream);
            var users = JsonSerializer.Deserialize<List<CalculatedUser>>(answer);
            Packages.Send(Stream, Commands.ShowOrders.GetString());
            answer = Packages.Recv(Stream);
            var orders = JsonSerializer.Deserialize<List<Order>>(answer);
            users.ForEach(u => u.Count = orders.Count(o => o.user.userId == u.userId));
            UGrid.ItemsSource = users;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var newAdmin = new Admin
            {
                adminId = admin.adminId, position = Position.Text, name = Name.Text, login = Login.Text,
                password = admin.password, userId = admin.userId, card = admin.card
            };
            var data = Commands.EditAdmin.GetString() + newAdmin;
            Packages.Send(Stream, data);
            if (Packages.Recv(Stream) == Answer.Success.GetString())
            {
                admin = newAdmin;
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
            FillAccount();
        }

        void FillAccount()
        {
            Packages.Send(Stream, Commands.ShowAdmin.GetString() + admin);
            FillAdminData(JsonSerializer.Deserialize<Admin>(Packages.Recv(Stream)));
            Packages.Send(Stream, Commands.ShowUserOrders.GetString() + admin);
            FillOrderTable(JsonSerializer.Deserialize<List<Order>>(Packages.Recv(Stream)));
        }


        private void ShowSelectedUser_OnClick(object sender, RoutedEventArgs e)
        {
            var user = GetSelectedUser();
            if (user != null) FillUserToAdminData(user);
            else
            {
                ShowSelectedUser.IsExpanded = false;
                MessageBox.Show("Выберите пользователя");
            }
        }

        private User GetSelectedUser()
        {
            return (User)UGrid.SelectedItem;
        }

        private void FillUserToAdminData(User user)
        {
            ShowUserLogin.Text = user.login;
            ShowUserName.Text = user.name;
        }

        private void SubmitNewAdmin_OnClick(object sender, RoutedEventArgs e)
        {
            if (CheckBoxNewAdmin.IsChecked == true && SetPosition.Text != string.Empty)
            {
                Packages.Send(Stream,
                    Commands.SetNewAdmin.GetString() + new Admin
                        { login = ShowUserLogin.Text, position = SetPosition.Text });
                FillUsersPage();
            }
        }

        private void Goods_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FillGoodsTable();
        }

        private void FillGoodsTable()
        {
            Packages.Send(Stream, Commands.ShowGoods.GetString());
            var goods = JsonSerializer.Deserialize<List<CalculatedProduct>>(Packages.Recv(Stream));
            Packages.Send(Stream, Commands.GetRates.GetString());
            var rates = JsonSerializer.Deserialize<List<Review>>(Packages.Recv(Stream));
            List<ProductAvRate> avRGoods = new List<ProductAvRate>();
            goods.ForEach(p => p.SetCount());
            goods.ForEach(p =>
            {
                if (rates.Count != 0)
                {
                    List<Review> productRates = new List<Review>();
                    for (int i = 0; i < rates.Count; i++)
                    {
                        if (rates[i].product.productId == p.productId)
                        {
                            productRates.Add(rates[i]);
                        }
                    }

                    
                    if(productRates.Count == 0) avRGoods.Add(new ProductAvRate(p, null));
                    else avRGoods.Add(new ProductAvRate(p, rates.FindAll(r => r.product.productId == p.productId)));
                }
                else avRGoods.Add(new ProductAvRate(p, null));
            });

            GGrid.ItemsSource = avRGoods;
        }

        private ProductAvRate GetProduct(Product product)
        {
            Packages.Send(Stream, Commands.ShowProduct.GetString() + product);
            CalculatedProduct newProduct = JsonSerializer.Deserialize<CalculatedProduct>(Packages.Recv(Stream));
            Packages.Send(Stream, Commands.GetProductRates.GetString() + product);
            var rates = JsonSerializer.Deserialize<List<Review>>(Packages.Recv(Stream));
            if (rates.Count != 0)
                return new ProductAvRate(newProduct,
                    rates.Where(r => r.product.productId == product.productId).ToList());
            return new ProductAvRate(newProduct, null);
        }

        private void GGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var product = (ProductAvRate)GGrid.SelectedItem;
            SelectProductTabControl(product);
        }

        private void SelectProductTabControl(ProductAvRate product)
        {
            Dispatcher.BeginInvoke((Action)(() => AdminTabControl.SelectedItem = Product));
            FillProductDataGrid(product);
        }

        private void FillProductDataGrid(ProductAvRate product)
        {
            PrDataGrid.Items.Clear();
            PrDataGrid.Items.Add(product.product);
            SizesDataGrid.ItemsSource = product.product.sizes;
            NameProductBlock.Text = product.product.name;
            DescriptionProductBlock.Text = product.product.description;
            averageRate.Text = product.averageRate.ToString();
    
            if (product.reviews != null && product.reviews.Count(r => r.user.userId == admin.userId) != 0)
            {
                RateStackPanel.Visibility = Visibility.Hidden;
                SetRate.Visibility = Visibility.Hidden;
            }
            else
            {
                RateStackPanel.Visibility = Visibility.Visible;
                SetRate.Visibility = Visibility.Visible;
            }
        }

        private void CreateOrder_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (admin.card == null || admin.card.PaymentCardId == 0) new Card(admin, Stream).ShowDialog();
                var count = int.Parse(CountOfProduct.Text);
                if (count <= 0 || deliveryDate.SelectedDate < DateTime.Now.AddDays(1)) throw new Exception();
                var product = (ProductAvRate)GGrid.SelectedItem;
                product.product.sizes = new List<Size> { (Size)SizesDataGrid.SelectedItem };
                if (product.product.sizes[0] == null) throw new Exception();
                var order = DeliveryAddress.Text == string.Empty
                    ? new Order(admin, product.product, count, DateTime.Now,
                        deliveryDate.SelectedDate == null ? DateTime.Now.AddDays(3) : deliveryDate.SelectedDate.Value,
                        0)
                    : new Order(admin, product.product, count, DateTime.Now, DeliveryAddress.Text,
                        deliveryDate.SelectedDate == null ? DateTime.Now.AddDays(3) : deliveryDate.SelectedDate.Value,
                        0);
                Packages.Send(Stream, Commands.AddOrder.GetString() + order);
                if (Packages.Recv(Stream) == Answer.Success.GetString())
                {
                    MessageBox.Show("Заказ успешно создан");
                    FillProductDataGrid(GetProduct(product.product));
                }
                else MessageBox.Show("Ошибка при составлении заказа");
            }
            catch (Exception)
            {
                MessageBox.Show("Неверный введены данные или не выбран размер");
            }
        }

        private void FillEditProductDataGrid(Product product)
        {
            EditPrDataGrid.ItemsSource = new List<Product> { product };
            EditSizesDataGrid.ItemsSource = product.sizes;
            EditNameProductBlock.Text = product.name;
            EditDescriptionProductBlock.Text = product.description;
        }

        private void EditSelectedProduct_OnClick(object sender, RoutedEventArgs e)
        {
            var product = (Product)GGrid.SelectedItem;
            if (product != null)
            {
                Dispatcher.BeginInvoke((Action)(() => AdminTabControl.SelectedItem = EditProductTabItem));
                FillEditProductDataGrid(product);
            }
        }

        private void Confirm_OnClick(object sender, RoutedEventArgs e)
        {
            var product = (Product)EditPrDataGrid.Items[0];
            product.name = EditNameProductBlock.Text;
            product.description = EditDescriptionProductBlock.Text;
            if (!product.isFull())
            {
                MessageBox.Show("Присутствуют пустые поля");
                return;
            }

            if (product.productId == 0) AddProduct(product);
            else EditProduct(product);
        }

        private void AddProduct(Product product)
        {
            Packages.Send(Stream, Commands.AddProduct.GetString() + product);
            SelectProductTabControl(GetProduct(product));
        }

        private void EditProduct(Product product)
        {
            Packages.Send(Stream, Commands.EditProduct.GetString() + product);
            if (Packages.Recv(Stream) == Answer.Success.GetString())
            {
                SelectProductTabControl(GetProduct(product));
            }
            else
                MessageBox.Show(
                    "Ошибка изменения\nОна не должна была никогда появиться, а это значит с приложением явно что-то не то");
        }

        private void AddNewProduct_OnClick(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => AdminTabControl.SelectedItem = EditProductTabItem));
            FillEditProductDataGrid(new Product());
        }

        private void DeleteSelectedProduct_OnClick(object sender, RoutedEventArgs e)
        {
            var product = (Product)GGrid.SelectedItem;
            if (product != null)
            {
                Packages.Send(Stream, Commands.DeleteProduct.GetString() + product);
                FillGoodsTable();
            }
        }

        private void UserGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var user = UGrid.SelectedItem as User;
            if (user != null)
            {
                FillUserData(user);
                ShowUserTabControl();
            }
        }

        private void FillUserData(User user)
        {
            UserName.Text = user.name;
            UserLogin.Text = user.login;
            UserStatus.Text = user.status ? "заблокирован" : "нет блокировки";
            Packages.Send(Stream, Commands.ShowUserOrders.GetString() + user);
            SelectedUserOrderGrid.ItemsSource = JsonSerializer.Deserialize<List<Order>>(Packages.Recv(Stream));
        }

        private void ShowMyAccountTabControl()
        {
            Dispatcher.BeginInvoke((Action)(() => AdminTabControl.SelectedItem = Account));
            FillAccount();
        }

        private void ShowUserTabControl()
        {
            Dispatcher.BeginInvoke((Action)(() => AdminTabControl.SelectedItem = UserAccount));
        }

        private void ShowCard_OnClick(object sender, RoutedEventArgs e)
        {
            Card card = new Card(admin, Stream);
            card.ShowDialog();
            Packages.Send(Stream, Commands.ShowAdmin.GetString() + admin);
            admin = JsonSerializer.Deserialize<Admin>(Packages.Recv(Stream));
            FillAdminData(admin);
        }

        private void ChangeStatus_OnClick(object sender, RoutedEventArgs e)
        {
            User user = new User();
            user.login = UserLogin.Text;
            user.status = UserStatus.Text != "заблокирован";

            Packages.Send(Stream, Commands.EditUserStatus.GetString() + user);
            Packages.Send(Stream, Commands.ShowUser.GetString() + user);
            user = JsonSerializer.Deserialize<User>(Packages.Recv(Stream));
            FillUserData(user);
        }

        string getOrdersFilter(string filter)
        {
            switch (filter)
            {
                case "Название": return "test.product.name";
                case "Количество": return "countInOrder";
                case "Адрес": return "deliveryAddress";
                case "Дата": return "date";
                case "Логин": return "login";
                case "Этап": return "deliveryStatus";
            }

            return string.Empty;
        }

        private void SubmitOrderFilter_OnClick(object sender, RoutedEventArgs e)
        {
            string column = getOrdersFilter(OrderFilterColumns.Text);
            if (column == String.Empty) return;
            var filter = new SortConfiguration(column,
                OrderFilter.Text, admin.userId);
            Packages.Send(Stream, Commands.FilterUserOrders.GetString() + filter);
            FillOrderTable(JsonSerializer.Deserialize<List<Order>>(Packages.Recv(Stream)));
        }

        string getUsersFilter(string filter)
        {
            switch (filter)
            {
                case "Логин": return "login";
                case "Имя": return "name";
            }

            return string.Empty;
        }

        private void SubmitUsersFilter_OnClick(object sender, RoutedEventArgs e)
        {
            string column = getUsersFilter(UsersFilterColumns.Text);
            if (column == String.Empty) return;
            var filter = new SortConfiguration(column,
                UsersFilter.Text);
            Packages.Send(Stream, Commands.FIlterUsers.GetString() + filter);
            UGrid.ItemsSource = JsonSerializer.Deserialize<List<User>>(Packages.Recv(Stream));
        }

        string getGoodsFilter(string filter)
        {
            switch (filter)
            {
                case "Название": return "name";
                case "Тип": return "type";
                case "Материал": return "material";
                case "Цвет": return "color";
                case "Цена": return "price";
            }

            return string.Empty;
        }

        private void SubmitGoodsFilter_OnClick(object sender, RoutedEventArgs e)
        {
            string column = getGoodsFilter(GoodsFilterColumns.Text);
            if (column == String.Empty) return;
            var filter = new SortConfiguration(column,
                GoodsFilter.Text);
            Packages.Send(Stream, Commands.FilterGoods.GetString() + filter);
            GGrid.ItemsSource = JsonSerializer.Deserialize<List<Product>>(Packages.Recv(Stream));
        }

        private void Analytics_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Packages.Send(Stream, Commands.ShowOrders.GetString());
            OGrid.ItemsSource = JsonSerializer.Deserialize<List<Order>>(Packages.Recv(Stream));
        }

        private void TypeButton_OnClick(object sender, RoutedEventArgs e)
        {
            Packages.Send(Stream, Commands.ShowOrders.GetString());
            new LifiChartDiagram(JsonSerializer.Deserialize<List<Order>>(Packages.Recv(Stream))).ShowDialog();
        }

        private void ProfitButton_OnClick(object sender, RoutedEventArgs e)
        {
            Packages.Send(Stream, Commands.ShowOrders.GetString());
            new profitDiagram(JsonSerializer.Deserialize<List<Order>>(Packages.Recv(Stream))).ShowDialog();
        }

        private void SubmitMyOrder_OnClick(object sender, RoutedEventArgs e)
        {
            var order = PrMyOrderDataGrid.Items[0] as Order;
            order.orderStatus = 3;
            Packages.Send(Stream, Commands.EditDeliveryStatus.GetString() + order);
            FillMyOrderDataGrid(order);
        }

        private void SelectMyOrderTabControl(Order order)
        {
            Dispatcher.BeginInvoke((Action)(() => AdminTabControl.SelectedItem = MyOrderTabItem));
            FillMyOrderDataGrid(order);
        }

        void FillMyOrderDataGrid(Order order)
        {
            PrMyOrderDataGrid.Items.Clear();
            PrMyOrderDataGrid.Items.Add(order);
            NameProductMyOrderBlock.Text = order.product.name;
            DescriptionMyOrderProductBlock.Text = order.product.description;
            SizeMyOrder.Text = order.product.sizes[0].size;
            CountMyOrder.Text = order.count.ToString();
            MyOrderDate.Text = order.date.ToLongDateString();
            MyOrderDeliveryDate.Text = order.deliveryDate.ToLongDateString();
            if (order.delivery)
            {
                MyOrderDeliveryAddress.Text = order.deliveryAddress;
            }
            else MyOrderDeliveryAddress.Text = "адрес не выбран";

            MyOrderDeliveryStatus.Text = getStatus(order.orderStatus);
            if (order.orderStatus == 3) SubmitMyOrder.Visibility = Visibility.Hidden;
            else SubmitMyOrder.Visibility = Visibility.Visible;
            MyOrderChat.Items.Clear();
            Packages.Send(Stream, Commands.GetOrderMessages.GetString() + order);
            List<Message> messages = JsonSerializer.Deserialize<List<Message>>(Packages.Recv(Stream));
            foreach (var m in messages)
            {
                SetMessage(m);
            }
        }

        private string getStatus(int status)
        {
            switch (status)
            {
                case 0: return "Подготовка к отправке";
                case 1: return "Отправлен";
                case 2: return "В пункте выдачи";
                case 3: return "Получен";
            }

            return "Ошибка";
        }

        private void UserOrderGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var order = (Order)UserOrderGrid.SelectedItem;
            if (order != null) SelectMyOrderTabControl(order);
        }

        private void SubmitUserOrder_OnClick(object sender, RoutedEventArgs e)
        {
            var order = PrUserOrderDataGrid.Items[0] as Order;
            if (wait.IsChecked == true) order.orderStatus = 0;
            else if (send.IsChecked == true) order.orderStatus = 1;
            else if (ready.IsChecked == true) order.orderStatus = 2;
            else if (received.IsChecked == true) order.orderStatus = 3;
            Packages.Send(Stream, Commands.EditDeliveryStatus.GetString() + order);
            FillUserOrderDataGrid(order);
        }

        private void SelectUserOrderTabControl(Order order)
        {
            Dispatcher.BeginInvoke((Action)(() => AdminTabControl.SelectedItem = UserOrderTabItem));
            FillUserOrderDataGrid(order);
        }

        void FillUserOrderDataGrid(Order order)
        {
            PrUserOrderDataGrid.Items.Clear();
            PrUserOrderDataGrid.Items.Add(order);
            NameProductUserOrderBlock.Text = order.product.name;
            DescriptionUserOrderProductBlock.Text = order.product.description;
            SizeUserOrder.Text = order.product.sizes[0].size;
            CountUserOrder.Text = order.count.ToString();
            UserOrderDate.Text = order.date.ToLongDateString();
            UserOrderDeliveryDate.Text = order.deliveryDate.ToLongDateString();
            if (order.delivery)
            {
                UserOrderDeliveryAddress.Text = order.deliveryAddress;
                ready.Visibility = Visibility.Hidden;
                received.Visibility = Visibility.Visible;
            }
            else
            {
                UserOrderDeliveryAddress.Text = "адрес не выбран";
                ready.Visibility = Visibility.Visible;
                received.Visibility = Visibility.Hidden;
            }

            if (order.orderStatus == 3)
            {
                UserOrderIsDelivered.Visibility = Visibility.Visible;
                SubmitUserOrder.Visibility = Visibility.Hidden;
                wait.Visibility = Visibility.Hidden;
                send.Visibility = Visibility.Hidden;
                ready.Visibility = Visibility.Hidden;
                received.Visibility = Visibility.Hidden;
            }
            else
            {
                SubmitUserOrder.Visibility = Visibility.Visible;
                UserOrderIsDelivered.Visibility = Visibility.Hidden;
                switch (order.orderStatus)
                {
                    case 0:
                        wait.IsChecked = true;
                        break;
                    case 1:
                        send.IsChecked = true;
                        break;
                    case 2:
                        ready.IsChecked = true;
                        break;
                    default:
                        wait.IsChecked = true;
                        break;
                }
            }
            UserOrderChat.Items.Clear();
            Packages.Send(Stream, Commands.GetOrderMessages.GetString() + order);
            List<Message> messages = JsonSerializer.Deserialize<List<Message>>(Packages.Recv(Stream));
            foreach (var m in messages)
            {
                SetAdminMessage(m);
            }
        }

        private void OGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var order = OGrid.SelectedItem as Order;
            SelectUserOrderTabControl(order);
        }

        private void SetRate_OnClick(object sender, RoutedEventArgs e)
        {
            int rate = 0;
            if (one.IsChecked == true) rate = 1;
            else if (two.IsChecked == true) rate = 2;
            else if (three.IsChecked == true) rate = 3;
            else if (four.IsChecked == true) rate = 4;
            else if (five.IsChecked == true) rate = 5;
            else
            {
                MessageBox.Show("Поставьте оценку");
                return;
            }

            Review review = new Review(rate, admin, PrDataGrid.Items[0] as Product);
            Packages.Send(Stream, Commands.SetRate.GetString() + review);
            if (averageRate.Text == "0")
            {
                averageRate.Text = rate.ToString();
                RateStackPanel.Visibility = Visibility.Hidden;
                SetRate.Visibility = Visibility.Hidden;
            }

        }

        private void Exit_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void EditUser_OnClick(object sender, RoutedEventArgs e)
        {
            EditPassword oEditPassword = new EditPassword(admin, Stream);
            oEditPassword.ShowDialog();
            Packages.Send(Stream, Commands.ShowAdmin.GetString() + admin);
            admin = JsonSerializer.Deserialize<Admin>(Packages.Recv(Stream));
            FillAdminData(admin);
        }
        private void SetAdminMessage(Message message)
        {
            string str = message.type ? "получено" : "отправлено";
            str += " | " + message.date + " >> ";
            str += message.message;
            UserOrderChat.Items.Add(str);
        }
        private void SetMessage(Message message)
        {
            string str = message.type ? "отправлено" : "получено";
            str += " | " + message.date + " >> ";
            str += message.message;
            MyOrderChat.Items.Add(str);
        }
        private void SendUserMessage_OnClick(object sender, RoutedEventArgs e)
        {
            if (UserMessage.Text == String.Empty) return;

            var message = new Message(DateTime.Now, true, PrMyOrderDataGrid.Items[0] as Order, UserMessage.Text);
            Packages.Send(Stream, Commands.AddMessage.GetString() + message);
            SetMessage(message);
            UserMessage.Text = string.Empty;
        }

        private void SendAdminMessage_OnClick(object sender, RoutedEventArgs e)
        {
            if (AdminMessage.Text == String.Empty) return;

            var message = new Message(DateTime.Now, false, PrUserOrderDataGrid.Items[0] as Order, AdminMessage.Text);
            Packages.Send(Stream, Commands.AddMessage.GetString() + message);
            SetAdminMessage(message);
            AdminMessage.Text = string.Empty;
        }

        private void ResendUserChat_OnClick(object sender, RoutedEventArgs e)
        {
            
            MyOrderChat.Items.Clear();
            Packages.Send(Stream, Commands.GetOrderMessages.GetString() + (PrMyOrderDataGrid.Items[0] as Order));
            foreach (var m in JsonSerializer.Deserialize<List<Message>>(Packages.Recv(Stream)))
            {
                SetMessage(m);
            }
        }

        private void UpdateAdminMessage_OnClick(object sender, RoutedEventArgs e)
        {
            UserOrderChat.Items.Clear();
            Packages.Send(Stream, Commands.GetOrderMessages.GetString() + (PrUserOrderDataGrid.Items[0] as Order));
            foreach (var m in JsonSerializer.Deserialize<List<Message>>(Packages.Recv(Stream)))
            {
                SetAdminMessage(m);
            }
        }

        private void AllOrderFilterSubmit_OnClick(object sender, RoutedEventArgs e)
        {
            string column = getOrdersFilter(AllOrderFilterColumns.Text);
            if (column == String.Empty) return;
            var filter = new SortConfiguration(column,
                AllOrderFilter.Text);
            Packages.Send(Stream, Commands.FilterOrders.GetString() + filter);
            OGrid.ItemsSource = JsonSerializer.Deserialize<List<Order>>(Packages.Recv(Stream));
        }
    }
}