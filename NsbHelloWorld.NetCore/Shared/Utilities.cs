using System;
using System.Reflection;

namespace Shared
{
    public static class c
    {
        public static void w(string message)
        {
            Console.WriteLine(message);
        }

        public static void DisplayCustomTitleOnSmallWindow(string environment, string group, string service)
        {
            //custom title
            Console.Title = $"{environment}.{group}.{service}";

            //small window
            var width = Console.WindowWidth / 2;
            var height = Console.WindowHeight / 2;
            Console.SetWindowSize(width, height);
        }
    }
}