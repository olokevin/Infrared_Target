using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace InfraredRayTarget
{
    public partial class SetSerialPort : MetroWindow
    {
        private string m_name;
        private string m_state;
        private DataManager m_data;
        private EventNotifier m_notifier;

        public SetSerialPort(string name, string state)
        {
            m_name = name;
            m_state = state;
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            portName.Text = m_name;
            m_data = DataManager.Instance;
            m_notifier = new EventNotifier(ServiceID.UI);
            m_notifier.AddEvent(EventDef.PortStateChange, new Action<PortStateChangeEvent>(OnPortStateChange));

            if (m_state == "打开")
            {
                LockInfo();
            }
            if (m_state == "关闭")
            {
                UnLockInfo();
            }

            if (m_data.portParamDic.ContainsKey(m_name))
            {
                SerialPortParam param = m_data.portParamDic[m_name];
                baudRateBox.Text = param.baudRate.ToString();
                dataBitsBox.Text = param.dataBits.ToString();
                parityBox.SelectedIndex = (int)param.parity;
                stopBitsBox.SelectedIndex = (int)param.stopBits;
                readTimeoutBox.Text = param.readTimeout.ToString();
                writeTimeoutBox.Text = param.writeTimeout.ToString();
            }
        }

        private void OnPortStateChange(PortStateChangeEvent ev)
        {
            if (ev.portname != m_name) return;

            if (ev.state == SerialState.Open)
            {
                LockInfo();
            }
            else if (ev.state == SerialState.Close)
            {
                UnLockInfo();
            }
        }

        private void LockInfo()
        {
            openBtn.IsEnabled = false;
            closeBtn.IsEnabled = true;
            infoGrid.IsEnabled = false;
            normalBtn.IsEnabled = false;
            Title = "串口（运行中）";
            portName.Foreground = Brushes.Green;
        }

        private void UnLockInfo()
        {
            openBtn.IsEnabled = true;
            infoGrid.IsEnabled = true;
            normalBtn.IsEnabled = true;
            closeBtn.IsEnabled = false;
            Title = "串口（未运行）";
            portName.Foreground = Brushes.Black;
        }

        private void OnlyNumber(object sender, TextCompositionEventArgs e)
        {
            Regex re = new Regex("[^0-9]+");
            e.Handled = re.IsMatch(e.Text);
        }

        private void OnOpenPort(object sender, RoutedEventArgs e)
        {
            baudRateBox.Background = Brushes.White;
            readTimeoutBox.Background = Brushes.White;
            writeTimeoutBox.Background = Brushes.White;

            bool validBaudRate = int.TryParse(baudRateBox.Text, out int buadRate);
            bool validReadTimeout = int.TryParse(readTimeoutBox.Text, out int readTimeout);
            bool validWriteTimeout = int.TryParse(writeTimeoutBox.Text, out int writeTimeout);

            if (!validBaudRate) baudRateBox.Background = Brushes.Red;
            if (!validReadTimeout) readTimeoutBox.Background = Brushes.Red;
            if (!validWriteTimeout) writeTimeoutBox.Background = Brushes.Red;

            if (!validBaudRate || !validReadTimeout || !validWriteTimeout) return;

            SerialPortParam param = new SerialPortParam();
            param.baudRate = buadRate;
            param.dataBits = Convert.ToInt32(dataBitsBox.SelectedIndex + 5);
            param.parity = (System.IO.Ports.Parity)parityBox.SelectedIndex;
            param.stopBits = (System.IO.Ports.StopBits)stopBitsBox.SelectedIndex;
            param.readTimeout = readTimeout;
            param.writeTimeout = writeTimeout;

            OpenPortEvent ev = new OpenPortEvent(portName.Text, param);
            ServiceManager.Instance.m_serviceArr[ServiceID.SerialPort].PostEvent(ev);

            if (m_data.portParamDic.ContainsKey(m_name))
            {
                m_data.portParamDic[m_name] = param;
            }
            else
            {
                m_data.portParamDic.Add(m_name, param);
            }
        }

        private void OnClosePort(object sender, RoutedEventArgs e)
        {
            ServiceManager.Instance.m_serviceArr[ServiceID.SerialPort].PostEvent(new ClosePortEvent(m_name));
        }

        private void OnClosed(object sender, EventArgs e)
        {
            m_notifier.RemoveEvent(EventDef.PortStateChange, new Action<PortStateChangeEvent>(OnPortStateChange));
            m_data.portInfoDic.Remove(m_name);
        }

        private void OnNormalSet(object sender, RoutedEventArgs e)
        {
            baudRateBox.Text = "115200";
            dataBitsBox.SelectedIndex = 3;
            parityBox.SelectedIndex = 0;
            stopBitsBox.SelectedIndex = 1;
            readTimeoutBox.Text = "30";
            writeTimeoutBox.Text = "30";
        }
    }
}
