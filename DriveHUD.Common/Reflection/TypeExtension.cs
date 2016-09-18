using System;
using System.Linq;
using System.Reflection;

namespace DriveHUD.Common.Reflection
{
    public static class TypeExtension
    {
        public static bool InterfaceFilter(Type type, object filterCriteria)
        {
            Type filterInterface = filterCriteria as Type;
            return filterInterface == null ? false : filterInterface.IsAssignableFrom(type);
        }

        public static bool TypeIsInterface(this Type type, Type interfaceType)
        {
            if (interfaceType.IsGenericType)
            {
                Type[] genericArguments = interfaceType.GetGenericArguments();
                foreach (var intf in type.GetInterfaces().Where(i => i.IsGenericType))
                {
                    var intfGenericArguments = intf.GetGenericArguments();
                    if (genericArguments
                        .Where((t, i) => intfGenericArguments[i] == t || intfGenericArguments[i].IsSubclassOf(t))
                        .Count() == genericArguments.Length)
                        return true;
                }
                return false;
            }
            TypeFilter typeFilter = InterfaceFilter;
            return type.FindInterfaces(typeFilter, interfaceType).Length > 0;
        }
    }
}