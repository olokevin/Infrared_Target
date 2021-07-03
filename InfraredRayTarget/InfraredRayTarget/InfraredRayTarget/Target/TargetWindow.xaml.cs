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

namespace InfraredRayTarget
{
    public partial class TargetWindow : Window
    {
        private bool m_loaded;
        public bool canMovaWindow;
        public float centerX;
        public float centerY;
        public float radius;
        public float diameter;
        public float screenMin;
        public float screenMax;
        public System.Drawing.Rectangle rect;
        private Ellipse m_curHighlight;
        public static TargetWindow self;
        public bool canClose;

        public TargetWindow()
        {
            InitializeComponent();
            self = this;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            canMovaWindow = true;
            m_loaded = true;
            m_circleCount = 4;
            AdaptationScreen();
            CreateAxislines();
            InitArmorMove();
        }

        public void SetTargetHideOrShow(Visibility _visibility)
        {
            canvas_ring.Visibility = _visibility;
        }

        //设置靶子环数
        public void SetCircleCount(int _count)
        {
            m_circleCount = _count;
            float circleSpace = radius / m_circleCount;

            canvas_ring.Children.Clear();
            Ellipse last = null;
            for (int i = m_circleCount; i >= 1; --i)
            {
                Color color = Colors.White;
                if (i % 2 != 0) color = Color.FromRgb(171, 171, 171);
                Ellipse ellipse = Painting(centerX, centerY, i * circleSpace, color, canvas_ring);
                ellipse.ToolTip = m_circleCount - i + 1 + "环";
                if (last != null)
                {
                    float stroke = (float)(last.Width - ellipse.Width) * 0.02f;
                    last.StrokeThickness = stroke;
                }
                last = ellipse;
            }
        }

        //设置背景透明度
        public void SetBackgroundOpacity(double _percent)
        {
            double percent = _percent / 100.0f;
            canvas_bg.Opacity = percent;
        }

        public double GetBackgroundOpacity()
        {
            return canvas_bg.Opacity * 100;
        }

        public void SetAxisOpacity(double _percent)
        {
            double percent = _percent / 100.0f;
            canvas_axis.Opacity = percent;
        }

        public double GetAxisOpacity()
        {
            return canvas_axis.Opacity * 100;
        }

        public void SetTargetOpacity(double _percent)
        {
            double percent = _percent / 100.0f;
            canvas_ring.Opacity = percent;
            canvas_armor.Opacity = percent;
        }

        public double GetTargetOpacity()
        {
            return canvas_ring.Opacity * 100;
        }

        //移除子弹
        public void RemoveBulletEllipseByIndex(int _index, Bullet bullet)
        {
            if (DataManager.Instance.currentGameType == GameType.Armor)
            {
                if (DataManager.Instance.currentShowBulletsType == BulletAttribute.Hit)
                {
                    if (DataManager.Instance.currentArmorType == ArmorType.Big) armor_big.RemoveBullet(_index);
                    if (DataManager.Instance.currentArmorType == ArmorType.Small) armor_small.RemoveBullet(_index);
                }
                else if (DataManager.Instance.currentShowBulletsType == BulletAttribute.Miss) RemoveBullet(_index);
            }
            else
            {
                RemoveBullet(_index);
            }
        }

        public void RemoveBullet(int _index)
        {
            if (_index == -1 || _index >= canvas_bullet.Children.Count) return;
            if (_index == canvas_bullet.Children.Count - 1)
            {
                canvas_bullet.Children.RemoveAt(_index);
                if (canvas_bullet.Children.Count > 0)
                {
                    Ellipse last = (Ellipse)canvas_bullet.Children[canvas_bullet.Children.Count - 1];
                    last.Fill = new SolidColorBrush(AttributeInfo.curBulletColor);
                    last.Stroke = new SolidColorBrush(AttributeInfo.curBulletColor);
                }
            }
            else
            {
                canvas_bullet.Children.RemoveAt(_index);
            }
        }

        //获取在几环
        public string GetBulletAtWhichCircle(double _x, double _y)
        {
            for (int i = canvas_ring.Children.Count - 1; i >= 0; --i)
            {
                Ellipse ellipse = (Ellipse)canvas_ring.Children[i];
                double radius = ellipse.Width / 2;
                double cx = Canvas.GetLeft(ellipse) + radius;
                double cy = Canvas.GetTop(ellipse) + radius;
                double dis = Math.Sqrt(Math.Pow(cx - _x, 2) + Math.Pow(cy - _y, 2));
                if (dis < ellipse.Width / 2)
                {
                    return string.Format("{0}/{1}环", canvas_ring.Children.Count, i + 1);
                }
            }
            return "出界";
        }

        //高亮靶子子弹
        public void HighlightTargetBullet(int _index)
        {
            if (_index == -1 || _index >= canvas_bullet.Children.Count) return;
            if (m_curHighlight != null)
            {
                m_curHighlight.Fill = new SolidColorBrush(AttributeInfo.BulletColor);
                m_curHighlight.Stroke = new SolidColorBrush(AttributeInfo.BulletColor);
                m_curHighlight = null;
            }
            if (_index != canvas_bullet.Children.Count - 1)
            {
                m_curHighlight = (Ellipse)canvas_bullet.Children[_index];
                m_curHighlight.Fill = new SolidColorBrush(AttributeInfo.highLightColor);
                m_curHighlight.Stroke = new SolidColorBrush(AttributeInfo.highLightColor);
            }
        }

        public void HighlightArmorBullet(int _index)
        {
            if (DataManager.Instance.currentArmorType == ArmorType.Big) armor_big.HightlightBullet(_index);
            if (DataManager.Instance.currentArmorType == ArmorType.Small) armor_small.HightlightBullet(_index);
        }

        //截图
        public string Capture(string _name)
        {
            string root = "./RM红外靶截图";
            if (!Directory.Exists(root)) Directory.CreateDirectory(root);
            string path = System.IO.Path.Combine(root, _name + ".png");

            try
            {
                RenderTargetBitmap rtp = new RenderTargetBitmap((int)ActualWidth, (int)ActualHeight, 96, 96, PixelFormats.Pbgra32);
                rtp.Render(this);
                PngBitmapEncoder png = new PngBitmapEncoder();
                png.Frames.Add(BitmapFrame.Create(rtp));
                FileStream fs = new FileStream(path, FileMode.CreateNew);
                png.Save(fs);
                fs.Close();
                fs.Dispose();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        //清除
        public void ClearAllCanvas()
        {
            armor_big.ClearBullets();
            armor_small.ClearBullets();
            canvas_bullet.Children.Clear();
            canvas_avecircle.Children.Clear();
            canvas_minicircle.Children.Clear();
            armor_big.canvas_avecircle.Children.Clear();
            armor_big.canvas_minicircle.Children.Clear();
            armor_small.canvas_avecircle.Children.Clear();
            armor_small.canvas_minicircle.Children.Clear();
        }

        //根据条件清除
        public void ClearCanvas(bool _bulletCanvas, bool _targetCanvas, bool _aveCanvas, bool _miniCanvas)
        {
            if (_bulletCanvas) canvas_bullet.Children.Clear();
            if (_aveCanvas) canvas_avecircle.Children.Clear();
            if (_miniCanvas) canvas_minicircle.Children.Clear();
        }

        //关闭
        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!canClose)
            {
                Hide();
                e.Cancel = true;
            }
        }
    }
}
