using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Forms;
using System.Threading;
using System.Collections.Concurrent;

namespace MinecraftAM
{
	public class MapCollector
	{
		TgaWriter tgaWriter = null;
		// Fields unrelated to the current cache
		public MapFlattener cFlattener = new MapFlattener(true);
		public GraphicsDevice gd = null;
		//Texture2D tex = null;
		int chunkWidth = Chunk.chunkSize;
		int chunkHeight = Chunk.chunkSize;
		ConcurrentQueue<KeyValuePair<MCTile[,], IntVector2>> flatChunkQueue;

		volatile bool suspendUpdates = false;
		// Fields related to the current cache
		bool initialized = false;


		FileInfo MapFile;
		public MapCollector()
		{
			flatChunkQueue = new ConcurrentQueue<KeyValuePair<MCTile[,], IntVector2>>();
			MapFile = new FileInfo(Globals.getStaticMapName());
			if (MapFile.Exists)
			{
				TgaReader tgar = new TgaReader(MapFile.FullName);
				tgar.Start();
				tgar.ReadMetaData();
				tgar.Stop();

				ushort cacheWidth = 2048;
				ushort cacheHeight = 2048;
				if (AMSettings.iInternalStaticMapGeneration == 2)
				{
					cacheWidth = tgar.width;
					cacheHeight = tgar.height;
				}
				else if (AMSettings.iInternalStaticMapGeneration == 3)
				{
					cacheWidth = (ushort)tgar.mapWidth;
					cacheHeight = (ushort)tgar.mapHeight;
				}
				int left = tgar.originX;
				int top = tgar.originY;

				byte colorDepth = tgar.bitsPerPixel;

				Start(left, top, cacheWidth, cacheHeight, colorDepth);

				if (AMSettings.iInternalStaticMapGeneration == 3)
				{
					FixedStaticMap.Load(MapFile.FullName);
					Globals.loadStaticMapImmediately = true;
				}

				initialized = true;
			}
		}

		private void Start(int left, int top, ushort cacheWidth, ushort cacheHeight, byte colorDepth)
		{
			if (AMSettings.iInternalStaticMapGeneration == 2)
			{
				tgaWriter = new TgaWriter(MapFile.FullName, "Minecraft AutoMap Static Map v1", left, top, cacheWidth, cacheHeight, colorDepth);
				tgaWriter.Start();
			}
			else if (AMSettings.iInternalStaticMapGeneration == 3)
			{
				FixedStaticMap.Start(left, top, cacheWidth, cacheHeight);
			}
		}

		public void Stop(bool wait)
		{
			if (AMSettings.iInternalStaticMapGeneration == 2)
			{
				if (tgaWriter != null)
					tgaWriter.Stop(wait);
			}
			else if (AMSettings.iInternalStaticMapGeneration == 3)
			{
				FixedStaticMap.Stop();
			}
		}
		public void SetChunk(Chunk c, bool notCreatingThread = false)
		{
			if (gd == null || (AMSettings.bDisableStaticMapStreaming && !notCreatingThread) || AMSettings.bDisableStaticMap)
				return;
			MCTile[,] output = new MCTile[chunkWidth, chunkHeight];
			// These FlattenParams are for static map flattening.
			int mapDepth = Chunk.height;
			int mapStart = Chunk.height - 1;
			if (Globals.worldDimension == -1 && AMSettings.iNetherStaticMapHeightStart >= 0)
			{
				mapDepth = AMSettings.iNetherStaticMapHeightStart + 1;
				mapStart = AMSettings.iNetherStaticMapHeightStart;
			}
			MapFlattener.FlattenParams fp = new MapFlattener.FlattenParams(new IntVector2(0, 0), c, output, chunkWidth, chunkHeight, mapDepth, mapStart, 0, 1, true);
			cFlattener.Flatten(fp);
			if (notCreatingThread || suspendUpdates)
				flatChunkQueue.Enqueue(new KeyValuePair<MCTile[,], IntVector2>(output, c.o));
			else
				SaveFlatChunk(output, c.o, false);
		}

		private void SaveFlatChunk(MCTile[,] output, IntVector2 origin, bool isQueued)
		{
			KeyValuePair<MCTile[,], IntVector2> queuedChunk;
			while (!isQueued && flatChunkQueue.TryDequeue(out queuedChunk))
			{
				SaveFlatChunk(queuedChunk.Key, queuedChunk.Value, true);
			}
			if (!initialized)
			{
				ushort cacheWidth = 2048;
				ushort cacheHeight = 2048;
				int left = origin.X - (cacheWidth / 2);
				int top = origin.Y - (cacheHeight / 2);

				Start(left, top, cacheWidth, cacheHeight, (byte)AMSettings.iStaticMapBitsPerPixel);

				initialized = true;
			}

			int airCount = 0;
			Color[] colorData = new Color[chunkWidth * chunkHeight];
			for (int y = 0; y < chunkHeight; y++)
			{
				for (int x = 0; x < chunkWidth; x++)
				{
					MCTile t = output[y, x];
					while (t.nextTile != null)
						t = t.nextTile;
					//int idx = ((y * chunkWidth) + x) * bytesPerBlock;
					byte blockType = (byte)t.blockType;
					if (blockType == 0)
						airCount++;
					float av = 1 - (((t.c.R + t.c.B + t.c.G) / 3) / 127f);
					Color c = new Color(0, 0, 0, 255);
					colorData[x * chunkWidth + y] = Color.Lerp(Block.pureColor[blockType], c, av);
				}
			}
			if (Globals.multiplayer && (Globals.worldDimension != -1 && airCount > 14))
				return; // This must be from SMP where Minecraft didn't have the chunk completely downloaded.  Do not use it or it might overwrite some good data.
			if (AMSettings.iInternalStaticMapGeneration == 2)
				tgaWriter.WriteRectangle(colorData, origin.X, origin.Y, chunkWidth, chunkHeight);
			else if (AMSettings.iInternalStaticMapGeneration == 3)
				FixedStaticMap.WriteChunk(colorData, origin.X, origin.Y, chunkWidth, chunkHeight);
		}
	}
}
