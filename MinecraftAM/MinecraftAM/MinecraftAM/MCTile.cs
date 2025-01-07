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

namespace MinecraftAM
{
    public class MCTile
    {
		public int height = 0;
        /// <summary>
        /// The next tile to be drawn on top of this one.  If null, this is the top of the column.
		/// </summary>
        public MCTile nextTile = null;
        public bool solidColor = false;
		public Block block;
        private BlockType bType
		{
			get
			{
				return block.blockType;
			}
			set
			{
				block.blockType = value;
			}
		}
        public BlockType blockType
        {
            get
            {
                return block.blockType;
            }
            set
            {
                bType = value;
            }
        }
        public Color c = AMSettings.cNotDrawnBlockColor;
        private int CountList()
        {
            int cnt = 0;
            MCTile top = nextTile;
            while (top != null)
            {
                cnt++;
                top = top.nextTile;
            }
            return cnt;
        }
        public override string ToString()
        {
            return bType.ToString() + " " + c.R + ":" + c.G + ":" + c.B + " " + CountList();
        }
        public MCTile(Color color)
        {
			block = ChunkManager.airBlock;
            solidColor = true;
            c = color;
        }
		//public MCTile(BlockType blockType)
		//{
		//    bType = blockType;
		//    if (Block.useItemPic[(byte)blockType])
		//        isItem = true;
		//    SetSourceRect();
		//}
        public MCTile(Block blck, Color color)
        {
            SetColorB(color.R, color.G, color.B);
			Reset(blck);
        }
        internal void Reset(float r, float g, float b)
		{
			block = ChunkManager.airBlock;
            solidColor = true;
            nextTile = null;
            SetColor(r, g, b);
        }
        internal void Reset(Color c1)
        {
			block = ChunkManager.airBlock;
            solidColor = true;
            nextTile = null;
            c = c1;
        }

        internal void Reset(Block blck)
        {
            block = blck;
            nextTile = null;
            solidColor = false;
        }
        internal void Reset(Block blck, float r, float g, float b)
        {
            SetColor(r, g, b);
            Reset(blck);
        }

        private void SetColorB(byte r, byte g, byte b)
        {
            c.R = r;
            c.G = g;
            c.B = b;
        }
        private void SetColor(float r, float g, float b)
        {
            c.R = (byte)(255 * r);
            c.G = (byte)(255 * g);
            c.B = (byte)(255 * b);
        }
        /// <summary>
        /// Returns true if this block is fully or partially transparent. 
        /// </summary>
        /// <returns></returns>
        public bool IsTransp()
        {
			return ItemDrawing.transparentBlocks[(int)bType];
        }

    }
}
