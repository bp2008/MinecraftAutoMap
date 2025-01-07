using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace McrReader
{
	public class Region
	{
		public int X;
		public int Y;
		public int ChunkCount = 0;
		private byte[] data;
		private int currentChunk = 0;
		public Region(string filePath) : this(new FileInfo(filePath))
		{
		}
		public Region(FileInfo fi)
		{
			string[] nameParts = fi.Name.Split('.');
			if (nameParts.Length != 4)
				return;
			if (!int.TryParse(nameParts[1], out Y) || !int.TryParse(nameParts[2], out X))
				return;
			X *= -1;
			X -= 1;
			data = loadRegion(fi);
			if (data != null && data.Length > 0)
				ChunkCount = 1024;
			
		}
		/// <summary>
		/// Returns a new chunk every time this is called.  When the region runs out of chunks, null is returned.  Be sure to remove all references to each chunk once finished with it.
		/// </summary>
		/// <param name="fileInfo"></param>
		/// <returns></returns>
		public Chunk GetNextChunk()
		{
			if (currentChunk < ChunkCount)
			{
				int x = (31 - currentChunk / 32);
				int y = currentChunk % 32;
				currentChunk++;
				return new Chunk(x, y, (x + (32 * X)) + 1, y + (32 * Y), data);
			}
			return null;
		}
		private static byte[] loadRegion(FileInfo fileInfo)
		{
			using (FileStream inFile = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				using (MemoryStream bytesOut = new MemoryStream())
				{
					try
					{
						inFile.CopyTo(bytesOut);
					}
					catch (InvalidDataException)
					{
#if DEBUG
						Console.WriteLine("Chunk with bad header: " + fileInfo.Name);
#endif
					}
					return bytesOut.ToArray();
				}
			}
		}
	}
}
