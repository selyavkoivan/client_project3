using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using client.Models;
using LiveCharts;
using LiveCharts.Wpf;

namespace client
{
    public partial class LifiChartDiagram : Window
    {
        private List<Order> orders;
        private List<Order> filteredOrders;
        public LifiChartDiagram(List<Order> orders)
        {
            InitializeComponent();
            this.orders = orders;
            filteredOrders = orders;
            
            FillDiagram();
        }

        void FillDiagram()
        {
            pieChart1.Series = new SeriesCollection();
            Func<ChartPoint, string> labelPoint = chartPoint =>
                string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);
            foreach (var v in this.filteredOrders.GroupBy(o => o.product.type))
            {

                pieChart1.Series.Add(new PieSeries
                {
                    Title = v.Key,
                    Values = new ChartValues<double> { v.Count() },
                    DataLabels = true,
                    LabelPoint = labelPoint
                });
            }
        }

        private void Sunmit_OnClick(object sender, RoutedEventArgs e)
        {
            filteredOrders = orders.Where(o => o.date >= start.SelectedDate && o.date <= End.SelectedDate).ToList();
            FillDiagram();
        }
    }
}