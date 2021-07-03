using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

public class DecodeConfig
{
    public static void Do()
    {
        if (!File.Exists("./OnlineFirmwareConfig.xml")) return;
        XmlDocument doc = new XmlDocument();
        //doc.Load(xmlPath);
    }
}
