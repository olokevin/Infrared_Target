using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace InfraredRayTarget
{
    public partial class MainWindow : MetroWindow
    {
        //设置靶环
        private void OnCircleCountChange(object sender, TextChangedEventArgs e)
        {
            if (!m_loaded) return;
            circleCountBox.Background = Brushes.White;
            bool valid = int.TryParse(circleCountBox.Text, out int val);
            if (valid) circleCountSlider.Value = val;
            else circleCountBox.Background = Brushes.Red;
        }

        //设置靶环
        private void OnSetCircleCount(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!m_loaded) return;
            int count = (int)circleCountSlider.Value;
            m_target.SetCircleCount(count);
            circleCountBox.Text = count + "";
        }

        //设置装甲板速度
        private void OnSetArmorSpeed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!m_loaded) return;
            txt_armor_speed.Text = (int)slider_armor_speed.Value + "";
            TargetWindow.self.SetArmorMoveSpeed((int)slider_armor_speed.Value);
        }

        //设置装甲板移动类型
        private void OnSelectArmorMoveType(object sender, SelectionChangedEventArgs e)
        {
            if (!m_loaded) return;
            int index = com_armorMoveType.SelectedIndex;
            TargetWindow.self.SetArmorMoveType((MoveType)index);
        }

        private void OnOpenColorPanel(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog dialog = new System.Windows.Forms.ColorDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                SolidColorBrush scb = new SolidColorBrush();
                Color color = new Color();
                color.A = dialog.Color.A;
                color.B = dialog.Color.B;
                color.G = dialog.Color.G;
                color.R = dialog.Color.R;
                scb.Color = color;
                ((Button)sender).Background = scb;
                m_target.SetArmorColor(new SolidColorBrush(color));
            }
        }
    }
}
