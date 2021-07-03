using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

//切换视图
namespace InfraredRayTarget
{
    public partial class MainWindow : MetroWindow
    {
        //显示串口视图
        private void OnShowSerialView(object sender, RoutedEventArgs e)
        {
            if (!m_loaded) return;
            btn_screen.Foreground = Brushes.Gray;
            btn_serial.Foreground = Brushes.Black;
            btn_firmware.Foreground = Brushes.Gray;
            view_screen.Visibility = Visibility.Hidden;
            view_serial.Visibility = Visibility.Visible;
            grid_firmware.Visibility = Visibility.Hidden;
        }

        //显示固件视图
        private void OnShowFirmwareGrid(object sender, RoutedEventArgs e)
        {
            if (!m_loaded) return;
            btn_serial.Foreground = Brushes.Gray;
            btn_screen.Foreground = Brushes.Gray;
            btn_firmware.Foreground = Brushes.Black;
            view_screen.Visibility = Visibility.Hidden;
            view_serial.Visibility = Visibility.Hidden;
            grid_firmware.Visibility = Visibility.Visible;
            ShowLog(LogType.Warning, "正在查询远程固件版本...");
            //m_notifier.PostEvent(ServiceID.UpdateFirmware, new DownloadConfigEvent());
            m_notifier.PostEvent(ServiceID.UI, new DownloadConfigFinishEvent());
        }

        //显示屏幕视图
        private void OnShowScreenView(object sender, RoutedEventArgs e)
        {
            if (!m_loaded) return;
            btn_serial.Foreground = Brushes.Gray;
            btn_screen.Foreground = Brushes.Black;
            btn_firmware.Foreground = Brushes.Gray;
            view_serial.Visibility = Visibility.Hidden;
            view_screen.Visibility = Visibility.Visible;
            grid_firmware.Visibility = Visibility.Hidden;
            view_screen.ItemsSource = System.Windows.Forms.Screen.AllScreens;
        }

        //显示靶子视图
        private void OnShowTargetGrid(object sender, RoutedEventArgs e)
        {
            if (!m_loaded) return;
            btn_target.Foreground = Brushes.Black;
            btn_armor.Foreground = Brushes.Gray;
            btn_custom.Foreground = Brushes.Gray;
            grid_goal_target.Visibility = Visibility.Visible;
            grid_goal_armor.Visibility = Visibility.Hidden;
            grid_goal_custom.Visibility = Visibility.Hidden;
            m_data.currentGameType = GameType.Target;
            UpdateByGameTypeOrShowTypeOrArmorTypeChanged();
            TargetWindow.self.SetGoalType(m_data.currentGameType);
        }

        //显示装甲板视图
        private void OnShowArmorGrid(object sender, RoutedEventArgs e)
        {
            if (!m_loaded) return;
            btn_target.Foreground = Brushes.Gray;
            btn_armor.Foreground = Brushes.Black;
            btn_custom.Foreground = Brushes.Gray;
            grid_goal_target.Visibility = Visibility.Hidden;
            grid_goal_armor.Visibility = Visibility.Visible;
            grid_goal_custom.Visibility = Visibility.Hidden;
            m_data.currentGameType = GameType.Armor;
            UpdateByGameTypeOrShowTypeOrArmorTypeChanged();
            TargetWindow.self.SetGoalType(m_data.currentGameType);
        }

        //显示自定义视图
        private void OnShowCustomGrid(object sender, RoutedEventArgs e)
        {
            if (!m_loaded) return;
            btn_target.Foreground = Brushes.Gray;
            btn_armor.Foreground = Brushes.Gray;
            btn_custom.Foreground = Brushes.Black;
            grid_goal_target.Visibility = Visibility.Hidden;
            grid_goal_armor.Visibility = Visibility.Hidden;
            grid_goal_custom.Visibility = Visibility.Visible;
            m_data.currentGameType = GameType.Custom;
            TargetWindow.self.SetGoalType(m_data.currentGameType);
        }

        //显示平均距离圆视图
        private void OnShowAveCircleGrid(object sender, RoutedEventArgs e)
        {
            if (!m_loaded) return;
            aveCircleGrid.Visibility = Visibility.Visible;
            miniCircleGrid.Visibility = Visibility.Hidden;
            calStaGrid.Visibility = Visibility.Hidden;
            aveBtn.Foreground = Brushes.Black;
            miniBtn.Foreground = Brushes.Gray;
            calStaBtn.Foreground = Brushes.Gray;
            m_target.canvas_avecircle.Children.Clear();
            LogicAveCircle();
        }

        //显示最小圆视图
        private void OnShowMiniCircle(object sender, RoutedEventArgs e)
        {
            if (!m_loaded) return;
            aveCircleGrid.Visibility = Visibility.Hidden;
            miniCircleGrid.Visibility = Visibility.Visible;
            calStaGrid.Visibility = Visibility.Hidden;
            aveBtn.Foreground = Brushes.Gray;
            miniBtn.Foreground = Brushes.Black;
            calStaBtn.Foreground = Brushes.Gray;
            m_target.canvas_minicircle.Children.Clear();
            LogicMiniCircle();
        }

        //显示标准差视图
        private void OnShowCalSta(object sender, RoutedEventArgs e)
        {
            if (!m_loaded) return;
            aveCircleGrid.Visibility = Visibility.Hidden;
            miniCircleGrid.Visibility = Visibility.Hidden;
            calStaGrid.Visibility = Visibility.Visible;
            aveBtn.Foreground = Brushes.Gray;
            miniBtn.Foreground = Brushes.Gray;
            calStaBtn.Foreground = Brushes.Black;
            LogicCalSta();
        }

        public void UpdateByGameTypeOrShowTypeOrArmorTypeChanged()
        {
            ClearData();
            m_target.ClearAllCanvas();
            dataView.ItemsSource = null;
            if (m_curPlayer == null) return;
            if (m_data.currentGameType == GameType.Armor) UpdateByArmor();
            if (m_data.currentGameType == GameType.Target) UpdateByTarget();
            if (check_autoReckon.IsChecked.Value) ReckonData();
            m_curPlayer.OnPropertyChanged();
        }

        private void UpdateByTarget()
        {
            IList<Bullet> list = null;
            if (m_data.currentShowBulletsType == BulletAttribute.All) list = m_curPlayer.target_all;
            if (m_data.currentShowBulletsType == BulletAttribute.Hit) list = m_curPlayer.target_hit;
            if (m_data.currentShowBulletsType == BulletAttribute.Miss) list = m_curPlayer.target_miss;
            dataView.ItemsSource = list;
            m_target.PaintingBullets(list, BulletAtArea.Target);
        }

        private void UpdateByArmor()
        {
            IList<Bullet> list = null;
            if (m_data.currentArmorType == ArmorType.Big)
            {
                if (m_data.currentShowBulletsType == BulletAttribute.All) list = m_curPlayer.bigArmor_all;
                if (m_data.currentShowBulletsType == BulletAttribute.Hit) list = m_curPlayer.bigArmor_hit;
                if (m_data.currentShowBulletsType == BulletAttribute.Miss) list = m_curPlayer.bigArmor_miss;
            }
            if (m_data.currentArmorType == ArmorType.Small)
            {
                if (m_data.currentShowBulletsType == BulletAttribute.All) list = m_curPlayer.smallArmor_all;
                if (m_data.currentShowBulletsType == BulletAttribute.Hit) list = m_curPlayer.smallArmor_hit;
                if (m_data.currentShowBulletsType == BulletAttribute.Miss) list = m_curPlayer.smallArmor_miss;
            }
            dataView.ItemsSource = list;

            if (m_data.currentShowBulletsType == BulletAttribute.All)
            {
                if (m_data.currentArmorType == ArmorType.Big)
                {
                    m_target.PaintingBullets(m_curPlayer.bigArmor_hit, BulletAtArea.BigArmor);
                    m_target.PaintingBullets(m_curPlayer.bigArmor_miss, BulletAtArea.Target);
                }
                if (m_data.currentArmorType == ArmorType.Small)
                {
                    m_target.PaintingBullets(m_curPlayer.smallArmor_hit, BulletAtArea.SmallArmor);
                    m_target.PaintingBullets(m_curPlayer.smallArmor_miss, BulletAtArea.Target);
                }
            }
            if (m_data.currentShowBulletsType == BulletAttribute.Hit)
            {
                if (m_data.currentArmorType == ArmorType.Big)
                    m_target.PaintingBullets(m_curPlayer.bigArmor_hit, BulletAtArea.BigArmor);
                if (m_data.currentArmorType == ArmorType.Small)
                    m_target.PaintingBullets(m_curPlayer.smallArmor_hit, BulletAtArea.SmallArmor);
            }
            if (m_data.currentShowBulletsType == BulletAttribute.Miss)
            {
                if (m_data.currentArmorType == ArmorType.Big)
                    m_target.PaintingBullets(m_curPlayer.bigArmor_miss, BulletAtArea.Target);
                if (m_data.currentArmorType == ArmorType.Small)
                    m_target.PaintingBullets(m_curPlayer.smallArmor_miss, BulletAtArea.Target);
            }
        }

        private IList<Bullet> GetCurrentList()
        {
            if (m_curPlayer == null) return null;
            if (m_data.currentGameType == GameType.Target)
            {
                if (m_data.currentShowBulletsType == BulletAttribute.All) return m_curPlayer.target_all;
                if (m_data.currentShowBulletsType == BulletAttribute.Hit) return m_curPlayer.target_hit;
                if (m_data.currentShowBulletsType == BulletAttribute.Miss) return m_curPlayer.target_miss;
            }
            if (m_data.currentGameType == GameType.Armor)
            {
                if (m_data.currentShowBulletsType == BulletAttribute.Hit && m_data.currentArmorType == ArmorType.Big)
                    return m_curPlayer.bigArmor_hit;
                if (m_data.currentShowBulletsType == BulletAttribute.Miss && m_data.currentArmorType == ArmorType.Big)
                    return m_curPlayer.bigArmor_miss;
                if (m_data.currentShowBulletsType == BulletAttribute.Hit && m_data.currentArmorType == ArmorType.Small)
                    return m_curPlayer.smallArmor_hit;
                if (m_data.currentShowBulletsType == BulletAttribute.Miss && m_data.currentArmorType == ArmorType.Small)
                    return m_curPlayer.smallArmor_miss;
            }
            return null;
        }
    }
}
