using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

public class EnCrypt
{
    //固件头
    private IMAGE_HEADER m_headerInfo = new IMAGE_HEADER();

    //固件头信息文本
    public string HeaderText;

    //标记位
    public int bEncrypt = 0;

    //结构体转字节数组
    private byte[] StructToBytes()
    {
        byte[] headerArray = Utility.StructToBytes(m_headerInfo);
        return headerArray;
    }

    //进行CRC
    private byte[] CRCcheck(byte[] CRCHeaderArray)
    {
        UInt16 crc16 = CRCCheck.GetCRC16(CRCHeaderArray, CRCHeaderArray.Length - 2);
        byte[] crcArray = Utility.StructToBytes(crc16);
        Array.Copy(crcArray, 0, CRCHeaderArray, CRCHeaderArray.Length - 2, 2);
        m_headerInfo.crc16 = crc16;
        return CRCHeaderArray;
    }

    //============================== 加密函数 ==============================  

    //对明文MD5加密并填充结构体
    private void BinToMD5(byte[] arrays)
    {
        //对明文进行MD5
        byte[] md5Arr = Utility.GetMd5(arrays, arrays.Length);

        //把值传给结构体       
        m_headerInfo.image_plaintext_digest = new byte[32];
        Array.Copy(md5Arr, 0, m_headerInfo.image_plaintext_digest, 0, 16);
    }


    //AES加密
    private byte[] DoAESBlock(byte[] binBuffer)
    {
        //bin文件大小
        int binSize = binBuffer.Length;

        //enc字节数组
        int enc1Size = ((binSize % 256) == 0) ? (binSize) : (((binSize / 256) + 1) * 256);
        byte[] enc1Buffer = new byte[enc1Size];

        //AES 初始化
        AES2 aes2 = new AES2();
        aes2.initKey(0, 0);
        aes2.aesEncInit();

        int remainSize = binSize;

        //逐片加密
        while (remainSize > 0)
        {
            int offset = binSize - remainSize;

            if (remainSize < 256)
            {
                Array.Copy(binBuffer, offset, enc1Buffer, offset, remainSize);
            }
            else
            {
                Array.Copy(binBuffer, offset, enc1Buffer, offset, 256);
            }

            //片加密
            aes2.aesEncryptBlock(enc1Buffer, offset, 256);
            remainSize -= 256;
        }

        return enc1Buffer;
    }

    //读xml文件,加头
    public static HeaderConfigInfo XmlParse(string binpath)
    {
        int index = binpath.LastIndexOf('\\');
        string filePath = binpath.Substring(0, index);
        string str = binpath.Substring(index + 1, binpath.Length - index - 1);

        int indexName = str.LastIndexOf('.');
        string name = str.Substring(0, indexName);
        string xmlpath = filePath + "\\" + name + ".xml";
        HeaderConfigInfo info = new HeaderConfigInfo();

        try
        {
            XmlDocument doc = new XmlDocument();

            //读入Xml字符流
            doc.Load(xmlpath);

            //根节点
            XmlNode root = doc.DocumentElement;

            //设备节点
            XmlElement deviceNode = root.FirstChild as XmlElement;

            //设备节点id属性
            XmlAttribute attr = deviceNode.Attributes["value"];
            info.ProductHeaderVersion = attr.Value;

            //取得AppVersion
            XmlElement ele_appv = deviceNode.GetElementsByTagName("app_version")[0] as XmlElement;
            info.ProductAppVersion = ele_appv.GetAttribute("value");

            //取得app_startup_address
            XmlElement ele_startaddress = deviceNode.GetElementsByTagName("app_startup_address")[0] as XmlElement;
            info.ProductStartupAddress = ele_startaddress.GetAttribute("value");

            //取得app_param_address
            XmlElement ele_param = deviceNode.GetElementsByTagName("app_param_address")[0] as XmlElement;
            info.ProductParamAddress = ele_param.GetAttribute("value");

            //取得image_type
            XmlElement ele_image = deviceNode.GetElementsByTagName("image_type")[0] as XmlElement;
            info.ProductImageType = ele_image.GetAttribute("value");

            //取得image_write_addr
            XmlElement ele_imawrite = deviceNode.GetElementsByTagName("image_write_addr")[0] as XmlElement;
            info.ProductImageWriteAddr = ele_imawrite.GetAttribute("value");

            //取得hardware_code
            XmlElement ele_hard = deviceNode.GetElementsByTagName("hardware_code")[0] as XmlElement;
            info.ProductHardwareCode = ele_hard.GetAttribute("value");

            //取得security_information
            XmlElement ele_info = deviceNode.GetElementsByTagName("security_information")[0] as XmlElement;
            info.ProductSecurityInformation = ele_info.GetAttribute("value");
        }
        catch (Exception ex)
        {
            //throw new Exception(ex.Message + ex.StackTrace);
            return null;
        }

        //如果是.pack文件,赋值为1
        //string endName = str.Substring(indexName + 1, 4);
        //if (endName == "pack")
        //{
        //    info.ProductSecurityInformation = "1";
        //}

        return info;
    }

    //填充头结构体
    private void FillHeaderPackage(int length, string filePath, HeaderConfigInfo info)
    {
        //HeaderConfigInfo info = XmlParse(filePath);

        m_headerInfo.image_format_version = Utility.MakeLongVersionInt(info.ProductHeaderVersion);
        m_headerInfo.app_version = Utility.MakeLongVersionInt(info.ProductAppVersion);
        m_headerInfo.app_startup_address = Convert.ToUInt32(info.ProductStartupAddress, 16);
        m_headerInfo.app_param_address = Convert.ToUInt32(info.ProductParamAddress, 16);
        m_headerInfo.image_type = Convert.ToUInt32(info.ProductImageType);
        m_headerInfo.image_size = Convert.ToUInt32(length);
        m_headerInfo.image_write_addr = Convert.ToUInt32(info.ProductImageWriteAddr, 16);
        m_headerInfo.hardware_code = Convert.ToUInt32(info.ProductHardwareCode, 16);
        m_headerInfo.security_information = Convert.ToUInt32(info.ProductSecurityInformation);

        m_headerInfo.reserved1 = new UInt32[3];
    }

    //对外接口 - 打包
    public byte[] Encrypt(byte[] arrays, string filePath)
    {
        //读xml文件
        HeaderConfigInfo info = XmlParse(filePath);
        if (info == null) return null;

        if (info.ProductSecurityInformation == "1") bEncrypt = 1;

        //对明文进行MD5加密
        BinToMD5(arrays);

        //A. 获取bin文件加密后的数组
        byte[] aesBinData = DoAESBlock(arrays);

        //B. 填充文件头结构体
        FillHeaderPackage(arrays.Length, filePath, info);

        //将头文件的结构体转换成字节数组
        byte[] headerArray = StructToBytes();

        //进行CRC校验
        byte[] HeaderArray = CRCcheck(headerArray);

        //将头文件的结构体进行AES加密
        byte[] aesHeaderArray = DoAESBlock(HeaderArray);

        //拼接未加密头+bin
        byte[] deHeaderAndDebinArray = new byte[HeaderArray.Length + arrays.Length];
        Array.Copy(HeaderArray, 0, deHeaderAndDebinArray, 0, HeaderArray.Length);
        Array.Copy(arrays, 0, deHeaderAndDebinArray, HeaderArray.Length, arrays.Length);

        //拼接加密头 + bin
        byte[] deCryarrays = new byte[aesHeaderArray.Length + arrays.Length];
        Array.Copy(aesHeaderArray, 0, deCryarrays, 0, aesHeaderArray.Length);
        Array.Copy(arrays, 0, deCryarrays, aesHeaderArray.Length, arrays.Length);

        //拼接加密的头 + 加密后的bin
        byte[] aesHeaderAndaesBinArray = new byte[aesHeaderArray.Length + aesBinData.Length];
        Array.Copy(aesHeaderArray, 0, aesHeaderAndaesBinArray, 0, aesHeaderArray.Length);
        Array.Copy(aesBinData, 0, aesHeaderAndaesBinArray, aesHeaderArray.Length, aesBinData.Length);

        //-------------------------------------------------------------
        if (info.ProductSecurityInformation == "0")
        {
            //定义总长数组
            byte[] totalFirmwareArray = new byte[HeaderArray.Length + arrays.Length + Marshal.SizeOf(typeof(IMAGE_TAIL))];

            //拼接头
            Array.Copy(HeaderArray, totalFirmwareArray, HeaderArray.Length);

            //C. 填充.bin加密后的数组
            Array.Copy(arrays, 0, totalFirmwareArray, HeaderArray.Length, arrays.Length);

            //对bin+头进行MD5
            byte[] fullMd = Utility.GetMd5(deHeaderAndDebinArray, deHeaderAndDebinArray.Length);

            //尾部填充
            Array.Copy(fullMd, 0, totalFirmwareArray, HeaderArray.Length + arrays.Length, fullMd.Length);

            return totalFirmwareArray;
        }
        else
        {
            //定义总长数组
            byte[] totalFirmwareArray = new byte[aesHeaderArray.Length + aesBinData.Length + Marshal.SizeOf(typeof(IMAGE_TAIL))];

            ////拼接头
            //Array.Copy(aesHeaderArray, totalFirmwareArray, aesHeaderArray.Length);

            ////C. 填充.bin加密后的数组
            //Array.Copy(aesBinData, 0, totalFirmwareArray, aesHeaderArray.Length, aesBinData.Length);

            Array.Copy(aesHeaderAndaesBinArray, 0, totalFirmwareArray, 0, aesHeaderAndaesBinArray.Length);

            //对bin+头进行MD5
            byte[] fullMd = Utility.GetMd5(aesHeaderAndaesBinArray, aesHeaderAndaesBinArray.Length);

            //尾部填充
            Array.Copy(fullMd, 0, totalFirmwareArray, aesHeaderAndaesBinArray.Length, fullMd.Length);

            return totalFirmwareArray;
        }
    }

    //pack--->encx
    public byte[] EncryptPack(byte[] arrays)
    {
        //第一步：分解出头
        byte[] headArrays = new byte[Marshal.SizeOf(typeof(IMAGE_HEADER))];
        Array.Copy(arrays, 0, headArrays, 0, headArrays.Length);

        //头加密
        //byte[] aesHeaderArray = new byte[headArrays.Length];

        //第二步：分解出要AES加密的数组--中间部分
        byte[] AESArrays = new byte[arrays.Length - 16 - headArrays.Length];
        Array.Copy(arrays, headArrays.Length, AESArrays, 0, arrays.Length - 16 - headArrays.Length);

        //第三步：AES加密
        byte[] AESEncrypt = DoAESBlock(AESArrays);

        //第四步：取出头数组
        byte[] headerArrays = new byte[256];
        Array.Copy(arrays, 0, headerArrays, 0, 256);

        //4.5步：对头进行解密
        //byte[] headerDearray = AESToBin(headerArrays, (uint)headerArrays.Length);

        //4.6:转换成结构体
        IMAGE_HEADER theader = (IMAGE_HEADER)Utility.BytesToStruct(headArrays, 0, typeof(IMAGE_HEADER));

        //4.7:替换值0-->1
        theader.security_information = 1;

        //同时修改包头的CRC8值
        UInt16 crcNew = CRCCheck.GetCRC16(Utility.StructToBytes(theader), Marshal.SizeOf(theader) - 2);
        theader.crc16 = crcNew;

        //4.8:转换成数组并加密
        byte[] headerEncryptArray = Utility.StructToBytes(theader);
        byte[] AESHeaderArray = DoAESBlock(headerEncryptArray);

        //4.9:加密头 + 加密bin
        byte[] headerAndAESbinarray = new byte[AESHeaderArray.Length + AESEncrypt.Length];
        Array.Copy(AESHeaderArray, 0, headerAndAESbinarray, 0, AESHeaderArray.Length);
        Array.Copy(AESEncrypt, 0, headerAndAESbinarray, AESHeaderArray.Length, AESEncrypt.Length);

        //第五步：对（加密头 + 加密bin）进行MD5，并填到尾部
        byte[] tailArrays = Utility.GetMd5(headerAndAESbinarray, headerAndAESbinarray.Length);

        //第六步：数组总长度
        byte[] totalFirmwareArray = new byte[headerArrays.Length + AESEncrypt.Length + tailArrays.Length];

        //第七步：填充
        Array.Copy(headerAndAESbinarray, 0, totalFirmwareArray, 0, headerAndAESbinarray.Length);
        Array.Copy(tailArrays, 0, totalFirmwareArray, headerAndAESbinarray.Length, tailArrays.Length);

        return totalFirmwareArray;
    }

    //==============================拆包函数 ==============================  
    //对外接口 - 解密
    public byte[] DisEncrypt(byte[] sourceArray, ref IMAGE_HEADER theader)
    {
        //第一步：对ENC1文件进行MD5校验
        byte[] MD5Arrays = Utility.GetMd5(sourceArray, sourceArray.Length - 16);

        //第一步：获取ENC1文件中的MD5值
        byte[] GetMd5Arrays = new byte[16];
        Array.Copy(sourceArray, sourceArray.Length - 16, GetMd5Arrays, 0, 16);

        //第一步：校验
        bool bEqual = CompareMD5(MD5Arrays, GetMd5Arrays);
        if (bEqual == false)
        {
            throw new Exception("file is error");
        }

        //第二步：取出头
        byte[] headArrays = new byte[Marshal.SizeOf(typeof(IMAGE_HEADER))];
        Array.Copy(sourceArray, 0, headArrays, 0, headArrays.Length);

        //第二步：把头转换成结构体
        //byte[] headerDearray = AESToBin(headArrays, Convert.ToUInt32(256));
        theader = (IMAGE_HEADER)Utility.BytesToStruct(headArrays, 0, typeof(IMAGE_HEADER));


        //第二步：打印结构体中的数据
        string strVersion = Utility.MakeLongVersionStr(theader.image_format_version);
        string strAppVersion = Utility.MakeLongVersionStr(theader.app_version);
        HeaderText = string.Format("{0}, {1}, 0x{2:X000}, {3:X000}, {4}, 0x{5:X000}, {6} , 0x{7:X000}, {8}",
            strVersion,
            strAppVersion,
            theader.app_startup_address,
            theader.app_param_address,
            theader.image_type,
            theader.image_write_addr,
            theader.image_size,
            theader.hardware_code,
            theader.security_information);

        //获取未加密头 + .bin数组
        byte[] AESHeaderAndBinArray = new byte[sourceArray.Length - 16];
        Array.Copy(sourceArray, 0, AESHeaderAndBinArray, 0, AESHeaderAndBinArray.Length);

        //对（未加密头 + .bin数组）进行MD5
        byte[] tailArrays = Utility.GetMd5(AESHeaderAndBinArray, AESHeaderAndBinArray.Length);

        //分解出.bin文件的数组
        byte[] DisAESArrays = new byte[sourceArray.Length - 16 - headArrays.Length];
        Array.Copy(sourceArray, headArrays.Length, DisAESArrays, 0, sourceArray.Length - 16 - headArrays.Length);

        //第三步：把.bin解密出来
        if (Convert.ToString(theader.security_information) == "0")
        {
            return DisAESArrays;
        }
        else
        {
            byte[] DeCryArrays = AESToBin(DisAESArrays, theader.image_size);

            return DeCryArrays;
        }

    }

    //encx--->pack
    public byte[] DisEncryptPack(byte[] arrays)
    {
        //原bin文件的长度
        uint binlength = 0;

        //1：取出头数组
        byte[] headArrays = new byte[256];
        Array.Copy(arrays, 0, headArrays, 0, 256);

        //2:头解密
        byte[] deAESheaderArray = AESToBin(headArrays, (uint)headArrays.Length);

        //3:修改值：1-->0
        IMAGE_HEADER theader = (IMAGE_HEADER)Utility.BytesToStruct(deAESheaderArray, 0, typeof(IMAGE_HEADER));
        theader.security_information = 0;
        binlength = theader.image_size;

        //4:同时修改包头的CRC8值
        UInt16 crcNew = CRCCheck.GetCRC16(Utility.StructToBytes(theader), Marshal.SizeOf(theader) - 2);
        theader.crc16 = crcNew;

        //5:转换成数组
        byte[] headerEncryptArray = Utility.StructToBytes(theader);
        //byte[] AESHeaderArray = DoAESBlock(headerEncryptArray);

        //6：分解要解密的bin文件数组
        byte[] AESArrays = new byte[arrays.Length - 16 - 256];
        Array.Copy(arrays, headArrays.Length, AESArrays, 0, arrays.Length - 16 - 256);

        //7：AES解密
        byte[] DeAESEncrypt = AESToBin(AESArrays, binlength);

        //8:拼接未加密头 + 解密bin
        byte[] AESheaderAndDeAESbinArray = new byte[headerEncryptArray.Length + DeAESEncrypt.Length];
        Array.Copy(headerEncryptArray, 0, AESheaderAndDeAESbinArray, 0, headerEncryptArray.Length);
        Array.Copy(DeAESEncrypt, 0, AESheaderAndDeAESbinArray, headerEncryptArray.Length, DeAESEncrypt.Length);

        //9：对（未加密头 + 解密bin）进行MD5，并填到尾部
        byte[] tailArrays = Utility.GetMd5(AESheaderAndDeAESbinArray, AESheaderAndDeAESbinArray.Length);

        //10：数组总长度
        byte[] totalFirmwareArray = new byte[headerEncryptArray.Length + DeAESEncrypt.Length + tailArrays.Length];

        //11：填充
        Array.Copy(headerEncryptArray, 0, totalFirmwareArray, 0, headerEncryptArray.Length);
        Array.Copy(DeAESEncrypt, 0, totalFirmwareArray, headerEncryptArray.Length, DeAESEncrypt.Length);
        Array.Copy(tailArrays, 0, totalFirmwareArray, DeAESEncrypt.Length + headerEncryptArray.Length, tailArrays.Length);

        return totalFirmwareArray;
    }

    //解密过程
    private byte[] AESToBin(byte[] DeCryArrays, UInt32 binSize)
    {
        //ENC1文件的大小
        int encSize = DeCryArrays.Length;

        //bin字节数组
        byte[] binArrays = new byte[binSize];

        //AES初始化
        AES2 aes = new AES2();
        aes.initKey(0, 0);
        aes.aesDecInit();

        int remainSize = (int)binSize;

        //逐片解密
        while (remainSize > 0)
        {
            int offset = (int)binSize - remainSize;

            if (remainSize < 256)
            {
                //片解密
                aes.aesDecryptBlock(DeCryArrays, offset, 256);

                Array.Copy(DeCryArrays, offset, binArrays, offset, remainSize);
            }
            else
            {
                Array.Copy(DeCryArrays, offset, binArrays, offset, 256);

                //片解密
                aes.aesDecryptBlock(binArrays, offset, 256);
            }

            remainSize -= 256;
        }



        return binArrays;
    }

    //比较
    public static bool CompareMD5(byte[] MD5Arrays, byte[] GetMd5Arrays)
    {
        for (int i = 0; i < MD5Arrays.Length; i++)
        {
            if (MD5Arrays[i] != GetMd5Arrays[i]) return false;
        }
        return true;
    }
}

public class HeaderConfigInfo
{
    public string ProductHeaderVersion;
    public string ProductAppVersion;
    public string ProductStartupAddress;
    public string ProductParamAddress;
    public string ProductImageType;
    public string ProductImageWriteAddr;
    public string ProductHardwareCode;
    public string ProductSecurityInformation;
}
