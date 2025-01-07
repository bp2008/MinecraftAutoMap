using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using MinecraftAM;

namespace McrReader
{
	public class Chunk
	{
		private int localX;
		private int localY;
		public int X;
		public int Y;
		public byte[] data = new byte[0];
		private static int chunkDataSize = 16 * 16 * Globals.worldHeight;
		/// <summary>
		/// A chunk is a byte array of size 32768 (the number of blocks).
		/// </summary>
		/// <param name="localX">X coordinate of the chunk in the region (0-31)</param>
		/// <param name="localY">Y coordinate of the chunk in the region (0-31)</param>
		/// <param name="X">X coordinate of the chunk out of all chunks.</param>
		/// <param name="Y">Y coordinate of the chunk out of all chunks.</param>
		/// <param name="Region"></param>
		public Chunk(int localX, int localY, int X, int Y, byte[] Region)
		{
			this.localX = localX;
			this.localY = localY;
			this.X = X;
			this.Y = Y;
			LoadChunkFromRegion(Region);
		}
		/// <summary>
		/// Blocks are arranged in the data array in sets of vertical columns, east to west, then north to south.
		/// </summary>
		/// <param name="X"></param>
		/// <param name="Y"></param>
		/// <param name="Z"></param>
		/// <returns></returns>
		public byte GetBlockID(int X, int Y, int Z)
		{
			return data[Z + (15 - X) * Globals.worldHeight + Y * 2048];
		}
		private void LoadChunkFromRegion(byte[] Region)
		{
			int chunkOffsetLocation = 4 * (localY + (31 - localX) * 32);
			int chunkOffset = 4096 * (int)(Region[chunkOffsetLocation] << 16 | Region[chunkOffsetLocation + 1] << 8 | Region[chunkOffsetLocation + 2]);
			int chunkSize = 4096 * Region[chunkOffsetLocation + 3];
			if (chunkOffset == 0 || chunkSize == 0)
				return;
			byte[] ChunkUnzipped = UnzipChunk(Region, chunkOffset, chunkSize);
			data = loadBlocks(ChunkUnzipped);
		}

		private static byte[] UnzipChunk(byte[] Region, int chunkOffset, int chunkSize)
		{
			if (Region.Length > chunkOffset + 5)
			{
				int length = (int)(Region[chunkOffset] << 24 | Region[chunkOffset + 1] << 16 | Region[chunkOffset + 2] << 8 | Region[chunkOffset + 3]);
				if (Region[chunkOffset + 4] == 2)
				{
					using (MemoryStream bytesOut = new MemoryStream())
					{
						using (MemoryStream bytesIn = new MemoryStream(Region, chunkOffset + 7, length - 3))
						{
							using (DeflateStream decompressor = new DeflateStream(bytesIn, CompressionMode.Decompress))
							{
								try
								{
									decompressor.CopyTo(bytesOut);
                                }
#if DEBUG
								catch (System.IO.InvalidDataException ex)
								{
									Console.WriteLine("Error: " + ex.ToString());
                                }
#else
                                catch (System.IO.InvalidDataException)
                                {
                                }
#endif
							}
							return bytesOut.ToArray();
						}
					}
				}
			}
			return new byte[0];
		}
		private static byte[] loadBlocks(byte[] ChunkUnzipped)
		{
			// blockTagTitle is the byte 7 specifying TAG_Byte_Array, 0, 6, specifying 6 UTF 8 characters, B, l, o, c, k, s
			byte[] blockTagTitle = new byte[9];
			blockTagTitle[0] = 0x07;
			blockTagTitle[1] = 0x00;
			blockTagTitle[2] = 0x06;
			blockTagTitle[3] = 0x42;
			blockTagTitle[4] = 0x6c;
			blockTagTitle[5] = 0x6f;
			blockTagTitle[6] = 0x63;
			blockTagTitle[7] = 0x6b;
			blockTagTitle[8] = 0x73;


			using (MemoryStream bytesIn = new MemoryStream(ChunkUnzipped))
			{
				BinaryReader inReader = new BinaryReader(bytesIn);
				int position = -1;
				for (int index = 0; index < ChunkUnzipped.Length - blockTagTitle.Length; index++)
				{
					if (ChunkUnzipped[index] == blockTagTitle[0]
						&& ChunkUnzipped[index + 1] == blockTagTitle[1]
						&& ChunkUnzipped[index + 2] == blockTagTitle[2]
						&& ChunkUnzipped[index + 3] == blockTagTitle[3]
						&& ChunkUnzipped[index + 4] == blockTagTitle[4]
						&& ChunkUnzipped[index + 5] == blockTagTitle[5]
						&& ChunkUnzipped[index + 6] == blockTagTitle[6]
						&& ChunkUnzipped[index + 7] == blockTagTitle[7]
						&& ChunkUnzipped[index + 8] == blockTagTitle[8]
						)
					{
						position = index;
						break;
					}
				}
				if (position != -1 && position < ChunkUnzipped.Length)
				{
					inReader.BaseStream.Seek(position + blockTagTitle.Length, SeekOrigin.Begin);
					byte[] lengthBytes = inReader.ReadBytes(4);
					if (BitConverter.IsLittleEndian)
						Array.Reverse(lengthBytes);
					int length = BitConverter.ToInt32(lengthBytes, 0);
					if (length != chunkDataSize)
						return new byte[0];
					return inReader.ReadBytes(length);
				}
				else
				{
					return null;
				}
			}
		}
	}
}
