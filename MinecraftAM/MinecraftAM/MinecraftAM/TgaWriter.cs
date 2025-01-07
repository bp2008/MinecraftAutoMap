using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Threading;
using System.Collections.Concurrent;

/// <summary>
/// TgaWriter v1
/// By Brian Pearce
/// </summary>
namespace MinecraftAM
{
	/// <summary>
	/// TgaWriter v1
	/// By Brian Pearce
	/// </summary>
	public class TgaWriter
	{
		public static SemaphoreSlim mapSync = new SemaphoreSlim(1, 1);
		public FileInfo file;
		public FileStream fs;
		private string imageIDstr;
		private ushort width;
		private ushort height;
		private byte bpp;
		private byte bytesPerPixel;
		private byte[] imageID;
		private byte[] transparentBytes;
		private volatile bool bAbort = false;
		private Thread MapWritingThread;
		private ConcurrentQueue<RectangleWriteParams> queuedRectangles;
		private int expandAmount = 2048;

		// top is the block Y coordinate of the topmost block in the cache.  ditto for left, right, bottom.  example values are set already.
		private int top, left, bottom, right; // bounds tracking
		private class RectangleWriteParams
		{
			public Color[] colors;
			public int x;
			public int y;
			public int rectWidth;
			public int rectHeight;

			public RectangleWriteParams(Color[] colors, int x, int y, int rectWidth, int rectHeight)
			{
				// TODO: Complete member initialization
				this.colors = colors;
				this.x = x;
				this.y = y;
				this.rectWidth = rectWidth;
				this.rectHeight = rectHeight;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="path"></param>
		/// <param name="imageID">Must not be larger than 255 bytes, ASCII encoded. (255 chars).</param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="bpp">Must be 16, 24, or 32.  Other values will cause undesirable behavior.</param>
		public TgaWriter(string path, string imageID, int left, int top, ushort width, ushort height, byte bpp = 16, int mapWidth = 0, int mapHeight = 0)
		{
			file = new FileInfo(path);
			this.left = left;
			this.top = top;
			this.right = left + width;
			this.bottom = top + height;
			this.imageIDstr = imageID;
			imageID = imageID + "," + left + "," + top;
			if (mapWidth > 0 && mapHeight > 0)
				imageID = imageID + "," + mapWidth + "," + mapHeight;
			this.imageID = ASCIIEncoding.ASCII.GetBytes(imageID.Length > 255 ? imageID.Substring(0, 255) : imageID);
			this.width = width;
			this.height = height;
			this.bpp = bpp;
			this.bytesPerPixel = (byte)(this.bpp / 8);
			if (bytesPerPixel == 2)
				transparentBytes = BitConverter.GetBytes(Convert.ToUInt16("1000000000000000", 2));
			else if (bytesPerPixel == 3)
			{
				byte gray = Convert.ToByte("01001111", 2);
				transparentBytes = new byte[] { gray, gray, gray };
			}
			else if (bytesPerPixel == 4)
				transparentBytes = BitConverter.GetBytes((UInt32)0);
		}
		public void MapWritingLoop()
		{
			StartPrivate();
			while (!bAbort)
			{
				RectangleWriteParams rwp;
				if (!queuedRectangles.TryDequeue(out rwp))
				{
					Thread.Sleep(5);
					continue;
				}
				bool expandBorders = false;
				int amountLeft = 0, amountTop = 0, amountRight = 0, amountBottom = 0;
				if (rwp.x < this.left)
				{
					amountLeft = ((Math.Abs(this.left - rwp.x) / expandAmount) + 1) * expandAmount;
					expandBorders = true;
				}
				if (rwp.x + rwp.rectWidth >= this.right)
				{
					amountRight = ((Math.Abs(this.right - (rwp.x + rwp.rectWidth)) / expandAmount) + 1) * expandAmount;
					expandBorders = true;
				}
				if (rwp.y < this.top)
				{
					amountTop = ((Math.Abs(this.top - rwp.y) / expandAmount) + 1) * expandAmount;
					expandBorders = true;
				}
				if (rwp.y + rwp.rectHeight >= this.bottom)
				{
					amountBottom = ((Math.Abs(this.bottom - (rwp.y + rwp.rectHeight)) / expandAmount) + 1) * expandAmount;
					expandBorders = true;
				}
				if (expandBorders)
				{
					// Borders need expanded.  Can we expand?
					if (((amountLeft > 0 || amountRight > 0) && this.width > (65000 - expandAmount)) || ((amountTop > 0 || amountBottom > 0) && this.height > (65000 - expandAmount)))
						continue; // This expand would overrun the possible borders.  Throw away this chunk instead.
					// We can expand.

					///////////////// Wait until the reader is finished, then disallow it access again until we are done expanding.
					mapSync.Wait();
					/////////////////
					// Stop the writer
					this.StopPrivate();

					// Rename the old file so its data can be copied into the new file.
					string originalFilePath = this.file.FullName;
					FileInfo tempFile = new FileInfo(this.file.FullName + ".temp");
					if (tempFile.Exists)
						tempFile.Delete();
					this.file.MoveTo(tempFile.FullName);
					this.file = new FileInfo(originalFilePath);

					// Set new field values
					this.left -= amountLeft;
					this.right += amountRight;
					this.top -= amountTop;
					this.bottom += amountBottom;
					this.width = (ushort)(this.right - this.left);
					this.height = (ushort)(this.bottom - this.top);
					string imageIDstrLocal = this.imageIDstr + "," + left + "," + top;
					this.imageID = ASCIIEncoding.ASCII.GetBytes(imageIDstrLocal.Length > 255 ? imageIDstrLocal.Substring(0, 255) : imageIDstrLocal);

					// Start the writer again, which will cause a new image to be created with the new field values.
					this.StartPrivate();

					// Read the old file data and write it to the new file
					TgaReader tgar = new TgaReader(tempFile.FullName);
					tgar.Start();
					tgar.ReadMetaData();
					int seekIndex = 18 + this.imageID.Length + (amountTop * width) * bytesPerPixel;
					byte[] lineBuffer;
					while (tgar.ReadNextLine(out lineBuffer) && !bAbort)
					{
						seekIndex += amountLeft * bytesPerPixel;
						this.fs.Seek(seekIndex, SeekOrigin.Begin);
						this.fs.Write(lineBuffer, 0, lineBuffer.Length);
						seekIndex += lineBuffer.Length;
						seekIndex += amountRight * bytesPerPixel;
					}
					tgar.Stop();
					if (bAbort)
					{
						Thread.Sleep(1);
						this.file.Delete();
						Thread.Sleep(1);
						tempFile.MoveTo(originalFilePath);
						return;
					}
					tempFile.Delete();

					///////////////// Allow the map to be accessed again by the reader.
					mapSync.Release();
					/////////////////
				}
				WriteRectangleData(rwp.colors, rwp.x - this.left, rwp.y - this.top, rwp.rectWidth, rwp.rectHeight);
			}
			StopPrivate();
		}

		public void Start()
		{
			queuedRectangles = new ConcurrentQueue<RectangleWriteParams>();
			bAbort = false;
			if (MapWritingThread != null && MapWritingThread.IsAlive)
			{
				bAbort = true;
				MapWritingThread.Join();
			}
			MapWritingThread = new Thread(MapWritingLoop);
			MapWritingThread.Name = "MapWritingThread";
			MapWritingThread.Start();
		}
		public void Stop(bool wait)
		{
			bAbort = true;
			if (wait && MapWritingThread != null && MapWritingThread.IsAlive)
				MapWritingThread.Join();
		}
		public void WriteRectangle(Color[] colors, int x, int y, int rectWidth, int rectHeight)
		{
			queuedRectangles.Enqueue(new RectangleWriteParams(colors, x, y, rectWidth, rectHeight));
		}
		/// <summary>
		/// Starts the Writer, writes the specified color data block, and stops the writer.
		/// </summary>
		/// <param name="colors"></param>
		public void WriteImage(Color[] colors)
		{
			try
			{
				StartPrivate();
				fs.Seek(18 + imageID.Length, SeekOrigin.Begin);
				WriteNextPixels(colors);
			}
			catch (Exception ex)
			{
				System.IO.File.AppendAllText("warningdump.txt", DateTime.Now.ToString() + Environment.NewLine + ex.ToString() + Environment.NewLine + Environment.NewLine);
			}
			StopPrivate();
		}
		private void StartPrivate()
		{
			fs = new FileStream(file.FullName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
			long expectedLength = 18 + imageID.Length + width * height * bytesPerPixel;
			long actualLength = fs.Length;
			if (actualLength < expectedLength)
			{
				fs.Seek(18 + imageID.Length, SeekOrigin.Begin);
				while (fs.Position < expectedLength)
					fs.Write(transparentBytes, 0, transparentBytes.Length);
			}
			WriteHeaderData();
		}
		private void WriteNextPixel(byte R, byte G, byte B, byte A)
		{
			WriteNextPixelData(R, G, B, A);
		}
		private void WriteRectangleData(Color[] colors, int x, int y, int rectWidth, int rectHeight)
		{
			int seekIndex = 18 + imageID.Length + (x + y * width) * bytesPerPixel;
			for (int oy = 0; oy < rectHeight; oy++)
			{
				fs.Seek(seekIndex, SeekOrigin.Begin);
				WriteNextPixels(colors, oy * rectWidth, rectWidth);
				seekIndex += width * bytesPerPixel;
			}
		}
		/// <summary>
		/// Writes all the colors in the array, starting at the beginning of the array or at the optional start position, and continuing through the end of the array, or through the optional count.
		/// </summary>
		/// <param name="colors">The array of colors to write.</param>
		/// <param name="start">The start position.</param>
		/// <param name="count">The number of colors to write from this array.</param>
		private void WriteNextPixels(Color[] colors, int start = 0, int count = -1)
		{
			int end = start + (count < 0 ? colors.Length : count);
			if (end > colors.Length)
				end = colors.Length;
			for (int i = start; i < end; i++)
				WriteNextPixelData(colors[i]);
		}
		private void StopPrivate()
		{
			fs.Close();
		}

		private void WriteHeaderData()
		{
			fs.Seek(0, SeekOrigin.Begin);
			// Field no. 1.	Length 1 byte.	ID length.	Length of the image ID field
			fs.WriteByte((byte)imageID.Length);
			// Field no. 2.	Length 1 byte.	Color map type.	Whether a color map is included
			fs.WriteByte((byte)0);
			// Field no. 3.	Length 1 byte.	Image type.	Compression and color types
			fs.WriteByte((byte)2);
			// Field no. 4.	Length 5 bytes.	Color map specification.	Describes the color map
			fs.Write(new byte[5], 0, 5);
			// Field no. 5.	Length 10 bytes.	Image specification.	Image dimensions and format
			// 5.1 2b X-origin (lower left)
			fs.Write(BitConverter.GetBytes((short)0), 0, 2);
			// 5.2 2b Y-origin (lower left)
			fs.Write(BitConverter.GetBytes((short)0), 0, 2);
			// 5.3 2b Width
			fs.Write(BitConverter.GetBytes(width), 0, 2);
			// 5.4 2b Height
			fs.Write(BitConverter.GetBytes(height), 0, 2);
			// 5.5 1b Color Depth
			fs.WriteByte(bpp);
			// 5.6 1b Image Descriptor
			fs.WriteByte(Convert.ToByte("0010000" + (bytesPerPixel == 2 ? "1" : "0"), 2));

			// Write the Image ID
			fs.Write(imageID, 0, imageID.Length);

			// No Color map, so don't write it.
		}
		private void WriteNextPixelData(Color color)
		{
			WriteNextPixelData(color.R, color.G, color.B, color.A);
		}
		private void WriteNextPixelData(byte R, byte G, byte B, byte A)
		{
			if (bytesPerPixel != 4 && A != 255)
				fs.Write(transparentBytes, 0, transparentBytes.Length);
			else if (bytesPerPixel == 2)
			{
				short color = (short)(((R >> 3) << 10) + ((G >> 3) << 5) + (B >> 3));
				fs.Write(BitConverter.GetBytes(color), 0, 2);
			}
			else if (bytesPerPixel == 3)
			{
				fs.WriteByte(B);
				fs.WriteByte(G);
				fs.WriteByte(R);
			}
			else if (bytesPerPixel == 4)
			{
				fs.WriteByte(B);
				fs.WriteByte(G);
				fs.WriteByte(R);
				fs.WriteByte(A);
			}
		}
	}
	public class TgaReader
	{
		public FileInfo file;
		public FileStream fs;
		/// <summary>
		/// Width of this tga image.
		/// </summary>
		public ushort width = 0;
		/// <summary>
		/// Height of this tga image.
		/// </summary>
		public ushort height = 0;
		public string imageID = "";
		public int originX = 0;
		public int originY = 0;
		/// <summary>
		/// Width of the area being mapped.
		/// </summary>
		public int mapWidth = 0;
		/// <summary>
		/// Height of the area being mapped.
		/// </summary>
		public int mapHeight = 0;
		private int imageIDBytesLength = 0;
		private float imgPixelsPerTexturePixelW;
		private float imgPixelsPerTexturePixelH;
		public byte bitsPerPixel = 16;
		private int bytesPerPixel = 2;
		private ushort first1Bit = Convert.ToUInt16("1000000000000000", 2);
		private ushort first5BitSet = Convert.ToUInt16("0111110000000000", 2);
		private ushort second5BitSet = Convert.ToUInt16("0000001111100000", 2);
		private ushort third5BitSet = Convert.ToUInt16("0000000000011111", 2);
		private int weightAccuracy = 1000;
		private int currentLine = 0;
		public bool bReadThreadAbort = false;
		public TgaReader(string path)
		{
			file = new FileInfo(path);
		}
		public void Start()
		{
			fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
		}
		public void ReadMetaData()
		{
			fs.Seek(0, SeekOrigin.Begin);
			byte[] header = new byte[18];
			fs.Read(header, 0, 18);
			width = BitConverter.ToUInt16(header, 12);
			height = BitConverter.ToUInt16(header, 14);
			bitsPerPixel = header[16];
			bytesPerPixel = bitsPerPixel / 8;
			imageIDBytesLength = header[0];
			byte[] imageIDBytes = new byte[imageIDBytesLength];
			fs.Read(imageIDBytes, 0, imageIDBytesLength);
			imageID = ASCIIEncoding.ASCII.GetString(imageIDBytes);
			string[] parts = imageID.Split(',');
			originX = int.Parse(parts[1]);
			originY = int.Parse(parts[2]);
			if (parts.Length >= 5)
			{
				mapWidth = int.Parse(parts[3]);
				mapHeight = int.Parse(parts[4]);
			}
		}
		public void Stop()
		{
			try
			{
				fs.Close();
			}
			catch (Exception) { }
		}

		public bool ReadNextLine(out byte[] lineBuffer)
		{
			if (currentLine >= this.height)
			{
				lineBuffer = new byte[0];
				return false;
			}
			int seekPosition = 18 + imageIDBytesLength + currentLine * width * bytesPerPixel;
			currentLine++;
			lineBuffer = new byte[width * bytesPerPixel];
			this.fs.Read(lineBuffer, 0, lineBuffer.Length);
			return true;
		}

		public Texture2D ReadImageIntoTexture(GraphicsDevice gd)
		{
			fs.Seek(18 + imageIDBytesLength, SeekOrigin.Begin);
			int texWidth = Math.Min(2048, (int)width);
			int texHeight = Math.Min(2048, (int)height);

			// If one of the values was higher than 2048, we need to shrink the image and maintain the aspect ratio.
			if (width > 2048 || height > 2048)
			{
				if (width > height)
					texHeight = (int)((float)texWidth * ((float)height / (float)width));
				else if (height > width)
					texWidth = (int)((float)texHeight * ((float)width / (float)height));

				// Make darn sure we are not exceeding the texture limit.
				texWidth = Math.Min(2048, texWidth);
				texHeight = Math.Min(2048, texHeight);
			}

			Texture2D tex = new Texture2D(gd, texWidth, texHeight);

			imgPixelsPerTexturePixelW = (float)width / (float)texWidth;

			imgPixelsPerTexturePixelH = (float)height / (float)texHeight;

			Color[] texColors = new Color[tex.Width * tex.Height];
			for (int i = 0; i < texHeight; i++)
			{
				if (bReadThreadAbort)
					break;
				GetColorLineForTexture(tex, i, texColors);
			}
			tex.SetData<Color>(texColors);
			return tex;
		}

		private void GetColorLineForTexture(Texture2D tex, int lineIndex, Color[] texColors)
		{
			float fStartY = lineIndex * imgPixelsPerTexturePixelH;
			int startY = (int)fStartY;
			float fEndY = fStartY + imgPixelsPerTexturePixelH;
			int endY = (int)fEndY;
			if (fEndY - endY > 0) endY++;
			if (endY > height) endY = height;
			if (startY == endY) startY--;
			if (startY < 0)
			{
				int loopStart2 = lineIndex * tex.Width;
				int loopEnd2 = loopStart2 + tex.Width;
				for (int i = loopStart2; i < loopEnd2; i++)
					texColors[i] = Color.Transparent;
				return;
			}

			int numRowsToRead = (endY - startY);

			Color[] colorsRaw = new Color[width * numRowsToRead];
			byte[] buffer = new byte[colorsRaw.Length * bytesPerPixel];
			fs.Seek(18 + this.imageIDBytesLength + (startY * width) * bytesPerPixel, SeekOrigin.Begin);
			fs.Read(buffer, 0, buffer.Length);
			for (int i = 0, j = 0; i < buffer.Length; i += bytesPerPixel, j++)
			{
				colorsRaw[j] = GetColor(buffer, i);
			}

			// Calculate weights
			int weightTop = (int)((Math.Ceiling(fStartY) - fStartY) / (1f / weightAccuracy));
			int weightBottom = (int)((fEndY - Math.Floor(fEndY)) / (1f / weightAccuracy));

			int loopStart = lineIndex * tex.Width;
			int loopEnd = loopStart + tex.Width;
			for (int i = loopStart, j = 0; i < loopEnd; i++, j++)
			{
				texColors[i] = GetColorForTex(colorsRaw, j, numRowsToRead, weightTop, weightBottom);
			}
		}

		private Color GetColorForTex(Color[] colorsRaw, int columnIndex, int numRowsRead, int weightTop, int weightBottom)
		{
			float fStartX = columnIndex * imgPixelsPerTexturePixelW;
			int startX = (int)fStartX;
			float fEndX = fStartX + imgPixelsPerTexturePixelW;
			int endX = (int)fEndX;
			if (fEndX - endX > 0) endX++;
			if (endX > width) endX = width;
			if (startX == endX) startX--;
			if (startX < 0)
				return Color.Transparent;

			int weightLeft = (int)((Math.Ceiling(fStartX) - fStartX) / (1f / weightAccuracy));
			int weightRight = (int)((fEndX - Math.Floor(fEndX)) / (1f / weightAccuracy));

			float aR = 0;
			float aG = 0;
			float aB = 0;
			float aA = 0;
			int numColsToRead = endX - startX;
			int h = 0;
			for (int oy = 0; oy < numRowsRead; oy++)
			{
				for (int ox = startX; ox < endX; ox++)
				{
					Color c = colorsRaw[ox + oy * width];

					// Apply Weight
					int pixelWeight = GetPixelWeight(ox, oy, startX, endX - 1, 0, numRowsRead - 1, weightTop, weightBottom, weightLeft, weightRight);
					if (pixelWeight <= 0) pixelWeight = 1;

					h += pixelWeight;
					aR += (c.R * pixelWeight);
					aG += (c.G * pixelWeight);
					aB += (c.B * pixelWeight);
					aA += (c.A * pixelWeight);
				}
			}
			return new Color((int)(aR / h), (int)(aG / h), (int)(aB / h), (int)(aA / h));
		}

		private int GetPixelWeight(int ox, int oy, int minX, int maxX, int minY, int maxY, int weightTop, int weightBottom, int weightLeft, int weightRight)
		{
			if (ox == minX)
			{
				if (oy == minY)
				{
					float f1 = weightTop / (float)weightAccuracy;
					float f2 = weightLeft / (float)weightAccuracy;
					return (int)((f1 * f2) * weightAccuracy);
				}
				else if (oy == maxY)
				{
					float f1 = weightBottom / (float)weightAccuracy;
					float f2 = weightLeft / (float)weightAccuracy;
					return (int)((f1 * f2) * weightAccuracy);
				}
				else
				{
					return weightLeft;
				}
			}
			else if (ox == maxX)
			{
				if (oy == minY)
				{
					float f1 = weightTop / (float)weightAccuracy;
					float f2 = weightRight / (float)weightAccuracy;
					return (int)((f1 * f2) * weightAccuracy);
				}
				else if (oy == maxY)
				{
					float f1 = weightBottom / (float)weightAccuracy;
					float f2 = weightRight / (float)weightAccuracy;
					return (int)((f1 * f2) * weightAccuracy);
				}
				else
				{
					return weightRight;
				}
			}
			else
			{

				if (oy == minY)
				{
					return weightTop;
				}
				else if (oy == maxY)
				{
					return weightBottom;
				}
				else
				{
					return weightAccuracy;
				}
			}
		}
		public Color GetColor(byte[] buffer, int start)
		{
			if (bytesPerPixel == 2)
			{
				ushort color = BitConverter.ToUInt16(buffer, start);

				if ((color >> 15) == 1)
					return Color.Transparent;
				else
				{
					Color c = new Color((color & first5BitSet) >> 7, (color & second5BitSet) >> 2, (color & third5BitSet) << 3, 255);
					return c;
				}
			}
			else if (bytesPerPixel == 3)
			{
				byte bB = buffer[start];
				byte bG = buffer[start + 1];
				byte bR = buffer[start + 2];
				byte bA = 255;
				Color c = new Color(bR, bG, bB, bA);
				return c;
			}
			else if (bytesPerPixel == 4)
			{
				byte bB = buffer[start];
				byte bG = buffer[start + 1];
				byte bR = buffer[start + 2];
				byte bA = buffer[start + 3];
				Color c = new Color(bR, bG, bB, bA);
				return c;
			}
			return Color.Transparent;
		}
		public void ReadImageDataDirectIntoArray(Color[] color)
		{
			byte[] lineBuffer;
			int colorIdx = 0;
			while (ReadNextLine(out lineBuffer) && colorIdx < color.Length)
				for (int i = 0; i < lineBuffer.Length && colorIdx < color.Length; i += bytesPerPixel)
					color[colorIdx++] = GetColor(lineBuffer, i);
		}
	}
}
