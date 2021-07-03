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
using System.Windows.Threading;

//装甲板
namespace InfraredRayTarget
{
    public partial class TargetWindow : Window
    {
		private bool m_selectState = false;
        private Point m_nodePoint;
		private Point m_mousePoint;
        private FrameworkElement m_currentArmor;
        private DispatcherTimer m_armorMoveTimer;

        private void InitArmorMove()
        {
            SetArmorMoveType(MoveType.LeftRight);
            m_armorMoveTimer = new DispatcherTimer();
            m_armorMoveTimer.Interval = TimeSpan.FromMilliseconds(20);
            m_armorMoveTimer.Tick += DoMove;
            m_armorMoveTimer.Start();
        }

        public void SetGoalType(GameType _type)
        {
            switch (_type)
            {
                case GameType.Target:
                    canvas_ring.Visibility = Visibility.Visible;
                    canvas_armor.Visibility = Visibility.Hidden;
                    canvas_custom.Visibility = Visibility.Hidden;
                    SetArmorMoveEnabel(false);
                    break;
                case GameType.Armor:
                    canvas_ring.Visibility = Visibility.Hidden;
                    canvas_armor.Visibility = Visibility.Visible;
                    canvas_custom.Visibility = Visibility.Hidden;
                    SetArmorMoveEnabel(true);
                    break;
                case GameType.Custom:
                    canvas_ring.Visibility = Visibility.Hidden;
                    canvas_armor.Visibility = Visibility.Hidden;
                    canvas_custom.Visibility = Visibility.Visible;
                    SetArmorMoveEnabel(false);
                    break;
            }
        }

        private void SetMoveAction(FrameworkElement _element, MoveType _type)
        {
            int speed = 5;
            if (_element.Tag != null) speed = ((BaseMove)_element.Tag).speed;

            switch (_type)
            {
                case MoveType.UpDown: _element.Tag = new UpDownMoveAction(_element); break;
                case MoveType.LeftRight: _element.Tag = new LeftRightMoveAction(_element); break;
                case MoveType.Random: _element.Tag = new RandomMoveAction(_element); break;
                case MoveType.Rebound: _element.Tag = new ReboundMoveAction(_element); break;
            }

            BaseMove moveAction = (BaseMove)_element.Tag;
            SetMaxBound();
            moveAction.speed = speed;
            moveAction.enabel = true;
        }

        private void SetMaxBound()
        {
            BaseMove bigMoveAction = (BaseMove)armor_big.Tag;
            if (bigMoveAction != null)
            {
                bigMoveAction.maxRight = (int)Width;
                bigMoveAction.maxBottom = (int)Height;
            }

            BaseMove smallMoveAction = (BaseMove)armor_small.Tag;
            if (smallMoveAction != null)
            {
                smallMoveAction.maxRight = (int)Width;
                smallMoveAction.maxBottom = (int)Height;
            }
        }

        private void DoMove(object sender, EventArgs e)
        {
            BaseMove big = (BaseMove)armor_big.Tag;
            BaseMove small = (BaseMove)armor_small.Tag;
            big.DoMove();
            small.DoMove();
        }


        //===================================== 手移 =====================================

        private void OnMouseEnterArmor(object sender, MouseEventArgs e)
        {
            canMovaWindow = false;
            m_armorMoveTimer.Stop();
            m_currentArmor = (FrameworkElement)sender;
            if (armor_big.Visibility == Visibility.Visible) armor_big.SetBoderVisibility(Visibility.Visible);
            if (armor_small.Visibility == Visibility.Visible) armor_small.SetBoderVisibility(Visibility.Visible);
        }

        private void OnMouseLeaveArmor(object sender, MouseEventArgs e)
        {
            m_selectState = false;
            canMovaWindow = true;
            m_armorMoveTimer.Start();
            if (armor_big.Visibility == Visibility.Visible) armor_big.SetBoderVisibility(Visibility.Hidden);
            if (armor_small.Visibility == Visibility.Visible) armor_small.SetBoderVisibility(Visibility.Hidden);
        }

        private void OnHandleMoveArmorBegin(object sender, MouseButtonEventArgs e)
		{
            m_selectState = true;
			m_nodePoint.X = Canvas.GetLeft(m_currentArmor);
			m_nodePoint.Y = Canvas.GetTop(m_currentArmor);
			m_mousePoint = Mouse.GetPosition(canvas_armor);
		}

		private void OnHandleMoveArmorEnd(object sender, MouseButtonEventArgs e)
		{
            m_selectState = false;
        }

		private void OnHandleMoveArmor(object sender, MouseEventArgs e)
		{
            if (!m_selectState) return;
			Point pt = Mouse.GetPosition(canvas_armor);
			Point vec = new Point(pt.X - m_mousePoint.X, pt.Y - m_mousePoint.Y);
			Point newPos = new Point(m_nodePoint.X + vec.X, m_nodePoint.Y + vec.Y);
			Canvas.SetLeft(m_currentArmor, newPos.X);
			Canvas.SetTop(m_currentArmor, newPos.Y);
		}

        public string InArmorArea(Bullet _bullet)
        {
            if (DataManager.Instance.currentArmorType == ArmorType.Big)
            {
                double top = Canvas.GetTop(armor_big);
                double left = Canvas.GetLeft(armor_big);
                if (_bullet.X > left && _bullet.X < left + armor_big.Width &&
                    _bullet.Y > top && _bullet.Y < top + armor_big.Height)
                {
                    _bullet.Y -= (float)top;
                    _bullet.X -= (float)left;
                    return "命中";
                }
            }
            if (DataManager.Instance.currentArmorType == ArmorType.Small)
            {
                double top = Canvas.GetTop(armor_small);
                double left = Canvas.GetLeft(armor_small);
                if (_bullet.X > left && _bullet.X < left + armor_small.Width &&
                    _bullet.Y > top && _bullet.Y < top + armor_small.Height)
                {
                    _bullet.Y -= (float)top;
                    _bullet.X -= (float)left;
                    return "命中";
                }
            }
            return "未命中";
        }
        //===================================== set =====================================

        public void SetArmorNumber(string _number)
        {
            armor_big.SetNumber(_number);
            armor_small.SetNumber(_number);
        }

        public void SetArmorColor(Brush _brush)
        {
            armor_big.SetColor(_brush);
            armor_small.SetColor(_brush);
            //if (_brush == Brushes.Red) currentBulletColor = Colors.Red;
            //if (_brush == Brushes.Blue) currentBulletColor = Colors.Blue;

        }

        public void SetArmorMoveType(MoveType _type)
        {
            SetMoveAction(armor_big, _type);
            SetMoveAction(armor_small, _type);
        }

        public void SetArmorMoveEnabel(bool _enabel)
        {
            BaseMove big = (BaseMove)armor_big.Tag;
            BaseMove small = (BaseMove)armor_small.Tag;
            big.enabel = _enabel;
            small.enabel = _enabel;
            if (armor_big.Visibility == Visibility.Hidden && armor_small.Visibility == Visibility.Hidden) armor_small.Visibility = Visibility.Visible;
        }

        public void SetArmorMoveSpeed(int _speed)
        {
            BaseMove big = (BaseMove)armor_big.Tag;
            BaseMove small = (BaseMove)armor_small.Tag;
            big.speed = _speed;
            small.speed = _speed;
        }

        public void SetArmorType(ArmorType type)
        {
            if (type == ArmorType.Small)
            {
                armor_small.Visibility = Visibility.Visible;
                armor_big.Visibility = Visibility.Hidden;
                double top = Canvas.GetTop(armor_big);
                double left = Canvas.GetLeft(armor_big);
                double center = left + armor_big.Width / 2;
                double newLeft = center - armor_small.Width / 2;
                Canvas.SetTop(armor_small, top);
                Canvas.SetLeft(armor_small, newLeft);
            }
            else
            {
                armor_small.Visibility = Visibility.Hidden;
                armor_big.Visibility = Visibility.Visible;
                double top = Canvas.GetTop(armor_small);
                double left = Canvas.GetLeft(armor_small);
                double center = left + armor_small.Width / 2;
                double newLeft = center - armor_big.Width / 2;
                Canvas.SetTop(armor_big, top);
                Canvas.SetLeft(armor_big, newLeft);
            }
        }
    }
}
