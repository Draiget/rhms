using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gpu_radeon.Api.Helpers
{
    public class LinkedDelegateAttribute : Attribute
    {
        public LinkedDelegateAttribute(string externFunctionName){
            FunctionName = externFunctionName;
        }

        public string FunctionName {
            get;
        }
    }
}
