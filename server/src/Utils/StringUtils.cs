using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server.Utils
{
    public static class StringUtils
    {
        public static string ConvertToIdString(this string self, string emptyReplacer = "_") {
            var str = self.ToLower().Replace(" ", emptyReplacer);
            while (str.StartsWith("-") && str.Length > 0) {
                str = str.Substring(1);
            }

            return str;
        }
    }
}
