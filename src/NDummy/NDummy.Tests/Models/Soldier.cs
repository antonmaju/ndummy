using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDummy.Tests.Models
{
    public class Soldier
    {
        public int Id { get; set; }

        public string address;

        public string Name { get; set; }

        public int Age { get; set; }

        public bool Gender { get; set; }
    }


    public class FriendlySoldier : Soldier
    {
        public FriendlySoldier Friend { get; set; }
    }

    public class Lieutenant
    {
        public Soldier Soldier { get; set; }
    }

    public class Captain
    {
        public string Name { get; set; }

        public IEnumerable<Soldier> Soldiers { get; set; }
    }
}
