using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}