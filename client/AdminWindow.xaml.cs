using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            fillAdminData(admin);
        }

        private void fillAdminData(Admin admin)
        {
            Name.Text = admin.name;
            Login.Text = admin.login;
            Position.Text = admin.position;
        }


        private void Users_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FillUsersPage();
        }

        private void FillUsersPage()
        {
            Packages.Send(Stream, Commands.ShowUsers.GetString());
            var answer = Packages.Recv(Stream);
            var users = JsonSerializer.Deserialize<List<User>>(answer);
            UGrid.ItemsSource = users;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var newAdmin = new Admin
            {
                adminId = admin.adminId, position = Position.Text, name = Name.Text, login = Login.Text, password = admin.password, userId = admin.userId
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
            Packages.Send(Stream, Commands.ShowAdmin.GetString() + admin);
            fillAdminData(JsonSerializer.Deserialize<Admin>(Packages.Recv(Stream)));
        }


        private void ShowSelectedUser_OnClick(object sender, RoutedEventArgs e)
        {
            var user = GetSelectedUser();
            if (user != null) FillUserData(user);
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

        private void FillUserData(User user)
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
            Packages.Send(Stream, Commands.ShowGoods.GetString());
            var data = Packages.Recv(Stream);

            GGrid.ItemsSource = JsonSerializer.Deserialize<List<Product>>(data);
        }

        private void GGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var product = (Product)GGrid.SelectedItem;
            SelectProductTabControl(product);
        }

        private void SelectProductTabControl(Product product)
        {
            Dispatcher.BeginInvoke((Action)(() => AdminTabControl.SelectedItem = Product));
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
                var count = int.Parse(CountOfProduct.Text);
                if (count <= 0) throw new Exception();
                var product = (Product)GGrid.SelectedItem;
                product.sizes = new List<Size> { (Size)SizesDataGrid.SelectedItem };
                if(product.sizes.Count == 0) throw new Exception();
                var order = DeliveryAddress.Text == string.Empty ? 
                    new Order(admin, product, count, DateTime.Now) : 
                    new Order(admin, product, count, DateTime.Now, DeliveryAddress.Text);  
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
                Dispatcher.BeginInvoke((Action)(() => AdminTabControl.SelectedItem = EditProduct));
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
            Packages.Send(Stream, Commands.EditProduct.GetString() + product);
            if (Packages.Recv(Stream) == Answer.Success.GetString())
            {
                SelectProductTabControl(product);
            }
            else MessageBox.Show("Ошибка изменения\nОна не должна была никогда появиться, а это значит с приложением явно что-то не то");
        }

    }

}