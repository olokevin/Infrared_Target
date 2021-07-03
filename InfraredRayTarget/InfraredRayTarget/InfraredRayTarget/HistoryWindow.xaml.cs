using MahApps.Metro.Controls;
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
using System.Windows.Shapes;

namespace InfraredRayTarget
{
    public partial class HistoryWindow : MetroWindow
    {
        public HistoryWindow()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            string root = "./RM红外靶弹道记录";
            if (!Directory.Exists(root)) Directory.CreateDirectory(root);
            DirectoryInfo directory = new DirectoryInfo(root);
            fileView.ItemsSource = directory.GetFiles();
        }

        private void OnSelectFile(object sender, SelectionChangedEventArgs e)
        {
            object select = fileView.SelectedItem;
            if (select == null) return;
            FileInfo file = (FileInfo)select;
            ServiceManager.Instance.m_serviceArr[ServiceID.UI].SendEvent(new LoadDataEvent(file.FullName));
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }
    }
}
