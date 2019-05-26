using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace DVC_CODE_SERVER
{
    class Message
    {
        //for all flags
        int flags;
        //for 1 flags
        string room_info;
        //for 2 flags
        string room_name;
        int max_people;
        bool bool_pw;
        string room_pw;
        //for 3 flags
        bool room_make_success;
        //for 4 flags
        int room_number;
        //for 5 flags
        int room_enter_code;
        //for 6 flags
        bool room_exit_success;

        /* flags
         * 0 : 방 정보 요청
         * 1 : 방 정보 답장
         * 
         * 2 : 방 개설 요청
         * 3 : 방 개설 답장
         * 
         * 4 : 방 입장 요청
         * 5 : 방 입장 답장
         * 
         * 6 : 방 퇴장 요청
         * 7 : 방 퇴장 답장
         */

        public Message DoMessage(Player p)
        {
            Message m = new Message();

            Console.WriteLine(p.name + "유저가 " + flags + "번 명령을 요청하였습니다.");

            if(flags==0)
            {
                m.flags = 1;
                m.room_info = Server.Room_Info_Return();
                Console.WriteLine(flags + "번 명령 : 방 정보 요청");
                Console.WriteLine("답장 : " + m.flags + "번 명령 : 방 정보 답장");
                Console.WriteLine(m.room_info);
            } else if (flags == 2)
            {
                room_make_success = Server.Room_Make_Request(room_name, p, max_people, bool_pw, room_pw);
                m.flags = 3;
            } else if (flags == 4)
            {
                room_enter_code = Server.Room_Enter_Request(room_number, p, room_pw);
                m.flags = 5;
            } else if (flags ==6)
            {
                room_exit_success = Server.Room_Exit_Request(room_number, p);
                m.flags = 7;
            }
            return m;
        }
    }
}
