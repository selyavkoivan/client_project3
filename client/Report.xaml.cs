using System.IO;
using System.Windows;

namespace client
{
    public partial class Report : Window
    {
        private string location;
        public Report(string location)
        {
            this.location = location;
            InitializeComponent();
            report.Text = location;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(report.Text);
        }
    }
}