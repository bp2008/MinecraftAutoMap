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
using System.Threading;
using MinecraftAM.MinecraftAM;

namespace MinecraftAM
{
	public class MapFlattener
	{

		public MapFlattener(bool threadless = false)
		{
			// Kick off the threads.

			if (!threadless)
			{
				// First kick off sync thread.  It will spawn the rest.
				syncThread = new Thread(FlattenSync);
				syncThread.Name = "SyncThread";
				syncThread.Start();
			}
		}

		/// <summary>
		/// Keeps all the worker threads in sync, making sure one buffer gets built at a time.
		/// </summary>
		Thread syncThread = null;
		/// <summary>
		/// The worker threads that actually crunch blocks.
		/// </summary>
		List<Thread> workers = new List<Thread>();

		public object syncLock = new object();

		/// <summary>
		/// LOCK syncLock before changing or looking.  Also lock this object before touching newData.  
		/// Update this, set newData = true, set the "trigger" wait handle, and the 
		/// sync thread will ensure that the buffer soon reflects the coordinates in this JobSpec.
		/// </summary>
		public JobSpec nextJob = new JobSpec();

		/// <summary>
		/// Set to true to indicate to the sync thread that a new buffer is required.
		/// </summary>
		public bool newData = false;

		/// <summary>
		/// The current data to render is always here.
		/// </summary>
		public tilebuffer frontBuffer = new tilebuffer();
		/// <summary>
		/// New data gets built directly in to this. Once complete, frontBuffer 
		/// points to the buffer created here, and this points at the buffer that was in frontbuffer.
		/// </summary>
		private tilebuffer backBuffer = new tilebuffer();

		private JobSpec currentJob;

		/// <summary>
		/// Number of worker threads
		/// </summary>
		private int iNumThreads = Environment.ProcessorCount == 2 ? 2 : (Environment.ProcessorCount > 2 ? 3 : 1);

		private bool bThreadAbort;

		/// <summary>
		/// Only do this if you're shutting down.
		/// </summary>
		public void StopThreads()
		{
			bThreadAbort = true;
			trigger.Set();

			syncThread.Join();
			foreach (EventWaitHandle wh in waithandles)
				wh.Set();
			foreach (EventWaitHandle wh in donehandles)
				wh.Set();
			foreach (Thread t in workers)
			{

				t.Join();
			}
		}

		public EventWaitHandle trigger = new EventWaitHandle(false, EventResetMode.AutoReset);
		EventWaitHandle[] waithandles = null;
		EventWaitHandle[] donehandles = null;

		public int GetCurrentWidth()
		{
			return width;
		}
		public int GetCurrentHeight()
		{
			return height;
		}
		// parameters for the current job. worker threads must not change these.
		int width;
		int height;
		/// <summary>
		/// The depth to use, either the indoor or the outdoor draw depth.
		/// </summary>
		int depth;
		/// <summary>
		/// The z position to start drawing at (the ceiling or the virtual ceiling).
		/// </summary>
		int zstart;
		private void FlattenSync()
		{
			waithandles = new EventWaitHandle[iNumThreads];
			donehandles = new EventWaitHandle[iNumThreads];
			for (int i = 0; i < iNumThreads; i++)
			{
				Thread t = new Thread(WorkerThread);
				t.Name = "FlattenWorker" + i;
				EventWaitHandle wh = new EventWaitHandle(false, EventResetMode.AutoReset);
				waithandles[i] = wh;
				wh = new EventWaitHandle(false, EventResetMode.AutoReset);
				donehandles[i] = wh;
				workers.Add(t);
				t.Start(i);
			}
			while (!bThreadAbort)
			{
				trigger.WaitOne();
				if (bThreadAbort)
					return;
				if (newData)
				{
					lock (syncLock)
					{
						currentJob = nextJob;
						newData = false;
					}

					width = currentJob.bottomRight.X - currentJob.topLeft.X;
					height = currentJob.bottomRight.Y - currentJob.topLeft.Y;

					if (backBuffer.buffer == null || backBuffer.buffer.Length != (width * height))
						backBuffer.buffer = new MCTile[width, height];
					backBuffer.topLeft = currentJob.topLeft.Copy();

					//if (currentJob.player != null)
					//{
					int VirtualCeiling = AMSettings.iOutdoorCeiling;
					int MaxCeiling = AMSettings.iMaxCeilingHeight;

					int zPlayer = currentJob.player.Z;
					int xPlayer = currentJob.player.X + Chunk.chunkSizeM1;
					int yPlayer = currentJob.player.Y;

					// Add the depth offset to the player's height (pretend that he is offset by that much).
					if (zPlayer + Globals.depthOffset >= Globals.worldHeightM1)
						zPlayer = Globals.worldHeightM1;
					else if (zPlayer + Globals.depthOffset < 0)
						zPlayer = 0;
					else
						zPlayer = (zPlayer + Globals.depthOffset);

					zstart = Math.Min(Globals.worldHeightM1, zPlayer + VirtualCeiling);
					//p = Globals.userPlayer;
					// Determine where drawing ceiling is
					// when this loop exits, zstart will be the ceiling, if one exists.  Otherwise, zstart will be the virtual ceiling.
					for (int z = zPlayer + 1; z < zPlayer + VirtualCeiling && z < Globals.worldHeightM1; z++)
					{
						Block b = Globals.chunks.GetBlock(xPlayer, yPlayer, z);
						if (b == null)
						{
							break;
						}
						if (b.blockType != BlockType.Air && b.blockType != BlockType.Leaves && b.blockType != BlockType.Torch_D && b.blockType != BlockType.Redstone_torch_off_state_D && b.blockType != BlockType.Redstone_torch_on_state_D && b.blockType != BlockType.Ladder_D)
						{
							zstart = (z - 1);
							break;
						}
					}
					if (!DataSource.PermissionManager.allowCaveMapping)
					{
						zstart = Globals.worldHeightM1;
						depth = Globals.worldHeightM1;
					}
					else if (zstart == Globals.worldHeightM1 || zstart - zPlayer > MaxCeiling)
						depth = AMSettings.iOutdoorDepth;
					else
						depth = AMSettings.iIndoorDepth;

					//if ((short)zPlayer + (short)VirtualCeiling > Globals.worldHeightM1)
					//    depth += (((short)zPlayer + (short)VirtualCeiling) - Globals.worldHeightM1);
					//}
					//else
					//{
					//    depth = Chunk.height;
					//    zstart = Chunk.height;
					//}
					foreach (EventWaitHandle wh in waithandles)
						wh.Set();

					EventWaitHandle.WaitAll(donehandles);

					if (bThreadAbort)
						return;
					Globals.flattens++;

					// this pass is done, swap the buffers.
					tilebuffer temp = frontBuffer;
					frontBuffer = backBuffer;
					backBuffer = temp;
				}
			}
		}

		private void WorkerThread(object threadNum)
		{
			int tNum = (int)threadNum;
			while (!bThreadAbort)
			{
				waithandles[tNum].WaitOne();
				if (bThreadAbort)
					return;

				FlattenParams fp = new FlattenParams(currentJob.topLeft, currentJob.customChunk, backBuffer.buffer, width, height, depth, zstart, tNum, iNumThreads);

				Flatten(fp);

				donehandles[tNum].Set();
			}
		}

		private bool ThreadsStillRunning(Thread[] workingThreads)
		{
			for (int i = 0; i < workingThreads.Length; i++)
				if (workingThreads[i].ThreadState == ThreadState.Running)
					return true;
			return false;
		}
		private void mtFlatten(object param)
		{
			Flatten((FlattenParams)param);
		}

		/// <summary>
		/// This gets called automatically in the background.  Calling it manually is probably asking for bad performance.
		/// </summary>
		/// <param name="fp"></param>
		public void Flatten(FlattenParams fp)
		{
			Block b = null;
			bool encounteredAir = false;
			bool encounteredTransparent = false;
			bool encounteredOre = false;
			bool tileApplied = false;
			ChunkManager cm = Globals.chunks;
			bool custChunk = false;
			if (fp.customChunk != null)
			{
				//fp.startXForThisThread = 0;
				//fp.width = Chunk.chunkSize;
				//fp.height = Chunk.chunkSize;
				//fp.zstart = Chunk.height;
				//fp.depth = Chunk.height;
				custChunk = true;
			}
			int oreHeight = -1;
			for (int x = fp.startXForThisThread; x < fp.width; x += fp.threadCount)
			{
				for (int y = 0; y < fp.height; y++)
				{
					b = null;
					encounteredAir = false;
					encounteredTransparent = false;
					encounteredOre = false;
					tileApplied = false;
					oreHeight = -1;
					TransparentBlockCounter tbc = new TransparentBlockCounter(AMSettings.iTransparentLiquidMax);
					for (int z = fp.zstart; fp.zstart - z <= fp.depth && z >= 0; z--)
					{
						if (custChunk)
							b = fp.customChunk.blocks[x, y, z];
						else
							b = cm.GetBlock(fp.topLeft.X + x, fp.topLeft.Y + y, z);
						if (b == null)
							continue;
						// Find first air block.
						if (b.blockType == BlockType.Air)
						{
							encounteredAir = true;
							continue;
						}
						else if (!AMSettings.bWaterDetection && !fp.isStaticMapFlatten
							&& (b.blockType == BlockType.Stationary_water_D || b.blockType == BlockType.Water_D))
						{
							encounteredAir = true;
							continue;
						}
						else if (!AMSettings.bLavaDetection && !fp.isStaticMapFlatten
							&& (b.blockType == BlockType.Stationary_lava_D || b.blockType == BlockType.Lava_D))
						{
							encounteredAir = true;
							continue;
						}
						else if (!encounteredAir && ItemDrawing.transparentBlocks[(int)b.blockType])
							encounteredTransparent = true;
						// 
						if (encounteredAir || encounteredTransparent || (AMSettings.bOreDetection && DataSource.PermissionManager.allowOreDetection && !fp.isStaticMapFlatten && Block.IsOre((int)b.blockType)))
						{
							if (AMSettings.bOreDetection && DataSource.PermissionManager.allowOreDetection && !fp.isStaticMapFlatten && Block.IsOre((int)b.blockType))
							{
								if (oreHeight == -1)
									oreHeight = z;
								encounteredOre = true;
							}
							float shade = CalcShade(fp, z);
							if (!tileApplied)
							{
								tileApplied = true;
								if (fp.data[x, y] == null)
									fp.data[x, y] = new MCTile(b, new Color(shade, shade, shade));
								else
									fp.data[x, y].Reset(b, shade, shade, shade);
								fp.data[x, y].height = z;
								ColorTrail(b, fp.data[x, y], fp.isStaticMapFlatten);
							}
							else
							{
								MCTile newTile = new MCTile(b, new Color(shade, shade, shade));
								newTile.nextTile = fp.data[x, y];
								ColorTrail(b, newTile, fp.isStaticMapFlatten);
								fp.data[x, y] = newTile;
								fp.data[x, y].height = z;
							}
							if (b != null && AMSettings.MarkBlocks.Count > 0 && AMSettings.MarkBlocks.Contains((int)b.blockType))
								ColorMark(fp.data[x, y], fp.isStaticMapFlatten);
							if (!tbc.TreatAsTransparent((int)b.blockType))
							{
								break;
							}
						}
					}
					if (!encounteredAir && !encounteredTransparent && !encounteredOre)
					{
						if (fp.data[x, y] == null)
							fp.data[x, y] = new MCTile(AMSettings.cNotDrawnBlockColor);
						else
							fp.data[x, y].Reset(AMSettings.cNotDrawnBlockColor);
					}
					else if (b != null && (b.blockType == BlockType.Air || tbc.TreatAsTransparent((int)b.blockType)))
					{
						AddBlackBlock(fp, x, y, tileApplied);
					}
					else if (encounteredOre && oreHeight != -1)
					{
						fp.data[x, y].height = oreHeight;
					}
				}
			}
		}
		private static void ColorMark(MCTile newTile, bool isStaticMapFlatten = false)
		{
			if (isStaticMapFlatten)
				return;
			newTile.c.R = 255;
			newTile.c.G = 0;
			newTile.c.B = 0;
		}
		private static void ColorTrail(Block b, MCTile newTile, bool isStaticMapFlatten = false)
		{

			if (AMSettings.bGlowingPath && !isStaticMapFlatten && b.trail && Globals.currentTrail.active)
			{
				int newVal = newTile.c.R + AMSettings.iGlowingIntensity;
				if (newVal > 255)
					newVal = 255;
				byte reduction = (byte)(AMSettings.iGlowingIntensity - (newVal - newTile.c.R));
				newTile.c.B -= reduction;
				newTile.c.G -= reduction;
				newTile.c.R = (byte)newVal;
			}
		}

		private static float CalcShade(FlattenParams fp, int z)
		{

			float shade = ((1f * (float)z - (fp.zstart - fp.depth)) / fp.depth);
			return shade;
		}

		private static void AddBlackBlock(FlattenParams fp, int x, int y, bool tileApplied)
		{
			if (fp.data[x, y] == null)
				fp.data[x, y] = new MCTile(Color.Black);
			else if (tileApplied)
			{
				MCTile newTile = new MCTile(Color.Black);
				newTile.nextTile = fp.data[x, y];
				fp.data[x, y] = newTile;
			}
			else if (!tileApplied)
				fp.data[x, y].Reset(0, 0, 0);

		}

		public class JobSpec
		{
			public IntVector2 topLeft;
			public IntVector2 bottomRight;
			public IntVector3 player;

			public Chunk customChunk = null;
		}
		public class FlattenParams
		{
			public IntVector2 topLeft;
			//public ChunkManager cm;
			public MCTile[,] data;
			public int width;
			public int height;
			public int depth;
			public int zstart;
			public int startXForThisThread;
			public int threadCount;
			public bool isStaticMapFlatten;
			public Chunk customChunk = null;
			/// <summary>
			/// 
			/// </summary>
			/// <param name="topLeft"></param>
			/// <param name="specialCaseChunkToRender">Assign this to simply render this chunk and ignore other params and the chunk manager.</param>
			/// <param name="data"></param>
			/// <param name="width"></param>
			/// <param name="height"></param>
			/// <param name="depth"></param>
			/// <param name="zstart"></param>
			/// <param name="x"></param>
			public FlattenParams(IntVector2 topLeft, Chunk specialCaseChunkToRender, MCTile[,] data, int width, int height, int depth, int zstart, int x, int numberOfThreads, bool isStaticMapFlatten = false)
			{
				this.topLeft = topLeft;
				this.data = data;
				this.width = width;
				this.height = height;
				this.depth = depth;
				this.zstart = zstart;
				this.startXForThisThread = x;
				this.customChunk = specialCaseChunkToRender;
				this.threadCount = numberOfThreads;
				this.isStaticMapFlatten = isStaticMapFlatten;
			}

			public FlattenParams(FlattenParams fp, int x)
			{
				this.topLeft = fp.topLeft;
				this.data = fp.data;
				this.width = fp.width;
				this.height = fp.height;
				this.depth = fp.depth;
				this.zstart = fp.zstart;
				this.startXForThisThread = x;
				this.customChunk = fp.customChunk;
				this.threadCount = fp.threadCount;
			}
		}
	}

	public enum ScanDirection
	{
		UP,
		DOWN
	}
}
