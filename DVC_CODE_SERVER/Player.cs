using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using JsonExSerializer;
using System.Threading;

namespace DVC_CODE_SERVER
{
    class Player
    {
        public static Serializer serializer = new Serializer(typeof(Message));
        public Room room;
        public Socket socket;
        public string name;

        public Player(Socket s)
        {
            socket = s;
            byte[] buffer = new byte[1024];
            int recvCnt = socket.Receive(buffer);
            byte[] formatted = new byte[recvCnt];
            for(int i=0; i<recvCnt; i++)
            {
                formatted[i] = buffer[i];
            }
            name = Encoding.UTF8.GetString(formatted);
            Console.WriteLine("접속 // 닉네임 : " + name + " IP : " + socket.RemoteEndPoint.ToString());

            Server.p.Add(this);

            string tmp = Server.Room_Info_Return();
            Console.WriteLine("방 정보 전송 : " + tmp);
            buffer = Encoding.UTF8.GetBytes(tmp);
            socket.Send(buffer);

            Thread t = new Thread(new ThreadStart(ThreadBody));
            t.Start();
        }

        public void ThreadBody()
        {
            byte[] buffer = new byte[1024];
            string tmp;
            while (true)
            {
                int recvCnt = socket.Receive(buffer);
                if (recvCnt == 0) // 연결 종료로 판단, 플레이어 반환
                {
                    Server.Player_Exit(this);
                    Console.WriteLine("종료 // 닉네임 : " + name + " IP : " + socket.RemoteEndPoint.ToString());
                    break;
                }
                tmp = Encoding.UTF8.GetString(buffer);
                Message m = (Message) serializer.Deserialize(tmp);
                m = m.DoMessage(this);
                tmp = serializer.Serialize(m);
                buffer = Encoding.UTF8.GetBytes(tmp);
                socket.Send(buffer);
            }
            socket.Close();
        }
    }
}
