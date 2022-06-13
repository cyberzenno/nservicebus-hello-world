﻿using System.Reflection;

namespace Shared
{
    public static class License
    {
        public static string Path()
        {
            var path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"License\License.xml");
            return path;
        }
    }
}