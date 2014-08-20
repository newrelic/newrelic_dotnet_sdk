using System;
using System.IO;
using System.Reflection;

namespace NewRelic.Platform.Sdk.Extensions
{
    public static class ExtensionsForAssembly
    {
        public static string GetLocalPath(this Assembly assembly)
        {
            Uri uri = new Uri(assembly.CodeBase);
            return Path.GetDirectoryName(uri.LocalPath);
        }
    }
}
