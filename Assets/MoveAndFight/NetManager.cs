using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class NetManager : MonoBehaviour
{
    //定义套接字
    static Socket socket;
    
    //接收缓冲区
    static byte[] readBuff = new byte[1024];
    
    //委托类型
    public delegate void MsgListener(string str);
    
    //监听列表
    private static Dictionary<string, MsgListener> listeners = new Dictionary<string, MsgListener>();
    
    //消息列表
    static List<string> msgList = new List<string>();

    //添加监听
    public static void AddListener(string msgName,MsgListener listener)
    {
        listeners[msgName] = listener;
    }

    //获取描述
    public static string GetDesc()
    {
        if (socket == null) return "";
        if (!socket.Connected) return "";

        return socket.LocalEndPoint.ToString();
    }

    //连接
    public static void Connect(string ip,int port)
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Connect(ip, port);
        socket.BeginReceive(readBuff, 0, 1024, 0, ReceiveCallBack, socket);
    }

    private static void ReceiveCallBack(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            int count = socket.EndReceive(ar);
            string recvStr = Encoding.UTF8.GetString(readBuff, 0, count);

            msgList.Add(recvStr);

            socket.BeginReceive(readBuff, 0, 1024, 0, ReceiveCallBack, socket);
        }
        catch (SocketException se)
        {
            Debug.LogError("ReceiveCallBack error:" + se.ToString());
        }
    }

    public static void Send(string sendStr)
    {
        if (socket == null) return;
        if (!socket.Connected) return;

        byte[] sendByte = Encoding.UTF8.GetBytes(sendStr);
        socket.BeginSend(sendByte,0,sendByte.Length,0,SendCallBack,socket);
    }

    private static void SendCallBack(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
        }
        catch (SocketException ex)
        {
            Debug.Log("Socket Send fail" + ex.ToString());
        }
    }



    void Update()
    {
        if (msgList.Count <= 0) return;

        string msgStr = msgList[0];
        msgList.RemoveAt(0);
        string[] split = msgStr.Split('|');
        string msgName = split[0];
        string msgArgs = split[1];
        //回调监听,如果字典包含了对应的协议名
        if (listeners.ContainsKey(msgName))
        {
            listeners[msgName](msgArgs);//MsgListener(string)
        }
    }
}
