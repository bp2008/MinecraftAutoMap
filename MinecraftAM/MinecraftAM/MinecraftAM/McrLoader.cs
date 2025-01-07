using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace MinecraftAM
{
	public class McrLoader
	{
		public volatile float renderProgress = 0;
		public volatile float renderCompleteAt = 0;
		public volatile bool isLoading = false;
		public volatile bool bAbort = false;
		Thread LWThread;
		/// <summary>
		/// </summary>
		/// <param name="worldPath">Provide a world path for the world to load.  The "region" folder will be appended if necessary.</param>
		public void LoadWorld(MapCollector mc, string worldPath)
		{
			try
			{
				DebugDump.LogEntry("McrLoader.LoadWorld called. MapCollector: " + mc + ", worldPath: " + worldPath, includecallstack:true);
				isLoading = true;
				DirectoryInfo worldPathDir = new DirectoryInfo(worldPath);
				DirectoryInfo[] dirs = worldPathDir.GetDirectories("region", SearchOption.TopDirectoryOnly);
				foreach (DirectoryInfo di in dirs)
					if (di.Name.ToLower() == "region")
					{
						worldPathDir = new DirectoryInfo(Path.Combine(worldPathDir.FullName, "region"));
						break;
					}
				LWThread = new Thread(LoadWorldThreadStart);
				LWThread.Name = "MCRegion Parsing Thread";
				LWThread.Start(new object[] { mc, worldPathDir });
			}
			catch (Exception)
			{
				isLoading = false;
			}
		}
		private void LoadWorldThreadStart(object param)
		{
			try
			{
				object[] parameters = (object[])param;
				MapCollector mc = (MapCollector)parameters[0];
				DirectoryInfo di = (DirectoryInfo)parameters[1];
				DebugDump.LogEntry("McrLoader.LoadWorldThreadStart called. MapCollector: " + mc + ", Directory: " + (di == null ? "null" : di.FullName));
				// Select the desired region directory.
				McrReader.World mcrWorld = new McrReader.World(di);
				renderCompleteAt = mcrWorld.RegionCount * 1024;
				// Loop through all the region files.
				McrReader.Region region = mcrWorld.GetNextRegion();
				while (region != null)
				{
					if (bAbort) return;
					// Loop through all the chunk files.
					McrReader.Chunk chunk = region.GetNextChunk();
					while (chunk != null)
					{
						if (bAbort) return;
						if (chunk.data != null && chunk.data.Length > 0)
						{
							// Create a MinecraftAM.Chunk from this one.
							// Make sure chunkSize is 16 first or you will get gaps!
							Chunk amChunk = new Chunk();
							amChunk.LoadFromMcrReader(chunk);
							mc.SetChunk(amChunk, true);
						}
						if (bAbort) return;
						chunk = region.GetNextChunk();
						renderProgress++;
					}
					if (bAbort) return;
					region = mcrWorld.GetNextRegion();
				}
				Globals.loadStaticMapImmediately = true;
			}
			catch (Exception ex) { File.AppendAllText("errordump.txt", "\r\nException in McrLoader thread: " + ex.ToString() + "\r\n"); }
			finally
			{
				isLoading = false;
			}
		}
		public double percentLoaded
		{
			get
			{
				if (renderCompleteAt == 0)
					return 100;
				return Math.Round((renderProgress / renderCompleteAt) * 100);
			}
		}

		public void Abort()
		{
			bAbort = true;
			try
			{
				if (LWThread != null)
					if (LWThread.IsAlive)
					{
						LWThread.Join(50);
						LWThread.Abort();
					}
			}
			catch (Exception) { }
		}
	}
}
