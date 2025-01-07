using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MinecraftAM
{
	internal class MapLabel
	{
		public enum Style { Normal, Red, Blue, Green, Orange, OrangeRed, Yellow, Purple, Pulsing };
		private Vector2 posVector = new Vector2();
		private Vector2 originVect1 = new Vector2();
		private Vector2 originVect2 = new Vector2();
		private Vector2 originVect3 = new Vector2();
		private Vector2 originVect4 = new Vector2();
		private Vector2 originVect = new Vector2();
		internal void Draw(SpriteBatch spriteBatch, string text, int x, int y, float scale, float rotation, SpriteFont toolTipFont, Style labelStyle, int pixelOffsetX = 0, int pixelOffsetY = 0, Color color = new Color())
		{
			posVector.X = x;
			posVector.Y = y;
			Vector2 textSize = toolTipFont.MeasureString(text);
			originVect.X = (float)((textSize.X / 2) + pixelOffsetX);
			originVect.Y = (float)((textSize.Y * 1.5) + pixelOffsetY);
			originVect1.X = originVect.X + 1;
			originVect1.Y = originVect.Y - 1;
			originVect2.X = originVect.X + 1;
			originVect2.Y = originVect.Y + 1;
			originVect3.X = originVect.X - 1;
			originVect3.Y = originVect.Y + 1;
			originVect4.X = originVect.X - 1;
			originVect4.Y = originVect.Y - 1;

			Color outerColor = Color.White;
			Color innerColor = Color.Black;
			if (color.Equals(Color.Orange) && labelStyle == Style.OrangeRed)
				color = new Color();
			if (!color.Equals(new Color()))
			{
				outerColor = Color.Transparent;
				innerColor = color;
			}
			else if (labelStyle == Style.Red)
			{
				outerColor = Color.Black;
				innerColor = Color.Red;
			}
			else if (labelStyle == Style.Blue)
			{
				outerColor = Color.DeepSkyBlue;
				innerColor = Color.Blue;
			}
			else if (labelStyle == Style.Green)
			{
				outerColor = Color.Green;
				innerColor = Color.LightGreen;
			}
			else if (labelStyle == Style.OrangeRed)
			{
				outerColor = Color.Black;
				innerColor = Color.OrangeRed;
			}
			else if (labelStyle == Style.Orange)
			{
				outerColor = Color.Transparent;
				innerColor = Color.Orange;
			}
			else if (labelStyle == Style.Yellow)
			{
				outerColor = Color.Black;
				innerColor = Color.Yellow;
			}
			else if (labelStyle == Style.Purple)
			{
				outerColor = Color.Black;
				innerColor = Color.Purple;
			}
			else if (labelStyle == Style.Pulsing)
			{
				outerColor = Color.Black;
				innerColor = ColorHelper.GetPulsingColor();
			}
			if (outerColor != Color.Transparent)
			{
				spriteBatch.DrawString(toolTipFont, text, posVector, outerColor, rotation, originVect1, scale, SpriteEffects.None, 0);
				spriteBatch.DrawString(toolTipFont, text, posVector, outerColor, rotation, originVect2, scale, SpriteEffects.None, 0);
				spriteBatch.DrawString(toolTipFont, text, posVector, outerColor, rotation, originVect3, scale, SpriteEffects.None, 0);
				spriteBatch.DrawString(toolTipFont, text, posVector, outerColor, rotation, originVect4, scale, SpriteEffects.None, 0);
			}
			spriteBatch.DrawString(toolTipFont, text, posVector, innerColor, rotation, originVect, scale, SpriteEffects.None, 0);
		}
	}
}
