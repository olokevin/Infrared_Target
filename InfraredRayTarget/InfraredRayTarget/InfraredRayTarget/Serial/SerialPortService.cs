using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class SerialPortService : BaseService
{
    public static float deltaTime = 0;
    private double totalMilliseconds = 0;
    private SerialPortManager m_mgr = null;
    public SerialPortService() : base(ServiceID.SerialPort) { }

    protected override void Start()
    {
        m_mgr = new SerialPortManager();
    }

    public override void Run()
    {
        BaseEvent ev = null;
        DateTime start = new DateTime();
        TimeSpan span = new TimeSpan();

        while (true)
        {
            start = DateTime.Now;
            m_updateLoop.Update();

            while (true)
            {
                ev = m_eventLoop.GetEvent();
                if (ev == null) break;
                if (ev.id == EventDef.Quit) return;
                m_eventLoop.SendEvent(ev);
                span = DateTime.Now - start;
                if (span.TotalMilliseconds >= 20) break;
            }

            span = DateTime.Now - start;
            totalMilliseconds = span.TotalMilliseconds;
            if (totalMilliseconds < 20) Thread.Sleep(20 - (int)totalMilliseconds);
            span = DateTime.Now - start;
            totalMilliseconds = span.TotalMilliseconds;
            if (totalMilliseconds > 20) totalMilliseconds = 20;
            deltaTime = (float)totalMilliseconds / 1000;
        }
    }
}
