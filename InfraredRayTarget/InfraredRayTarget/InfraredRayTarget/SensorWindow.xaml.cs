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
using System.Windows.Threading;

namespace InfraredRayTarget
{
    /// <summary>
    /// SensorWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SensorWindow : MetroWindow
    {
        private Point topStartPos;
        private Point btmStartPos;
        private Point leftStartPos;
        private Point rightStartPos;

        private DataManager m_data;
        private DispatcherTimer m_timer;

        public SensorWindow()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            leftStartPos = new Point(20, 680);
            rightStartPos = new Point(1100, 680);
            topStartPos = new Point(1085, 25);
            btmStartPos = new Point(1070, 700);
            m_data = DataManager.Instance;
            InitYLamp();
            InitXLamp();
            OnUpdate(null, null);
            m_timer = new DispatcherTimer();
            m_timer.Interval = TimeSpan.FromSeconds(10);
            m_timer.Tick += OnUpdate;
            m_timer.Start();
        }

        private void OnUpdate(object sender, EventArgs e)
        {
            canvas_lightbead.Children.Clear();
            Test();
            InitYLamp();
            InitXLamp();
        }

        private void Test()
        {
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
        }

        private void InitYLamp()
        {
            StringBuilder ySB = new StringBuilder();
            ySB.Append("Y轴故障灯珠ID：\n");

            float deltaY = 0;
            for (int i = 0; i < 40; ++i)
            {
                if (i % 8 == 0) deltaY += 10;

                Image leftLamp = new Image();
                Image rightLamp = new Image();

                leftLamp.Height = 15;
                leftLamp.Width = 25;
                rightLamp.Height = 15;
                rightLamp.Width = 25;
                ScaleTransform scale = new ScaleTransform();
                scale.ScaleX = -1;
                rightLamp.RenderTransform = scale;

                if (m_data.lightbead_y[i])
                {
                    leftLamp.Source = new BitmapImage(new Uri("Images/lamp_green.png", UriKind.Relative));
                    rightLamp.Source = new BitmapImage(new Uri("Images/lamp_green.png", UriKind.Relative));
                }
                else
                {
                    leftLamp.Source = new BitmapImage(new Uri("Images/lamp_red.png", UriKind.Relative));
                    rightLamp.Source = new BitmapImage(new Uri("Images/lamp_red.png", UriKind.Relative));
                    ySB.Append(i / 8 + 1);
                    ySB.Append("-");
                    ySB.Append(i % 8 + 1);
                    ySB.Append("   ");
                }

                canvas_lightbead.Children.Add(leftLamp);
                canvas_lightbead.Children.Add(rightLamp);

                Canvas.SetTop(leftLamp, leftStartPos.Y - i * leftLamp.Height - deltaY);
                Canvas.SetLeft(leftLamp, leftStartPos.X);
                Canvas.SetTop(rightLamp, rightStartPos.Y - i * rightLamp.Height - deltaY);
                Canvas.SetLeft(rightLamp, rightStartPos.X);

                TextBlock number = new TextBlock();
                number.Text = i % 8 + 1 + "";
                canvas_lightbead.Children.Add(number);
                Canvas.SetTop(number, rightStartPos.Y - i * rightLamp.Height - deltaY);
                Canvas.SetLeft(number, 1105);
            }
            yLabel.Text = ySB.ToString();
        }

        private void InitXLamp()
        {
            StringBuilder xSB = new StringBuilder();
            xSB.Append("X轴故障灯珠ID：\n");

            float deltaX = 0;
            for (int i = 0; i < 64; ++i)
            {
                if (i % 8 == 0) deltaX += 10;
                Image topLamp = new Image();
                Image btmLamp = new Image();
                topLamp.Height = 15;
                topLamp.Width = 25;
                btmLamp.Height = 15;
                btmLamp.Width = 25;
                RotateTransform topRotate = new RotateTransform();
                topRotate.Angle = 90;
                topLamp.RenderTransform = topRotate;
                RotateTransform btmRotate = new RotateTransform();
                btmRotate.Angle = 270;
                btmLamp.RenderTransform = btmRotate;
                if (m_data.lightbead_x[i])
                {
                    topLamp.Source = new BitmapImage(new Uri("Images/lamp_green.png", UriKind.Relative));
                    btmLamp.Source = new BitmapImage(new Uri("Images/lamp_green.png", UriKind.Relative));
                }
                else
                {
                    topLamp.Source = new BitmapImage(new Uri("Images/lamp_red.png", UriKind.Relative));
                    btmLamp.Source = new BitmapImage(new Uri("Images/lamp_red.png", UriKind.Relative));
                    xSB.Append(i / 8 + 1);
                    xSB.Append("-");
                    xSB.Append(i % 8 + 1);
                    xSB.Append("   ");
                }
                canvas_lightbead.Children.Add(topLamp);
                canvas_lightbead.Children.Add(btmLamp);
                Canvas.SetTop(topLamp, topStartPos.Y);
                Canvas.SetLeft(topLamp, topStartPos.X - i * topLamp.Height - deltaX);
                Canvas.SetTop(btmLamp, btmStartPos.Y);
                Canvas.SetLeft(btmLamp, btmStartPos.X - i * btmLamp.Height - deltaX);

                TextBlock number = new TextBlock();
                number.Text = i % 8 + 1 + "";
                canvas_lightbead.Children.Add(number);
                Canvas.SetTop(number, btmStartPos.Y);
                Canvas.SetLeft(number, btmStartPos.X - i * btmLamp.Height - deltaX + 5);
            }
            xLabel.Text = xSB.ToString();
        }
    }
}
