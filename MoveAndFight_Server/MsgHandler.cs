using System;
using System.Text;

namespace EchoServer
{
    class MsgHandler
    {
        //127.0.0.1:4564,3,0,5,0
        public static void MsgEnter(ClientState clientState, string msgString)
        {
            //解析参数
            string[] split = msgString.Split(',');
            string desc = split[0];
            float x = float.Parse(split[1]);
            float y = float.Parse(split[2]);
            float z = float.Parse(split[3]);
            float eulY = float.Parse(split[4]);

            //赋值
            clientState.hp = 100;
            clientState.x = x; 
            clientState.y = y; 
            clientState.z = z;
            clientState.eulY = eulY;

            //广播,keyber改
            string sendStr = "Enter|" + msgString;
            byte[] sendByte = Encoding.UTF8.GetBytes(sendStr);
            foreach(ClientState cs in Program.clients.Values)
            {
                cs.socket.Send(sendByte);
            }
        }

        public static void MsgList(ClientState clientState, string msgArgs)
        {
            string sendStr = "List|";
            foreach(ClientState cs in Program.clients.Values)
            {
                sendStr += cs.socket.RemoteEndPoint.ToString() + ",";
                sendStr += cs.x.ToString() + ",";
                sendStr += cs.y.ToString() + ",";
                sendStr += cs.z.ToString() + ",";
                sendStr += cs.eulY.ToString() + ",";
                sendStr += cs.hp.ToString() + ",";
            }
            byte[] sendByte = Encoding.UTF8.GetBytes(sendStr);
            Console.WriteLine(sendStr);
            clientState.socket.Send(sendByte);
        }

        public static void MsgMove(ClientState clientState,string msgArgs)
        {
            //解析参数
            string[] split = msgArgs.Split(',');
            string desc = split[0];
            float x = float.Parse(split[1]);
            float y = float.Parse(split[2]);
            float z = float.Parse(split[3]);
            float eulY = float.Parse(split[4]);

            clientState.x = x;
            clientState.y = y;
            clientState.z = z;
            clientState.eulY = eulY;

            string sendStr = "Move|" + msgArgs;
            byte[] sendByte = Encoding.UTF8.GetBytes(sendStr);
            foreach(ClientState cs in Program.clients.Values)
            {
                cs.socket.Send(sendByte);
            }
        }

        public static void MsgAttack(ClientState clientState, string msgArgs)
        {
            //解析参数
            string sendStr = "Attack|" + msgArgs;
            byte[] sendByte = Encoding.UTF8.GetBytes(sendStr);
            foreach(ClientState cs in Program.clients.Values)
            {
                cs.socket.Send(sendByte);
            }
        }

        public static void MsgHit(ClientState clientState, string msgArgs)
        {
            //解析参数
            string[] split = msgArgs.Split(',');
            string attDesc = split[0];
            string hitDesc = split[1];

            //找出被攻击的角色
            ClientState hitCS = null;
            foreach(ClientState cs in Program.clients.Values)
            {
                if(cs.socket.RemoteEndPoint.ToString() == hitDesc)
                {
                    hitCS = cs;
                }
            }
            if (hitCS == null) return;
            //扣血
            hitCS.hp -= 25;
            //死亡
            if (hitCS.hp < 0)
            {
                string sendStr = "Die|" + hitCS.socket.RemoteEndPoint.ToString();
                byte[] sendByte = Encoding.UTF8.GetBytes(sendStr);
                foreach(ClientState cs in Program.clients.Values)
                {
                    cs.socket.Send(sendByte);
                }
            }
        }
    }
}
