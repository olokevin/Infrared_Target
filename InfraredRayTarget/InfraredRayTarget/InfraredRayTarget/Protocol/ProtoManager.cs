using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO.Ports;

public class ProtoManager
{
    //协议结构体转字节数组
    public static byte[] StructToBytes(object structObj)
    {
        int size = Marshal.SizeOf(structObj);
        byte[] bytes = new byte[size];
        IntPtr structPtr = Marshal.AllocHGlobal(size);
        Marshal.StructureToPtr(structObj, structPtr, false);
        Marshal.Copy(structPtr, bytes, 0, size);
        Marshal.FreeHGlobal(structPtr);
        return bytes;
    }

    //字节数组转协议结构体
    public static object BytesToStruct(byte[] bytes, int startIndex, Type type)
    {
        int size = Marshal.SizeOf(type);
        if (size > bytes.Length - startIndex)
        {
            return null;
        }
        IntPtr structPtr = Marshal.AllocHGlobal(size);
        Marshal.Copy(bytes, startIndex, structPtr, size);
        object obj = Marshal.PtrToStructure(structPtr, type);
        Marshal.FreeHGlobal(structPtr);
        return obj;
    }

    //获取协议头长度
    public static int GetHeaderLength()
    {
        return Marshal.SizeOf(typeof(T_HEADER));
    }

    //发送协议
    public static void SendProto(UInt16 cmdid, UInt16 seq, byte[] data, UInt16 sendlen, T_ROBOT robot, T_MODULE receiver)
    {
        int headerlen = Marshal.SizeOf(typeof(T_HEADER));
        int packagelen = headerlen + 2;
        if (data != null && sendlen > 0) packagelen += sendlen;
        byte[] sendpackage = new byte[packagelen];
        Array.Clear(sendpackage, 0, packagelen);
        T_HEADER tHeader;
        tHeader.flag = 0xA5;
        tHeader.length = sendlen;
        tHeader.seq_num = 0;
        //tHeader.robot = robot;
        //tHeader.receiver = receiver;
        //tHeader.sender.type = (byte)ModuleType.PC;
        //tHeader.sender.id = 0;
        tHeader.cmd_Id = cmdid;
        tHeader.crc8 = 0;

        byte[] byteHeader = ProtoManager.StructToBytes(tHeader);
        byteHeader[headerlen - 3] = CRCCheck.GetCRC8(byteHeader, headerlen - 3);
        Array.Copy(byteHeader, sendpackage, headerlen);

        if (data != null && sendlen > 0) Array.Copy(data, 0, sendpackage, headerlen, sendlen);
        UInt16 checksum = CRCCheck.GetCRC16(sendpackage, headerlen + sendlen);
        sendpackage[packagelen - 2] = (byte)(checksum & 0x00ff);
        sendpackage[packagelen - 1] = (byte)((checksum >> 8) & 0x00ff);

        SendProtoEvent ev = new SendProtoEvent();
        ev.data = sendpackage;
        ServiceManager.Instance.m_serviceArr[ServiceID.SerialPort].PostEvent(ServiceID.SerialPort, ev);
    }

    //发送协议
    public static void SendProto(UInt16 cmdid, UInt16 seq, byte[] data, UInt16 sendlen)
    {
        int headerlen = Marshal.SizeOf(typeof(T_HEADER));
        int packagelen = headerlen + 2;
        if (data != null && sendlen > 0) packagelen += sendlen;
        byte[] sendpackage = new byte[packagelen];
        Array.Clear(sendpackage, 0, packagelen);
        T_HEADER tHeader;
        tHeader.flag = 0xA5;
        tHeader.length = sendlen;
        tHeader.seq_num = 0;
        //tHeader.robot = robot;
        //tHeader.receiver = receiver;
        //tHeader.sender.type = (byte)ModuleType.PC;
        //tHeader.sender.id = 0;
        tHeader.cmd_Id = cmdid;
        tHeader.crc8 = 0;

        byte[] byteHeader = ProtoManager.StructToBytes(tHeader);
        byteHeader[headerlen - 3] = CRCCheck.GetCRC8(byteHeader, headerlen - 3);
        Array.Copy(byteHeader, sendpackage, headerlen);

        if (data != null && sendlen > 0) Array.Copy(data, 0, sendpackage, headerlen, sendlen);
        UInt16 checksum = CRCCheck.GetCRC16(sendpackage, headerlen + sendlen);
        sendpackage[packagelen - 2] = (byte)(checksum & 0x00ff);
        sendpackage[packagelen - 1] = (byte)((checksum >> 8) & 0x00ff);

        SendProtoEvent ev = new SendProtoEvent();
        ev.data = sendpackage;
        ServiceManager.Instance.m_serviceArr[ServiceID.SerialPort].PostEvent(ServiceID.SerialPort, ev);
    }

    //解析协议
    public static object DecodeProto(byte[] package, Type toType, ref T_HEADER tHeader)
    {
        //解析包头
        tHeader = (T_HEADER)BytesToStruct(package, 0, typeof(T_HEADER));

        //现在的设备代码有误（空包直接返回）
        if (tHeader.length == 0)
        {
            //Debug.LogError("Error: tHeader.length = 0,打印包信息: cmd:0x{0:X}, robotType:0x{1:X},rcvType:0x{2:X},senderType:0x{3:X}",
            //    tHeader.cmd_Id,
            //    tHeader.robot.type,
            //    tHeader.receiver.type,
            //    tHeader.sender.type);
            return null;
        }

        //解析包体
        return BytesToStruct(package, GetHeaderLength(), toType);
    }
}

