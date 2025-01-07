using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Threading;
using Microsoft.Xna.Framework.Graphics;

namespace MinecraftAM
{
	public class FixedStaticMap
	{
		#region Public Statics
		protected static FixedStaticMap inst = new FixedStaticMap();

		public static void Start(int left, int top, ushort cacheWidth, ushort cacheHeight)
		{
			inst.left = left;
			inst.top = top;
			inst.right = left + cacheWidth;
			inst.bottom = top + cacheHeight;
			SetScale(cacheWidth, cacheHeight);
			// Wipe out the existing cached map.
			for (int i = 0; i < inst.arr.Length; i++)
				inst.arr[i] = Color.Transparent;
			inst.enabled = true;
		}

		public static void Stop()
		{
			inst.enabled = false;
		}

		public static void Load(string path)
		{
			lock (inst.arr.SyncRoot)
			{
				TgaReader tgar = new TgaReader(path);
				tgar.Start();
				tgar.ReadMetaData();
				tgar.ReadImageDataDirectIntoArray(inst.arr);
				tgar.Stop();
			}
		}
		public static void Update(GraphicsDevice gd, ref Texture2D worldMap)
		{
			if (!inst.enabled)
				return;
			// Update Texture
			Texture2D tex = new Texture2D(gd, 2048, 2048);
			lock (inst.arr.SyncRoot)
			{
				tex.SetData<Color>(inst.arr);
			}
			worldMap = tex;
			Globals.worldMapOriginX = inst.left * -1;
			Globals.worldMapOriginY = inst.top * -1;
			Globals.worldMapWidth = inst.right - inst.left;
			Globals.worldMapHeight = inst.bottom - inst.top;

			// Save texture to disk
			lock (inst.arr.SyncRoot)
			{
				TgaWriter tgaWriter = new TgaWriter(Globals.getStaticMapName(), "Minecraft AutoMap Static Map v3", inst.left, inst.top, 2048, 2048, 32, inst.right - inst.left, inst.bottom - inst.top);
				tgaWriter.WriteImage(inst.arr);
			}
		}
		public static void WriteChunk(Color[] colorData, int originX, int originY, int chunkWidth, int chunkHeight)
		{
			if (!inst.enabled)
				return;
			if (chunkWidth != 16 || chunkHeight != 16)
				throw new ApplicationException("The FixedStaticMap only supports chunks of size 16x16.  Received chunk size: " + chunkWidth + "x" + chunkHeight);
			lock (inst.arr.SyncRoot)
			{
				inst.WriteChunk(colorData, originX, originY);
			}
		}
		#endregion

		#region Protected Internals
		protected Color[] arr;
		protected volatile bool enabled = false;
		// top is the block Y coordinate of the topmost block in the cache.  ditto for left, right, bottom.
		protected int top, left, bottom, right; // bounds tracking
		protected int scale = 1;
		private static void SetScale(int width, int height)
		{
			if (width > 16384 || height > 16384)
				inst.scale = 16;
			else if (width > 8192 || height > 8192)
				inst.scale = 8;
			else if (width > 4096 || height > 4096)
				inst.scale = 4;
			else if (width > 2048 || height > 2048)
				inst.scale = 2;
			else
				inst.scale = 1;
		}
		protected FixedStaticMap()
		{
			arr = new Color[2048 * 2048];
			for (int i = 0; i < arr.Length; i++)
				arr[i] = Color.Transparent;
		}
		protected void WriteChunk(Color[] colorData, int originX, int originY)
		{
			int x = originX - left;
			int y = originY - top;
			x /= scale;
			y /= scale;
			int xOrigin = x;
			if ((x + 16) >= 2048 || x < 0 || y + 16 >= 2048 || y < 0)
			{
				if (scale == 16)
					return;
				DecreaseResolution();
				WriteChunk(colorData, originX, originY);
				return;
			}
			// Transfer the color data to the cache.
			Color[] colorBuffer = new Color[scale * scale];
			for (int iy = 0; iy < 16; iy += scale)
			{
				for (int ix = 0; ix < 16; ix += scale)
				{
					if (scale == 1)
					{
						inst.arr[x + y * 2048] = colorData[ix + iy * 16];
					}
					else if (scale == 16)
					{
						inst.arr[x + y * 2048] = Average(colorData);
					}
					else
					{
						int cbi = 0;
						for (int ay = 0; ay < scale; ay++)
						{
							for (int ax = 0; ax < scale; ax++)
							{
								colorBuffer[cbi++] = colorData[ix + ax + (iy + ay) * 16];
							}
						}
						inst.arr[x + y * 2048] = Average(colorBuffer);
					}
					x++;
				}
				y++;
				x = xOrigin;
			}
		}
		protected void DecreaseResolution()
		{
			Color[] colors = new Color[2048 * 2048];
			int x = 512;
			int y = 512;
			int decreaseAmount = 2;
			int newLineWidth = 2048 / decreaseAmount;
			int maxX = x + newLineWidth;
			Color[] colorBuffer = new Color[decreaseAmount * decreaseAmount];
			for (int iy = 0; iy < 2048; iy += decreaseAmount)
			{
				for (int ix = 0; ix < 2048; ix += decreaseAmount)
				{
					int cbi = 0;
					for (int ay = 0; ay < decreaseAmount; ay++)
					{
						for (int ax = 0; ax < decreaseAmount; ax++)
						{
							colorBuffer[cbi++] = inst.arr[ix + ax + (iy + ay) * 2048];
						}
					}
					colors[x + y * 2048] = Average(colorBuffer);
					x++;
					if (x >= maxX)
						x = 512;
				}
				y++;
			}
			inst.arr = colors;
			// Reset instance vars.
			int height = bottom - top;
			int width = right - left;
			int dx = width / 2;
			int dy = height / 2;
			top -= dy;
			bottom += dy;
			left -= dx;
			right += dx;
			scale *= 2;
		}

		protected Color Average(Color[] colorBuffer)
		{
			int R = 0, G = 0, B = 0, A = 0;
			for (int i = 0; i < colorBuffer.Length; i++)
			{
				R += colorBuffer[i].R;
				G += colorBuffer[i].G;
				B += colorBuffer[i].B;
				A += colorBuffer[i].A;
			}
			R /= colorBuffer.Length;
			G /= colorBuffer.Length;
			B /= colorBuffer.Length;
			A /= colorBuffer.Length;
			return new Color(R, G, B, A);
		}
		//private class Standardizer
		//{
		//    public Standardizer()
		//    {
		//    }

		//    /// <summary>
		//    /// Converts from Fixed Static Map internal format to a standard color data array.
		//    /// </summary>
		//    /// <param name="inst">The FixedStaticMap instance to use.</param>
		//    /// <returns></returns>
		//    public Color[] Standardize(FixedStaticMap inst)
		//    {
		//        Color[] colors = new Color[inst.arr.Length];
		//        int i = 0;
		//        colors[i++] = inst.arr[inst.xStart + inst.yStart * 2048];
		//        int y = inst.yStart + 1;
		//        while(y != inst.yStart)
		//        {
		//            int preCalcY = y * 2048;
		//            int x = inst.xStart + 1;
		//            while (x != inst.xStart)
		//            {
		//                colors[i++] = inst.arr[x + preCalcY];
		//                if (++x >= 2048)
		//                    x = 0;
		//            }
		//            if(++y >= 2048)
		//                y = 0;
		//        }

		//        return colors;
		//    }
		//    public void LeftAlign(FixedStaticMap inst)
		//    {
		//        Color[] colors = new Color[inst.arr.Length];
		//        int x = inst.xStart;
		//        for (int i = 0; i < 2048; i++)
		//        {
		//            CopyColumn(inst.arr, x, colors, i);
		//            if (++x >= 2048)
		//                x = 0;
		//        }
		//        inst.arr = colors;
		//    }
		//    public void RightAlign(FixedStaticMap inst)
		//    {
		//        Color[] colors = new Color[inst.arr.Length];
		//        int x = inst.xStart;
		//        for (int i = 0; i < 2048; i++)
		//        {
		//            CopyColumn(inst.arr, x, colors, i);
		//            if (++x >= 2048)
		//                x = 0;
		//        }
		//        inst.arr = colors;
		//    }
		//    /// <summary>
		//    /// Copies the column from the first array at X index [idxFrom] and places it in the second array at X index [idxTo]
		//    /// </summary>
		//    /// <param name="from">Array to copy from.</param>
		//    /// <param name="idxFrom">X index of the column to copy from.</param>
		//    /// <param name="to">Array to copy to.</param>
		//    /// <param name="idxTo">X index of the colum to copy to.</param>
		//    public void CopyColumn(Color[] from, int idxFrom, Color[] to, int idxTo)
		//    {
		//        for (int i = 0; i < from.Length; i += 2048)
		//            to[idxTo + i] = from[idxFrom + i];
		//    }
		//    /// <summary>
		//    /// Copies the row from the first array at Y index [idxFrom] and places it in the second array at Y index [idxTo]
		//    /// </summary>
		//    /// <param name="from">Array to copy from.</param>
		//    /// <param name="idxFrom">Y index of the row to copy from.</param>
		//    /// <param name="to">Array to copy to.</param>
		//    /// <param name="idxTo">Y index of the row to copy to.</param>
		//    public void CopyRow(Color[] from, int idxFrom, Color[] to, int idxTo)
		//    {
		//        idxFrom *= 2048;
		//        idxTo *= 2048;
		//        for (int i = 0; i < 2048; i++)
		//            to[idxTo + i] = from[idxFrom + i];
		//    }
		//}
		#endregion
	}
}
