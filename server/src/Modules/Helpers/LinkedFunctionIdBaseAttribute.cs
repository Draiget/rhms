using System;

namespace server.Modules.Helpers
{
    [AttributeUsage(AttributeTargets.Field)]
    public class LinkedFunctionIdBaseAttribute : Attribute
    {
        public LinkedFunctionIdBaseAttribute(uint functionId) {
            FunctionId = functionId;
        }

        public uint FunctionId { get; set; }
    }
}
