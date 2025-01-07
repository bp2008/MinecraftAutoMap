using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProtoUI
{
	public class Box : Shape
	{
		/// <summary>
		/// A rectangle which describes the lines to draw.
		/// </summary>
		protected Box[] lines;
		/// <summary>
		/// A rectangle which describes the fill area.
		/// </summary>
		protected Rectangle InnerRect;
		protected bool drawBorder;
		protected bool drawFill;
		private Rectangle boxArea;
		private Theme theme;
		public Box(Rectangle rect, Theme theme = null, int thickness = 1)
		{
			if (theme == null)
				theme = Theme.defaultTheme;
			this.theme = theme;
			boxArea = rect;
			drawBorder = theme.borderColor != Color.Transparent;
			if (drawBorder)
			{
				lines = new Box[4];
				Theme themeLines = theme.Copy();
				themeLines.borderColor = Color.Transparent;
				themeLines.fillColor = theme.borderColor;
				// [0] is Top, [1] is Left, [2] is Right, [3] is Bottom
				lines[0] = new Box(new Rectangle(rect.X, rect.Y, rect.Width, thickness), themeLines);
				lines[1] = new Box(new Rectangle(rect.X, rect.Y + thickness, thickness, rect.Height - thickness - thickness), themeLines);
				lines[2] = new Box(new Rectangle(rect.X + rect.Width - thickness, rect.Y + thickness, thickness, rect.Height - thickness - thickness), themeLines);
				lines[3] = new Box(new Rectangle(rect.X, rect.Y + rect.Height - thickness, rect.Width, thickness), themeLines);
			}
			drawFill = theme.fillColor != Color.Transparent;
			if (drawFill)
			{
				this.InnerRect = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
				if (drawBorder)
					this.InnerRect.Inflate(-thickness, -thickness); // If the border is not 0 (in other words, if the border is visible), we want the filled section to be one pixel smaller around the edges.
			}
		}
		public override void Draw(SpriteBatch sb, Texture2D pixelWhite)
		{
			// Draw the fill
			if (drawFill)
				sb.Draw(pixelWhite, InnerRect, theme.fillColor);
			if (drawBorder)
				for (int i = 0; i < lines.Length; i++)
					lines[i].Draw(sb, pixelWhite);
		}

		public bool PointIsInBox(int x, int y)
		{
			return boxArea.Contains(x, y);
		}
	}
}
