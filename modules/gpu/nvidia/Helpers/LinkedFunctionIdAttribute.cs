using server.Modules.Helpers;

namespace gpu_nvidia.Helpers
{
    public class LinkedFunctionIdAttribute : LinkedFunctionIdBaseAttribute
    {
        public LinkedFunctionIdAttribute(FunctionId functionId)
            : base((uint)functionId){
        }
    }
}
