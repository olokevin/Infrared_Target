using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace InfraredRayTarget
{
    public partial class MainWindow : MetroWindow
    {
        //红外靶菜单打开时
        private void OnMenuOpen(object sender, RoutedEventArgs e)
        {
            Visibility targetVis = m_target.Visibility;
            if (targetVis == Visibility.Visible) btnChangeSize.Header = "隐藏";
            else btnChangeSize.Header = "显示";
        }

        //保存文档
        private void OnSaveData(object sender, RoutedEventArgs e)
        {
            if (m_curPlayer == null) ShowLog(LogType.Error, "没有选择要保存的玩家数据");
            else
            {
                int sum = m_curPlayer.target_all.Count + m_curPlayer.bigArmor_all.Count + m_curPlayer.smallArmor_all.Count;
                if (sum == 0) ShowLog(LogType.Warning, "该玩家没有弹道数据");
                else
                {
                    string fileName = string.Format("{0}弹道数据 {1}", m_curPlayer.PlayerName, DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒"));
                    ConfirmWindow confirm = new ConfirmWindow("保存数据", fileName, new Action<string>(SaveData));
                }
            }
        }

        //打开导入面板
        private void OnOpenHistoryPanel(object sender, RoutedEventArgs e)
        {
            HistoryWindow form = new HistoryWindow();
            form.ShowDialog();
        }

        //靶子截图
        private void OnCaptureTarget(object sender, RoutedEventArgs e)
        {
            string name = string.Empty;
            int index = playerView.SelectedIndex;
            if (index == -1)
            {
                name = string.Format("截图 {0}", DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒"));
            }
            else
            {
                ListViewItem item = (ListViewItem)playerView.Items[index];
                Player player = (Player)item.Content;
                name = string.Format("{0} {1}", player.PlayerName, DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒"));
            }
            ConfirmWindow confirm = new ConfirmWindow("截图命名", name, new Action<string>(OnCapureCallback));
        }

        //截图回调
        private void OnCapureCallback(string name)
        {
            string result = m_target.Capture(name);
            if (string.IsNullOrWhiteSpace(result)) ShowLog(LogType.Suc, "已截图“{0}”", name);
            else ShowLog(LogType.Error, "截图失败，原因：{0}", result);
        }

        //隐藏显示靶子
        private void OnShowOrHideTarget(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            if (menuItem.Header.ToString() == "隐藏") m_target.Hide();
            else
            {
                m_target.Show();
                m_target.WindowState = WindowState.Normal;
            }
        }

        //清除最小圆画板
        private void OnClearMiniCircleCanvas(object sender, RoutedEventArgs e)
        {
            m_target.canvas_minicircle.Children.Clear();
            m_target.armor_big.canvas_minicircle.Children.Clear();
            m_target.armor_small.canvas_minicircle.Children.Clear();
        }

        //清除平均圆画板
        private void OnClearAveCircleCanvas(object sender, RoutedEventArgs e)
        {
            m_target.canvas_avecircle.Children.Clear();
            m_target.armor_big.canvas_avecircle.Children.Clear();
            m_target.armor_small.canvas_avecircle.Children.Clear();
        }

        //打开帮助面板
        private void OnOpenHelpPanel(object sender, RoutedEventArgs e)
        {
            HelpPanel form = new HelpPanel();
            form.ShowDialog();
        }

        //清除日志
        private void OnClearLogs(object sender, RoutedEventArgs e)
        {
            m_observableLogList.Clear();
        }

        //打开截图文件夹
        private void OnNavigationCapturePanel(object sender, RoutedEventArgs e)
        {
            string root = System.IO.Directory.GetCurrentDirectory();
            string folder = System.IO.Path.Combine(root, "RM红外靶截图");
            if (!System.IO.Directory.Exists(folder)) System.IO.Directory.CreateDirectory(folder);
            System.Diagnostics.Process.Start(folder);
        }

        //打开文档文件夹
        private void OnNavigationData(object sender, RoutedEventArgs e)
        {
            string root = System.IO.Directory.GetCurrentDirectory();
            string folder = System.IO.Path.Combine(root, "RM红外靶弹道记录");
            if (!System.IO.Directory.Exists(folder)) System.IO.Directory.CreateDirectory(folder);
            System.Diagnostics.Process.Start(folder);
        }

        //模拟小子弹
        private void OnInventedSmall(object sender, RoutedEventArgs e)
        {
            Random random = new Random();
            int x = random.Next(0, m_target.rect.Width);
            int y = random.Next(0, m_target.rect.Height);
            TargetReport(x, y, 0);
        }

        //模拟大子弹
        private void OnInventedBig(object sender, RoutedEventArgs e)
        {
            Random random = new Random();
            int x = random.Next(0, m_target.rect.Width);
            int y = random.Next(0, m_target.rect.Height);
            TargetReport(x, y, 1);
        }

        private void OnShowAllBullets(object sender, RoutedEventArgs e)
        {
            menu_showAll.IsChecked = true;
            menu_showHit.IsChecked = false;
            menu_showMiss.IsChecked = false;
            menu_showType.Header = "显示全部弹道";
            m_data.currentShowBulletsType = BulletAttribute.All;
            UpdateByGameTypeOrShowTypeOrArmorTypeChanged();
        }

        private void OnShowHitBullets(object sender, RoutedEventArgs e)
        {
            menu_showAll.IsChecked = false;
            menu_showHit.IsChecked = true;
            menu_showMiss.IsChecked = false;
            menu_showType.Header = "显示命中弹道";
            m_data.currentShowBulletsType = BulletAttribute.Hit;
            UpdateByGameTypeOrShowTypeOrArmorTypeChanged();
        }

        private void OnShowMissBullets(object sender, RoutedEventArgs e)
        {
            menu_showAll.IsChecked = false;
            menu_showHit.IsChecked = false;
            menu_showMiss.IsChecked = true;
            menu_showType.Header = "显示未命中弹道";
            m_data.currentShowBulletsType = BulletAttribute.Miss;
            UpdateByGameTypeOrShowTypeOrArmorTypeChanged();
        }
    }
}
