using server.Modules.Helpers;

namespace gpu_nvidia.Helpers
{
    public class FunctionIdAttribute : FunctionIdBaseAttribute
    {
        public FunctionIdAttribute(FunctionId functionId)
            : base((uint)functionId){
        }
    }
}
