using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

public class ReboundMoveAction : BaseMove
{
    private UpDownMoveType m_vertical;
    private LeftRightMoveType m_horizontal;

    public ReboundMoveAction(FrameworkElement _parent) : base(_parent)
    {
        m_vertical = (UpDownMoveType)m_random.Next(0, 2);
        m_horizontal = (LeftRightMoveType)m_random.Next(0, 2);
    }

    public override void DoMove()
    {
        if (!enabel) return;
        double top = Canvas.GetTop(m_parent);
        double left = Canvas.GetLeft(m_parent);

        if (m_vertical == UpDownMoveType.Up)
        {
            top -= speed;
            if (top < 0)
            {
                top = 0;
                m_vertical = UpDownMoveType.Down;
            }
        }
        else
        {
            top += speed;
            if (top + m_parent.Height > maxBottom)
            {
                top = maxBottom - m_parent.Height;
                m_vertical = UpDownMoveType.Up;
            }
        }

        if (m_horizontal == LeftRightMoveType.Left)
        {
            left -= speed;
            if (left < 0)
            {
                left = 0;
                m_horizontal = LeftRightMoveType.Right;
            }
        }
        else
        {
            left += speed;
            if (left + m_parent.Width > maxRight)
            {
                left = maxRight - m_parent.Width;
                m_horizontal = LeftRightMoveType.Left;
            }
        }

        Canvas.SetTop(m_parent, top);
        Canvas.SetLeft(m_parent, left);
    }
}
