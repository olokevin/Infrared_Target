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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace InfraredRayTarget.Custom
{
    /// <summary>
    /// ArmorEntity.xaml 的交互逻辑
    /// </summary>
    public partial class BigArmorEntity : UserControl
    {
        private Ellipse m_curHighlight;

        public BigArmorEntity()
        {
            InitializeComponent();
        }

        public void SetBoderVisibility(Visibility visibility)
        {
            border.Visibility = visibility;
        }

        public void SetNumber(string _number)
        {
            number.Text = _number;
        }

        public void SetColor(Brush _brush)
        {
            leftLight.Fill = _brush;
            rightLight.Fill = _brush;
        }

        public void Painting(float _x, float _y, float _r)
        {
            Ellipse last = null;
            int count = canvas_bullet.Children.Count;
            if (count > 0) last = (Ellipse)canvas_bullet.Children[count - 1];
            if (last != null) last.Fill = new SolidColorBrush(AttributeInfo.BulletColor);
            Painting(_x, _y, _r, AttributeInfo.curBulletColor, canvas_bullet);
        }

        private void Painting(float _x, float _y, float _r, Color _color, Canvas _canvas)
        {
            Ellipse ellipse = new Ellipse();
            ellipse.Width = _r * 2;
            ellipse.Height = _r * 2;
            ellipse.StrokeThickness = _r * 0.1f;
            _canvas.Children.Add(ellipse);
            Canvas.SetTop(ellipse, _y - _r);
            Canvas.SetLeft(ellipse, _x - _r);
            ellipse.Fill = new SolidColorBrush(_color);
            ellipse.ToolTip = string.Format("{0:F2},{1:F2}", _x, _y);
        }

        public void PaintingMiniCircle(float _x, float _y, float _r)
        {
            canvas_minicircle.Children.Clear();
            Painting(_x, _y, _r, AttributeInfo.MiniCircleColor, canvas_minicircle);
        }

        public void PaintingAveCircle(float _x, float _y, float _r)
        {
            canvas_avecircle.Children.Clear();
            Painting(_x, _y, _r, AttributeInfo.AveCircleColor, canvas_avecircle);
        }

        public void ClearBullets()
        {
            canvas_bullet.Children.Clear();
        }

        public void HightlightBullet(int _index)
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
    }
}
