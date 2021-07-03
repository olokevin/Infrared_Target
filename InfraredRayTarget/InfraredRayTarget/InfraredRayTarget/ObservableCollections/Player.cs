using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

public class Player : DataBinding
{
    private string m_name;
    private int m_hitCount, m_subTotalCount, m_sumCount;
    public double amassX, amassY, amassR, amassBullets;//集中圆的 (x y z 子弹数)
    public double scatteredX, scatteredY, scatteredR, scatteredBullets;//散布圆的 (x y z 子弹数)

    public ObservableCollection<Bullet> target_hit, target_miss, target_all;
    public ObservableCollection<Bullet> bigArmor_hit, bigArmor_miss, bigArmor_all;
    public ObservableCollection<Bullet> smallArmor_hit, smallArmor_miss, smallArmor_all;

    public Player()
    {
        target_hit = new ObservableCollection<Bullet>();
        target_miss = new ObservableCollection<Bullet>();
        target_all = new ObservableCollection<Bullet>();
        bigArmor_hit = new ObservableCollection<Bullet>();
        bigArmor_all = new ObservableCollection<Bullet>();
        bigArmor_miss = new ObservableCollection<Bullet>();
        smallArmor_hit = new ObservableCollection<Bullet>();
        smallArmor_all = new ObservableCollection<Bullet>();
        smallArmor_miss = new ObservableCollection<Bullet>();
    }

    public string PlayerName
    {
        get { return m_name; }
        set { m_name = value; OnPropertyChanged("PlayerName"); }
    }

    public int Hit
    {
        get { return m_hitCount; }
        set { m_hitCount = value; OnPropertyChanged("Hit"); }
    }

    public int SubTotal
    {
        get { return m_subTotalCount; }
        set { m_subTotalCount = value; OnPropertyChanged("SubTotal"); }
    }

    public int Sum
    {
        get { return m_sumCount; }
        set { m_sumCount = value; OnPropertyChanged("Sum"); }
    }

    public void AddBullet(Bullet bullet, IList<Bullet> list)
    {
        list.Add(bullet);
        bullet.parent = list;
        OnPropertyChanged();
    }

    public void RemoveBullet(Bullet bullet)
    {
        DataManager data = DataManager.Instance;
        if (data.currentShowBulletsType == BulletAttribute.All)
        {
            if (data.currentGameType == GameType.Target) 
            {
                RemoveBullet(bullet, target_all);
                RemoveBullet(bullet, target_hit);
                RemoveBullet(bullet, target_miss);
            }
            if (data.currentGameType == GameType.Armor && data.currentArmorType == ArmorType.Big)
            {
                RemoveBullet(bullet, bigArmor_all);
                RemoveBullet(bullet, bigArmor_hit);
                RemoveBullet(bullet, bigArmor_miss);
            }
            if (data.currentGameType == GameType.Armor && data.currentArmorType == ArmorType.Small)
            {
                RemoveBullet(bullet, smallArmor_all);
                RemoveBullet(bullet, smallArmor_hit);
                RemoveBullet(bullet, smallArmor_miss);
            }
        }
        else
        {
            if (data.currentGameType == GameType.Target)
            {
                RemoveBullet(bullet, target_all);
            }
            if (data.currentGameType == GameType.Armor)
            {
                if (data.currentArmorType == ArmorType.Big) RemoveBullet(bullet, bigArmor_all);
                if (data.currentArmorType == ArmorType.Small) RemoveBullet(bullet, smallArmor_all);
            }
        }
        OnPropertyChanged();
    }

    private void RemoveBullet(Bullet bullet, IList<Bullet> list)
    {
        foreach (var cur in list)
        {
            if (cur.Time == bullet.Time)
            {
                list.Remove(cur);
                break;
            }
        }
    }

    public void OnPropertyChanged()
    {
        if (DataManager.Instance.currentGameType == GameType.Target)
        {
            Hit = target_hit.Count;
            SubTotal = target_all.Count;
        }
        if (DataManager.Instance.currentGameType == GameType.Armor)
        {
            if (DataManager.Instance.currentArmorType == ArmorType.Big)
            {
                Hit = bigArmor_hit.Count;
                SubTotal = bigArmor_all.Count;
            }
            if (DataManager.Instance.currentArmorType == ArmorType.Small)
            {
                Hit = smallArmor_hit.Count;
                SubTotal = smallArmor_all.Count;
            }
        }
        Sum = target_all.Count + bigArmor_all.Count + smallArmor_all.Count;
    }

    public void ClearBullets()
    {
        target_all.Clear();
        target_hit.Clear();
        target_miss.Clear();
        bigArmor_hit.Clear();
        bigArmor_all.Clear();
        bigArmor_miss.Clear();
        smallArmor_hit.Clear();
        smallArmor_all.Clear();
        smallArmor_miss.Clear();
        OnPropertyChanged();
    }
}
