using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace InfraredRayTarget
{
    public partial class MainWindow : MetroWindow
    {
        //添加玩家
        private void OnAddPlayer(object sender, RoutedEventArgs e)
        {
            if (m_observablePlayerList.Count >= 100)
            {
                ShowLog(LogType.Error, "无法添加新玩家，已达到上限");
                return;
            }
            ConfirmWindow confirm = new ConfirmWindow("添加玩家", "player" + m_playerIndex, new Action<string>(AddPlyer));
        }

        //添加玩家
        private void AddPlyer(string name)
        {
            bool exisit = false;
            int i = 0;
            for (; i < m_observablePlayerList.Count; ++i)
            {
                Player cur = (Player)m_observablePlayerList[i].Content;
                if (cur.PlayerName == name)
                {
                    exisit = true;
                    break;
                }
            }

            if (exisit)
            {
                ShowLog(LogType.Warning, "该玩家已存在");
                ListViewItem exItem = m_observablePlayerList[i];
                playerView.ScrollIntoView(exItem);
            }
            else
            {
                Player player = new Player();
                player.PlayerName = name;
                ListViewItem item = new ListViewItem();
                item.Content = player;
                m_observablePlayerList.Add(item);
                if (name == "player" + m_playerIndex) ++m_playerIndex;

                int index = playerView.Items.Count - 1;
                playerView.SelectedIndex = index;
                playerView.ScrollIntoView(item);
                ShowLog(LogType.Suc, "添加玩家：{0}", player.PlayerName);
            }
        }

        //移除玩家 - 根据名字
        private void RemovePlayer(string name)
        {
            ListViewItem item = null;
            for (int i = 0; i < m_observablePlayerList.Count; ++i)
            {
                ListViewItem curitem = m_observablePlayerList[i];
                Player cur = (Player)curitem.Content;
                if (cur.PlayerName == name)
                {
                    item = curitem;
                    break;
                }
            }

            if (item != null) RemovePlayer(item);
        }

        //移除玩家 - 直接移除
        private void RemovePlayer(ListViewItem item)
        {
            if (item == null) return;
            m_observablePlayerList.Remove(item);
            Player player = (Player)item.Content;
            ShowLog(LogType.Warning, "移除玩家：{0}", player.PlayerName);
        }

        //发起移除玩家
        private void OnRemovePlayer(object sender, RoutedEventArgs e)
        {
            ListViewItem item = (ListViewItem)playerView.SelectedItem;
            if (item == null) ShowLog(LogType.Warning, "没有选择玩家");
            else RemovePlayer(item);
        }

        //清除玩家
        private void OnClearPlayer(object sender, RoutedEventArgs e)
        {
            m_curPlayer = null;
            playerView.SelectedIndex = -1;
            m_observablePlayerList.Clear();
            ShowLog(LogType.Warning, "移除所有玩家");
        }

        //选择玩家
        private void OnSelectPlayer(object sender, SelectionChangedEventArgs e)
        {
            ListViewItem item = (ListViewItem)playerView.SelectedItem;
            ClearData();
            if (item == null)
            {
                m_target.ClearAllCanvas();
                dataView.ItemsSource = null;
                dataView.Items.Clear();
                m_curPlayer = null;
            }
            else
            {
                Player player = (Player)item.Content;
                m_curPlayer = player;
                UpdateByGameTypeOrShowTypeOrArmorTypeChanged();
            }
        }

        //清空玩家子弹
        private void OnClearPlayerBullets(object sender, RoutedEventArgs e)
        {
            ListViewItem item = (ListViewItem)playerView.SelectedItem;
            if (item == null)
            {
                ShowLog(LogType.Warning, "没有选择玩家");
            }
            else
            {
                Player player = (Player)item.Content;
                player.ClearBullets();
                m_target.ClearAllCanvas();
                ClearData();
            }
        }

        //修改玩家名字
        private void OnChangePlayerName(object sender, MouseButtonEventArgs e)
        {
            ListViewItem item = (ListViewItem)playerView.SelectedItem;
            if (item == null) return;
            Player player = (Player)item.Content;
            ConfirmWindow confirm = new ConfirmWindow("修改名字", player.PlayerName, new Action<string>(ChangePlayerName));
        }

        //修改玩家名字
        private void ChangePlayerName(string name)
        {
            bool exist = false;
            for (int i = 0; i < m_observablePlayerList.Count; ++i)
            {
                ListViewItem item = m_observablePlayerList[i];
                Player player = (Player)item.Content;
                if (player.PlayerName == name)
                {
                    exist = true;
                    break;
                }
            }

            if (exist)
            {
                ShowLog(LogType.Error, "修改名字失败，已存在该名字");
            }
            else
            {
                ListViewItem item = (ListViewItem)playerView.SelectedItem;
                Player player = (Player)item.Content;
                player.PlayerName = name;
                ShowLog(LogType.Suc, "{0}名字修改为{1}", player.PlayerName, name);
            }
        }
    }
}
