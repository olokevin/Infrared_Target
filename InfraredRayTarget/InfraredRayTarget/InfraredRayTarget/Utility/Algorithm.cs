using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

public class Algorithm
{
    public static double aveCircleX = 0, aveCircleY = 0;

    //计算差
    public static void CalSta(IList<Bullet> list, out double value1, out double value2)
    {
        value1 = 0;
        value2 = 0;
        if (list == null || list.Count == 0) return;
        double xi = 0, yi = 0;
        for (int i = 0; i < list.Count; ++i)
        {
            xi += list[i].X;
            yi += list[i].Y;
        }
        double x0 = xi / list.Count;
        double y0 = yi / list.Count;
        double di = 0;
        double s = 0;
        for (int i = 0; i < list.Count; ++i)
        {
            list[i].distance = Math.Sqrt(Math.Pow(list[i].X - x0, 2) + Math.Pow(list[i].Y - y0, 2));
            di += list[i].distance;
        }
        double r = di / list.Count;
        for (int i = 0; i < list.Count; ++i)
        {
            s += Math.Pow(r - list[i].distance, 2);
        }
        value1 = s / list.Count;
        value2 = Math.Sqrt(s / list.Count);
    }

    //计算平均距离圆
    public static CircleData CalAveCircle(Player player, IList<Bullet> list)
    {
        CircleData result = new CircleData();
        if (list == null || list.Count == 0) return null;

        double xi = 0, yi = 0;
        for (int i = 0; i < list.Count; ++i)
        {
            xi += list[i].X;
            yi += list[i].Y;
        }
        double x0 = xi / list.Count;
        double y0 = yi / list.Count;
        double di = 0;

        for (int i = 0; i < list.Count; ++i)
        {
            list[i].distance = Math.Sqrt(Math.Pow(list[i].X - x0, 2) + Math.Pow(list[i].Y - y0, 2));
            di += list[i].distance;
        }

        double r = di / list.Count;
        player.scatteredBullets = CalculBulletInCircle(list.ToList(), x0, y0, r);
        //y0 *= 25.4 / 93;
        //x0 *= 25.4 / 93;
        r *= 25.4 / 93;

        player.scatteredX = x0;
        player.scatteredY = y0;
        player.scatteredY = r;

        result.sx = x0.ToString("0.0");
        result.sy = y0.ToString("0.0");
        result.sr = r.ToString("0.0");
        result.sCount = player.scatteredBullets.ToString();

        di = 0;
        xi = 0;
        yi = 0;
        List<Bullet> bullets = new List<Bullet>();
        bullets = list.OrderBy(a => a.distance).ToList();
        for (int i = 0; i < bullets.Count / 2; ++i)
        {
            xi += bullets[i].X;
            yi += bullets[i].Y;
            di += bullets[i].distance;
        }
        aveCircleX = x0 = xi / (bullets.Count / 2);
        aveCircleY = y0 = yi / (bullets.Count / 2);
        r = di / (bullets.Count / 2);
        player.amassBullets = CalculBulletInCircle(list.ToList(), x0, y0, r);
        InfraredRayTarget.TargetWindow.self.PaintingAveCircle((float)x0, (float)y0, (float)r);
        y0 *= 25.4 / 93;
        x0 *= 25.4 / 93;
        r *= 25.4 / 93;
        player.amassX = x0;
        player.amassY = y0;
        player.amassR = r;
        result.jx = x0.ToString("0.0");
        result.jy = y0.ToString("0.0");
        result.jr = r.ToString("0.0");
        result.jCount = player.amassBullets.ToString();
        return result;
    }

    //计算最小包围圆
    public static CircleData CalMiniCircle(Player player, IList<Bullet> list)
    {
        CircleData result = new CircleData();
        if (list == null || list.Count == 0) return null;
        var tup = MinCircleCalc(list.ToList());
        player.scatteredBullets = CalculBulletInCircle(list.ToList(), tup.Item2, tup.Item3, tup.Item1);
        InfraredRayTarget.TargetWindow.self.PaintingMiniCircle((float)tup.Item2, (float)tup.Item3, (float)tup.Item1);
        double w = Convert.ToDouble(tup.Item2) * 25.4 / 93;
        double h = Convert.ToDouble(tup.Item3) * 25.4 / 93;
        double r = Convert.ToDouble(tup.Item1) * 25.4 / 93;
        player.scatteredX = w;
        player.scatteredY = h;
        player.scatteredR = r;
        result.sx = w.ToString("0.0");
        result.sy = h.ToString("0.0");
        result.sr = r.ToString("0.0");
        result.sCount = player.scatteredBullets.ToString();
        for (int i = 0; i < list.Count; ++i)
        {
            list[i].distance = Math.Sqrt(Math.Pow(list[i].X - tup.Item2, 2) + Math.Pow(list[i].Y - tup.Item3, 2));
        }
        if (list.Count == 1) return null;
        tup = MinCircleCalc(list.OrderBy(a => a.distance).Take(list.Count / 2).ToList());
        player.amassBullets = CalculBulletInCircle(list.ToList(), tup.Item2, tup.Item3, tup.Item1);
        w = Convert.ToDouble(tup.Item2) * 25.4 / 93;
        h = Convert.ToDouble(tup.Item3) * 25.4 / 93;
        r = Convert.ToDouble(tup.Item1) * 25.4 / 93;
        player.amassX = w;
        player.amassY = h;
        player.amassR = r;
        result.jx = w.ToString("0.0");
        result.jy = h.ToString("0.0");
        result.jr = r.ToString("0.0");
        result.jCount = player.amassBullets.ToString();
        return result;
    }

    //为显示装甲计算出平均集中圆中心
    public static void CalAveCircleCenter(IList<Bullet> list)
    {
        if (aveCircleX == 0)
        {
            if (list == null || list.Count == 0) return;
            double xi = 0, yi = 0;
            for (int i = 0; i < list.Count; ++i)
            {
                xi += list[i].X;
                yi += list[i].Y;
            }
            double x0 = xi / list.Count;
            double y0 = yi / list.Count;
            double di = 0;

            for (int i = 0; i < list.Count; ++i)
            {
                list[i].distance = Math.Sqrt(Math.Pow(list[i].X - x0, 2) + Math.Pow(list[i].Y - y0, 2));
                di += list[i].distance;
            }
            double r = di / list.Count;
            di = 0;
            xi = 0;
            yi = 0;
            List<Bullet> bullets = new List<Bullet>();
            bullets = list.OrderBy(a => a.distance).ToList();
            for (int i = 0; i < bullets.Count / 2; ++i)
            {
                xi += bullets[i].X;
                yi += bullets[i].Y;
                di += bullets[i].distance;
            }
            aveCircleX = x0 = xi / (bullets.Count / 2);
            aveCircleY = y0 = yi / (bullets.Count / 2);
        }
    }

    //统计圆内的子弹个数的方法
    public static int CalculBulletInCircle(List<Bullet> bullets, double x, double y, double r)
    {
        double dis;
        int count = 0;
        for (int i = 0; i < bullets.Count; ++i)
        {
            dis = Math.Sqrt(Math.Pow(bullets[i].X - x, 2) + Math.Pow(bullets[i].Y - y, 2));
            if (dis <= r) ++count;
        }
        return count;
    }

    //判断新点是否在圆内，用于统计最小圆
    public static bool IsInsideCircle(Bullet b, double circleR, double circleX, double circleY)
    {
        if (Math.Sqrt(Math.Pow(b.X - circleX, 2) + Math.Pow(b.Y - circleY, 2)) <= circleR + 0.01)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public static Tuple<double, double, double> TwoPointCalcCircle(Bullet point1, Bullet point2)
    {
        double x = (point1.X + point2.X) / 2;
        double y = (point1.Y + point2.Y) / 2;
        double r = Math.Sqrt(Math.Pow(point1.X - point2.X, 2) + Math.Pow(point1.Y - point2.Y, 2)) / 2.0;
        Tuple<double, double, double> tup = new Tuple<double, double, double>(r, x, y);
        return tup;
    }

    public static Tuple<double, double, double> ThreePointCalcCircle(Bullet point1, Bullet point2, Bullet point3)
    {
        double a = point1.X - point2.X;
        double b = point1.Y - point2.Y;
        double c = point1.X - point3.X;
        double d = point1.Y - point3.Y;

        double e = ((Math.Pow(point1.X, 2) - Math.Pow(point2.X, 2)) - (Math.Pow(point2.Y, 2) - Math.Pow(point1.Y, 2))) / 2.0;
        double f = ((Math.Pow(point1.X, 2) - Math.Pow(point3.X, 2)) - (Math.Pow(point3.Y, 2) - Math.Pow(point1.Y, 2))) / 2.0;

        double x = -(d * e - b * f) / (b * c - a * d);
        double y = -(a * f - c * e) / (b * c - a * d);

        double r = Math.Sqrt(Math.Pow(x - point1.X, 2) + (Math.Pow(y - point1.Y, 2)));

        Tuple<double, double, double> tup = new Tuple<double, double, double>(r, x, y);
        return tup;
    }

    public static Tuple<double, double, double> MinCircleCalc(List<Bullet> bullets)
    {
        double circleX = bullets[0].X;
        double circleY = bullets[0].Y;
        double circleR = 0.0;
        int i = 1;
        int j = 0;
        int k = 0;
        while (i < bullets.Count)
        {
            if (IsInsideCircle(bullets[i], circleR, circleX, circleY))
            {
                circleX = bullets[i].X;
                circleY = bullets[i].Y;
                circleR = 0.0;
                j = 0;
                while (j < i)
                {
                    if (IsInsideCircle(bullets[j], circleR, circleX, circleY))
                    {
                        var tups = TwoPointCalcCircle(bullets[i], bullets[j]);
                        circleR = tups.Item1;
                        circleX = tups.Item2;
                        circleY = tups.Item3;
                        k = 0;
                        while (k < j)
                        {
                            if (IsInsideCircle(bullets[k], circleR, circleX, circleY))
                            {
                                tups = ThreePointCalcCircle(bullets[i], bullets[j], bullets[k]);
                                circleR = tups.Item1;
                                circleX = tups.Item2;
                                circleY = tups.Item3;
                            }
                            k += 1;
                        }
                    }
                    j += 1;
                }
            }
            i += 1;
        }
        Tuple<double, double, double> tup = new Tuple<double, double, double>(circleR, circleX, circleY);
        return tup;
    }
}
