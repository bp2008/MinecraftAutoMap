using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;
using System.IO;

namespace MinecraftAM
{
	public static class TextureBuilder
	{
		public static List<Texture2D> gameTextures = new List<Texture2D>();

		private const int textureSize = 2048;
		public static string BuildTextures(GraphicsDevice gd, params DirectoryInfo[] dis)
		{
			bool[][] usedPixels = new bool[textureSize][];
			for (int i = 0; i < usedPixels.Length; i++)
				usedPixels[i] = new bool[textureSize];
			Bitmap bmp = new Bitmap(textureSize, textureSize);

			int x = 0;
			int y = 0;
			StringBuilder mergeTextureName = new StringBuilder();
			foreach (DirectoryInfo di in dis)
			{
				string texturePrefix = di.Name.ToLower() == "blocks" ? "tile." : "item.";
				mergeTextureName.Append(di.Name);
				foreach (FileInfo fi in di.GetFiles())
				{
					string texName = texturePrefix + fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length);
					if (fi.Extension == ".txt")
					{
						string[] lines = File.ReadAllLines(fi.FullName);
						ItemDrawing.SetAnimationSteps(texName, lines);
					}
					if (fi.Extension != ".png")
						continue;
					//Console.WriteLine(fi.FullName);
					Bitmap b = (Bitmap)Bitmap.FromFile(fi.FullName);
					if (b.Width > textureSize || b.Height > textureSize)
						throw new Exception("Texture(s) too large.");
					while (!ValidLocation(x, y, b.Width, b.Height, usedPixels))
					{
						x++;
						if (x > textureSize - b.Width)
						{
							x = 0;
							y++;
						}
						if (y > textureSize - b.Height)
							break;
					}
					if (!ValidLocation(x, y, b.Width, b.Height, usedPixels))
					{
						x = y = 0;
						while (!ValidLocation(x, y, b.Width, b.Height, usedPixels))
						{
							x++;
							if (x > textureSize - b.Width)
							{
								x = 0;
								y++;
							}
							if (y > textureSize - b.Height)
								break;
						}
					}
					if (ValidLocation(x, y, b.Width, b.Height, usedPixels))
					{
						ConsumeLocation(x, y, b.Width, b.Height, usedPixels);
						CopyBitmapTo(b, x, y, bmp);

						ItemDrawing.TextureAddedToTextureMap(texName, new Microsoft.Xna.Framework.Rectangle(x, y, b.Width, b.Height), 0);

						x += b.Width;
					}
					b.Dispose();
					b = null;
				}
			}
			string mergedTexName = mergeTextureName.ToString() + ".png";
			bmp.Save(mergedTexName);
			return mergedTexName;
		}

		private static void CopyBitmapTo(Bitmap b, int x, int y, Bitmap bmp)
		{
			int xEnd = x + b.Width;
			int yEnd = y + b.Height;
			int bx = 0;
			int by = 0;
			for (int j = y; j < yEnd; j++, by++)
			{
				for (int i = x; i < xEnd; i++, bx++)
					bmp.SetPixel(i, j, b.GetPixel(bx, by));
				bx = 0;
			}
		}

		private static void ConsumeLocation(int x, int y, int w, int h, bool[][] usedPixels)
		{
			int xEnd = x + w;
			int yEnd = y + h;
			for (int j = y; j < yEnd; j++)
				for (int i = x; i < xEnd; i++)
					usedPixels[i][j] = true;
		}

		private static bool ValidLocation(int x, int y, int w, int h, bool[][] usedPixels)
		{
			if (x + w > textureSize || y + h > textureSize)
				return false;

			int xEnd = x + w;
			int yEnd = y + h;
			for (int j = y; j < yEnd; j++)
				for (int i = x; i < xEnd; i++)
				{
					if (i >= usedPixels.Length || j >= usedPixels[i].Length)
						return false;
					if (usedPixels[i][j])
						return false;
				}
			return true;
		}
	}
}
