using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Domen
{
    public class Igrac
    {
        public string Username { get; set; }
        public Socket Soket { get; set; }
        public int BrojBodova { get; set; } = 10;

    }
}
