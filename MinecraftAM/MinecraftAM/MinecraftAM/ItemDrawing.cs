using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Concurrent;
using System.IO;
using MinecraftAM.MinecraftAM;

namespace MinecraftAM
{
	public static class ItemDrawing
	{
		#region Drawing
		public static void Draw(SpriteBatch spriteBatch, Texture2D tex, MCTile theTile, Rectangle targetRect)
		{
			if (theTile.block.blockID <= 0 /*|| theTile.block.blockID > 65534*/)
				return;
			TextureLocation texLoc = textureLocations[theTile.block.blockID];
			if (texLoc == null)
			{
				if (textureNameRequestedExpiration[theTile.block.blockID] < DateTime.Now)
				{
					DataSource.DS.GetTextureName(theTile.block.blockID);
					textureNameRequestedExpiration[theTile.block.blockID] = DateTime.Now.AddSeconds(10);
				}
				texLoc = questionMark;
			}
			if (texLoc == null)
				return;

			spriteBatch.Draw(tex, targetRect, texLoc.srcRect, LightingManager.GetBlockColor(theTile));
		}

		public static void Draw(SpriteBatch spriteBatch, Texture2D tex, DataSource.Entity pl, Rectangle targetRect, float rotation = 0f)
		{
			if (pl.itemID <= 0 || pl.itemID > 65534)
				return;

			TextureLocation texLoc = textureLocations[pl.itemID];
			if (texLoc == null)
			{
				if (textureNameRequestedExpiration[pl.itemID] < DateTime.Now)
				{
					DataSource.DS.GetTextureName(pl.itemID);
					textureNameRequestedExpiration[pl.itemID] = DateTime.Now.AddSeconds(10);
				}
				texLoc = questionMark;
			}
			if (texLoc == null)
				return;

			spriteBatch.Draw(tex, targetRect, texLoc.srcRect, Color.White, rotation, Globals.TileOrigin, SpriteEffects.None, 0.0f);
		}
		#endregion
		#region Block Data Arrays and Maps
		static ConcurrentDictionary<string, TextureLocation> TextureNameToTextureLocationMap = new ConcurrentDictionary<string, TextureLocation>(1, 65535);
		public static bool[] transparentBlocks = new bool[65535];
		static TextureLocation[] textureLocations = new TextureLocation[65535];
		static DateTime[] textureNameRequestedExpiration = new DateTime[65535];
		static TextureLocation questionMark = null;
		//static SortedList<string, string> textureNameBackup = new SortedList<string, string>();
		static SortedList<string, string> textureNameReplace = new SortedList<string, string>();
		static SortedList<int, string> textureIdReplace = new SortedList<int, string>();
		#endregion
		#region Static constructor
		static ItemDrawing()
		{
			//textureNameBackup.Add("tile.grass", "tile.grass_top");
			//textureNameBackup.Add("tile.mushroom", "tile.mushroom_brown");
			//textureNameBackup.Add("tile.fence", "tile.lever");
			//textureNameBackup.Add("tile.stoneslab", "tile.stoneslab_top");
			//textureNameBackup.Add("tile.stairsstone", "tile.stoneslab_side");
			//textureNameBackup.Add("tile.workbench", "tile.workbench_top");
			//textureNameBackup.Add("tile.thinglass", "tile.glass");
			//textureNameBackup.Add("tile.log", "tile.tree_top");
			//textureNameBackup.Add("tile.farmland", "tile.farmland_wet");
			//textureNameBackup.Add("tile.sign", "item.sign");
			//textureNameBackup.Add("tile.crops", "tile.crops_5");
			//textureNameBackup.Add("tile.fencegate", "tile.doorwood_upper");
			//textureNameBackup.Add("tile.netherfence", "tile.lever");
			//textureNameBackup.Add("tile.sandstone", "tile.sandstone_top");
			//textureNameBackup.Add("tile.cactus", "tile.cactus_top");
			//textureNameBackup.Add("tile.pressureplate", "tile.stoneslab_top");
			//textureNameBackup.Add("item.dyepoweder", "item.dyePowder_black");
			//textureNameBackup.Add("tile.fire", "tile.fire_0");
			//textureNameBackup.Add("tile.example", "tile.example");
			//textureNameBackup.Add("tile.example", "tile.example");
			//textureNameBackup.Add("tile.example", "tile.example");
			textureNameReplace.Add("tile.door_wood_upper", "item.door_wood");
			textureNameReplace.Add("tile.door_wood", "item.door_wood");
			textureNameReplace.Add("tile.door_wood_lower", "item.door_wood");
			textureNameReplace.Add("tile.door_iron_upper", "item.door_iron");
			textureNameReplace.Add("tile.door_iron", "item.door_iron");
			textureNameReplace.Add("tile.door_iron_lower", "item.door_iron");

			textureIdReplace.Add(47, "tile.bookshelf");
			textureIdReplace.Add(61, "tile.furnace_front");
			textureIdReplace.Add(62, "tile.furnace_front_lit");
			textureIdReplace.Add(63, "item.sign");
			textureIdReplace.Add(68, "item.sign");
			textureIdReplace.Add(85, "tile.wood_fence");
			textureIdReplace.Add(107, "tile.wood_fence_gate");
			textureIdReplace.Add(113, "tile.netherbrick_fence");

			transparentBlocks[0] = true; // Air
			transparentBlocks[6] = true; // Sapling_D
			transparentBlocks[8] = true; // Water_D
			transparentBlocks[9] = true; // Stationary_water_D
			transparentBlocks[10] = true; // Lava_D
			transparentBlocks[11] = true; // Stationary_lava_D
			transparentBlocks[18] = true; // Leaves
			transparentBlocks[20] = true; // Glass
			transparentBlocks[27] = true; // Powered_Rail
			transparentBlocks[28] = true; // Detector_Rail
			transparentBlocks[30] = true; // Cobweb
			transparentBlocks[31] = true; // Tall_Grass
			transparentBlocks[32] = true; // Dead_Shrubs
			transparentBlocks[34] = true; // Piston Extension
			transparentBlocks[37] = true; // Yellow_flower
			transparentBlocks[38] = true; // Red_rose
			transparentBlocks[39] = true; // Brown_Mushroom
			transparentBlocks[40] = true; // Red_Mushroom
			transparentBlocks[50] = true; // Torch_D
			transparentBlocks[51] = true; // Fire
			transparentBlocks[52] = true; // Mob_Spawner
			transparentBlocks[55] = true; // Redstone_Wire_D
			transparentBlocks[59] = true; // Crops_D
			transparentBlocks[63] = true; // Sign_Post_D
			transparentBlocks[64] = true; // Wooden_Door_D
			transparentBlocks[65] = true; // Ladder_D
			transparentBlocks[66] = true; // Minecart_Tracks_D
			transparentBlocks[68] = true; // Wall_Sign_D
			transparentBlocks[69] = true; // Lever_D
			transparentBlocks[71] = true; // Iron_Door_D
			transparentBlocks[75] = true; // Redstone_torch_off_state_D
			transparentBlocks[76] = true; // Redstone_torch_on_state_D
			transparentBlocks[77] = true; // Stone_Button_D
			transparentBlocks[79] = true; // Ice
			transparentBlocks[81] = true; // Cactus
			transparentBlocks[83] = true; // Reed
			transparentBlocks[85] = true; // Fence
			transparentBlocks[90] = true; // Portal
			transparentBlocks[96] = true; // Trapdoor
			transparentBlocks[97] = true; // Hidden Silverfish
			transparentBlocks[101] = true; // Iron Bars
			transparentBlocks[102] = true; // Glass Pane
			transparentBlocks[104] = true; // Pumpkin Stem
			transparentBlocks[105] = true; // Melon Stem
			transparentBlocks[106] = true; // Vines
			transparentBlocks[107] = true; // Fence Gate
			transparentBlocks[111] = true; // Lily Pad
			transparentBlocks[113] = true; // Nether Brick Fence
			transparentBlocks[115] = true; // Nether Wart
			transparentBlocks[117] = true; // Brewing Stand
			transparentBlocks[127] = true; // Cocoa Plant
			transparentBlocks[131] = true; // Tripwire Hook
			transparentBlocks[132] = true; // Tripwire
			transparentBlocks[138] = true; // Beacon
			transparentBlocks[141] = true; // Carrots
			transparentBlocks[142] = true; // Potatoes
			transparentBlocks[143] = true; // Wooden Button
			transparentBlocks[145] = true; // Anvil
			transparentBlocks[157] = true; // Activator Rail
		}
		#endregion
		#region Helpers
		public static void TextureAddedToTextureMap(string textureName, Rectangle location, byte textureFileID)
		{
			textureName = textureName.ToLower();
			TextureNameToTextureLocationMap[textureName] = new TextureLocation(location, textureFileID);
			if (textureName == "tile.qmark")
				questionMark = TextureNameToTextureLocationMap[textureName];
		}
		public static void SetAnimationSteps(string textureName, string[] lines)
		{
			TextureLocation tl;
			if (TextureNameToTextureLocationMap.TryGetValue(textureName, out tl))
				tl.setAnimationSteps(lines);
		}
		public static void ResolveTexture(int id, string textureName)
		{
			//Console.WriteLine("Block " + id.ToString().PadLeft(4, ' ') + " requests texture " + textureName);
			if (id < 0 || id > 65534)
				return;
			if (textureName.EndsWith("|0"))
				transparentBlocks[id] = true;
			textureName = textureName.Substring(0, textureName.Length - 2).ToLower();
			string newTextureName;
			TextureLocation tl;
			if (textureIdReplace.TryGetValue(id, out newTextureName))
			{
				if (TextureNameToTextureLocationMap.TryGetValue(newTextureName, out tl))
				{
					textureLocations[id] = tl;
				}
				else
				{
					try
					{
						File.AppendAllText("errordump.txt", "\r\n" + "Block " + id.ToString().PadLeft(4, ' ') + ": Default texture is \"" + textureName + "\", but this block id is remapped to \"" + newTextureName + "\" by AutoMap.  The remapped texture does not exist!  Reverting to default texture.\r\n");
					}
					catch (Exception) { }
				}
			}
			else if (textureNameReplace.TryGetValue(textureName, out newTextureName))
			{
				if (TextureNameToTextureLocationMap.TryGetValue(newTextureName, out tl))
				{
					textureLocations[id] = tl;
				}
				else
				{
					try
					{
						File.AppendAllText("errordump.txt", "\r\n" + "Block " + id.ToString().PadLeft(4, ' ') + ": Default texture is \"" + textureName + "\", but is remapped to \"" + newTextureName + "\" by AutoMap.  The remapped texture does not exist!  Reverting to default texture.\r\n");
					}
					catch (Exception) { }
				}
			}
			if (textureLocations[id] == null)
			{
				if (TextureNameToTextureLocationMap.TryGetValue(textureName, out tl))
				{
					textureLocations[id] = tl;
				}
			}
			if (textureLocations[id] == null)
			{
				try
				{
					File.AppendAllText("errordump.txt", "\r\n" + "Block " + id.ToString().PadLeft(4, ' ') + ": Default texture is \"" + textureName + "\", but this texture does not exist!\r\n");
				}
				catch (Exception) { }
			}
			//else
			//{
			//    // Unable to resolve directly.  See if it has been mapped to a backup texture.
			//    string backupName;
			//    if (textureNameBackup.TryGetValue(textureName, out backupName))
			//    {
			//        if (TextureNameToTextureLocationMap.TryGetValue(backupName, out tl))
			//            textureLocations[id] = tl;
			//    }
			//}
			//if (textureLocations[id] == null)
			//{
			//string autoTextureName = textureName + "_top";
			//// Unable to resolve a texture so far.  Try with _top appended to the name
			//if (TextureNameToTextureLocationMap.TryGetValue(autoTextureName, out tl))
			//    textureLocations[id] = tl;
			//else
			//{
			//    autoTextureName = "";
			//    // Nope.  Last chance: see if there is a texture that starts with this.
			//    foreach (string key in TextureNameToTextureLocationMap.Keys)
			//    {
			//        if (key.StartsWith(textureName))
			//        {
			//            autoTextureName = key;
			//            if (TextureNameToTextureLocationMap.TryGetValue(autoTextureName, out tl))
			//                textureLocations[id] = tl;
			//            break;
			//        }
			//    }
			//}
			//try
			//{
			//    File.AppendAllText("errordump.txt", "\r\n" + "Block " + id.ToString().PadLeft(4, ' ') + " requests texture " + textureName + "\r\n");
			//    if (!string.IsNullOrEmpty(autoTextureName))
			//        File.AppendAllText("errordump.txt", "\r\n" + "Block " + id.ToString().PadLeft(4, ' ') + " was auto-resolved to use " + autoTextureName + "\r\n");
			//    if (textureLocations[id] == null)
			//        File.AppendAllText("errordump.txt", "\r\n" + "Block " + id.ToString().PadLeft(4, ' ') + " could not obtain a texture!" + "\r\n");
			//}
			//catch (Exception)
			//{
			//}
			//}
		}
		#endregion
		class TextureLocation
		{
			private Rectangle baseRect;
			private Rectangle currentRect;
			public Rectangle srcRect
			{
				get
				{
					if (animationSteps.Count > 0)
						currentRect.Y = baseRect.Y + baseRect.Height * animationSteps[(int)(Globals.fasterAnimationFrame % (ulong)animationSteps.Count)];
					return currentRect;
				}
			}
			public byte textureFileID;
			private List<int> animationSteps = new List<int>();
			public TextureLocation(Rectangle rect, byte textureFileID)
			{
				this.textureFileID = textureFileID;
				if (rect.Width < rect.Height && rect.Width != 0 && rect.Height != 0 && rect.Height % rect.Width == 0)
				{
					// This is an animated texture! (most likley!!!)
					int animationFrameCount = rect.Height / rect.Width;
					for (int i = 0; i < animationFrameCount; i++)
						animationSteps.Add(i);
					rect.Height = rect.Width;
				}
				this.baseRect = this.currentRect = rect;
			}
			public void setAnimationSteps(string[] lines)
			{
				if (baseRect.Height != baseRect.Width)
					return; // Return because when this TextureLocation was created, it was determined that the texture was not suitable for animation.
				List<int> newAnimationSteps = new List<int>(lines.Length);
				if (lines.Length > 1)
				{
					foreach (string line in lines)
					{
						string s = line.Replace(",", "");
						int timesToAdd = 1;
						int animationFrame = 0;
						int idxStar = s.IndexOf("*");
						if (idxStar > -1)
						{
							if (!int.TryParse(s.Substring(idxStar + 1), out timesToAdd))
								return;
							if (!int.TryParse(s.Substring(0, idxStar), out animationFrame))
								return;
						}
						else
						{
							if (!int.TryParse(s, out animationFrame))
								return;
						}
						for (int i = 0; i < timesToAdd; i++)
							newAnimationSteps.Add(animationFrame);
					}
					animationSteps = newAnimationSteps;
				}
			}
		}
	}
}
