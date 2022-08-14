using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using Google.Protobuf;


public class SocketConnect : MonoBehaviour
{
    public static Socket client;
    
    public void Start()
    {
        ConnectAndListen();
    }
    public void ConnectAndListen()
    {
        client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPAddress ip = IPAddress.Parse("127.0.0.1");
        IPEndPoint point = new IPEndPoint(ip, int.Parse("20001"));
        client.Connect(point);
        Thread th = new Thread(ReceiveMsg);
        th.IsBackground = true;
        th.Start();
    }
    void ReceiveMsg()
    {    
        while (true)
        {
            try
            {
                byte[] data = new byte[4096];
                int dataSize = client.Receive(data);
                byte[] temp =new byte[dataSize];
                int tLen = 0;
                for (int i = 0; i < dataSize; i++)
                {
                    temp[tLen] = data[i];
                    tLen++;
                    if (i==dataSize && data[i] != (byte)'#')
                    {       
                        throw new Exception("不完整的包");
                    }
                    if (data[i] == (byte)'#')
                    {      
                        ToProto(temp);
                        tLen = 0;
                    }                                      
                }
            }
            catch (Exception ex)
            {

                Debug.LogException(ex);
                client.Close();
                break;
            }

        }
   
        Debug.Log("接收 结束 ");

    }



    public void ToProto(byte[] buffer)
    {
        
        string s = Encoding.UTF8.GetString(buffer, 0, 2);
        int nameLen = int.Parse(s);
        string msgName = Encoding.UTF8.GetString(buffer, 2, nameLen);
        int offset = nameLen + 2;

        Debug.Log(msgName);
        switch (msgName.Trim())
        {
            case "SRespGameScene":
                MsgHandelr.respList.Add(DeSerialize(buffer, offset, new SRespGameScene()));
                break;
            case "SRespCancelCurOpt":
                MsgHandelr.respList.Add(DeSerialize(buffer, offset, new SRespCancelCurOpt()));
                break;
            //SRespUserInfo 刷新好友列表
            case "SRespUserInfo":
                MsgHandelr.respList.Add(DeSerialize(buffer, offset, new SRespUserInfo()));
                break;

            //ChatMsg 接收聊天消息
            case "ChatMsg":
                MsgHandelr.respList.Add(DeSerialize(buffer, offset, new ChatMsg()));
                break;


            //SNoticeAccept 其他用户 同意了 “我”的好友申请

            case "SNoticeAccept":
                MsgHandelr.noticeList.Add(DeSerialize(buffer, offset, new SNoticeAccept()));
                break;

            //SNoticeReqFriend  其他用户 向“我” 发送好友申请

            case "SNoticeReqFriend":
                MsgHandelr.noticeList.Add(DeSerialize(buffer, offset, new SNoticeReqFriend()));
                break;
            case "SNoticeEnterPhase":
                MsgHandelr.noticeList.Add(DeSerialize(buffer, offset, new SNoticeEnterPhase()));
                break;
            case "SNoticeDrawCard":
                MsgHandelr.noticeList.Add(DeSerialize(buffer, offset, new SNoticeDrawCard()));
                break;
            case "SNoticeGameStart":
                MsgHandelr.noticeList.Add(DeSerialize(buffer, offset, new SNoticeGameStart()));
                break;
            case "SNoticeGameOver":
                MsgHandelr.noticeList.Add(DeSerialize(buffer, offset, new SNoticeGameOver()));
                break;
            case "SNoticeOp":
                MsgHandelr.noticeList.Add(DeSerialize(buffer, offset, new SNoticeOp()));
                break;


            case "SSyncCard":
                MsgHandelr.syncList.Add(DeSerialize(buffer, offset, new SSyncCard()));
                break;
            case "SSyncCamp":
                MsgHandelr.syncList.Add(DeSerialize(buffer, offset, new SSyncCamp()));
                break;
            case "SSyncAct":
                MsgHandelr.syncList.Add(DeSerialize(buffer, offset, new SSyncAct()));
                break;

            case "SRespLogin":
                MsgHandelr.respList.Add(DeSerialize(buffer, offset, new SRespLogin()));
                break;

            case "SRespMatch":
                MsgHandelr.respList.Add(DeSerialize(buffer, offset, new SRespMatch()));
                break;

            case "SRespCancelMatch":
                MsgHandelr.respList.Add(DeSerialize(buffer, offset, new SRespCancelMatch()));
                break;
                
            default:
                break;
        }
   
    }
    public  T DeSerialize<T>(byte[] bytes,int offset,  T s)
    {
        int length = 0;
        for(int i=offset; i < bytes.Length; i++)
        {
            if (bytes[i] == (byte)'#')
            {
                break;
            }
            length++;
        }
        try
        {
            IMessage msg = (IMessage)s;
            try
            {
                s = (T)msg.Descriptor.Parser.ParseFrom(bytes,offset,length);
            
            }
            catch(Exception e)
            {
                Debug.LogFormat("解码错误");
                //Debug.LogFormat("数据长度={0},offset={1}", n - VP.offset,VP.offset);
                //for (int i = VP.offset; i <= n; i++)
                //{
                //    Debug.Log(bytes[i]);
                //}
                Debug.LogException(e);
              
            }
           
        }
        catch (Exception ex)
        {

           
            Debug.LogException(ex);
             
        }
      
        return s;
    }

    public void Close()
    {
        client.Close();
    }

}



