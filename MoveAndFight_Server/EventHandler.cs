using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace EchoServer
{
    class EventHandler
    {
        public static void OnDisconnect(ClientState c)
        {
            string desc = c.socket.RemoteEndPoint.ToString();
            string sendStr = "Leave|" + desc + ",";
            byte[] sendByte = Encoding.UTF8.GetBytes(sendStr);
            foreach(ClientState cs in Program.clients.Values)
            {
                cs.socket.Send(sendByte);
            }
            Console.WriteLine("OnDisconnect");
        }
    }
}
