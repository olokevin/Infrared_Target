using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace InfraredRayTarget.Custom
{
    /// <summary>
    /// Armor.xaml 的交互逻辑
    /// </summary>
    public partial class Armor : UserControl
    {
        private Brush[] colorArr;
        private int index;

        public Armor()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            index = 0;
            PropertyInfo[] props = typeof(Brushes).GetProperties();
            colorArr = new Brush[props.Length];
            for (int i = 0; i < props.Length; ++i)
            {
                colorArr[i] = (Brush)props[i].GetValue(null, null);
            }
            colorLabel.Content = btn_smallLeft.Background.ToString();
        }

        private void OnChangeNumber(object sender, RoutedEventArgs e)
        {
            string changed = string.Empty;
            Button btn = (Button)sender;
            if (int.TryParse(btn.Content.ToString(), out int number))
            {
                ++number;
                if (number < 10) changed = number + "";
            }
            else
            {
                changed = 0 + "";
            }
            btn_smallArmor.Content = changed;
            btn_bigArmor.Content = changed;
            TargetWindow.self.SetArmorNumber(changed);
        }

        private void OnChangeArmor(object sender, RoutedEventArgs e)
        {
            if (smallArmor.Visibility == Visibility.Visible)
            {
                smallArmor.Visibility = Visibility.Hidden;
                bigArmor.Visibility = Visibility.Visible;
                DataManager.Instance.currentArmorType = ArmorType.Big;
                TargetWindow.self.SetArmorType(ArmorType.Big);
            }
            else
            {
                smallArmor.Visibility = Visibility.Visible;
                bigArmor.Visibility = Visibility.Hidden;
                DataManager.Instance.currentArmorType = ArmorType.Small;
                TargetWindow.self.SetArmorType(ArmorType.Small);
            }
            MainWindow.self.UpdateByGameTypeOrShowTypeOrArmorTypeChanged();
        }

        private void OnChangeColor(object sender, RoutedEventArgs e)
        {
            Brush color = btn_smallLeft.Background;
            if (color == Brushes.Red) ChangeColor(Brushes.Blue);
            else ChangeColor(Brushes.Red);
        }

        private void OnChangeColorMore(object sender, RoutedEventArgs e)
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
                ChangeColor(scb);
            }

            return;
            //old
            ChangeColor(colorArr[index]);
            ++index;
            if (index >= colorArr.Length) index = 0;
        }

        private void ChangeColor(Brush color)
        {
            btn_smallLeft.Background = color;
            btn_smallRight.Background = color;
            btn_bigLeft.Background = color;
            btn_bigRight.Background = color;
            TargetWindow.self.SetArmorColor(color);
            colorLabel.Content = btn_smallLeft.Background.ToString();
        }
    }
}
