using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Sockets;
using JsonExSerializer;


namespace DVC_CODE_SERVER
{
    class Server
    {
        static List<Room> r = new List<Room>();
        public static List<Player> p = new List<Player>();
        static int room_number_assign = 1;

        static void Main(string[] args)
        {
            Console.WriteLine("다빈치코드 서버 가동");
            Login_Handler l = new Login_Handler();
            Thread t = new Thread(new ThreadStart(l.Login_Handler_Thread));
            t.Start();
            while (true)
            {
                Console.WriteLine("현재 접속 원 수 : " + p.Count());
                Thread.Sleep(1000);
            }
            // t.Join();
        }

        public static string Room_Info_Return()
        {
            string tmp;
            lock (r)
            {
                tmp = r.Count().ToString(); // 방 몇개인지 알려줌
                for (int i = 0; i < r.Count(); i++)
                {
                    tmp += " " + r[i].room_number + " " + r[i].room_name + " " + r[i].p[0].name + " "
                        + r[i].p.Count() + " " + r[i].max_people + " " + r[i].isPWRoom().ToString();
                } // "(방갯수) 방번호 방이름 방장 인원수 최대인원수 비밀번호방여부"
            }
            return tmp;
        }

        public static bool Room_Make_Request(string room_name, Player player, int max_people, bool bool_pw, string room_pw)
        {
            Room room = new Room(room_number_assign++, room_name, player, max_people, bool_pw, room_pw);
            lock (r)
            {
                r.Add(room);
                player.room = room;
            }
            return true;
        }

        public static int Room_Enter_Request(int room_number, Player player, string room_pw)
        {
            /*
             * 0 : 입장 성공
             * 1 : 그런 방 번호가 없는데요?
             * 2 : 방이 꽉참
             * 3 : 비밀번호 방인데 비번이 틀림
             */
            lock (r)
            {
                for (int i = 0; i < r.Count(); i++)
                {
                    if (r[i].room_number == room_number)
                    {
                        // 비밀번호 방이면 비밀번호는 맞는가?
                        if ((r[i].isPWRoom() && r[i].PWcheck(room_pw)) || !r[i].isPWRoom())
                        {
                            // 방에 자리가 있는가?
                            if (r[i].max_people <= r[i].p.Count()) return 2;
                            // 방 번호도 있고, 비번도 맞고, 자리도 있으면 추가
                            r[i].p.Add(player);
                            return 0;
                        }
                    }
                }
            }
            return 1;
        }

        public static bool Room_Exit_Request(int room_number, Player player)
        {
            lock (r)
            {
                for (int i = 0; i < r.Count(); i++)
                {
                    if (r[i].room_number == room_number)
                    {
                        for (int j = 0; j < r[j].p.Count(); j++)
                        {
                            if (r[i].p[j].name == player.name)
                            {
                                r[i].p.Remove(player);
                                if (r[i].p.Count() == 0) r.Remove(r[i]);
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public static void Player_Exit(Player player)
        {
            lock (p) p.Remove(player);
        }
    }

    class Login_Handler
    {
        int portNumber = 13799;
        int maxConnectionNumber = 1000;
        Socket socket;
        IPAddress addr;

        public Login_Handler()
        {
            string outip = new WebClient().DownloadString("https://api.ipify.org/");
            //addr = IPAddress.Parse(new WebClient().DownloadString("https://api.ipify.org/"));
            IPHostEntry host = Dns.Resolve(Dns.GetHostName());
            string myip = host.AddressList[0].ToString();
            addr = IPAddress.Parse(myip);
            Console.WriteLine("공인IP : " + outip + " 서버개설IP : " + myip);
        }

        public void Login_Handler_Thread()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(addr, portNumber));
            socket.Listen(maxConnectionNumber);
            Console.WriteLine("서버 초기화 완료.. 연결 대기중");

            while (true)
            {
                Socket accepted = socket.Accept(); // 새로운 접속 -> 게임 유저 생성처리, 현재 개설된 방 정보 전송
                Player p = new Player(accepted);
            }
        }
    }
}