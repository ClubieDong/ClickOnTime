using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private int FinishCount;
        private Timer Timer = new Timer(20);

        public MainWindow()
        {
            InitializeComponent();
            Timer.Elapsed += UpdateTime;
            Timer.Start();
        }

        private void UpdateTime(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                stbTime.Content = "当前时间：" + DateTime.Now.ToString("HH:mm:ss.fff");
            });
        }

        private void Open(object sender, RoutedEventArgs e)
        {
            if (stpClickItem.Children.Count > 0)
            {
                var result = MessageBox.Show("加载将会清空现有项，确定继续吗？", "确认", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.Cancel)
                    return;
            }
            OpenFileDialog dialog = new OpenFileDialog()
            {
                Filter = "文本文件 (*.txt)|*.txt"
            };
            var succ = dialog.ShowDialog();
            if (succ.HasValue && succ.Value)
            {
                var text = File.ReadAllText(dialog.FileName).Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                stpClickItem.Children.Clear();
                foreach (var i in text)
                {
                    if (!ClickItemView.FromString(Delete, Finish, stpClickItem.Children.Count, i, out ClickItemView view))
                    {
                        stpClickItem.Children.Clear();
                        MessageBox.Show("无法读取文件！");
                        return;
                    }
                    stpClickItem.Children.Add(view);
                }
            }
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            if (stpClickItem.Children.Count == 0)
            {
                MessageBox.Show("至少添加一项！");
                return;
            }
            foreach (ClickItemView i in stpClickItem.Children)
                if (!i.IsReady)
                {
                    MessageBox.Show("请检查输入是否有误！");
                    return;
                }
            SaveFileDialog dialog = new SaveFileDialog()
            {
                Filter = "文本文件 (*.txt)|*.txt"
            };
            var succ = dialog.ShowDialog();
            if (succ.HasValue && succ.Value)
            {
                StringBuilder text = new StringBuilder();
                foreach (ClickItemView i in stpClickItem.Children)
                    text.AppendLine(i.ToString());
                File.WriteAllText(dialog.FileName, text.ToString());
            }
        }

        private void Add(object sender, RoutedEventArgs e)
        {
            stpClickItem.Children.Add(new ClickItemView(Delete, Finish, stpClickItem.Children.Count));
        }

        private void Delete(ClickItemView item)
        {
            stpClickItem.Children.Remove(item);
        }

        private void Start(object sender, RoutedEventArgs e)
        {
            if (stpClickItem.Children.Count == 0)
            {
                MessageBox.Show("至少添加一项！");
                return;
            }
            foreach (ClickItemView i in stpClickItem.Children)
                if (!i.IsReady)
                {
                    MessageBox.Show("请检查输入是否有误！");
                    return;
                }
            foreach (ClickItemView i in stpClickItem.Children)
                i.Start();
            menuStart.IsEnabled = false;
            menuStop.IsEnabled = true;
            menuAdd.IsEnabled = false;
            menuBatch.IsEnabled = false;
            menuShift.IsEnabled = false;
            menuOpen.IsEnabled = false;
            FinishCount = 0;
            stbStatus.Content = $"运行中 (0/{stpClickItem.Children.Count})";
        }

        private void Stop(object sender, RoutedEventArgs e)
        {
            foreach (ClickItemView i in stpClickItem.Children)
                i.Stop();
            menuStart.IsEnabled = true;
            menuStop.IsEnabled = false;
            menuAdd.IsEnabled = true;
            menuBatch.IsEnabled = true;
            menuShift.IsEnabled = true;
            menuOpen.IsEnabled = true;
            stbStatus.Content = "编辑";
        }

        private void Finish()
        {
            ++FinishCount;
            if (FinishCount == stpClickItem.Children.Count)
                stbStatus.Content = $"已完成 ({FinishCount}/{stpClickItem.Children.Count})";
            else
                stbStatus.Content = $"运行中 ({FinishCount}/{stpClickItem.Children.Count})";
        }

        private void AddBatch(object sender, RoutedEventArgs e)
        {
            var view = new AddBatchView
            {
                Owner = this
            };
            var result = view.ShowDialog();
            if (!result.HasValue || !result.Value)
                return;
            DateTime time = view.StartTime.Value;
            TimeSpan interval = new TimeSpan(0, 0, 0, 0, view.Interval.Value);
            for (int i = 0; i < view.Count; ++i)
            {
                stpClickItem.Children.Add(new ClickItemView(Delete, Finish, stpClickItem.Children.Count, time));
                time += interval;
            }
        }

        private void Shift(object sender, RoutedEventArgs e)
        {
            var view = new ShiftView
            {
                Owner = this
            };
            var result = view.ShowDialog();
            if (!result.HasValue || !result.Value)
                return;
            TimeSpan time = new TimeSpan(0, 0, 0, 0, view.Time.Value);
            foreach (ClickItemView i in stpClickItem.Children)
                i.Shift(time, view.Advance.Value);
        }
    }
}
