using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtelierXNA
{
    public static class Flags
    {
        public static bool Combat { get; set; }
        static Flags()
        {
            Combat = false;
        }
    }
}
