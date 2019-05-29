using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using JsonExSerializer;
using Newtonsoft.Json;
using System.Threading;

namespace DVC_CODE_SERVER
{
    class Player
    {
        public bool exit_thread = false;
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
            lock (Server.p)
            {
                Server.p.Add(this);
            }

            string tmp = Server.Room_Info_Return();
            Console.WriteLine("방 정보 전송 : " + tmp);
            buffer = Encoding.UTF8.GetBytes(tmp + "<EOF>");
            Console.WriteLine(tmp);
            socket.Send(buffer);

            Thread t = new Thread(new ThreadStart(ThreadBody));
            t.Start();
        }

        public void ThreadBody()
        {
            byte[] buffer = new byte[1024];
            string tmp = "";

            bool exit_flag = false;
            int recvCnt;

            while (true)
            {
                try
                {
                    while (true)
                    {
                        recvCnt = socket.Receive(buffer);
                        if (recvCnt == 0 || exit_thread) // 연결 종료로 판단, 플레이어 반환
                        {
                            if (room != null) Server.Room_Exit_Request(this);
                            Server.Player_Exit(this);
                            Console.WriteLine("종료 // 닉네임 : " + name + " IP : " + socket.RemoteEndPoint.ToString());
                            exit_flag = true;
                            break;
                        }
                        tmp += Encoding.UTF8.GetString(buffer, 0 , recvCnt);
                        if (tmp.IndexOf("<EOF>") > -1) break;
                    }
                    if (exit_flag) break;
                    Console.WriteLine("" + recvCnt);
                    Console.WriteLine(tmp);

                    Message m = (Message)serializer.Deserialize(tmp);
                    m = m.DoMessage(this);
                    tmp = JsonConvert.SerializeObject(m);

                    Console.WriteLine(tmp);
                    
                    buffer = Encoding.UTF8.GetBytes(tmp);
                    socket.Send(buffer);
                    tmp = "";
                } catch (SocketException e)
                {
                    if (room != null) Server.Room_Exit_Request(this);
                    Server.Player_Exit(this);
                    Console.WriteLine("현재 연결은 원격 호스트에 의해 강제로 끊겼습니다. // 닉네임 : " + name + " IP : " + socket.RemoteEndPoint.ToString());
                    break;
                }
            }
            socket.Close();
        }
    }
}