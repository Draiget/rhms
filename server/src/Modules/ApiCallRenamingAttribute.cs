using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server.Modules
{
    internal class ApiCallRenamingAttribute : Attribute
    {
        public string ProcedureName;

        public ApiCallRenamingAttribute(string cProcedureName) {
            ProcedureName = cProcedureName;
        }
    }
}
