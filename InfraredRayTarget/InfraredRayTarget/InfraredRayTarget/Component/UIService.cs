using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class UIService : BaseService
{
    public UIService() : base(ServiceID.UI) { }

    public override void Run()
    {
        BaseEvent ev = null;
        m_updateLoop.Update();

        while (true)
        {
            ev = m_eventLoop.GetEvent();
            if (ev == null) break;
            if (ev.id == 0) return;
            m_eventLoop.SendEvent(ev);
        }
    }
}
