using InfraredRayTarget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

public class BaseState
{
    protected TargetWindow m_parent;
    public BaseState(TargetWindow _parent)
    {
        m_parent = _parent;
        m_parent.canvas_ring.Visibility = Visibility.Hidden;
        //m_parent.img_bigBlue.Visibility = Visibility.Hidden;
        m_parent.canvas_custom.Visibility = Visibility.Hidden;
        m_parent.canvas_custom.Children.Clear();
    }
    public virtual void Update() { }
}

public class TargetState : BaseState
{
    public TargetState(TargetWindow _parent) : base(_parent)
    {
        m_parent.canvas_ring.Visibility = Visibility.Visible;
    }
}

public class ArmorState : BaseState
{
    public ArmorState(TargetWindow _parent) : base(_parent)
    {
        //m_parent.img_bigBlue.Visibility = Visibility.Visible;
    }

    public override void Update()
    {
    }
}

public class CustomState : BaseState
{
    private int m_size;
    private int m_speed;
    private int m_count;
    private object m_customObj;
    private DateTime m_lastTime;
    private CustomType m_type;
    private Color m_color;

    public CustomState(TargetWindow _parent) : base(_parent)
    {
        m_parent.canvas_custom.Visibility = Visibility.Visible;
        m_lastTime = DateTime.Now;
    }

    public void SetAttribute(int _size, int _speed, int _count, Color _color, CustomType _type, object _obj = null)
    {
        m_size = _size;
        m_speed = _speed;
        m_count = _count;
        m_type = _type;
        m_color = _color;
        m_customObj = _obj;
    }

    public override void Update()
    {
        DateTime now = DateTime.Now;
        TimeSpan span = now - m_lastTime;
        m_lastTime = now;
        if (span.Seconds > 1 && m_parent.canvas_custom.Children.Count < m_count)
        {
            CreateGoal();
        }
        Move();
    }

    private void Move()
    {

    }

    private void CreateGoal()
    {
        if (m_type == CustomType.Circle) m_parent.canvas_custom.Children.Add(CreateCircle());
        if (m_type == CustomType.Rectangle) m_parent.canvas_custom.Children.Add(CreateRectangle());
        if (m_type == CustomType.Custom) m_parent.canvas_custom.Children.Add(CreateCustom());
    }

    private Ellipse CreateCircle()
    {
        Ellipse ell = new Ellipse();
        ell.Width = m_size * 2;
        ell.Height = m_size * 2;
        ell.Fill = new SolidColorBrush(m_color);
        return ell;
    }

    private Rectangle CreateRectangle()
    {
        Rectangle rect = new Rectangle();
        rect.Width = m_size * 2;
        rect.Height = m_size * 2;
        rect.Fill = new SolidColorBrush(m_color);
        return rect;
    }

    private System.Windows.Controls.Image CreateCustom()
    {
        System.Windows.Controls.Image image = new System.Windows.Controls.Image();
        //image.Source = new 
        return image;
    }
}
