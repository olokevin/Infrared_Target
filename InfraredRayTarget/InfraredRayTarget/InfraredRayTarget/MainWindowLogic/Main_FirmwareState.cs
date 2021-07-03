using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace InfraredRayTarget
{
    public partial class MainWindow : MetroWindow
    {
        private DispatcherTimer m_updateTimer;
        private float sendQueryTimer;
        private float checkOfflineTimer;

        private void InitTimer()
        {
            m_updateTimer = new DispatcherTimer();
            m_updateTimer.Interval = TimeSpan.FromMilliseconds(100);
            m_updateTimer.Tick += OnUpdateTick;
            m_updateTimer.Start();
        }

        private void OnUpdateTick(object sender, EventArgs e)
        {
            sendQueryTimer += 0.2f;

            if (sendQueryTimer >= 1.0f)
            {
                sendQueryTimer = 0;
                SendQueryReq();              
            }

            //SendQueryReq();

            if (m_data.currentFirmwareState != FirmwareState.Offline)
            {
                checkOfflineTimer += 0.1f;
                if (checkOfflineTimer >= 3.0f)
                {
                    checkOfflineTimer = 0;
                    SetEnvConnectState(FirmwareState.Offline);
                }
            }
        }

        private void SendQueryReq()
        {
            T_MODULE module = new T_MODULE();
            module.type = 0xff;
            module.id = 0xff;
            T_ROBOT robot = new T_ROBOT();
            robot.type = 0xff;
            robot.id = 0xff;
            ProtoManager.SendProto(EventDef.CMD_ID_QUERY_DEVICE_INFO, 0, null, 0, robot, module);
        }

        private void OnQueryAck(RecvProtoEvent ev)
        {
    
            checkOfflineTimer = 0;
            m_data.currentFirmwareState = FirmwareState.Online;
            T_HEADER header = new T_HEADER();
            object obBody = ProtoManager.DecodeProto(ev.proto, typeof(ACK_QEURYINFO), ref header);
            if (obBody == null)
            {
               // Debug.LogError("cmdId:[{0}]号协议,包体数据结构的长度异常,解析失败! SenderType = {1}", ev.id, header.sender.type);
                return;
            }
            ACK_QEURYINFO body = (ACK_QEURYINFO)obBody;
            m_data.firmware_version = Utility.MakeLongVersionStr(body.app_version);
            m_data.deviceId = body.deviceId;
            SetEnvConnectState(FirmwareState.Online);
        }

        private void SetEnvConnectState(FirmwareState state)
        {
            if (state == FirmwareState.Offline)
            {
                img_state.Source = new BitmapImage(new Uri("Images/disconnect.png", UriKind.Relative));
                txt_state.Text = "下位机已断开";
                txt_version.Text = "0.0.0.0";
            }
            if (state == FirmwareState.Warning)
            {
                img_state.Source = new BitmapImage(new Uri("Images/warning.png", UriKind.Relative));
                txt_state.Text = "下位机有错误";
                txt_version.Text = m_data.firmware_version;
            }
            if (state == FirmwareState.Online)
            {
                img_state.Source = new BitmapImage(new Uri("Images/connect.png", UriKind.Relative));
                txt_state.Text = "下位机连接";
                txt_version.Text = m_data.firmware_version;
            }
            m_data.currentFirmwareState = state;
        }

        private void OnConfigFinish(DownloadConfigFinishEvent obj)
        {
            ShowLog(LogType.Suc, "已经获取到远端版本");
            view_firmware.ItemsSource = Config.LoadFirmwareConfig();
        }

        private void OnStartUpdate(object sender, RoutedEventArgs e)
        {
            //if (view_firmware.SelectedIndex == -1)
            //{
            //    ShowLog(LogType.Error, "未选择任意固件");
            //}
            if (m_data.currentFirmwareState == FirmwareState.Offline)
            {
                ShowLog(LogType.Error, "下位机未连接，无法进行升级操作");
            }
            else
            {
                StartUpdateEvnet ev = new StartUpdateEvnet();
                System.Windows.Forms.OpenFileDialog log = new System.Windows.Forms.OpenFileDialog();
                log.InitialDirectory = System.IO.Directory.GetCurrentDirectory();
                log.ShowDialog();
                ev.path = log.FileName;
                m_notifier.PostEvent(ServiceID.UpdateFirmware, ev);
            }
        }
    }
}
