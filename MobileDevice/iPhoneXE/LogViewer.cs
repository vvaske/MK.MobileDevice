/*

 */
using System;

namespace MK.MobileDevice.XEDevice 
{
	/// <summary>
	/// Description of LogViewer.
	/// </summary>
	public class LogViewer
	{
		public LogViewer()
		{
		}
		/*
		public static void LogEvent(string a, params object[] args)
		{
			foreach (object o in args)
			{
				Console.WriteLine(o.ToString());
			}
		}
		*/
		public static void LogEvent(string arg)
		{
			Console.WriteLine(arg);
		}
        public static void LogEvent(int x, string arg)
        {
            Console.WriteLine(arg);
        }
    }
}
