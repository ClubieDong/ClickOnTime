using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClickOnTime
{
    /// <summary>
    /// ClickItemView.xaml 的交互逻辑
    /// </summary>
    public partial class ClickItemView : UserControl
    {
        private static readonly Brush DefaultBorderBrush = new SolidColorBrush(Color.FromRgb(171, 173, 179));
        private static readonly Brush ErrorBorderBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));

        private bool IsRunning = false;
        public WinAPI.POINT? Point { get; private set; }
        public DateTime? Time { get; private set; }

        public bool IsReady
        {
            get => Point.HasValue && Time.HasValue;
        }
        public event Action<ClickItemView> OnDelete;
        public event Action OnFinish;

        public ClickItemView(Action<ClickItemView> onDelete, Action onFinish, int id)
        {
            InitializeComponent();
            OnDelete += onDelete;
            OnFinish += onFinish;
            tbID.Text = $"No. {id + 1}";
        }
        public ClickItemView(Action<ClickItemView> onDelete, Action onFinish, int id, DateTime time)
        {
            InitializeComponent();
            OnDelete += onDelete;
            OnFinish += onFinish;
            tbID.Text = $"No. {id + 1}";
            Time = time;
            txtTime.Text = time.ToString("HH:mm:ss.fff");
        }

        public void Start()
        {
            txtTime.IsReadOnly = true;
            btnDelete.Visibility = Visibility.Hidden;
            tbWaiting.Visibility = Visibility.Visible;
            tbFinished.Visibility = Visibility.Hidden;
            IsRunning = true;
            Task.Run(Run);
        }
        public void Stop()
        {
            IsRunning = false;
            txtTime.IsReadOnly = false;
            btnDelete.Visibility = Visibility.Visible;
            tbWaiting.Visibility = Visibility.Hidden;
            tbFinished.Visibility = Visibility.Hidden;
        }

        private static readonly TimeSpan MinTime = new TimeSpan(0, 0, 0, 0, 0);
        private static readonly TimeSpan MaxTime = new TimeSpan(0, 0, 0, 1, 0);
        private void Run()
        {
            while (IsRunning)
            {
                Thread.Sleep(1);
                var delta = DateTime.Now.TimeOfDay - Time.Value.TimeOfDay;
                if (MinTime <= delta && delta <= MaxTime)
                {
                    WinAPI.Click(Point.Value);
                    IsRunning = false;
                    Dispatcher.Invoke(() =>
                    {
                        tbWaiting.Visibility = Visibility.Hidden;
                        tbFinished.Visibility = Visibility.Visible;
                        OnFinish?.Invoke();
                    });
                }
            }
        }

        private void Delete(object sender, RoutedEventArgs e)
        {
            OnDelete?.Invoke(this);
        }

        private void SpaceDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Space)
                return;
            if (WinAPI.GetCursorPos(out WinAPI.POINT point))
            {
                txtPos.Text = $"({point.x}, {point.y})";
                Point = point;
                txtPos.BorderBrush = DefaultBorderBrush;
            }
            else
            {
                txtPos.Text = string.Empty;
                Point = null;
                txtPos.BorderBrush = ErrorBorderBrush;
            }
        }

        private void TimeChanged(object sender, RoutedEventArgs e)
        {
            if (DateTime.TryParse(txtTime.Text, out DateTime time))
            {
                Time = time;
                txtTime.Text = time.ToString("HH:mm:ss.fff");
                txtTime.BorderBrush = DefaultBorderBrush;
            }
            else
            {
                Time = null;
                txtTime.BorderBrush = ErrorBorderBrush;
            }
        }

        public override string ToString()
        {
            return Time.Value.ToString("HH:mm:ss.fff") + $" ({Point.Value.x}, {Point.Value.y})";
        }
        public static bool FromString(Action<ClickItemView> onDelete, Action onFinish, int id, string str, out ClickItemView view)
        {
            view = null;
            if (!DateTime.TryParse(str.Substring(0, 12), out DateTime time))
                return false;
            int commaPos = str.IndexOf(',');
            if (!int.TryParse(str.Substring(14, commaPos - 14), out int x))
                return false;
            if (!int.TryParse(str.Substring(commaPos + 2, str.Length - commaPos - 3), out int y))
                return false;
            view = new ClickItemView(onDelete, onFinish, id)
            {
                Time = time,
                Point = new WinAPI.POINT()
                {
                    x = x,
                    y = y
                }
            };
            view.txtTime.Text = time.ToString("HH:mm:ss.fff");
            view.txtPos.Text = $"({x}, {y})";
            return true;
        }

        public void Shift(TimeSpan time, bool advance)
        {
            if (!Time.HasValue)
                return;
            if (advance)
                Time -= time;
            else
                Time += time;
            txtTime.Text = Time.Value.ToString("HH:mm:ss.fff");
        }
    }
}
