using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProtoUI
{
	public class Line : Shape
	{
		private Rectangle Rect;
		private Color color;
		private bool drawMe;

		public Line(int x1, int y1, int x2, int y2, int thickness, Color color)
		{
			drawMe = color != Color.Transparent;

			if (drawMe)
			{
				this.color = color;
				if (x1 == x2)
				{
					Rect = new Rectangle(x1, y1, thickness, y2 - y1);
				}
				else if (y1 == y2)
				{
					Rect = new Rectangle(x1, y1, x2 - x1, thickness);
				}
				else
					Rect = new Rectangle(x1, y1, x2 - x1, y2 - y1); // Not a vertical or horizontal line. Just draw a big rectangle!!!
			}
		}
		public override void Draw(SpriteBatch spriteBatch, Texture2D pixelWhite)
		{
			if (drawMe)
				spriteBatch.Draw(pixelWhite, Rect, color);
		}
	}
}
