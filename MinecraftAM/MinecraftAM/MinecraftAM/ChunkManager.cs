using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MinecraftAM
{
	public class ChunkManager : DataSource.DSHandlerChunk
	{
		const int cacheSize = 20;
		Chunk[,] cache = new Chunk[cacheSize, cacheSize];
		private Queue<Chunk> loadQ = new Queue<Chunk>();
		private Queue<Chunk> sentQ = new Queue<Chunk>();
		bool bThreadAbort = false;
		List<Thread> loaders = new List<Thread>();
		List<EventWaitHandle> waithandles = new List<EventWaitHandle>();
		public int chunksLoaded = 0;
		public int chunkLoadTotal = 0;
		public int loaderThreadCount = 1;
		public MapCollector collector;

		public ChunkManager()
		{
			DataSource.DS.InitializeChunks(this);
			StartLoaders();
		}

		public void StartLoaders()
		{
			for (int i = 0; i < loaderThreadCount; i++)
			{
				Thread t = new Thread(ChunkLoader);
				t.Name = "ChunkLoader" + i;
				waithandles.Add(new EventWaitHandle(false, EventResetMode.AutoReset));
				loaders.Add(t);
				t.Start(i);
			}
		}

		public void StopLoaders()
		{
			bThreadAbort = true;
			foreach (EventWaitHandle wh in waithandles)
				wh.Set();
			foreach (Thread t in loaders)
			{
				t.Join();
			}
		}

		internal void UpdateChunksAround(int blockx, int blocky)
		{
			int cx, cy;
			ChunkFromCoord(blockx, blocky, out cx, out cy);
			int ux, uy;

			lock (loadQ)
			{
				for (ux = cx - 1; ux <= cx + 1; ux++)
					for (uy = cy - 1; uy <= cy + 1; uy++)
						loadQ.Enqueue(GetChunk(ux, uy));
			}
			foreach (EventWaitHandle wh in waithandles)
				wh.Set();
		}
		public Chunk GetChunk(int cx, int cy)
		{
			Chunk c;

			int cachex = cx % cacheSize;
			int cachey = cy % cacheSize;

			if (cachex < 0)
				cachex = cacheSize + cachex;
			if (cachey < 0)
				cachey = cacheSize + cachey;


			c = cache[cachex, cachey];

			if (c != null && c.o.X == cx * Chunk.chunkSize && c.o.Y == cy * Chunk.chunkSize)
			{
				return c;
			}
			else
			{
				//if (c != null)
				//    Globals.cacheCollisions++;
				//else
				//    Globals.cacheMiss++;
				if (c == null)
					c = new Chunk();
				else
					c.Recycle();
				c.o.X = cx * Chunk.chunkSize;
				c.o.Y = cy * Chunk.chunkSize;
				cache[cachex, cachey] = c;
				lock (loadQ)
				{
					loadQ.Enqueue(c);
				}
				foreach (EventWaitHandle wh in waithandles)
					wh.Set();
				return c;
			}
		}
		public static Block airBlock = new Block((byte)BlockType.Air, Block.b0, Block.b15);
		public static Block brBlock = new Block((byte)BlockType.Bedrock_Adminium, Block.b0, Block.b0);

		/// <summary>
		/// Returns a block if its been loaded, otherwise null.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <returns></returns>
		public Block GetBlock(int x, int y, int z)
		{
			if (z < 0)
				return brBlock;
			if (z >= Chunk.height)
				return airBlock;
			int cx;
			int cy;

			ChunkFromCoord(x, y, out cx, out cy);
			Chunk c = GetChunk(cx, cy);

			int cox = c.o.X;
			int coy = c.o.Y;
			if (!c.isLoaded)
				return null;

			if (x < 0)
				x = x - ((cox * Chunk.chunkSize));
			if (y < 0)
				y = y - ((coy * Chunk.chunkSize));
			x = x % Chunk.chunkSize;
			y = y % Chunk.chunkSize;
			if (x < 0 || y < 0)
				return null;
			return c.blocks[x, y, z];
		}

		public void ChunkFromCoord(int x, int y, out int cx, out int cy)
		{
			cx = x / Chunk.chunkSize;
			cy = y / Chunk.chunkSize;
			if (x < 0 && x % Chunk.chunkSize != 0)
				cx--;
			if (y < 0 && y % Chunk.chunkSize != 0)
				cy--;
		}

		private Chunk GetTop(Queue<Chunk> q)
		{
			lock (q)
			{
				if (q.Count > 0)
					return q.Dequeue();
			}
			return null;
		}
		private RawChunkData GetTop(Queue<RawChunkData> q)
		{
			lock (q)
			{
				if (q.Count > 0)
					return q.Dequeue();
			}
			return null;
		}

		public void ClearCache()
		{
			cache = new Chunk[cacheSize, cacheSize];
		}

		Queue<RawChunkData> rawChunks = new Queue<RawChunkData>();

		public void WorldChunkReceived(int x, int y, int z, int dx, int dy, int dz, byte[] data)
		{
			Globals.loadingWorld = false;
			if (data == null)
				Globals.badpacketsRcvd++;

			Interlocked.Increment(ref chunksLoaded);

			Chunk c = null;
			bool isCorrectChunk = false;

			while (!isCorrectChunk)
			{
				c = GetTop(sentQ);
				if (c == null)
				{
					isCorrectChunk = false;
					NullUnloadedChunkAtBlockCoord(x, y);
				}
				else
				{
					isCorrectChunk = (c.o.X == x && c.o.Y == y);
					if (!isCorrectChunk)
					{
						NullUnloadedChunkAtBlockCoord(c.o.X, c.o.Y);
					}
				}
			}
			if (c == null)
				throw new ApplicationException("Received chunk " + x + "," + y + "," + z + " but did not expect a chunk with these coordinates. (Possibly out of order?)");
			if (data == null)
			{
				NullUnloadedChunkAtBlockCoord(c.o.X, c.o.Y); // DataSource detected a bad packet.
				return;
			}
			c.LoadFromData(new IntVector2(x, y), data);

			if (collector != null && !AMSettings.bDisableStaticMap && !AMSettings.bDisableStaticMapStreaming)
				collector.SetChunk(c);
		}

		private void NullUnloadedChunkAtBlockCoord(int x, int y)
		{

			int cx;
			int cy;
			ChunkFromCoord(x, y, out cx, out cy);
			int cachex = cx % cacheSize;
			int cachey = cy % cacheSize;

			if (cachex < 0)
				cachex = cacheSize + cachex;
			if (cachey < 0)
				cachey = cacheSize + cachey;
			if (cache[cachex, cachey] != null && !cache[cachex, cachey].isLoaded)
				cache[cachex, cachey] = null; // we never received this chunk's data.  Remove it so we can try again.
		}
		private void ChunkLoader(object threadNumber)
		{
			int tNum = (int)threadNumber;
			while (!bThreadAbort)
			{
				Chunk c;
				waithandles[tNum].WaitOne();
				if (bThreadAbort)
					return;
				while ((c = GetTop(loadQ)) != null)
				{
					Interlocked.Increment(ref chunkLoadTotal);
					lock (sentQ)
					{
						sentQ.Enqueue(c);
					}
					c.DoLoad();
				}
			}
		}
	}
}
