using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

public enum MoveType { UpDown, LeftRight, Rebound, Random }
public enum UpDownMoveType { Up, Down }
public enum LeftRightMoveType { Left, Right }
public enum RandomVerticalMoveType { Null, Up, Down }
public enum RandomHorizontalMoveType { Null, Left, Right }

public class BaseMove
{
    public bool enabel;

    public int speed;
    public int maxRight;
    public int maxBottom;
    protected Random m_random;
    protected FrameworkElement m_parent;

    public BaseMove(FrameworkElement _parent)
    {
        enabel = false;
        m_parent = _parent;
        m_random = new Random();
    }

    public virtual void DoMove() { }
}
