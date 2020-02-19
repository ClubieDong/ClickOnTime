using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClickOnTime
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void AddClickItem(object sender, RoutedEventArgs e)
        {
            stpClickItem.Children.Add(new ClickItemView(DeleteClickItem));
        }

        private void DeleteClickItem(ClickItemView item)
        {
            stpClickItem.Children.Remove(item);
        }

        private void StartOrStop(object sender, RoutedEventArgs e)
        {
            if ((string)btnStartOrStop.Content == "开始")
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
                btnStartOrStop.Content = "结束";
                btnAdd.IsEnabled = false;
                btnLoad.IsEnabled = false;
            }
            else
            {
                foreach (ClickItemView i in stpClickItem.Children)
                    i.Stop();
                btnStartOrStop.Content = "开始";
                btnAdd.IsEnabled = true;
                btnLoad.IsEnabled = true;
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

        private void Load(object sender, RoutedEventArgs e)
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
                    if (!ClickItemView.FromString(DeleteClickItem, i, out ClickItemView view))
                    {
                        stpClickItem.Children.Clear();
                        MessageBox.Show("无法读取文件！");
                        return;
                    }
                    stpClickItem.Children.Add(view);
                }
            }
        }
    }
}
