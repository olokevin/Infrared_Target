using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

public class FirmwareCryptUtility
{
    //.encx和.pack固件解析
    public static FirmwareContent ParseEncxOrPack(string strPath)
    {
        byte[] firmwareArray = File.ReadAllBytes(strPath);
        return ParseEncxOrPack(firmwareArray, strPath);
    }

    public static FirmwareContent ParseEncxOrPack(byte[] firmwareArray, string path)
    {
        //.enc和.pack头不一样
        bool isEncx = false;
        if (path.EndsWith(".encx") || path.EndsWith(".ENCX")) isEncx = true;

        FirmwareContent item = new FirmwareContent();
        item.filePath = path;

        IMAGE_HEADER theader;
        int headerlen;

        //.encx头需要进行AES解密
        if (isEncx)
        {
            //首先对encx或pack固件的头进行AES2解密：
            AES2 aes2 = new AES2();
            aes2.initKey(0, 0);
            aes2.aesDecInit();
            aes2.aesDecryptBlock(firmwareArray, 0, 256);

            //加密固件固定为256
            headerlen = 256;
        }

        else
        {
            headerlen = Marshal.SizeOf(typeof(IMAGE_HEADER));
        }

        //解析固件头结构体
        theader = (IMAGE_HEADER)ProtoManager.BytesToStruct(firmwareArray, 0, typeof(IMAGE_HEADER));

        //固件加密方式
        item.encrypt = theader.security_information;
        item.fileheader = theader;
        int tailorlen = Marshal.SizeOf(typeof(IMAGE_TAIL));
        int datalen = firmwareArray.Length - headerlen - tailorlen;
        item.fileData = new byte[datalen];
        Array.Copy(firmwareArray, headerlen, item.fileData, 0, datalen);

        //MD 5
        item.fileDataMd5 = new byte[16];
        Array.Copy(theader.image_plaintext_digest, 0, item.fileDataMd5, 0, 16);

        //模块类型
        item.moduleType = Utility.GetHardwareCode(theader.hardware_code).moduleType;

        return item;
    }

    //获取.encx或.pack固件头
    public static IMAGE_HEADER GetImageHeaderFromFileBytes(byte[] firmwareArray, bool isEncx)
    {
        if (isEncx)
        {
            //首先对encx或pack固件的头进行AES2解密：
            AES2 aes2 = new AES2();
            aes2.initKey(0, 0);
            aes2.aesDecInit();
            aes2.aesDecryptBlock(firmwareArray, 0, 256);
        }

        //获取固件头结构体
        IMAGE_HEADER theader = (IMAGE_HEADER)ProtoManager.BytesToStruct(firmwareArray, 0, typeof(IMAGE_HEADER));

        //单独获取头不需要校验
        return theader;
    }

    //通过文件名获取固件信息
    public static IMAGE_HEADER GetImageHeaderFromFile(string filename)
    {
        IMAGE_HEADER header = new IMAGE_HEADER();

        //第一步，读文件
        if (!File.Exists(filename))
        {
            return header;
        }

        byte[] data = File.ReadAllBytes(filename);

        //固件类型
        bool isEncx = false;

        if (filename.EndsWith(".encx") || filename.EndsWith(".ENCX")) isEncx = true;

        //解析头
        header = GetImageHeaderFromFileBytes(data, isEncx);

        return header;
    }

    //固件文件的哈希校验
    public static bool CompareFileMD5(string filename)
    {
        if (!File.Exists(filename))
        {
            return false;
        }
        byte[] data = File.ReadAllBytes(filename);

        //第一步：对ENC1文件进行MD5校验
        byte[] MD5Arrays = Utility.GetMd5(data, data.Length - 16);

        //第二步：获取ENC1文件中的MD5值
        byte[] GetMd5Arrays = new byte[16];
        Array.Copy(data, data.Length - 16, GetMd5Arrays, 0, 16);

        bool bEqual = EnCrypt.CompareMD5(MD5Arrays, GetMd5Arrays);
        return bEqual;
    }
}

