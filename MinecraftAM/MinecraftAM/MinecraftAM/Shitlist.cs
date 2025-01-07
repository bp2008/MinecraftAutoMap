using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftAM
{
	/// <summary>
	/// A list of people who have personally wronged an AutoMap developer.  Oops!
	/// </summary>
	public class Shitlist
	{
		private static HashSet<string> shitList;
		static Shitlist()
		{
			shitList = new HashSet<string>();
			shitList.Add("hitachi_irl");
			shitList.Add("hobberty");
			shitList.Add("detsiF");
		}
		public static bool IsOnShitlist(string name)
		{
			return shitList.Contains(name);
		}
	}
}
