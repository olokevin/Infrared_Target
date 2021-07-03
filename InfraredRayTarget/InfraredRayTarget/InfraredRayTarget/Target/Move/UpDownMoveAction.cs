using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

public class UpDownMoveAction : BaseMove
{
    private UpDownMoveType direction;
    public UpDownMoveAction(FrameworkElement _parent) : base(_parent)
    {
        direction = (UpDownMoveType)m_random.Next(0, 2);
    }

    public override void DoMove()
    {
        if (!enabel) return;
        double top = Canvas.GetTop(m_parent);

        if (direction == UpDownMoveType.Up)
        {
            top -= speed;
            if (top < 0)
            {
                top = 0;
                direction = UpDownMoveType.Down;
            }
        }
        else
        {
            top += speed;
            if (top + m_parent.Height > maxBottom)
            {
                top = maxBottom - m_parent.Height;
                direction = UpDownMoveType.Up;
            }
        }

        Canvas.SetTop(m_parent, top);
    }
}
