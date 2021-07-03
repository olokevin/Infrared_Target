using InfraredRayTarget.Custom.CustomGoal;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace InfraredRayTarget
{
    public partial class MainWindow : MetroWindow
    {
        private bool m_loaded;
        private int m_playerIndex;

        private Player m_curPlayer;
        private DataManager m_data;
        private TargetWindow m_target;
        public static MainWindow self;
        private EventNotifier m_notifier;

        private ObservableCollection<ListViewItem> m_observableLogList;
        private ObservableCollection<ListViewItem> m_observablePortList;
        private ObservableCollection<ListViewItem> m_observablePlayerList;

        private int showFrame = 0;
        private int logicFrame = 0;
        private DateTime m_lastTime = DateTime.Now;

        public MainWindow()
        {
            InitializeComponent();
            self = this;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            m_playerIndex = 0;
            m_data = DataManager.Instance;

            m_observableLogList = new ObservableCollection<ListViewItem>();
            m_observablePortList = new ObservableCollection<ListViewItem>();
            m_observablePlayerList = new ObservableCollection<ListViewItem>();

            logView.ItemsSource = m_observableLogList;
            view_serial.ItemsSource = m_observablePortList;
            playerView.ItemsSource = m_observablePlayerList;

            m_notifier = new EventNotifier(ServiceID.UI);
            m_notifier.AddEvent(EventDef.Message, new Action<MessageEvent>(OnShowLog));
            m_notifier.AddEvent(EventDef.UpdatePorts, new Action<UpdatePortsEvent>(OnUpdatePorts));
            m_notifier.AddEvent(EventDef.LoadData, new Action<LoadDataEvent>(OnLoadData));
            m_notifier.AddEvent(EventDef.TargetProto, new Action<RecvProtoEvent>(OnTargetReport));
            m_notifier.AddEvent(EventDef.SelectCustomGoal, new Action<SelectCusomGoalEvent>(OnSelectCustomGoal));
            m_notifier.AddEvent(EventDef.CMD_ID_QUERY_DEVICE_INFO, new Action<RecvProtoEvent>(OnQueryAck));
            m_notifier.AddEvent(EventDef.DownloadConfigFinish, new Action<DownloadConfigFinishEvent>(OnConfigFinish));

            InitTimer();

            m_target = new TargetWindow();
            m_target.Show();
            AddPlyer("Temp");

            m_loaded = true;
        }

        //显示日志
        private void ShowLog(LogType type, string message, params object[] args)
        {
            OnShowLog(new MessageEvent(type, message, args));
        }

        //显示日志
        private void OnShowLog(MessageEvent ev)
        {
            if (m_observableLogList.Count > 1000) m_observableLogList.Clear();
            ListViewItem item = new ListViewItem();
            LogItem logItem = new LogItem();
            logItem.Time = ev.time;
            logItem.Message = ev.message;
            switch (ev.type)
            {
                case LogType.Suc:
                    item.Foreground = Brushes.Green;
                    Debug.LogSuc(ev.message);
                    break;
                case LogType.Warning:
                    item.Foreground = Brushes.Orange;
                    Debug.LogWarning(ev.message);
                    break;
                case LogType.Error:
                    item.Foreground = Brushes.Red;
                    Debug.LogError(ev.message);
                    break;
                default:
                    item.Foreground = Brushes.Black;
                    Debug.Log(ev.message);
                    break;
            }
            item.Content = logItem;
            m_observableLogList.Add(item);
            logView.ScrollIntoView(item);
        }

        private void OnSelectCustomGoal(SelectCusomGoalEvent ev)
        {
            img_custom.Source = m_data.currentCustomGoalInfo.image.source;
            txt_customName.Text = m_data.currentCustomGoalInfo.name;
        }

        //计算平均距离圆
        private void LogicAveCircle()
        {
            CircleData result = Algorithm.CalAveCircle(m_curPlayer, GetCurrentList());
            if (result != null)
            {
                aveSX.Text = result.sx;
                aveSY.Text = result.sy;
                aveSRadius.Text = result.sr;
                aveSCount.Text = result.sCount;
                aveJX.Text = result.jx;
                aveJY.Text = result.jy;
                aveJRadius.Text = result.jr;
                aveJCount.Text = result.jCount;
            }
            else
            {
                aveSX.Text = string.Empty;
                aveSY.Text = string.Empty;
                aveSRadius.Text = string.Empty;
                aveSCount.Text = string.Empty;
                aveJX.Text = string.Empty;
                aveJY.Text = string.Empty;
                aveJRadius.Text = string.Empty;
                aveJCount.Text = string.Empty;
            }
        }

        //计算最小圆范围
        private void LogicMiniCircle()
        {
            CircleData result = Algorithm.CalMiniCircle(m_curPlayer, GetCurrentList());
            if (result != null)
            {
                minSX.Text = result.sx;
                minSY.Text = result.sy;
                minSRadius.Text = result.sr;
                minSCount.Text = result.sCount;
                minJX.Text = result.jx;
                minJY.Text = result.jy;
                minJRadius.Text = result.jr;
                minJCount.Text = result.jCount;
            }
            else
            {
                minSX.Text = string.Empty;
                minSY.Text = string.Empty;
                minSRadius.Text = string.Empty;
                minSCount.Text = string.Empty;
                minJX.Text = string.Empty;
                minJY.Text = string.Empty;
                minJRadius.Text = string.Empty;
                minJCount.Text = string.Empty;
            }
        }

        //计算差
        private void LogicCalSta()
        {
            Algorithm.CalSta(GetCurrentList(), out double value1, out double value2);
            variance.Text = value1.ToString("F2");
            deviation.Text = value2.ToString("F2");
        }

        //清除数据
        private void ClearData()
        {
            m_target.canvas_avecircle.Children.Clear();
            m_target.canvas_minicircle.Children.Clear();

            minSX.Text = string.Empty;
            minSY.Text = string.Empty;
            minSRadius.Text = string.Empty;
            minSCount.Text = string.Empty;
            minJX.Text = string.Empty;
            minJY.Text = string.Empty;
            minJRadius.Text = string.Empty;
            minJCount.Text = string.Empty;
            aveSX.Text = string.Empty;
            aveSY.Text = string.Empty;
            aveSRadius.Text = string.Empty;
            aveSCount.Text = string.Empty;
            aveJX.Text = string.Empty;
            aveJY.Text = string.Empty;
            aveJRadius.Text = string.Empty;
            aveJCount.Text = string.Empty;
            variance.Text = string.Empty;
            deviation.Text = string.Empty;
        }

        //========================================== 其他 =========================================

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            m_target.canClose = true;
            m_target.Close();
        }

        private void OnlyNumber(object sender, TextCompositionEventArgs e)
        {
            Regex re = new Regex("[^0-9]+");
            e.Handled = re.IsMatch(e.Text);
        }

        private void OnOpenCustomGoalWindow(object sender, RoutedEventArgs e)
        {
            ShowCustomGoalWindow form = new ShowCustomGoalWindow();
            form.ShowDialog();
        }

        private void OnAutoReckon(object sender, RoutedEventArgs e)
        {
            ReckonData();
        }

        private void OnUnAutoReckon(object sender, RoutedEventArgs e)
        {
            //ClearData();
        }

        private void OnClickScreenItem(object sender, MouseButtonEventArgs e)
        {
            if (view_screen.SelectedItem == null) return;
            m_target.Show();
            m_target.AdaptationScreen((System.Windows.Forms.Screen)view_screen.SelectedItem);
        }

        private void OnOpenLampBeadPanel(object sender, RoutedEventArgs e)
        {
            SensorWindow form = new SensorWindow();
            form.ShowDialog();
        }

        private void OnTopmost(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            if (item.IsChecked)
            {
                item.IsChecked = false;
                Topmost = false;
                ShowLog(LogType.Warning, "取消置顶");
            }
            else
            {
                item.IsChecked = true;
                Topmost = true;
                ShowLog(LogType.Warning, "已置顶");
            }
        }
    }
}
