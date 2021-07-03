using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

public class RandomMoveAction : BaseMove
{
    private int m_moveTime;
    private DateTime m_lastTime;
    private RandomVerticalMoveType m_vertical;
    private RandomHorizontalMoveType m_horizontal;

    public RandomMoveAction(FrameworkElement _parent) : base(_parent)
    {
        RandomData();
    }

    private void RandomData()
    {
        m_lastTime = DateTime.Now;
        m_moveTime = m_random.Next(1, 5);
        m_vertical = (RandomVerticalMoveType)m_random.Next(0, 3);
        m_horizontal = (RandomHorizontalMoveType)m_random.Next(0, 3);
    }

    public override void DoMove()
    {
        if (!enabel) return;
        DateTime now = DateTime.Now;

        if ((now - m_lastTime).Seconds > m_moveTime)
        {
            RandomData();
            return;
        }

        if (m_vertical == RandomVerticalMoveType.Null &&
            m_horizontal == RandomHorizontalMoveType.Null)
        {
            RandomData();
            return;
        }

        double top = Canvas.GetTop(m_parent);
        double left = Canvas.GetLeft(m_parent);

        if (m_horizontal == RandomHorizontalMoveType.Left)
        {
            left -= speed;
            if (left < 0)
            {
                left = 0;
                RandomData();
            }
        }
        else if (m_horizontal == RandomHorizontalMoveType.Right)
        {
            left += speed;
            if (left + m_parent.Width > maxRight)
            {
                left = maxRight - m_parent.Width;
                RandomData();
            }
        }

        if (m_vertical == RandomVerticalMoveType.Up)
        {
            top -= speed;
            if (top < 0)
            {
                top = 0;
                RandomData();
            }
        }
        else if (m_vertical == RandomVerticalMoveType.Down)
        {
            top += speed;
            if (top + m_parent.Height > maxBottom)
            {
                top = maxBottom - m_parent.Height;
                RandomData();
            }
        }

        Canvas.SetTop(m_parent, top);
        Canvas.SetLeft(m_parent, left);
    }
}
