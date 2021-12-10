using System;
using System.Net.Mime;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using client.Controller;
using client.Controller.Const;
using client.Models;

namespace client
{
    public partial class Card : Window
    {
        private User user;
        private NetworkStream Stream;
        bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }
        public Card(User user, NetworkStream stream)
        {
            Stream = stream;
            this.user = user;
            InitializeComponent();
            if(user.card != null) FillData(user);
        }

        private void FillData(User user)
        {
            CardNumber.Text = user.card.CardNumber;
            CardCVV.Text = user.card.CVV.ToString();
            CardMonth.Text = user.card.expityDate.Month.ToString();
            CardYear.Text = user.card.expityDate.Year.ToString();
            
        }

        private void SubmitCard_OnClick(object sender, RoutedEventArgs e)
        {
            
                var card = new PaymentCard(CardNumber.Text, int.Parse(CardCVV.Text),
                    new DateTime(int.Parse(CardYear.Text), int.Parse(CardMonth.Text), 1));
                
                
                if (!IsDigitsOnly(card.CardNumber) || card.CVV == 0 || card.expityDate < DateTime.Now || card.CardNumber.Length != 16 || card.CVV < 0 || card.CVV > 999)
                {
                    MessageBox.Show("Данные введены неверно");
                    return;
                }

                if (user.card.PaymentCardId == 0)
                {
                    user.card = card;
                    Packages.Send(Stream, Commands.AddCard.GetString() + user);
                }
                else
                {
                    card.PaymentCardId = user.card.PaymentCardId;
                    user.card = card;
                    Packages.Send(Stream, Commands.EditCard.GetString() + user);
                }

                Close();

        }

        private void DeleteCard_OnClick(object sender, RoutedEventArgs e)
        {
            if (user.card.PaymentCardId != 0)
            {
                Packages.Send(Stream, Commands.DeleteCard.GetString() + user);
                Close();
            } else MessageBox.Show("Карта отсутствует");
        }
    }
}