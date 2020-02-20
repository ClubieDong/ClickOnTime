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
    /// ShiftView.xaml 的交互逻辑
    /// </summary>
    public partial class ShiftView : Window
    {
        private static readonly Brush DefaultBorderBrush = new SolidColorBrush(Color.FromRgb(171, 173, 179));
        private static readonly Brush ErrorBorderBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));

        public bool? Advance { get; private set; }
        public int? Time { get; private set; }

        public ShiftView()
        {
            InitializeComponent();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Confirm(object sender, RoutedEventArgs e)
        {
            if (!Advance.HasValue || !Time.HasValue)
            {
                MessageBox.Show("请检查输入是否有误！");
                return;
            }
            DialogResult = true;
            Close();
        }

        private void AdvanceChecked(object sender, RoutedEventArgs e)
        {
            Advance = true;
        }

        private void PostponeChecked(object sender, RoutedEventArgs e)
        {
            Advance = false;
        }

        private void TimeChanged(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtTime.Text, out int time))
            {
                Time = time;
                txtTime.Text = $"{Time}";
                txtTime.BorderBrush = DefaultBorderBrush;
            }
            else
            {
                Time = null;
                txtTime.BorderBrush = ErrorBorderBrush;
            }
        }
    }
}
