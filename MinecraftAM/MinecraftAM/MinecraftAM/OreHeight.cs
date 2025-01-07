using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace MinecraftAM
{
	public class OreHeight
	{
		private Texture2D[] Textures = new Texture2D[9];
		public Rectangle SourceRect = new Rectangle(0, 0, 9, 9);
		public OreHeight()
		{
		}
		public void LoadContent(ContentManager Content)
		{
			for (int i = 0; i < 9; i++)
				Textures[i] = Content.Load<Texture2D>("Graphics/OreHeight/OreHeight" + i);
		}
		public Texture2D GetTexture(int playerPosition, int orePosition)
		{
			int diff = (orePosition - playerPosition) + 4;
			if (diff < 0) diff = 0;
			if (diff > 8) diff = 8;
			return Textures[diff];
		}
	}
}
