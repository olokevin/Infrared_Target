using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace InfraredRayTarget
{
    public partial class MainWindow : MetroWindow
    {
        //收到弹道事件
        public void OnTargetReport(RecvProtoEvent ev)
        {

            T_HEADER header = new T_HEADER();
            object obBody = ProtoManager.DecodeProto(ev.proto, typeof(TargetProto), ref header);
            TargetProto package = (TargetProto)obBody;
            string str = package.type == 0x01 ? "小弹丸" : "大弹丸";
            //ShowLog(LogType.Info, "原始数据：{0}  {1}  {2}", package.x, package.y, str);

            UInt16 transX = (UInt16)Math.Abs((package.x / 1260.0f * m_target.Width) - m_target.Width);
            UInt16 transY = (UInt16)Math.Abs((package.y / 780.0f * m_target.Height) - m_target.Height);

            float vecX = 0, vecY = 0;


            if (package.x > 630)
            {
                vecX = -(package.x - 630) / 30.0f * 2.5f;
                //ShowLog(LogType.Suc, "左 {0}", vecX);
            }
            else
            {
                vecX = Math.Abs((package.x - 630) / 40f);
                //ShowLog(LogType.Suc, "右 {0}", vecX);
            }

            if (package.y > 390)
            {
                vecY = -(package.y - 390) / 40.0f * 3;
                //ShowLog(LogType.Suc, "上 {0}", vecY);
            }
            else
            {
                vecY = Math.Abs((package.y - 390) / 60f * 10);
                //ShowLog(LogType.Suc, "下 {0}", vecY);
            }

            transX = (ushort)(transX + vecX);
            transY = (ushort)(transY + vecY);

            //ShowLog(LogType.Info, "转换后数据：{0} - {1}", package.x, package.y);
            //TargetReport(package.x, package.y, package.type);
            TargetReport(transX, transY, 0);
        }

        //收到弹道事件
        public void TargetReport(float _x, float _y, byte _t)
        {
            if (!IsValid()) return;
            if (check_single.IsChecked.Value)
            {
                ClearData();
                m_target.ClearAllCanvas();
                m_curPlayer.ClearBullets();
            }
            Bullet bullet = new Bullet(_x, _y, _t);

            if (m_data.currentGameType == GameType.Target) AddBulletToTarget(bullet);
            if (m_data.currentGameType == GameType.Armor && m_data.currentArmorType == ArmorType.Big) AddBulletToBigArmor(bullet);
            if (m_data.currentGameType == GameType.Armor && m_data.currentArmorType == ArmorType.Small) AddBulletToSmallArmor(bullet);

            dataView.ScrollIntoView(bullet);

            ++logicFrame;
            DateTime now = DateTime.Now;
            TimeSpan span = now - m_lastTime;
            if (span.TotalMilliseconds >= 1000)
            {
                showFrame = logicFrame;
                logicFrame = 0;
                m_lastTime = now;
                ShowLog(LogType.Warning, "1秒{0}个包", showFrame);
            }

            if (check_autoReckon.IsChecked.Value) ReckonData();
            //ShowLog(LogType.Info, "X：{0}  Y：{1}  T:{2}", _x, _y, _t);
        }

        private void AddBulletToTarget(Bullet _bullet)
        {
            _bullet.Index = m_curPlayer.target_all.Count + 1;
            _bullet.Other = m_target.GetBulletAtWhichCircle(_bullet.X, _bullet.Y);
            Bullet bullet = _bullet.Copy();
            if (_bullet.Other == "出界")
            {
                bullet.Index = m_curPlayer.target_miss.Count + 1;
                m_curPlayer.AddBullet(bullet, m_curPlayer.target_miss);
                if (m_data.currentShowBulletsType == BulletAttribute.All ||
                    m_data.currentShowBulletsType == BulletAttribute.Miss)
                    m_target.PaintingBulletToTarget(_bullet);
            }
            else
            {
                bullet.Index = m_curPlayer.target_hit.Count + 1;
                m_curPlayer.AddBullet(bullet, m_curPlayer.target_hit);
                if (m_data.currentShowBulletsType == BulletAttribute.All ||
                    m_data.currentShowBulletsType == BulletAttribute.Hit)
                    m_target.PaintingBulletToTarget(_bullet);
            }
            m_curPlayer.AddBullet(_bullet, m_curPlayer.target_all);
        }

        private void AddBulletToBigArmor(Bullet _bullet)
        {
            _bullet.Index = m_curPlayer.bigArmor_all.Count + 1;
            _bullet.Other = m_target.InArmorArea(_bullet);
            Bullet bullet = _bullet.Copy();
            if (_bullet.Other == "未命中")
            {
                bullet.Index = m_curPlayer.bigArmor_miss.Count + 1;
                m_curPlayer.AddBullet(bullet, m_curPlayer.bigArmor_miss);
                if (m_data.currentShowBulletsType == BulletAttribute.All ||
                    m_data.currentShowBulletsType == BulletAttribute.Miss)
                    m_target.PaintingBulletToTarget(_bullet);
            }
            else
            {
                bullet.Index = m_curPlayer.bigArmor_hit.Count + 1;
                m_curPlayer.AddBullet(bullet, m_curPlayer.bigArmor_hit);
                if (m_data.currentShowBulletsType == BulletAttribute.All ||
                    m_data.currentShowBulletsType == BulletAttribute.Hit)
                    m_target.PaintingBulletToBigArmor(_bullet);
            }
            m_curPlayer.AddBullet(_bullet, m_curPlayer.bigArmor_all);
        }

        private void AddBulletToSmallArmor(Bullet _bullet)
        {
            _bullet.Index = m_curPlayer.smallArmor_all.Count + 1;
            _bullet.Other = m_target.InArmorArea(_bullet);
            Bullet bullet = _bullet.Copy();
            if (_bullet.Other == "未命中")
            {
                bullet.Index = m_curPlayer.smallArmor_miss.Count + 1;
                m_curPlayer.AddBullet(bullet, m_curPlayer.smallArmor_miss);
                if (m_data.currentShowBulletsType == BulletAttribute.All ||
                    m_data.currentShowBulletsType == BulletAttribute.Miss)
                    m_target.PaintingBulletToTarget(_bullet);
            }
            else
            {
                bullet.Index = m_curPlayer.smallArmor_hit.Count + 1;
                m_curPlayer.AddBullet(bullet, m_curPlayer.smallArmor_hit);
                if (m_data.currentShowBulletsType == BulletAttribute.All ||
                    m_data.currentShowBulletsType == BulletAttribute.Hit)
                    m_target.PaintingBulletToSmallArmor(_bullet);
            }
            m_curPlayer.AddBullet(_bullet, m_curPlayer.smallArmor_all);
        }

        private bool IsValid()
        {
            if (m_observablePlayerList.Count == 0)
            {
                ShowLog(LogType.Warning, "没有玩家");
                return false;
            }
            if (m_curPlayer == null)
            {
                ShowLog(LogType.Error, "没有选中玩家");
                return false;
            }
            if (m_curPlayer.SubTotal == 20000)
            {
                ShowLog(LogType.Error, "该玩家在该模式的子弹已到上限");
                return false;
            }
            return true;
        }

        private void ReckonData()
        {
            LogicAveCircle();
            LogicMiniCircle();
            LogicCalSta();
        }

        //删除一项弹道
        private void OnDeleteBullet(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                ListViewItem playerItem = (ListViewItem)playerView.SelectedItem;
                Bullet bulletItem = (Bullet)dataView.SelectedItem;
                if (playerItem == null || bulletItem == null) return;

                Player player = (Player)playerItem.Content;
                player.RemoveBullet(bulletItem);
                m_target.RemoveBulletEllipseByIndex(bulletItem.Index - 1, bulletItem);
                for (int i = bulletItem.Index - 1; i < bulletItem.parent.Count; ++i)
                {
                    Bullet bullet = bulletItem.parent[i];
                    bullet.Index = i + 1;
                }
                if (check_autoReckon.IsChecked.Value) ReckonData();
                else ClearData();

                if (m_data.currentGameType == GameType.Armor &&
                    m_data.currentShowBulletsType == BulletAttribute.All)
                    UpdateByGameTypeOrShowTypeOrArmorTypeChanged();
            }
        }

        //选择一项子弹
        private void OnSelectBullet(object sender, SelectionChangedEventArgs e)
        {
            int index = dataView.SelectedIndex;

            if (m_data.currentGameType == GameType.Target) m_target.HighlightTargetBullet(index);
            if (m_data.currentGameType == GameType.Armor)
            {
                if (m_data.currentShowBulletsType == BulletAttribute.Miss) m_target.HighlightTargetBullet(index);
                if (m_data.currentShowBulletsType == BulletAttribute.Hit) m_target.HighlightArmorBullet(index);
                if (m_data.currentShowBulletsType == BulletAttribute.All)
                {
                    Bullet bullet = (Bullet)dataView.SelectedItem;
                    if (bullet == null) return;
                    IList<Bullet> hit = null, miss = null;
                    if (m_data.currentArmorType == ArmorType.Big)
                    {
                        hit = m_curPlayer.bigArmor_hit;
                        miss = m_curPlayer.bigArmor_miss;
                    }
                    if (m_data.currentArmorType == ArmorType.Small)
                    {
                        hit = m_curPlayer.smallArmor_hit;
                        miss = m_curPlayer.smallArmor_miss;
                    }
                    for (int i = 0; i < hit.Count; ++i)
                    {
                        if (hit[i].Time == bullet.Time)
                        {
                            m_target.HighlightArmorBullet(i);
                            m_target.HighlightTargetBullet(miss.Count - 1);
                        }
                    }
                    for (int i = 0; i < miss.Count; ++i)
                    {
                        if (miss[i].Time == bullet.Time)
                        {
                            m_target.HighlightTargetBullet(i);
                            m_target.HighlightArmorBullet(hit.Count - 1);
                        }
                    }
                }
            }
        }

        //保存文档回调
        private void SaveData(string name)
        {
            try
            {
                string root = "./RM红外靶弹道记录";
                string path = System.IO.Path.Combine(root, name);
                path += ".csv";

                if (System.IO.File.Exists(path)) throw new Exception("文件命名冲突");
                System.IO.FileStream file = System.IO.File.Open(path, System.IO.FileMode.CreateNew);
                System.IO.StreamWriter writer = new System.IO.StreamWriter(file, Encoding.GetEncoding("gb2312"));

                //写标题
                writer.WriteLine("模式,序号,弹型,X(像素),Y(像素),其他,时间");

                //写弹道
                ListViewItem item = (ListViewItem)playerView.SelectedItem;
                Player player = (Player)item.Content;
                WriteData("靶子模式", player.target_all, writer);
                WriteData("小装甲模式", player.smallArmor_all, writer);
                WriteData("大装甲模式", player.bigArmor_all, writer);
                writer.Flush();
                writer.Close();
                file.Close();
                writer.Dispose();
                file.Dispose();
                ShowLog(LogType.Suc, "保存文档成功");
            }
            catch (Exception ex)
            {
                ShowLog(LogType.Error, "保存文档失败：{0}", ex.Message);
            }
        }

        private void WriteData(string mode, IList<Bullet> list, System.IO.StreamWriter writer)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                Bullet bullet = list[i];
                string itemVal = "{0},{1},{2},{3},{4},{5},{6}";
                string index = bullet.Index.ToString();
                string type = bullet.Type;
                string x = bullet.X.ToString();
                string y = bullet.Y.ToString();
                string other = bullet.Other;
                string time = bullet.Time;
                writer.WriteLine(itemVal, mode, index, type, x, y, other, time);
            }
        }

        //加载文档
        private void OnLoadData(LoadDataEvent ev)
        {
            string name = System.IO.Path.GetFileNameWithoutExtension(ev.path);
            if (!CheckLoadValid(name)) return;
            m_target.ClearAllCanvas();
            ClearData();

            try
            {
                string data = System.IO.File.ReadAllText(ev.path, Encoding.GetEncoding("gb2312"));
                System.IO.StringReader reader = new System.IO.StringReader(data);
                string line = "";
                reader.ReadLine();
                while ((line = reader.ReadLine()) != null)
                {
                    if (line == "") continue;
                    string[] arr = line.Split(',');
                    LoadItem(arr);
                }
                UpdateByGameTypeOrShowTypeOrArmorTypeChanged();
                ReckonData();
            }
            catch (Exception ex)
            {
                ShowLog(LogType.Error, "导入失败：{0}", ex.Message);
                RemovePlayer(name);
            }
        }

        private bool CheckLoadValid(string name)
        {
            ListViewItem item = null;
            for (int i = 0; i < m_observablePlayerList.Count; ++i)
            {
                ListViewItem curItem = m_observablePlayerList[i];
                Player curPlayer = (Player)curItem.Content;
                if (curPlayer.PlayerName == name)
                {
                    item = curItem;
                    break;
                }
            }

            if (item != null)
            {
                playerView.SelectedItem = item;
                playerView.ScrollIntoView(item);
                ShowLog(LogType.Warning, "无法加载，已存在该玩家名字");
                return false;
            }
            else
            {
                AddPlyer(name);
                return true;
            }
        }

        private void LoadItem(string[] data)
        {
            int index = int.Parse(data[1]);
            string type = data[2];
            float x = float.Parse(data[3]);
            float y = float.Parse(data[4]);
            string other = data[5];
            string time = data[6];
            byte intType = 255;
            float size = -1;
            if (type == "小弹丸")
            {
                intType = 0;
                size = AttributeInfo.BulletSize;
            }
            if (type == "大弹丸")
            {
                intType = 1;
                size = AttributeInfo.GolfSize;
            }
            Bullet bullet = new Bullet(x, y, intType, time);
            bullet.Other = other;
            bullet.Index = index;
            if (other == "出界")
            {
                m_curPlayer.AddBullet(bullet, m_curPlayer.target_all);
                m_curPlayer.AddBullet(bullet, m_curPlayer.target_miss);
            }
            else if (other == "命中")
            {
                if (data[0] == "小装甲模式")
                {
                    m_curPlayer.AddBullet(bullet, m_curPlayer.smallArmor_all);
                    m_curPlayer.AddBullet(bullet, m_curPlayer.smallArmor_hit);
                }
                if (data[0] == "大装甲模式")
                {
                    m_curPlayer.AddBullet(bullet, m_curPlayer.bigArmor_all);
                    m_curPlayer.AddBullet(bullet, m_curPlayer.bigArmor_hit);
                }
            }
            else if (other == "未命中")
            {
                if (data[0] == "小装甲模式")
                {
                    m_curPlayer.AddBullet(bullet, m_curPlayer.smallArmor_all);
                    m_curPlayer.AddBullet(bullet, m_curPlayer.smallArmor_miss);
                }
                if (data[0] == "大装甲模式")
                {
                    m_curPlayer.AddBullet(bullet, m_curPlayer.bigArmor_all);
                    m_curPlayer.AddBullet(bullet, m_curPlayer.bigArmor_miss);
                }
            }
            else
            {
                m_curPlayer.AddBullet(bullet, m_curPlayer.target_all);
                m_curPlayer.AddBullet(bullet, m_curPlayer.target_hit);
            }
        }
    }
}
