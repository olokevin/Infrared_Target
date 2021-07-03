using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace InfraredRayTarget.Custom.CustomGoal
{
    /// <summary>
    /// ShowCustomGoalWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ShowCustomGoalWindow : MetroWindow
    {
        private bool isLoaded;
        private DataManager m_data;
        public static ShowCustomGoalWindow self;
        private ObservableCollection<ShowCustomGoalItem> items;
        private CustomOptin optin;
        private DateTime animationTime;

        public ShowCustomGoalWindow()
        {
            InitializeComponent();
            self = this;
            optin = CustomOptin.Create;
            isLoaded = true;
            animationTime = DateTime.Now;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            m_data = DataManager.Instance;
            items = new ObservableCollection<ShowCustomGoalItem>();
            foreach (KeyValuePair<string, CustomGoalInfo> cur in m_data.customGoalDic)
            {
                CustomGoalInfo info = cur.Value;
                ShowCustomGoalItem item = new ShowCustomGoalItem();
                item.Name = info.name;
                item.ShowImage = info.image.source;
                for (int i = 1; i < 20; ++i)
                    items.Add(item);
            }
            goalItems.ItemsSource = items;
        }

        private void OnCreateCustomGoalItem(object sender, RoutedEventArgs e)
        {
            TimeSpan span = DateTime.Now - animationTime;
            if (span.Seconds < 1) return;
            animationTime = DateTime.Now;
            optin = CustomOptin.Create;
            OnShowCreateCustomGoalPanel();
            Title = "创建自定义目标";
            
        }

        private void OnReviseCustomGoalItem(object sender, RoutedEventArgs e)
        {
            if (goalItems.SelectedItem == null) return;
            TimeSpan span = DateTime.Now - animationTime;
            if (span.Seconds < 1) return;
            animationTime = DateTime.Now;
            optin = CustomOptin.Revise;
            Title = "修改自定义目标";
            OnShowCreateCustomGoalPanel();
            InitEditCustomGoalScene();
        }

        private void OnShowSelectCustomGoalPanel(object sender, RoutedEventArgs e)
        {
            TimeSpan span = DateTime.Now - animationTime;
            if (span.Seconds < 1) return;
            animationTime = DateTime.Now;
            Storyboard hidden = Application.Current.Resources["CreateCustomToHidden"] as Storyboard;
            hidden.Begin(createGrid);
            Storyboard show = Application.Current.Resources["SelectCustomToShow"] as Storyboard;
            show.Begin(selectGrid);
            //selectGrid.RenderTransformOrigin = new Point(1, 1);
            //createGrid.RenderTransformOrigin = new Point(0, 0);
            Title = "自定义目标";
        }

        private void InitEditCustomGoalScene()
        {

        }

        private void OnShowCreateCustomGoalPanel()
        {
            Storyboard hidden = Application.Current.Resources["SelectCustomToHidden"] as Storyboard;
            hidden.Begin(selectGrid);
            Storyboard show = Application.Current.Resources["CreateCustomToShow"] as Storyboard;
            show.Begin(createGrid);
            //selectGrid.RenderTransformOrigin = new Point(0, 0);
            //createGrid.RenderTransformOrigin = new Point(1, 1);
        }

        private void HideAll()
        {
            btn_move.Foreground = Brushes.Gray;
            btn_rotate.Foreground = Brushes.Gray;
            btn_scale.Foreground = Brushes.Gray;
            btn_alive.Foreground = Brushes.Gray;
            btn_other.Foreground = Brushes.Gray;
            grid_move.Visibility = Visibility.Hidden;
            grid_rotate.Visibility = Visibility.Hidden;
            grid_scale.Visibility = Visibility.Hidden;
            grid_alive.Visibility = Visibility.Hidden;
            grid_other.Visibility = Visibility.Hidden;
        }

        private void OnMoveOption(object sender, RoutedEventArgs e)
        {
            HideAll();
            btn_move.Foreground = Brushes.Black;
            grid_move.Visibility = Visibility.Visible;
        }

        private void OnRotateOption(object sender, RoutedEventArgs e)
        {
            HideAll();
            btn_rotate.Foreground = Brushes.Black;
            grid_rotate.Visibility = Visibility.Visible;
        }

        private void OnScaleOption(object sender, RoutedEventArgs e)
        {
            HideAll();
            btn_scale.Foreground = Brushes.Black;
            grid_scale.Visibility = Visibility.Visible;
        }

        private void OnAliveOption(object sender, RoutedEventArgs e)
        {
            HideAll();
            btn_alive.Foreground = Brushes.Black;
            grid_alive.Visibility = Visibility.Visible;
        }

        private void OnOtherOption(object sender, RoutedEventArgs e)
        {
            HideAll();
            btn_other.Foreground = Brushes.Black;
            grid_other.Visibility = Visibility.Visible;
        }

        private void OnOpenHistoryWindow(object sender, RoutedEventArgs e)
        {
            HistoryImageWindow form = new HistoryImageWindow();
            form.ShowDialog();
        }

        private void OnDropCustomGoal(object sender, DragEventArgs e)
        {
            Array arr = (Array)e.Data.GetData(DataFormats.FileDrop);
            string path = (string)arr.GetValue(0);
            if (File.Exists(path))
            {
                string extension = System.IO.Path.GetExtension(path).ToLower();
                if (extension == ".png" ||
                    extension == ".ico" ||
                    extension == "jpeg" ||
                    extension == ".jpg")
                {
                    SetCustomGoalImage(path);
                }
                else MessageBox.Show("不符合规范");
            }
        }

        public void SetCustomGoalImage(string path)
        {
            img_goal.Source = new BitmapImage(new Uri(path));
            m_data.historyPath.Remove(path);
            m_data.historyPath.Add(path);
        }

        private void OnSliderValueChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!isLoaded) return;
            Slider slider = (Slider)sender;
            switch (slider.Name)
            {
                case "slider_move_speed":
                    txt_move_speed.Text = (int)slider.Value + "";
                    break;
                case "slider_scale_speed":
                    txt_scale_speed.Text = slider.Value.ToString("F2");
                    break;
            }
        }

        private void OnSelectCustomMoveType(object sender, SelectionChangedEventArgs e)
        {

        }

        private void OnSelectCustomGoalItem(object sender, SelectionChangedEventArgs e)
        {

        }

        private void OnDeleteCustomGoalItem(object sender, RoutedEventArgs e)
        {
            if (goalItems.SelectedItem == null) return;
            ShowCustomGoalItem item = (ShowCustomGoalItem)goalItems.SelectedItem;
            ConfirmWindow confirm = new ConfirmWindow("删除自定义目标项", item.Name, new Action<string>(OnDelectCustomGoalItem), false);
        }

        private void OnDelectCustomGoalItem(string name)
        {
            m_data.customGoalDic.Remove(name);
            ShowCustomGoalItem item = null;
            for (int i = 0; i < items.Count; ++i)
            {
                if (items[i].Name == name)
                {
                    item = items[i];
                    break;
                }
            }
            items.Remove(item);
        }

        private void OnSelectCustomGoalItem(object sender, RoutedEventArgs e)
        {
            if (goalItems.SelectedItem == null) return;
            m_data.currentCustomGoalInfo = m_data.customGoalDic[((ShowCustomGoalItem)goalItems.SelectedItem).Name];
            ServiceManager.Instance.m_serviceArr[ServiceID.UI].PostEvent(new SelectCusomGoalEvent());
            this.Close();
        }
    }
}
