using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVC_CODE_SERVER
{
    class Room
    {
        public int room_number;
        public string room_name;
        public List<Player> p = new List<Player>();
        public int max_people;
        string room_pw;

        public Room(int rnmb, string rn, Player p, int mp, bool bpw, string rpw)
        {
            room_number = rnmb;
            room_name = rn;
            this.p.Add(p);
            max_people = mp;
            if (bpw) room_pw = rpw;
        }

        public bool isPWRoom()
        {
            if (room_pw == null) return false;
            return true;
        }

        public bool PWcheck(string str)
        {
            if (room_pw == str) return true;
            return false;
        }
    }
}
