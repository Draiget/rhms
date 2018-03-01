using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gpu_radeon.Api.Helpers
{
    public class LinkedFieldAttribute : Attribute
    {
        public LinkedFieldAttribute(Type searchType){
            TargetLookType = searchType;
        }

        public Type TargetLookType {
            get;
        }
    }
}
