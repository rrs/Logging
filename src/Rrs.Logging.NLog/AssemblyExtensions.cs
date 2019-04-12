using System.IO;
using System.Reflection;

namespace Rrs.Logging.NLog
{
    static class AssemblyExtensions
    {
        public static Stream GetManifestResourceStreamFromName(this Assembly assembly, string resourceName)
        {
            return assembly.GetManifestResourceStream(assembly.GetName().Name.Replace('-', '_') + "." + resourceName);
        }
    }
}
