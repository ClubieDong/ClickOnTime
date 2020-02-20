using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ClickOnTime
{
    /// <summary>
    /// AddBatchView.xaml 的交互逻辑
    /// </summary>
    public partial class AddBatchView : Window
    {
        private static readonly Brush DefaultBorderBrush = new SolidColorBrush(Color.FromRgb(171, 173, 179));
        private static readonly Brush ErrorBorderBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));

        public DateTime? StartTime { get; private set; }
        public int? Interval { get; private set; }
        public int? Count { get; private set; }

        public AddBatchView()
        {
            InitializeComponent();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Confirm(object sender, RoutedEventArgs e)
        {
            if (!StartTime.HasValue || !Interval.HasValue ||!Count.HasValue)
            {
                MessageBox.Show("请检查输入是否有误！");
                return;
            }
            DialogResult = true;
            Close();
        }

        private void StartTimeChanged(object sender, RoutedEventArgs e)
        {
            if (DateTime.TryParse(txtStartTime.Text, out DateTime time))
            {
                StartTime = time;
                txtStartTime.Text = time.ToString("HH:mm:ss.fff");
                txtStartTime.BorderBrush = DefaultBorderBrush;
            }
            else
            {
                StartTime = null;
                txtStartTime.BorderBrush = ErrorBorderBrush;
            }
        }

        private void IntervalChanged(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtInterval.Text, out int interval))
            {
                Interval = interval;
                txtInterval.Text = $"{Interval}";
                txtInterval.BorderBrush = DefaultBorderBrush;
            }
            else
            {
                Interval = null;
                txtInterval.BorderBrush = ErrorBorderBrush;
            }
        }

        private void CountChanged(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtCount.Text, out int count))
            {
                Count = count;
                txtCount.Text = $"{Count}";
                txtCount.BorderBrush = DefaultBorderBrush;
            }
            else
            {
                Count = null;
                txtCount.BorderBrush = ErrorBorderBrush;
            }
        }
    }
}
