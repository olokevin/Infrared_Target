using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

public class SerialPortItem : DataBinding
{
    private string m_name;
    private string m_state;

    public string PortName
    {
        get { return m_name; }
        set { m_name = value; OnPropertyChanged("PortName"); }
    }

    public string PortState
    {
        get { return m_state; }
        set
        {
            if (m_state != value)
            {
                m_state = value; OnPropertyChanged("PortState");
            }
        }
    }
}
