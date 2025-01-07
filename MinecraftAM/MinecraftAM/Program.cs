using System;

namespace MinecraftAM
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			bool patch = false;
			if (args.Length > 0 && args[0] == "patch")
				patch = true;
			using (Game1 game = new Game1(patch))
			{
				try
				{
					game.Run();
				}
				catch (Exception err)
				{
					System.IO.File.WriteAllText("crashdump.txt", err.ToString());
				}
			}
		}
	}
}

