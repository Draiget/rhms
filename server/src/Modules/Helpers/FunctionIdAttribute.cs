using System;

namespace server.Modules.Helpers
{
    [AttributeUsage(AttributeTargets.Delegate)]
    public class FunctionIdBaseAttribute : Attribute
    {
        public FunctionIdBaseAttribute(uint functionId) {
            FunctionId = functionId;
        }

        public uint FunctionId { get; set; }
    }
}
