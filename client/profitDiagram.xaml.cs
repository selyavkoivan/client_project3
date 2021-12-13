using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using client.Models;
using LiveCharts;
using LiveCharts.Wpf;

namespace client
{
    public partial class profitDiagram : Window
    {
        private List<Order> orders;
        private List<Order> filteredOrder;
        public profitDiagram(List<Order> orders)
        {
            this.orders = orders;
            this.filteredOrder = orders;
            InitializeComponent();
            profitChart.AxisX.Add(new Axis
            {
                Title = "Месяца",
                Labels = new [] {"январь", "февраль", "март", "апрель", "май","июнь","июль","август","сентябрь","октябрь","ноябрь","декабрь", }
            });
            FillDiagram();
        }

        void FillDiagram()
        {
            
            profitChart.Series = new SeriesCollection();

            SeriesCollection collection = new SeriesCollection();
            collection.Add(new LineSeries()
            {
                Title = "Прибыль", Values = new ChartValues<double>(GetCollection())
            });
            profitChart.Series = collection;
        }

        double[] GetCollection()
        {
            var profitArr = new double[12];
            for (var i = 0; i < 12; i++)
            {
                profitArr[i] = filteredOrder.Where(o => o.date.Month == i + 1).Sum(o => o.product.price * o.count);
            }

            return profitArr;
        }

        private void Sunmit_OnClick(object sender, RoutedEventArgs e)
        {
            filteredOrder = orders.Where(o => o.date.Year == SelectYear.SelectedDate.Value.Year).ToList();
            FillDiagram();
        }

        private void Report_OnClick(object sender, RoutedEventArgs e)
        {
            if (SelectYear.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату");
                return;

            }

            String number = DateTime.Now.Year.ToString() +
                            DateTime.Now.Month.ToString() +
                            DateTime.Now.Hour.ToString() +
                            DateTime.Now.Second.ToString();
            String reportString = "			ОТДЕЛ ФИНАНСОВ\n" +
                                  "			ДАТА " + DateTime.Now.ToShortDateString() + "\n\n" +
                                  "ОТЧЕТ №" + number + "\n\n" +
                                  "Отчет по прибыли за " + SelectYear.SelectedDate.Value.Year + " год\n\n" +
                                  "+-------------+----------+----------+----------+----------+----------+----------+----------+----------+----------+----------+----------+----------+\n" +
                                  "| Тип         | январь   | февраль  | март     | апрель   | май      | июнь     | июль     | август   | сентябрь | октябрь  | ноябрь   | декабрь  |\n" +
                                  "+-------------+----------+----------+----------+----------+----------+----------+----------+----------+----------+----------+----------+----------+\n";

            foreach (var v in this.orders.GroupBy(o => o.product.type))
            {
                reportString += $"| {v.Key,-11}";
                for (int i = 0; i < 12; i++)
                {
                    reportString += $" | {" ",-8}";
                }

                reportString += " |\n";
                reportString += $"| {"Количество",-11}";
                for (int i = 0; i < 12; i++)
                {
                    reportString += $" | {v.Where(o => o.date.Month == i + 1).Sum(o => o.product.popularity),-8}";
                }

                reportString += " |\n";
                reportString += $"| {"Прибыль",-11}";
                for (int i = 0; i < 12; i++)
                {
                    reportString +=
                        $" | {v.Where(o => o.date.Month == i + 1).Sum(o => o.product.popularity * o.product.price),-8}";
                }

                reportString += " |\n";
                reportString += $"| {"Ср. цена",-11}";
                for (int i = 0; i < 12; i++)
                {
                    int count = v.Where(o => o.date.Month == i + 1).Sum(o => o.product.popularity);
                    double price = v.Where(o => o.date.Month == i + 1).Sum(o => o.product.popularity * o.product.price);
                    if (count != 0) reportString += $" | {Math.Round(100.0 * price / count) / 100.0,-8}";
                    else reportString += $" | {0,-8}";
                }

                reportString += " |\n";
                reportString +=
                    "+-------------+----------+----------+----------+----------+----------+----------+----------+----------+----------+----------+----------+----------+\n";
            }

        
            File.WriteAllText(number + ".txt", reportString);
            new Report(Directory.GetCurrentDirectory() + "\\" + number + ".txt").ShowDialog();

        }
    }
}