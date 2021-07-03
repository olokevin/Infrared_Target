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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

//背景和磁力点
namespace InfraredRayTarget
{
    public partial class TargetWindow : Window
    {
        private int wheelCount;
        private int lineSpace = 50;
        private Ellipse curGauge;

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            return;
            ++wheelCount;
            if (wheelCount >= 5)
            {
                wheelCount = 0;
                if (e.Delta > 0)
                {
                    if (lineSpace >= 100) return;
                    else lineSpace += 5;
                    CreateAxislines();
                }
                else
                {
                    if (lineSpace <= 10) return;
                    else lineSpace -= 5;
                    CreateAxislines();
                }
            }
        }

        private void CreateAxislines()
        {
            return;
            canvas_axis.Children.Clear();

            //x
            for (int i = 0; i < rect.Right; i += lineSpace)
            {
                Line line = new Line();
                line.Y2 = rect.Height;
                line.Stroke = Brushes.LightGray;
                line.StrokeThickness = 1;
                canvas_axis.Children.Add(line);
                Canvas.SetLeft(line, i);
            }

            //y
            for (int i = 0; i < rect.Bottom; i += lineSpace)
            {
                Line line = new Line();
                line.X2 = rect.Width;
                line.Stroke = Brushes.LightGray;
                line.StrokeThickness = 1;
                canvas_axis.Children.Add(line);
                Canvas.SetTop(line, i);
            }

            //point
            for (int i = 0; i < rect.Right; i += lineSpace)
            {
                for (int j = 0; j < rect.Bottom; j += lineSpace)
                {
                    Ellipse ellipse = Painting(i, j, 5, Colors.Gray, canvas_axis);
                    ellipse.MouseEnter += OnMouseEnterGaugePoint;
                    ellipse.MouseLeave += OnMouseLeaveGaugePoint;
                    ellipse.Opacity = 0;
                }
            }
        }

        private void OnMouseEnterGaugePoint(object sender, MouseEventArgs e)
        {
            return;
            Ellipse ellipse = (Ellipse)sender;
            ellipse.Opacity = 100;
            curGauge = ellipse;
        }

        private void OnMouseLeaveGaugePoint(object sender, MouseEventArgs e)
        {
            return;
            Ellipse ellipse = (Ellipse)sender;
            ellipse.Opacity = 0;
            curGauge = null;
        }

        private void OnMouseEnterEllipse(object sender, MouseEventArgs e)
        {
            return;
            Ellipse ellipse = (Ellipse)sender;
            ellipse.Stroke = Brushes.Orange;
        }

        private void OnMouseLeaveEllipse(object sender, MouseEventArgs e)
        {
            return;
            Ellipse ellipse = (Ellipse)sender;
            ellipse.Stroke = ellipse.Fill;
        }
    }
}
