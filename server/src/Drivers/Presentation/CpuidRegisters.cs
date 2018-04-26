using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server.Drivers.Presentation
{
    public class CpuidRegisters
    {
        /// <summary>
        /// Offset 0
        /// </summary>
        public uint Eax;

        /// <summary>
        /// Offset 1
        /// </summary>
        public uint Ebx;

        /// <summary>
        /// Offset 2
        /// </summary>
        public uint Ecx;

        /// <summary>
        /// Offset 3
        /// </summary>
        public uint Edx;

        public CpuidRegisters(){
            Clear();
        }

        public string GetVendorString(){
            var vendorBuilder = new StringBuilder();
            AppendRegister(vendorBuilder, Ebx);
            AppendRegister(vendorBuilder, Edx);
            AppendRegister(vendorBuilder, Ecx);
            return vendorBuilder.ToString();
        }

        public void GetVendorExtendedString(ref StringBuilder builder) {
            AppendRegister(builder, Eax);
            AppendRegister(builder, Ebx);
            AppendRegister(builder, Ecx);
            AppendRegister(builder, Edx);
        }

        private static void AppendRegister(StringBuilder b, uint value) {
            b.Append((char)(value & 0xff));
            b.Append((char)((value >> 8) & 0xff));
            b.Append((char)((value >> 16) & 0xff));
            b.Append((char)((value >> 24) & 0xff));
        }

        public void Clear(){
            Eax = Ebx = Ecx = Edx = 0;
        }
    }
}
