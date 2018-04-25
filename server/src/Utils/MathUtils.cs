using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server.Utils
{
    public static class MathUtils
    {
        public static int Clamp(int value, int min, int max) {
            return value < min ? min : (value > max ? max : value);
        }

        public static uint NextLog2(long x) {
            if (x <= 0) {
                return 0;
            }

            x--;
            uint count = 0;
            while (x > 0) {
                x >>= 1;
                count++;
            }

            return count;
        }
    }
}
