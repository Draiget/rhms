using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace server.Utils
{
    public static class AssemblyUtils
    {
        public static Type[] GetTypesInNamespace(Assembly assembly, string nameSpace) {
            return assembly.GetTypes().Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal)).ToArray();
        }

        public static Type[] GetTypesInNamespace(string nameSpace){
            return GetTypesInNamespace(Assembly.GetExecutingAssembly(), nameSpace);
        }

        public static Type[] GetAttributesInModule<T>(Type assemblyClass, T attribute) where T : Type {
            var classes = assemblyClass.Assembly.GetTypes().Where(t => t.GetCustomAttributes(attribute).Any());
            return classes.Where(x => x.IsClass).ToArray();
        }
    }
}
