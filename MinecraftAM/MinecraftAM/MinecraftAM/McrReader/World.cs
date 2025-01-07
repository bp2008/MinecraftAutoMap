using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace McrReader
{
	// Example Usage:
	//    // Select the desired region directory.
	//    McrReader.World mcrWorld = new McrReader.World(new DirectoryInfo("%AppData%/.minecraft/saves/World1/region/"));
	//    // Loop through all the region files.
	//    McrReader.Region region = mcrWorld.GetNextRegion();
	//    while (region != null)
	//    {
	//        // Loop through all the chunk files.
	//        McrReader.Chunk chunk = region.GetNextChunk();
	//        while (chunk != null)
	//        {
	//            if (chunk.data != null && chunk.data.Length > 0)
	//            {
	//                // Create a MinecraftAM.Chunk from this one.
	//                // Make sure chunkSize is 16 first!
	//                MinecraftAM.Chunk amChunk = new MinecraftAM.Chunk();
	//                amChunk.LoadFromMcrReader(chunk);
	//            }
	//            chunk = region.GetNextChunk();
	//        }
	//        region = mcrWorld.GetNextRegion();
	//    }
	public class World
	{
		private FileInfo[] regionFiles;
		private int currentRegion = 0;
		public int RegionCount
		{
			get
			{
				if (regionFiles == null)
					return 0;
				else
					return regionFiles.Length;
			}
		}
		/// <summary>
		/// Point me at a region folder.  Ex: "%AppData%\.minecraft\saves\World1\region"
		/// </summary>
		/// <param name="regionDir"></param>
		public World(DirectoryInfo regionDir)
		{
			if (!regionDir.Exists)
				return;
			regionFiles = regionDir.GetFiles("*.mcr", SearchOption.TopDirectoryOnly);
			if (regionFiles.Length <= 0)
				return;
		}
		/// <summary>
		/// Returns a new region every time this is called.  When the world runs out of regions, null is returned.  Be sure to remove all references to each region once finished with it.
		/// </summary>
		/// <returns></returns>
		public Region GetNextRegion()
		{
			if (currentRegion < RegionCount)
				return new Region(regionFiles[currentRegion++]);
			return null;
		}
	}
}
