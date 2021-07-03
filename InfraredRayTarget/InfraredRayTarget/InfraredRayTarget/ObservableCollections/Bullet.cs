using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Bullet : DataBinding
{
    private float m_x;
    private float m_y;
    private float m_r;
    private byte m_t;

    private int m_index;
    private string m_type;
    private string m_other;
    private string m_time;
    public double distance;
    public IList<Bullet> parent;

    public Bullet(float _x, float _y, byte _t, string _time = "")
    {
        m_x = _x;
        m_y = _y;
        
        m_t = _t;
        if (_t == 1)
        {
            Type = "大弹丸";
            m_r = AttributeInfo.GolfSize;
        }
        else
        {
            Type = "小弹丸";
            m_r = AttributeInfo.BulletSize;
        }
        if (_time == "")
        {
            Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff");
        }
        else
        {
            Time = _time;
        }
    }

    public float X
    {
        get { return m_x; }
        set { m_x = value; OnPropertyChanged("X"); }
    }

    public float Y
    {
        get { return m_y; }
        set { m_y = value; OnPropertyChanged("Y"); }
    }

    public float R
    {
        get { return m_r; }
        set { m_r = value; OnPropertyChanged("R"); }
    }

    public int Index
    {
        get { return m_index; }
        set { m_index = value; OnPropertyChanged("Index"); }
    }

    public string Type
    {
        get { return m_type; }
        set { m_type = value; OnPropertyChanged("Type"); }
    }

    public string Other
    {
        get { return m_other; }
        set { m_other = value; OnPropertyChanged("Other"); }
    }

    public string Time
    {
        get { return m_time; }
        set { m_time = value; OnPropertyChanged("Time"); }
    }

    public Bullet Copy()
    {
        Bullet bullet = new Bullet(m_x, m_y, m_t, m_time);
        bullet.Index = m_index;
        bullet.Type = m_type;
        bullet.Other = m_other;
        return bullet;
    }
}
