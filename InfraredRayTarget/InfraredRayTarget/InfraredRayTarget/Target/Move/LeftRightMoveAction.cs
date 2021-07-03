using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

public class LeftRightMoveAction : BaseMove
{
    private LeftRightMoveType direction;
    public LeftRightMoveAction(FrameworkElement _parent) : base(_parent)
    {
        direction = (LeftRightMoveType)m_random.Next(0, 2);
    }

    public override void DoMove()
    {
        if (!enabel) return;

        double left = Canvas.GetLeft(m_parent);
        if (direction == LeftRightMoveType.Left)
        {
            left -= speed;
            if (left < 0)
            {
                left = 0;
                direction = LeftRightMoveType.Right;
            }
        }
        else
        {
            left += speed;
            if (left + m_parent.Width > maxRight)
            {
                left = maxRight - m_parent.Width;
                direction = LeftRightMoveType.Left;
            }
        }

        Canvas.SetLeft(m_parent, left);
    }
}
