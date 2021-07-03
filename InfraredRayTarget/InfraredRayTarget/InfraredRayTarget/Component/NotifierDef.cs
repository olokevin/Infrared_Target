using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface INotifier
{
	void AddEvent(int id, Delegate del);
	void RemoveEvent(int evtId, Delegate del);
	void PostEvent(BaseEvent ev);
	void PostEvent(int serviceId, BaseEvent ev);
	void SendEvent(BaseEvent ev);
	void Clear();
}

public class DelegateRef
{
    public int id;
    public Delegate del;

    public DelegateRef(int _id, Delegate _del)
    {
        id = _id;
        del = _del;
    }
}
