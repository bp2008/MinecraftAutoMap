using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace MinecraftAM
{
	public class Block
	{
		public const byte b15 = (byte)15;
		public const byte b0 = (byte)0;
		public short blockID;
		public BlockType blockType
		{
			get { return (BlockType)blockID; }
			set { blockID = (short)value; }
		}
		public bool trail;
		public byte blockMeta;
		public byte blockLight;
		//public static int[] texNum;
		//public static bool[] transTextures;
		//public static bool[] useItemPic;
		public static bool[] ores;
		public static Color[] pureColor;

		public void Recycle()
		{
			//blockType = BlockType.Air;
			//trail = false;
			//blockMeta = blockLight = 0;
		}

		static Block()
		{
			//texNum = new int[65535];
			//texNum[1] = 1; // Stone
			//texNum[2] = 0; // Grass
			//texNum[3] = 2; // Dirt
			//texNum[4] = 16; // Cobblestone
			//texNum[5] = 4; // Wood
			//texNum[6] = 15; // Sapling_D
			//texNum[7] = 17; // Bedrock_Adminium
			//texNum[8] = 223; // Water_D
			//texNum[9] = 223; // Stationary_water_D
			//texNum[10] = 255; // Lava_D
			//texNum[11] = 255; // Stationary_lava_D
			//texNum[12] = 18; // Sand
			//texNum[13] = 19; // Gravel
			//texNum[14] = 32; // Gold_ore
			//texNum[15] = 33; // Iron_ore
			//texNum[16] = 34; // Coal_ore
			//texNum[17] = 21; // Log
			//texNum[18] = 52; // Leaves
			//texNum[19] = 48; // Sponge
			//texNum[20] = 49; // Glass
			//texNum[21] = 160; // Lapis Lazuli Ore
			//texNum[22] = 144; // Lapis Lazuli Block
			//texNum[23] = 46; // Dispenser
			//texNum[24] = 176; // Sandstone
			//texNum[25] = 74; // Note Block
			//texNum[26] = 135; // Bed
			//texNum[27] = 163; // Powered_Rail
			//texNum[28] = 179; // Detector_Rail
			//texNum[29] = 106; // Piston Sticky Base
			//texNum[30] = 11; // Cobweb
			//texNum[31] = 39; // Tall_Grass
			//texNum[32] = 55; // Dead_Shrubs
			//texNum[33] = 107; // Piston Base
			//texNum[34] = 96; // Piston Extension
			//texNum[35] = 64; // Gray_Cloth_White_Cloth
			//texNum[36] = 14; // Block moved by Piston
			//texNum[37] = 13; // Yellow_flower
			//texNum[38] = 12; // Red_rose
			//texNum[39] = 29; // Brown_Mushroom
			//texNum[40] = 28; // Red_Mushroom
			//texNum[41] = 23; // Gold_Block
			//texNum[42] = 22; // Iron_Block
			//texNum[43] = 5; // Double_Step
			//texNum[44] = 6; // Step
			//texNum[45] = 7; // Brick
			//texNum[46] = 8; // TNT
			//texNum[47] = 35; // Bookcase
			//texNum[48] = 36; // Mossy_Cobblestone
			//texNum[49] = 37; // Obsidian
			//texNum[50] = 80; // Torch_D
			//texNum[51] = 31; // Fire
			//texNum[52] = 65; // Mob_Spawner
			//texNum[53] = 4; // Wooden_Stairs_D
			//texNum[54] = 27; // Chest							-- CHEST USES a DIFFERENT file now
			//texNum[55] = 164; // Redstone_Wire_D
			//texNum[56] = 50; // Diamond_Ore
			//texNum[57] = 24; // Diamond_Block
			//texNum[58] = 43; // Workbench
			//texNum[59] = 95; // Crops_D
			//texNum[60] = 87; // Soil_D
			//texNum[61] = 44; // Furnace
			//texNum[62] = 61; // Burning_Furnace
			//texNum[63] = 42; // Sign_Post_D
			//texNum[64] = 43; // Wooden_Door_D
			//texNum[65] = 83; // Ladder_D
			//texNum[66] = 128; // Minecart_Tracks_D
			//texNum[67] = 5; // Cobblestone_Stairs_D
			//texNum[68] = 42; // Wall_Sign_D
			//texNum[69] = 96; // Lever_D
			//texNum[70] = 6; // Stone_Pressure_Plate_D
			//texNum[71] = 44; // Iron_Door_D
			//texNum[72] = 25; // Wooden_Pressure_Plate_D
			//texNum[73] = 51; // Redstone_Ore
			//texNum[74] = 51; // Glowing_Redstone_Ore
			//texNum[75] = 115; // Redstone_torch_off_state_D
			//texNum[76] = 99; // Redstone_torch_on_state_D
			//texNum[77] = 96; // Stone_Button_D
			//texNum[78] = 66; // Snow
			//texNum[79] = 67; // Ice
			//texNum[80] = 66; // Snow_Block
			//texNum[81] = 70; // Cactus
			//texNum[82] = 72; // Clay
			//texNum[83] = 73; // Reeds
			//texNum[84] = 75; // Jukebox
			//texNum[85] = 20; // Fence
			//texNum[86] = 102; // Pumpkin
			//texNum[87] = 103; // Bloodstone
			//texNum[88] = 104; // Slow Sand
			//texNum[89] = 105; // Lightstone
			//texNum[90] = 14; // Portal
			//texNum[91] = 120; // Jack-O-Lantern
			//texNum[92] = 121; // Cake
			//texNum[93] = 131; // Redstone Repeater Off
			//texNum[94] = 147; // Redstone Repeater On
			//texNum[95] = 27; // Locked Chest			           -- CHEST USES a DIFFERENT file now
			//texNum[96] = 84; // Trapdoor
			//texNum[97] = 89; // Hidden Silverfish
			//texNum[98] = 101; // Stone Brick
			//texNum[99] = 125; // Huge Red Mushroom
			//texNum[100] = 126; // Huge Brown Mushroom
			//texNum[101] = 85; // Iron Bars
			//texNum[102] = 49; // Glass Pane
			//texNum[103] = 137; // Melon
			//texNum[104] = 127; // Pumpkin Stem
			//texNum[105] = 111; // Melon Stem
			//texNum[106] = 143; // Vines
			//texNum[107] = 43; // Fence Gate
			//texNum[108] = 5; // Brick Stairs
			//texNum[109] = 5; // Stone Brick Stairs
			//texNum[110] = 78; // Mycelium
			//texNum[111] = 76; // Lily Pad
			//texNum[112] = 224; // Nether Brick
			//texNum[113] = 224; // Nether Brick Fence
			//texNum[114] = 5; // Nether Brick Stairs
			//texNum[115] = 228; // Nether Wart
			//texNum[116] = 166; // Enchantment Table
			//texNum[117] = 157; // Brewing Stand
			//texNum[118] = 139; // Cauldron
			//texNum[119] = 167; // End Portal
			//texNum[120] = 158; // End Portal Frame
			//texNum[121] = 175; // End Stone
			//texNum[122] = 167; // Dragon Egg
			//texNum[123] = 211; // Redstone Lamp Off
			//texNum[124] = 212; // Redstone Lamp On
			//texNum[125] = 4; // Wooden Double Slab
			//texNum[126] = 4; // Wooden Slab
			//texNum[127] = 168; // Cocoa Plant
			//texNum[128] = 230; // Sandstone Stairs
			//texNum[129] = 171; // Emerald Ore
			//texNum[130] = 167; // Ender Chest
			//texNum[131] = 172; // Tripwire Hook
			//texNum[132] = 164; // Tripwire
			//texNum[133] = 25; // Block of Emerald
			//texNum[134] = 198; // Spruce Wood Stairs
			//texNum[135] = 214; // Birch Wood Stairs
			//texNum[136] = 199; // Jungle Wood Stairs

			//transTextures = new bool[65535];


			//useItemPic = new bool[256];
			//useItemPic[0] = false; // Air
			//useItemPic[1] = false; // Stone
			//useItemPic[2] = false; // Grass
			//useItemPic[3] = false; // Dirt
			//useItemPic[4] = false; // Cobblestone
			//useItemPic[5] = false; // Wood
			//useItemPic[6] = false; // Sapling_D
			//useItemPic[7] = false; // Bedrock_Adminium
			//useItemPic[8] = false; // Water_D
			//useItemPic[9] = false; // Stationary_water_D
			//useItemPic[10] = false; // Lava_D
			//useItemPic[11] = false; // Stationary_lava_D
			//useItemPic[12] = false; // Sand
			//useItemPic[13] = false; // Gravel
			//useItemPic[14] = false; // Gold_ore
			//useItemPic[15] = false; // Iron_ore
			//useItemPic[16] = false; // Coal_ore
			//useItemPic[17] = false; // Log
			//useItemPic[18] = false; // Leaves
			//useItemPic[19] = false; // Sponge
			//useItemPic[20] = false; // Glass
			//useItemPic[21] = false; // Lapis Lazuli Ore
			//useItemPic[22] = false; // Lapis Lazuli Block
			//useItemPic[23] = false; // Dispenser
			//useItemPic[24] = false; // Sandstone
			//useItemPic[25] = false; // Note Block
			//useItemPic[26] = false; // Bed
			//useItemPic[27] = false; // Powered_Rail
			//useItemPic[28] = false; // Detector_Rail
			//useItemPic[29] = false; // Piston Sticky Base
			//useItemPic[30] = false; // Cobweb
			//useItemPic[31] = false; // Tall_Grass
			//useItemPic[32] = false; // Dead_Shrubs
			//useItemPic[33] = false; // Piston Base
			//useItemPic[34] = false; // Piston Extension
			//useItemPic[35] = false; // Gray_Cloth_White_Cloth
			//useItemPic[36] = false; // White_Cloth
			//useItemPic[37] = false; // Yellow_flower
			//useItemPic[38] = false; // Red_rose
			//useItemPic[39] = false; // Brown_Mushroom
			//useItemPic[40] = false; // Red_Mushroom
			//useItemPic[41] = false; // Gold_Block
			//useItemPic[42] = false; // Iron_Block
			//useItemPic[43] = false; // Double_Step
			//useItemPic[44] = false; // Step
			//useItemPic[45] = false; // Brick
			//useItemPic[46] = false; // TNT
			//useItemPic[47] = false; // Bookcase
			//useItemPic[48] = false; // Mossy_Cobblestone
			//useItemPic[49] = false; // Obsidian
			//useItemPic[50] = false; // Torch_D
			//useItemPic[51] = false; // Fire
			//useItemPic[52] = false; // Mob_Spawner
			//useItemPic[53] = false; // Wooden_Stairs_D
			//useItemPic[54] = false; // Chest
			//useItemPic[55] = false; // Redstone_Wire_D
			//useItemPic[56] = false; // Diamond_Ore
			//useItemPic[57] = false; // Diamond_Block
			//useItemPic[58] = false; // Workbench
			//useItemPic[59] = false; // Crops_D
			//useItemPic[60] = false; // Soil_D
			//useItemPic[61] = false; // Furnace
			//useItemPic[62] = false; // Burning_Furnace
			//useItemPic[63] = true; // Sign_Post_D
			//useItemPic[64] = true; // Wooden_Door_D
			//useItemPic[65] = false; // Ladder_D
			//useItemPic[66] = false; // Minecart_Tracks_D
			//useItemPic[67] = false; // Cobblestone_Stairs_D
			//useItemPic[68] = true; // Wall_Sign_D
			//useItemPic[69] = false; // Lever_D
			//useItemPic[70] = false; // Stone_Pressure_Plate_D
			//useItemPic[71] = true; // Iron_Door_D
			//useItemPic[72] = false; // Wooden_Pressure_Plate_D
			//useItemPic[73] = false; // Redstone_Ore
			//useItemPic[74] = false; // Glowing_Redstone_Ore
			//useItemPic[75] = false; // Redstone_torch_off_state_D
			//useItemPic[76] = false; // Redstone_torch_on_state_D
			//useItemPic[77] = false; // Stone_Button_D
			//useItemPic[78] = false; // Snow
			//useItemPic[79] = false; // Ice
			//useItemPic[80] = false; // Snow_Block
			//useItemPic[81] = false; // Cactus
			//useItemPic[82] = false; // Clay
			//useItemPic[83] = false; // Reed
			//useItemPic[84] = false; // Jukebox
			//useItemPic[85] = false; // Fence
			//useItemPic[86] = false; // Pumpkin
			//useItemPic[87] = false; // Bloodstone
			//useItemPic[88] = false; // Slow Sand
			//useItemPic[89] = false; // Lightstone
			//useItemPic[90] = false; // Portal
			//useItemPic[91] = false; // Jack-O-Lantern
			//useItemPic[92] = false; // Cake
			//useItemPic[93] = false; // Redstone Repeater Off
			//useItemPic[94] = false; // Redstone Repeater On
			//useItemPic[95] = false; // Locked Chest
			//useItemPic[96] = false; // Trapdoor
			//useItemPic[97] = true; // Hidden Silverfish
			//useItemPic[98] = false; // Stone Brick
			//useItemPic[99] = false; // Huge Red Mushroom
			//useItemPic[100] = false; // Huge Brown Mushroom
			//useItemPic[101] = false; // Iron Bars
			//useItemPic[102] = false; // Glass Pane
			//useItemPic[103] = false; // Melon
			//useItemPic[104] = false; // Pumpkin Stem
			//useItemPic[105] = false; // Melon Stem
			//useItemPic[106] = false; // Vines
			//useItemPic[107] = true; // Fence Gate
			//useItemPic[108] = false; // Brick Stairs
			//useItemPic[109] = false; // Stone Brick Stairs
			//useItemPic[110] = false; // Mycelium
			//useItemPic[111] = false; // Lily Pad
			//useItemPic[112] = false; // Nether Brick
			//useItemPic[113] = false; // Nether Brick Fence
			//useItemPic[114] = false; // Nether Brick Stairs
			//useItemPic[115] = false; // Nether Wart
			//useItemPic[116] = false; // Enchantment Table
			//useItemPic[117] = false; // Brewing Stand
			//useItemPic[118] = false; // Cauldron
			//useItemPic[119] = false; // End Portal
			//useItemPic[120] = false; // End Portal Frame
			//useItemPic[121] = false; // End Stone
			//useItemPic[122] = false; // Dragon Egg
			//useItemPic[123] = false; // Redstone Lamp Off
			//useItemPic[124] = false; // Redstone Lamp On
			//useItemPic[125] = false; // Wooden Double Slab
			//useItemPic[126] = false; // Wooden Slab
			//useItemPic[127] = false; // Cocoa Plant
			//useItemPic[128] = false; // Sandstone Stairs
			//useItemPic[129] = false; // Emerald Ore
			//useItemPic[130] = false; // Ender Chest
			//useItemPic[131] = false; // Tripwire Hook
			//useItemPic[132] = false; // Tripwire
			//useItemPic[133] = false; // Block of Emerald
			//useItemPic[134] = false; // Spruce Wood Stairs
			//useItemPic[135] = false; // Birch Wood Stairs
			//useItemPic[136] = false; // Jungle Wood Stairs
			//useItemPic[250] = false;

			ores = new bool[65535];
			ores[14] = true; // gold_ore
			ores[15] = true; // iron_ore
			ores[16] = true; // coal_ore
			ores[21] = true; // lapis lazuli ore
			ores[56] = true; // diamond_ore
			ores[73] = true; // redstone_ore
			ores[82] = true; // clay
			ores[89] = true; // lightstone
			ores[129] = true; // emerald ore

			LoadPureColors();
		}
		public static void LoadPureColors()
		{
			try
			{
				pureColor = new Color[256];
				XmlDocument xdoc = new XmlDocument();
				xdoc.Load("BlockColors.xml");
				for (int i = 0; i < 255; i++)
				{
					XmlNode node = xdoc.SelectSingleNode("//Block[@Id='" + i + "']");
					if (node != null)
					{
						Color c = new Color();
						XmlAttribute attrib = node.Attributes["R"];
						if (attrib == null)
							throw new Exception("BlockColors.xml R value missing from block type id " + i);
						c.R = Convert.ToByte(attrib.Value);
						attrib = node.Attributes["G"];
						if (attrib == null)
							throw new Exception("BlockColors.xml G value missing from block type id " + i);
						c.G = Convert.ToByte(node.Attributes["G"].Value);
						attrib = node.Attributes["B"];
						if (attrib == null)
							throw new Exception("BlockColors.xml B value missing from block type id " + i);
						c.B = Convert.ToByte(node.Attributes["B"].Value);
						c.A = 255;//node.Attributes["A"];
						pureColor[i] = c;
					}
					else
						pureColor[i] = AMSettings.cBackgroundColor;

				}
			}
			catch (Exception err)
			{
				System.Windows.Forms.MessageBox.Show("Error trying to load BlockColors.xml: " + err.Message);
			}
		}
		public Block(short ID, byte meta, byte light)
		{
			Recycle(ID, meta, light);
		}
		public void Recycle(short ID, byte meta, byte light)
		{
			blockID = ID;
			blockMeta = meta;
			if (blockMeta < 0) blockMeta = 0;
			if (blockMeta > 15) blockMeta = 15;
			blockLight = light;
		}
		public override string ToString()
		{
			return blockType.ToString() + "," + blockMeta + "," + blockLight;
		}
		public void Draw()
		{
		}

		public static bool IsOre(int id)
		{
			return ores[id] || (AMSettings.OresList.Count > 0 && AMSettings.OresList.Contains(id));
		}
	}
	public enum BlockType : short
	{
		Air = 0,
		Stone = 1,
		Grass = 2,
		Dirt = 3,
		Cobblestone = 4,
		Wood = 5,
		Sapling_D = 6,
		Bedrock_Adminium = 7,
		Water_D = 8,
		Stationary_water_D = 9,
		Lava_D = 10,
		Stationary_lava_D = 11,
		Sand = 12,
		Gravel = 13,
		Gold_ore = 14,
		Iron_ore = 15,
		Coal_ore = 16,
		Log = 17,
		Leaves = 18,
		Sponge = 19,
		Glass = 20,
		Lapis_Lazuli_Ore = 21,
		Lapis_Lazuli_Block = 22,
		Dispenser = 23,
		Sandstone = 24,
		Note_Block = 25,
		Bed = 26,
		Powered_Rail = 27,
		Detector_Rail = 28,
		Piston_Sticky_Base = 29,
		Cobweb = 30,
		Tall_Grass = 31,
		Dead_Shrubs = 32,
		Piston_Base = 33,
		Piston_Extension = 34,
		Gray_Cloth_White_Cloth = 35,
		White_Cloth = 36,
		Yellow_flower = 37,
		Red_rose = 38,
		Brown_Mushroom = 39,
		Red_Mushroom = 40,
		Gold_Block = 41,
		Iron_Block = 42,
		Double_Step = 43,
		Step = 44,
		Brick = 45,
		TNT = 46,
		Bookcase = 47,
		Mossy_Cobblestone = 48,
		Obsidian = 49,
		Torch_D = 50,
		Fire = 51,
		Mob_Spawner = 52,
		Wooden_Stairs_D = 53,
		Chest = 54,
		Redstone_Wire_D = 55,
		Diamond_Ore = 56,
		Diamond_Block = 57,
		Workbench = 58,
		Crops_D = 59,
		Soil_D = 60,
		Furnace = 61,
		Burning_Furnace = 62,
		Sign_Post_D = 63,
		Wooden_Door_D = 64,
		Ladder_D = 65,
		Minecart_Tracks_D = 66,
		Cobblestone_Stairs_D = 67,
		Wall_Sign_D = 68,
		Lever_D = 69,
		Stone_Pressure_Plate_D = 70,
		Iron_Door_D = 71,
		Wooden_Pressure_Plate_D = 72,
		Redstone_Ore = 73,
		Glowing_Redstone_Ore = 74,
		Redstone_torch_off_state_D = 75,
		Redstone_torch_on_state_D = 76,
		Stone_Button_D = 77,
		Snow = 78,
		Ice = 79,
		Snow_Block = 80,
		Cactus = 81,
		Clay = 82,
		Reed = 83,
		Jukebox = 84,
		Fence = 85,
		Pumpkin = 86,
		Netherrack = 87,
		Soul_Sand = 88,
		Glowstone = 89,
		Portal = 90,
		Jack_O_Lantern = 91,
		Cake = 92,
		Redstone_Repeater_Off = 93,
		Redstone_Repeater_On = 94,
		Locked_Chest = 95,
		Trapdoor = 96,
		Hidden_Silverfish = 97,
		Stone_Brick = 98,
		Huge_Red_Mushroom = 99,
		Huge_Brown_Mushroom = 100,
		Iron_Bars = 101,
		Glass_Pane = 102,
		Melon = 103,
		Pumpkin_Stem = 104,
		Melon_Stem = 105,
		Vines = 106,
		Fence_Gate = 107,
		Brick_Stairs = 108,
		Stone_Brick_Stairs = 109,
		Mycelium = 110,
		Lily_Pad = 111,
		Nether_Brick = 112,
		Nether_Brick_Fence = 113,
		Nether_Brick_Stairs = 114,
		Nether_Wart = 115,
		Enchantment_Table = 116,
		Brewing_Stand = 117,
		Cauldron = 118,
		End_Portal = 119,
		End_Portal_Frame = 120,
		End_Stone = 121,
		Dragon_Egg = 122,
		Ender_Chest = 130,
		Trapped_Chest = 146,
		Player = 250
	}
}
