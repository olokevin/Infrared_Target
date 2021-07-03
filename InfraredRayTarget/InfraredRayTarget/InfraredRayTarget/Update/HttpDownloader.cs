using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class HttpDownloader : UpdateObject
{
    private static HttpDownloader ins;
    public static HttpDownloader Instance
    {
        get
        {
            if (ins == null) ins = new HttpDownloader();
            return ins;
        }
    }

    private string m_url;
    private string m_desPath;
    public long curSize;
    public long fileSize;
    public string failedLabel;
    private Thread m_thread;

    public override void Start()
    {
        m_url = @"https://www.robomaster.com/api/firmwares/repo.xml";
        m_desPath = "OnlineFirmwareConfig.xml";
    }

    public void Download()
    {
        m_thread = new Thread(Work);
        m_thread.IsBackground = true;
        m_thread.Start();
    }

    private void Work()
    {
        try
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(m_url);
            httpWebRequest.Method = "GET";
            httpWebRequest.ContentType = "application/xml";
            httpWebRequest.Accept = "application/xml";
            httpWebRequest.Timeout = 10000;
            httpWebRequest.AllowAutoRedirect = false;

            WebResponse response = null;
            response = httpWebRequest.GetResponse();
            if (response == null) return;

            Stream netStream = response.GetResponseStream();
            Stream fileStream = new FileStream(m_desPath, FileMode.Create);
            byte[] read = new byte[1024];
            long progressBarValue = 0;
            int realReadLen = netStream.Read(read, 0, read.Length);
            while (realReadLen > 0)
            {
                fileStream.Write(read, 0, realReadLen);
                progressBarValue += realReadLen;
                realReadLen = netStream.Read(read, 0, read.Length);
            }
            netStream.Close();
            fileStream.Close();
            PostEvent(ServiceID.UI, new DownloadConfigFinishEvent());
        }
        catch (Exception ex)
        {
            failedLabel = ex.Message;
            PostEvent(ServiceID.UI, new MessageEvent(LogType.Error, ex.Message));
        }
    }
}
