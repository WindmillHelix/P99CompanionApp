using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WindmillHelix.Companion99.Common
{
    public static class ResourceHelper
    {
        public static byte[] GetResourceBytes(string resourceName)
        {
            Assembly callingAssembly = Assembly.GetCallingAssembly();
            return GetResourceBytes(callingAssembly, resourceName);
        }

        public static byte[] GetResourceBytes(Assembly assembly, string resourceName, bool explicitName)
        {
            Stream resourceStream = GetResourceStream(assembly, resourceName, explicitName);
            int streamLength = (int)resourceStream.Length; //trim to int, resources must be > 2GB, fairly safe assumption
            byte[] buffer = new byte[streamLength];
            resourceStream.Read(buffer, 0, streamLength);
            return buffer;
        }

        public static byte[] GetResourceBytes(Assembly assembly, string resourceName)
        {
            return GetResourceBytes(assembly, resourceName, false);
        }

        public static string GetResourceString(string resourceName, bool explicitName)
        {
            Assembly callingAssembly = Assembly.GetCallingAssembly();
            return GetResourceString(callingAssembly, resourceName, explicitName);
        }

        public static string GetResourceString(string resourceName)
        {
            Assembly callingAssembly = Assembly.GetCallingAssembly();
            return GetResourceString(callingAssembly, resourceName, false);
        }

        public static string GetResourceString(Assembly assembly, string resourceName, bool explicitName)
        {
            Stream resourceStream = GetResourceStream(assembly, resourceName, explicitName);
            StreamReader reader = new StreamReader(resourceStream);
            return reader.ReadToEnd();
        }

        public static string GetResourceString(Assembly assembly, string resourceName)
        {
            return GetResourceString(assembly, resourceName, false);
        }

        public static Stream GetResourceStream(string resourceName, bool explicitName)
        {
            Assembly callingAssembly = Assembly.GetCallingAssembly();
            return GetResourceStream(callingAssembly, resourceName, explicitName);
        }

        public static Stream GetResourceStream(string resourceName)
        {
            Assembly callingAssembly = Assembly.GetCallingAssembly();
            return GetResourceStream(callingAssembly, resourceName, false);
        }

        public static Stream GetResourceStream(Assembly assembly, string resourceName, bool explicitName)
        {
            string fullName = string.Empty;
            if (explicitName)
            {
                fullName = resourceName;
            }
            else
            {
                fullName = assembly.GetName().Name + "." + resourceName;
            }

            return assembly.GetManifestResourceStream(fullName);
        }

        public static string[] GetResourceList()
        {
            Assembly callingAssembly = Assembly.GetCallingAssembly();
            return GetResourceList(callingAssembly);
        }

        public static string[] GetResourceList(Assembly assembly)
        {
            string[] resourceNames = assembly.GetManifestResourceNames();
            return resourceNames;
        }
    }
}
