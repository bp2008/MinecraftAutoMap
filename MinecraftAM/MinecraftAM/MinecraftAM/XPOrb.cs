using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MinecraftAM
{
	public static class XPOrb
	{
		private static Rectangle reusableRectangle = new Rectangle();
		public static void Draw(SpriteBatch spriteBatch, DataSource.Entity player)
		{
			Globals.getPlayerWorldLocation(ref reusableRectangle, player);
			spriteBatch.Draw(Globals.experience_orb, reusableRectangle, GetSrcRect(Globals.gameTime), ColorHelper.GetPulsingColor(), (float)player.rotation, Globals.TileOrigin, SpriteEffects.None, 0.0f);
		}

		private static Rectangle reusableSrcRectangle = new Rectangle(0,0,16,16);
		private static Rectangle GetSrcRect(GameTime gameTime)
		{
			int framesWide = Globals.experience_orb.Width / 16;
			int framesHigh = Globals.experience_orb.Height / 16;
			int framesTotal = framesWide * framesHigh;
			int frameIdx = (int)(Globals.fastAnimationFrame % (ulong)framesTotal);
			int frameIdx_X = frameIdx % framesWide;
			int frameIdx_Y = frameIdx / framesWide;
			reusableSrcRectangle.X = frameIdx_X * 16;
			reusableSrcRectangle.Y = frameIdx_Y * 16;
			return reusableSrcRectangle;
		}
	}
}
