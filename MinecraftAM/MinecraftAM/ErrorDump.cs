using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace MinecraftAM
{
	public static class ErrorDump
	{
		public static void LogEntry(string message, Exception ex = null)
		{
			if (ex != null)
				message += ex.ToString();
		}
	}
	public static class DebugDump
	{
		[Conditional("DEBUG")]
		public static void LogEntry(string message, Exception ex = null, bool includecallstack = false)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine(DateTime.Now.ToString());
			sb.Append(message);
			if (ex != null)
				sb.Append(ex);
			sb.AppendLine();
			if (includecallstack)
			{
				StackTrace st = new StackTrace(true);
				for (int i = 0; i < st.FrameCount; i++)
				{
					StackFrame sf = st.GetFrame(i);
					sb.Append("   at ");
					sb.Append(sf.GetMethod());
					sb.Append(" ");
					sb.Append(sf.GetFileLineNumber().ToString());
					sb.Append(", ");
					sb.AppendLine(sf.GetFileColumnNumber().ToString());
				}
				sb.AppendLine();
			}
			sb.AppendLine();
			for (int i = 0; i < 5; i++)
			{
				try
				{
					System.IO.File.AppendAllText("debugdump.txt", sb.ToString());
					return;
				}
				catch (Exception) { }
			}
		}
	}
}
