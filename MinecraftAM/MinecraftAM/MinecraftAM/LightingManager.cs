using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MinecraftAM
{
	public static class LightingManager
	{
		private static Color[] LightArray = new Color[16];
		private static Color[] GrassArray = new Color[16];
		private static Color[] RedstoneWireArray = new Color[16];
		private static Color[] WoolColorArray = new Color[16];
		static LightingManager()
		{
			for (int i = 0; i < LightArray.Length; i++)
			{
				float amount = i / 15f;
				LightArray[i] = new Color(amount, amount, amount);
				GrassArray[i] = new Color(amount / 3, amount, amount / 3);
				RedstoneWireArray[i] = new Color(amount, amount / 3, amount / 3);
			}
			WoolColorArray[0] = Color.White;
			WoolColorArray[1] = Color.Orange;
			WoolColorArray[2] = Color.Magenta;
			WoolColorArray[3] = Color.LightBlue;
			WoolColorArray[4] = Color.Yellow;
			WoolColorArray[5] = Color.LightGreen;
			WoolColorArray[6] = Color.Pink;
			WoolColorArray[7] = Color.Gray;
			WoolColorArray[8] = Color.LightGray;
			WoolColorArray[9] = Color.Cyan;
			WoolColorArray[10] = Color.Purple;
			WoolColorArray[11] = Color.Blue;
			WoolColorArray[12] = Color.Brown;
			WoolColorArray[13] = Color.DarkGreen;
			WoolColorArray[14] = Color.Red;
			WoolColorArray[15] = new Color(25, 25, 25);
		}
		private static Color DarkenToMatch(Color orig, byte brightness, float baseMultiplier = 0f)
		{
			if (baseMultiplier > 1f) baseMultiplier = 1f;
			if (baseMultiplier < 0f) baseMultiplier = 0f;
			if (brightness > 15) brightness = 15;
			float multiplier = ((brightness / 15f) * (1 - baseMultiplier)) + baseMultiplier;
			float r = orig.R / 255f;
			float g = orig.G / 255f;
			float b = orig.B / 255f;
			return new Color(r * multiplier, g * multiplier, b * multiplier, orig.A / 255f);
		}
		public static Color GetBlockColor(MCTile tile)
		{
			if (AMSettings.bShowIngameLighting)
			{
				// Handle special cases.
				if (tile.block.blockLight <= AMSettings.iWarningLightLevel)
					return Color.Red;
				if (tile.blockType == BlockType.Grass || tile.blockType == BlockType.Leaves || tile.blockType == BlockType.Tall_Grass)
					return GrassArray[tile.block.blockLight];
				else if (tile.blockType == BlockType.Redstone_Wire_D)
				{
					//check metadata here to see if redstone wire is on
					return DarkenToMatch(RedstoneWireArray[tile.block.blockLight], tile.block.blockMeta, 0.5f);
				}
				else if (tile.blockType == BlockType.Gray_Cloth_White_Cloth)
				{
					// check cloth color
					return DarkenToMatch(WoolColorArray[tile.block.blockMeta], tile.block.blockLight);
				}
				//return new Color(tile.block.blockLightDisplay, tile.block.blockLightDisplay, tile.block.blockLightDisplay);
				return LightArray[tile.block.blockLight];
			}
			// No ingame lighting.  Handle special cases.
			if (tile.blockType == BlockType.Grass || tile.blockType == BlockType.Leaves || tile.blockType == BlockType.Tall_Grass || tile.blockType == BlockType.Lily_Pad || tile.blockType == BlockType.Vines)
				return new Color(tile.c.R / 3, tile.c.G, tile.c.B / 3);
			else if (tile.blockType == BlockType.Redstone_Wire_D)
				return DarkenToMatch(new Color(tile.c.R, tile.c.G / 3, tile.c.B / 3), tile.block.blockMeta, 0.5f);
			else if (tile.blockType == BlockType.Gray_Cloth_White_Cloth)
				return Color.Lerp(WoolColorArray[tile.block.blockMeta], tile.c, 0.2f);
			if (tile.block.blockLight <= AMSettings.iWarningLightLevel)
				return Color.Lerp(Color.Red, tile.c, 0.5f);
			return tile.c;
		}
	}
}
