using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace InfraredRayTarget
{
    public partial class MainWindow : MetroWindow
    {
        //打开串口面板
        private void OnClickSerialPortItem(object sender, MouseButtonEventArgs e)
        {
            ListViewItem item = (ListViewItem)view_serial.SelectedItem;
            if (item == null) return;
            SerialPortItem portItem = (SerialPortItem)item.Content;
            if (portItem.PortState == "打开")
            {
                ClosePortEvent ev = new ClosePortEvent(portItem.PortName);
                ServiceManager.Instance.m_serviceArr[ServiceID.SerialPort].PostEvent(ev);
            }
            else
            {
                OpenPortEvent ev = new OpenPortEvent(portItem.PortName);
                ServiceManager.Instance.m_serviceArr[ServiceID.SerialPort].PostEvent(ev);
            }

            //old code, don't delete
            //ListViewItem item = (ListViewItem)view_serial.SelectedItem;
            //if (item == null) return;
            //SerialPortItem portItem = (SerialPortItem)item.Content;
            //if (m_data.portInfoDic.ContainsKey(portItem.PortName))
            //{
            //    m_data.portInfoDic[portItem.PortName].Focus();
            //}
            //else
            //{
            //    SetSerialPort form = new SetSerialPort(portItem.PortName, portItem.PortState);
            //    m_data.portInfoDic.Add(portItem.PortName, form);
            //    form.Show();
            //}
        }

        //更新串口
        private void OnUpdatePorts(UpdatePortsEvent ev)
        {
            for (int i = 0; i < ev.listDel.Count; ++i)
            {
                ListViewItem del = null;
                foreach (ListViewItem item in m_observablePortList)
                {
                    SerialPortItem portItem = (SerialPortItem)item.Content;
                    if (portItem.PortName == ev.listDel[i])
                    {
                        del = item;
                        if (m_data.portInfoDic.ContainsKey(portItem.PortName))
                        {
                            SetSerialPort ui = m_data.portInfoDic[portItem.PortName];
                            m_data.portInfoDic.Remove(portItem.PortName);
                            ui.Close();
                        }
                        break;
                    }
                }
                if (del != null)
                {
                    m_observablePortList.Remove(del);
                }
            }

            for (int i = 0; i < ev.listAdd.Count; ++i)
            {
                ListViewItem item = new ListViewItem();
                SerialPortItem portItem = new SerialPortItem();
                portItem.PortName = ev.listAdd[i];
                portItem.PortState = "关闭";
                item.Content = portItem;
                m_observablePortList.Add(item);
            }

            foreach (ListViewItem item in m_observablePortList)
            {
                SerialPortItem portItem = (SerialPortItem)item.Content;
                if (ev.listWorking.Contains(portItem.PortName))
                {
                    portItem.PortState = "打开";
                    item.Foreground = Brushes.Green;
                }
                else
                {
                    portItem.PortState = "关闭";
                    item.Foreground = Brushes.Black;
                }
            }
        }
    }
}
