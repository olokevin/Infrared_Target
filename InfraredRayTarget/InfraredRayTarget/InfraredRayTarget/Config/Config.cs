using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

public class Config
{
    public static FirmwareItem[] LoadFirmwareConfig()
    {
        List<FirmwareItem> result = new List<FirmwareItem>();
        XmlDocument doc = new XmlDocument();
        doc.Load("./OnlineFirmwareConfig.xml");
        XmlElement root = doc.DocumentElement;
        XmlNodeList infolist = root.GetElementsByTagName("firmware_info");
        XmlElement infraredrayEle = null;
        foreach (XmlElement info in infolist)
        {
            if (info.GetAttribute("name") == "infraredray")
            {
                infraredrayEle = info;
                break;
            }
            
        }
        if (infraredrayEle != null)
        {
            XmlNodeList versions = infraredrayEle.GetElementsByTagName("version");
            foreach (XmlElement version in versions)
            {
                XmlElement nameEle = (XmlElement)version.GetElementsByTagName("name")[0];
                XmlElement pathEle = (XmlElement)version.GetElementsByTagName("path")[0];
                XmlElement dateEle = (XmlElement)version.GetElementsByTagName("date")[0];
                FirmwareItem item = new FirmwareItem();
                item.Url = pathEle.GetAttribute("path");
                item.Name = nameEle.GetAttribute("name");
                item.Date = dateEle.GetAttribute("date");
                item.Version = version.GetAttribute("version");
                result.Add(item);
            }
        }
        return result.ToArray();
    }
}
