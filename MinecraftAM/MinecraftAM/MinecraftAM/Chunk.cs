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
using DataSource;

namespace MinecraftAM
{
	public class IntVector3
	{
		public IntVector3(int x, int y, int z)
		{
			X = x;
			Y = y;
			Z = z;
		}
		public int X;
		public int Y;
		public int Z;

		internal IntVector3 Copy()
		{
			return new IntVector3(X, Y, Z);
		}
	}
	public class IntVector2
	{
		public IntVector2(int x, int y)
		{
			X = x;
			Y = y;
		}
		public int X;
		public int Y;

		internal IntVector2 Copy()
		{
			return new IntVector2(X, Y);
		}
	}
	public class Chunk
	{
		/// <summary>
		/// 0,0 in this chunk corresponds to the block coordinates in this variable.
		/// </summary>
		public IntVector2 o;
		public static int chunkSize = 16;
		public static int height = Globals.worldHeight;
		public static int chunkSizeM1 = chunkSize - 1;
		public Block[, ,] blocks;

		public bool isLoaded = false;

		public bool Is(int x, int y)
		{
			return o.X == x && o.Y == y;
		}

		public Chunk()
		{
			o = new IntVector2(0, 0);
			blocks = new Block[chunkSize, chunkSize, height];
		}
		public void Recycle()
		{
			//for (int i = 0; i < chunkSize; i++)
			//    for (int j = 0; j < chunkSize; j++)
			//        for (int k = 0; k < height; k++)
			//            if (blocks[i, j, k] == null)
			//                blocks[i, j, k] = new Block(0, 0, 0);
			//            else
			//                blocks[i, j, k].Recycle();
			o.X = o.Y = 0;
			isLoaded = false;
		}

		public delegate void VoidFunc();


		public bool DoLoad()
		{
			DataSource.DS.GetChunkData(o.X, o.Y, 0, chunkSize, chunkSize, height);
			DataSource.DS.GetPlayerLocation();
			return true;
		}

		public void LoadFromData(IntVector2 origin, byte[] data)
		{
			o = origin;
			int i = 0;
			short id;
			//Block[, ,] backbuffer = new Block[chunkSize, chunkSize, height];
			for (int a = chunkSize - 1; a >= 0; a--)
			{
				for (int b = 0; b < chunkSize; b++)
				{
					//Block firstBlock = null;
					//Block secondBlock = null;
					for (int c = 0; c < height; c++)
					{
						id = ByteConverter.ToInt16(data, i);
						i += 2;
						if (blocks[a, b, c] == null)
							blocks[a, b, c] = new Block(id, data[i++], data[i++]);
						else
							blocks[a, b, c].Recycle(id, data[i++], data[i++]);
						//byte id = data[i++];
						//byte meta = data[i++];
						//byte light = data[i++];
						//lightDisplay = DataSource.ByteConverter.ToSingle(data, i);
						//i += 4;
						//secondBlock = new Block(id, meta, light);
						//backbuffer[a, b, c] = secondBlock;
						//if (blocks[a, b, c] != null)
						//    backbuffer[a, b, c].trail = blocks[a, b, c].trail;
						//if (firstBlock != null && firstBlock.blockLight == 0)
						//    firstBlock.blockLight = secondBlock.blockLight;
						//firstBlock = secondBlock;
					}
				}
			}
			isLoaded = true;
			//blocks = backbuffer;
		}
		/// <summary>
		/// This function requires the chunk size to be 16.
		/// </summary>
		/// <param name="chnk"></param>
		public void LoadFromMcrReader(McrReader.Chunk chnk)
		{
			o = new IntVector2(chnk.X * 16 - 1, chnk.Y * 16);
			Block[, ,] backbuffer = new Block[chunkSize, chunkSize, height];
			for (int x = 0; x < chunkSize; x++)
			{
				for (int y = 0; y < chunkSize; y++)
				{
					for (int z = 0; z < height; z++)
					{
						backbuffer[x, y, z] = new Block(chnk.data[z + (chunkSizeM1 - x) * Globals.worldHeight + y * 2048], Block.b0, Block.b15);
						if (blocks[x, y, z] != null)
							backbuffer[x, y, z].trail = blocks[x, y, z].trail;
					}
				}
			}
			isLoaded = true;
			blocks = backbuffer;
		}

	}
}
