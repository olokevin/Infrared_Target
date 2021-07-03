using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace InfraredRayTarget
{
    public partial class HelpPanel : MetroWindow
    {
        private int curPage = 1;
        private int minPage = 1;
        private int maxPage = 4;
        private bool m_loaded;
        private float startX;
        private float startY;
        private float endX;
        private float endY;
        private DataManager m_data;
        private DispatcherTimer m_timer;

        public HelpPanel()
        {
            InitializeComponent();
            startX = 40;
            startY = 20;
            endX = 430;
            endY = 225;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            m_data = DataManager.Instance;
            opacitySlider.Value = TargetWindow.self.GetBackgroundOpacity();
            //InitLightBead();
            m_loaded = true;
        }

        private void OnTick(object sender, EventArgs e)
        {
            InitLightBead();
        }

        private void OnLeftPage(object sender, RoutedEventArgs e)
        {
            if (curPage == minPage) return;
            --curPage;
            SwitchPage();
        }

        private void OnRightPage(object sender, RoutedEventArgs e)
        {
            if (curPage == maxPage) return;
            ++curPage;
            SwitchPage();
        }

        private void HideAll()
        {
            portGrid.Visibility = Visibility.Hidden;
            playerGrid.Visibility = Visibility.Hidden;
            targetGrid.Visibility = Visibility.Hidden;
            otherGrid.Visibility = Visibility.Hidden;
        }

        private void SwitchPage()
        {
            HideAll();
            switch (curPage)
            {
                case 1:
                    ShowPortPage();
                    break;
                case 2:
                    ShowPlayerPage();
                    break;
                case 3:
                    ShowTargetPage();
                    break;
                case 4:
                    ShowOtherPage();
                    break;
            }
        }

        private void ShowPortPage()
        {
            portGrid.Visibility = Visibility.Visible;
            titleBox.Text = "串口";
            pageLabel.Text = "1/4";
        }

        private void ShowPlayerPage()
        {
            playerGrid.Visibility = Visibility.Visible;
            titleBox.Text = "玩家";
            pageLabel.Text = "2/4";
        }

        private void ShowTargetPage()
        {
            targetGrid.Visibility = Visibility.Visible;
            titleBox.Text = "目标";
            pageLabel.Text = "3/4";
        }

        private void ShowOtherPage()
        {
            otherGrid.Visibility = Visibility.Visible;
            titleBox.Text = "其他";
            pageLabel.Text = "4/4";
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            Button btn = (Button)sender;
            btn.Foreground = new SolidColorBrush(Color.FromRgb(28, 151, 234));
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            Button btn = (Button)sender;
            btn.Foreground = Brushes.Black;
        }

        private void OnChangeOpacity(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            switch (componentBox.SelectedIndex)
            {
                case 0: TargetWindow.self.SetBackgroundOpacity(opacitySlider.Value); break;
                case 1: TargetWindow.self.SetAxisOpacity(opacitySlider.Value); break;
                case 2: TargetWindow.self.SetTargetOpacity(opacitySlider.Value); break;
            }
        }

        private void OnSelectComponent(object sender, SelectionChangedEventArgs e)
        {
            if (!m_loaded) return;
            switch (componentBox.SelectedIndex)
            {
                case 0: opacitySlider.Value = TargetWindow.self.GetBackgroundOpacity(); break;
                case 1: opacitySlider.Value = TargetWindow.self.GetAxisOpacity(); break;
                case 2: opacitySlider.Value = TargetWindow.self.GetTargetOpacity(); break;
            }
        }

        private void OnlyNumber(object sender, TextCompositionEventArgs e)
        {
            Regex re = new Regex("[^0-9]+");
            e.Handled = re.IsMatch(e.Text);
        }

        private void InitLightBead()
        {
            canvas_lightbead.Children.Clear();
            Random ran = new Random();
            for (int i = 1; i < 41; ++i)
            {
                if (ran.Next(0, 2) == 0) m_data.lightbead_y[i] = true;
                else m_data.lightbead_y[i] = false;
            }
            for (int i = 1; i < 65; ++i)
            {
                if (ran.Next(0, 2) == 0) m_data.lightbead_x[i] = true;
                else m_data.lightbead_x[i] = false;
            }
            StringBuilder sb_x = new StringBuilder();
            StringBuilder sb_y = new StringBuilder();
            sb_x.Append("X轴：\n");
            sb_y.Append("Y轴：\n");

            for (int i = 1; i < 41; ++i)
            {
                Ellipse currentTop = new Ellipse();
                if (m_data.lightbead_y[i]) currentTop.Fill = Brushes.Green;
                else
                {
                    currentTop.Fill = Brushes.Red;
                    sb_y.Append(i);
                    sb_y.Append(",");
                }
                currentTop.Height = 5;
                currentTop.Width = 5;
                canvas_lightbead.Children.Add(currentTop);
                Canvas.SetTop(currentTop, i * -5 + endY);
                Canvas.SetLeft(currentTop, startX);

                Ellipse currentBtm = new Ellipse();
                if (m_data.lightbead_y[i]) currentBtm.Fill = Brushes.Green;
                else currentBtm.Fill = Brushes.Red;
                currentBtm.Height = 5;
                currentBtm.Width = 5;
                canvas_lightbead.Children.Add(currentBtm);
                Canvas.SetTop(currentBtm, i * -5 + endY);
                Canvas.SetLeft(currentBtm, endX);
            }

            for (int i = 1; i < 65; ++i)
            {
                Ellipse currentLeft = new Ellipse();
                if (m_data.lightbead_x[i]) currentLeft.Fill = Brushes.Green;
                else
                {
                    currentLeft.Fill = Brushes.Red;
                    sb_x.Append(i);
                    sb_x.Append(",");
                }
                currentLeft.Height = 5;
                currentLeft.Width = 5;
                canvas_lightbead.Children.Add(currentLeft);
                Canvas.SetTop(currentLeft, startY);
                Canvas.SetLeft(currentLeft, i * -6 + endX);

                Ellipse currentRight = new Ellipse();
                if (m_data.lightbead_x[i]) currentRight.Fill = Brushes.Green;
                else currentRight.Fill = Brushes.Red;
                currentRight.Height = 5;
                currentRight.Width = 5;
                canvas_lightbead.Children.Add(currentRight);
                Canvas.SetTop(currentRight, endY);
                Canvas.SetLeft(currentRight, i * -6 + endX);
            }

            txt_x.Text = sb_x.ToString().TrimEnd(',');
            txt_y.Text = sb_y.ToString().TrimEnd(',');
        }

        private void OnOpenLampBeadPanel(object sender, RoutedEventArgs e)
        {
            SensorWindow form = new SensorWindow();
            form.ShowDialog();
        }
    }
}
