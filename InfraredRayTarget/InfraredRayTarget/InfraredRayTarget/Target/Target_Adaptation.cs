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

//自适应屏幕
namespace InfraredRayTarget
{
    public partial class TargetWindow : Window
    {
        private int m_circleCount;
        private string m_curScreenName;

        private void AdaptationScreen()
        {
            System.Windows.Forms.Screen[] screens = System.Windows.Forms.Screen.AllScreens;
            AdaptationScreen(screens[0]);
        }

        public void AdaptationScreen(System.Windows.Forms.Screen _screen)
        {
            //System.Windows.Forms.Screen screen = GetScreen();
            //if (m_curScreenName == screen.DeviceName) return;
            //m_curScreenName = screen.DeviceName;
            rect = _screen.Bounds;
            Top = rect.Top;
            Left = rect.Left;
            Width = rect.Width;
            Height = rect.Height;
            centerX = (float)Width / 2;
            centerY = (float)Height / 2;

            double olddiameter = diameter;
            if (rect.Height > rect.Width)
            {
                screenMin = rect.Width;
                screenMax = rect.Height;
            }
            else
            {
                screenMin = rect.Height;
                screenMax = rect.Width;
            }

            diameter = screenMin;
            radius = diameter / 2;
            SetCircleCount(m_circleCount);

            Border border = (Border)canvas_bg.Children[0];
            border.Width = Width - 1;
            border.Height = Height - 1;

            SetMaxBound();
        }

        private System.Windows.Forms.Screen GetScreen()
        {
            if (WindowState == WindowState.Maximized) WindowState = WindowState.Normal;
            System.Drawing.Point mousPos = System.Windows.Forms.Control.MousePosition;
            //System.Drawing.Point mousPos = System.Windows.Forms.Cursor.Position;
            System.Windows.Forms.Screen[] screens = System.Windows.Forms.Screen.AllScreens;
            System.Windows.Forms.Screen screen = null;
            for (int i = 0; i < screens.Length; ++i)
            {
                if (mousPos.X > screens[i].Bounds.X && mousPos.Y < screens[i].Bounds.X + screens[i].Bounds.Width)
                {
                    screen = screens[i];
                    break;
                }
            }
            //if (screen == null) screen = screens[0];
            return screen == null ? screens[0] : screen;
        }
    }
}
