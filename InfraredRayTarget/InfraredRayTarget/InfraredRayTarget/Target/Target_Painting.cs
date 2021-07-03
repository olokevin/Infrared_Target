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

//绘制圆
namespace InfraredRayTarget
{
    public partial class TargetWindow : Window
    {
        //绘制子弹数组
        public void PaintingBullets(IList<Bullet> _list, BulletAtArea _atArea)
        {
            switch (_atArea)
            {
                case BulletAtArea.Target:
                    foreach (Bullet bullet in _list) PaintingBulletToTarget(bullet);
                    break;
                case BulletAtArea.BigArmor:
                    foreach (Bullet bullet in _list) PaintingBulletToBigArmor(bullet);
                    break;
                case BulletAtArea.SmallArmor:
                    foreach (Bullet bullet in _list) PaintingBulletToSmallArmor(bullet);
                    break;
            }
        }

        //绘制子弹到画板
        public void PaintingBulletToTarget(Bullet _bullet)
        {
            Ellipse last = null;
            int count = canvas_bullet.Children.Count;
            if (count > 0) last = (Ellipse)canvas_bullet.Children[count - 1];
            if (last != null) last.Fill = new SolidColorBrush(AttributeInfo.BulletColor);
            Painting(_bullet.X, _bullet.Y, _bullet.R, AttributeInfo.curBulletColor, canvas_bullet);
            canvas_minicircle.Children.Clear();
            canvas_avecircle.Children.Clear();
        }

        //绘制子弹到大装甲里
        public void PaintingBulletToBigArmor(Bullet _bullet)
        {
            armor_big.Painting(_bullet.X, _bullet.Y, _bullet.R);
        }

        //绘制子弹到小装甲里
        public void PaintingBulletToSmallArmor(Bullet _bullet)
        {
            armor_small.Painting(_bullet.X, _bullet.Y, _bullet.R);
        }

        public bool JudgeBulletIsAtArmor(Bullet _bullet)
        {
            if (armor_big.Visibility == Visibility.Visible)
            {
                double top = Canvas.GetTop(armor_big);
                double left = Canvas.GetLeft(armor_big);
                if (_bullet.X > left && _bullet.X < left + armor_big.Width &&
                    _bullet.Y > top && _bullet.Y < top + armor_big.Height)
                {
                    _bullet.X -= (float)Canvas.GetTop(armor_big);
                    _bullet.Y -= (float)Canvas.GetLeft(armor_big);
                    _bullet.Other = "命中";
                    return true;
                }
            }
            if (armor_small.Visibility == Visibility.Visible)
            {
                double top = Canvas.GetTop(armor_small);
                double left = Canvas.GetLeft(armor_small);
                if (_bullet.X > left && _bullet.X < left + armor_small.Width &&
                    _bullet.Y > top && _bullet.Y < top + armor_small.Height)
                {
                    _bullet.X -= (float)Canvas.GetTop(armor_small);
                    _bullet.Y -= (float)Canvas.GetLeft(armor_small);
                    _bullet.Other = "命中";
                    return true;
                }
            }
            _bullet.Other = "未命中";
            return false;
        }

        //绘制子弹
        //public void PaintingBullet(Bullet _bullet)
        //{
        //    PaintingBullet(_bullet.X, _bullet.Y, _bullet.R);
        //}

        //绘制子弹
        //public void Painting(ObservableCollection<Bullet> _bullets)
        //{
        //    if (_bullets == null) return;
        //    for (int i = 0; i < _bullets.Count; ++i)
        //    {
        //        PaintingBullet(_bullets[i].X, _bullets[i].Y, _bullets[i].R);
        //    }
        //}

        //绘制子弹
        //public void PaintingBullet(float _x, float _y, float _r)
        //{
        //    if (DataManager.Instance.currentGameType == GameType.Target)
        //    {
        //        Painting(_x, _y, _r);
        //    }
        //    else if (InArmorArea(_x, _y))
        //    {
        //        if (armor_small.Visibility == Visibility.Visible) armor_small.Painting(_x, _y, _r);
        //        if (armor_big.Visibility == Visibility.Visible) armor_big.Painting(_x, _y, _r);
        //    }
        //    else
        //    {
        //        Painting(_x, _y, _r);
        //    }
        //}

        //绘制
        private void Painting(float _x, float _y, float _r)
        {
            //Ellipse last = null;
            //int count = canvas_bullet.Children.Count;
            //if (count > 0) last = (Ellipse)canvas_bullet.Children[count - 1];
            //if (last != null) last.Fill = new SolidColorBrush(AttributeInfo.BulletColor);
            //Painting(_x, _y, _r, AttributeInfo.curBulletColor, canvas_bullet);
            //canvas_minicircle.Children.Clear();
            //canvas_avecircle.Children.Clear();
        }

        //绘制最小圆范围
        public void PaintingMiniCircle(float _x, float _y, float _r)
        {
            canvas_minicircle.Children.Clear();
            if (DataManager.Instance.currentGameType == GameType.Armor &&
                DataManager.Instance.currentShowBulletsType == BulletAttribute.Hit)
            {
                if (DataManager.Instance.currentArmorType == ArmorType.Small)
                    armor_small.PaintingMiniCircle(_x, _y, _r);
                if (DataManager.Instance.currentArmorType == ArmorType.Big)
                    armor_big.PaintingMiniCircle(_x, _y, _r);
            }
            else
            {
                Painting(_x, _y, _r, AttributeInfo.MiniCircleColor, canvas_minicircle);
            }
        }

        //绘制平均距离圆
        public void PaintingAveCircle(float _x, float _y, float _r)
        {
            canvas_avecircle.Children.Clear();
            if (DataManager.Instance.currentGameType == GameType.Armor &&
                DataManager.Instance.currentShowBulletsType == BulletAttribute.Hit)
            {
                if (DataManager.Instance.currentArmorType == ArmorType.Small)
                    armor_small.PaintingAveCircle(_x, _y, _r);
                if (DataManager.Instance.currentArmorType == ArmorType.Big)
                    armor_big.PaintingAveCircle(_x, _y, _r);
            }
            else
            {
                Painting(_x, _y, _r, AttributeInfo.AveCircleColor, canvas_avecircle);
            }
        }

        //绘制随机点目标
        public void PaintingTargetByRandom()
        {
            //if (m_goalType == GoalType.Single) targetCanvas.Children.Clear();
            Random random = new Random();
            int x = random.Next(rect.Left, rect.Right);
            int y = random.Next(rect.Top, rect.Bottom);
            //Painting(x, y, m_goalRadius, m_goalColor, targetCanvas);
        }

        //绘制数据点目标
        public void PaintingTargetByData(float _x, float _y, float _r, string _color)
        {
            Color color = (Color)ColorConverter.ConvertFromString(_color);
            //Painting(_x, _y, _r, color, targetCanvas);
        }

        //绘制手动点目标
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (canMovaWindow)
            {
                DragMove();
                //AdaptationScreen();
            }
            //if (DataManager.Instance.mouseInvented)
            //{
            //    TargetProto proto = new TargetProto();
            //    Point point = Mouse.GetPosition(canvas_ring);
            //    Point p = e.GetPosition(null);
            //    proto.x = (ushort)point.X;
            //    proto.y = (ushort)point.Y;
            //    proto.type = 0;
            //    MainWindow.self.OnTargetReport(proto);
            //}
        }

        /// <summary>
        /// 绘制圆函数
        /// </summary>
        /// <param name="_x">X点</param>
        /// <param name="_y">Y点</param>
        /// <param name="_r">半径</param>
        /// <param name="_color">颜色</param>
        /// <param name="_canvas">画板</param>
        /// <returns></returns>
        private Ellipse Painting(float _x, float _y, float _r, Color _color, Canvas _canvas)
        {
            Ellipse ellipse = new Ellipse();
            ellipse.Width = _r * 2;
            ellipse.Height = _r * 2;
            ellipse.StrokeThickness = _r * 0.1f;
            //ellipse.MouseEnter += OnMouseEnterEllipse;
            //ellipse.MouseLeave += OnMouseLeaveEllipse;
            _canvas.Children.Add(ellipse);
            Canvas.SetTop(ellipse, _y - _r);
            Canvas.SetLeft(ellipse, _x - _r);
            ellipse.Fill = new SolidColorBrush(_color);
            if (_canvas != canvas_ring) ellipse.ToolTip = string.Format("{0:F2},{1:F2}", _x, _y);
            return ellipse;
        }
    }
}
