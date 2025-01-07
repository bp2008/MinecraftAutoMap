using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ProtoUI
{
	public class Label : Widget
	{
		private Vector2 Position;
		private string text;
		private Rectangle drawArea;
		private HPosition hPos;
		private VPosition vPos;
		public Color color;
		public Rectangle DrawArea
		{
			get
			{
				return drawArea;
			}
			set
			{
				drawArea = value;
				Text = text;
			}
		}
		public HPosition hPosition
		{
			get
			{
				return hPos;
			}
			set
			{
				hPos = value;
				Text = text;
			}
		}
		public VPosition vPosition
		{
			get
			{
				return vPos;
			}
			set
			{
				vPos = value;
				Text = text;
			}
		}
		public string Text
		{
			get
			{
				return text;
			}
			set
			{
				text = value;
				if (hPos == HPosition.Left && vPos == VPosition.Top)
					Position = new Vector2(drawArea.X, drawArea.Y);
				else
				{
					Vector2 size = MinecraftAM.Globals.fontDebug.MeasureString(text);
					int xOffset, yOffset;
					// Handle horizontal position
					if (hPos == HPosition.Left)
						xOffset = 0;
					else if (hPos == HPosition.Center)
						xOffset = (drawArea.Width - (int)size.X) / 2;
					else
						xOffset = drawArea.Width - (int)size.X;
					// Handle vertical position
					if (vPos == VPosition.Top)
						yOffset = 0;
					else if (vPos == VPosition.Middle)
						yOffset = (drawArea.Height - (int)size.Y) / 2;
					else
						yOffset = drawArea.Height - (int)size.Y;
					Position = new Vector2(drawArea.X + xOffset, drawArea.Y + yOffset);
				}
			}
		}
		public Label(string text, Theme labelTheme, Rectangle drawArea, HPosition hPos, VPosition vPos)
		{
			this.color = labelTheme.textColor;
			this.drawArea = drawArea;
			this.hPos = hPos;
			this.vPos = vPos;
			this.Text = text;
		}

		public override bool Update(MouseState mouseState)
		{
			return false;
		}
		public override void Draw(SpriteBatch sb, Texture2D pixelWhite)
		{
			sb.DrawString(MinecraftAM.Globals.fontDebug, Text, Position, color);
		}
	}
}
