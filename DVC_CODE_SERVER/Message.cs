﻿using System;
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
        public int flags;
        //for 1 flags
        public string room_info;
        //for 2 flags
        public string room_name;
        public int max_people;
        public bool bool_pw;
        public string room_pw;
        //for 3 flags
        public bool room_make_success;
        //for 4 flags
        public int room_number;
        //for 5 flags
        public int room_enter_code;
        //for 6 flags
        public bool room_exit_success;

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
         * 
         * 8 : 플레이어 종료 요청
         */

        public Message DoMessage(Player p)
        {
            Message m = new Message();

            Console.WriteLine(p.name + "유저가 " + flags + "번 명령을 요청하였습니다.");

            if (flags == 0)
            {
                m.flags = 1;
                m.room_info = Server.Room_Info_Return();
                Console.WriteLine(flags + "번 명령 : 방 정보 요청");
                Console.WriteLine("답장 : " + m.flags + "번 명령 : 방 정보 답장");
                Console.WriteLine(m.room_info);
            }
            else if (flags == 2)
            {
                m.room_make_success = true;
                m.room_number = Server.Room_Make_Request(room_name, p, max_people, bool_pw, room_pw);
                m.room_pw = room_pw;
                m.flags = 3;
                Console.WriteLine(flags + "번 명령 : 방 개설 요청");
                Console.WriteLine("답장 : " + m.flags + "번 명령 : 방 개설 답장");
                Console.WriteLine("성공여부 : " + m.room_make_success);
            }
            else if (flags == 4)
            {
                m.room_enter_code = Server.Room_Enter_Request(room_number, p, room_pw);
                m.flags = 5;
                Console.WriteLine(flags + "번 명령 : 방 입장 요청");
                Console.WriteLine("답장 : " + m.flags + "번 명령 : 방 입장 답장");
                if (m.room_enter_code == 0)
                {
                    Console.WriteLine("입장 성공");
                }
                else if (m.room_enter_code == 1)
                {
                    Console.WriteLine("방 번호가 없음. 서버 오류거나 방이 폐쇄됨");
                }
                else if (m.room_enter_code == 2)
                {
                    Console.WriteLine("방 정원 초과로 입장 불가");
                }
                else if (m.room_enter_code == 3)
                {
                    Console.WriteLine("비밀번호 틀림");
                }
            }
            else if (flags == 6)
            {
                m.room_exit_success = Server.Room_Exit_Request(p);
                m.flags = 7;
                Console.WriteLine(flags + "번 명령 : 방 퇴장 요청");
                Console.WriteLine("답장 : " + m.flags + "번 명령 : 방 퇴장 답장");
                Console.WriteLine("성공여부 : " + m.room_exit_success);
            }
            else if (flags == 8)
            {
                p.exit_thread = true;
            }
            return m;
        }
    }
}
