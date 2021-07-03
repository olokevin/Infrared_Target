using MahApps.Metro.Controls;
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
using System.IO;

namespace InfraredRayTarget.Custom.CustomGoal
{
    /// <summary>
    /// HistoryImageWindow.xaml 的交互逻辑
    /// </summary>
    public partial class HistoryImageWindow : MetroWindow
    {
        private List<ShowCustomGoalItem> list;

        public HistoryImageWindow()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            list = new List<ShowCustomGoalItem>();
            List<string> pathList = DataManager.Instance.historyPath;
            for (int i = 0; i < pathList.Count; ++i)
            {
                if (File.Exists(pathList[i]))
                {
                    ShowCustomGoalItem item = new ShowCustomGoalItem();
                    item.Name = pathList[i];
                    item.ShowImage = new BitmapImage(new Uri(pathList[i]));
                    list.Add(item);
                }
            }
            imgBox.ItemsSource = list;
        }

        private void OnSelectImage(object sender, SelectionChangedEventArgs e)
        {
            ShowCustomGoalWindow.self.SetCustomGoalImage(list[imgBox.SelectedIndex].Name);
        }
    }
}
