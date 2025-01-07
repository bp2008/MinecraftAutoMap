using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ProtoUI
{
	public class Panel : Widget
	{
		#region Publics
		public bool Closable = true;
		public bool Movable = true;
		public Rectangle ContentRectangle
		{
			get
			{
				return contentRectangle;
			}
			set
			{
				contentRectangle = value;
				borderBox = new Box(contentRectangle, theme, borderThickness);
			}
		}

		public string Title
		{
			get
			{
				if (title == null)
					return "";
				return title.Text;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
					title = null;
				else if (title == null)
					title = new Label(value, theme, GetTitleDrawArea(), HPosition.Center, VPosition.Top);
				else
					title.Text = value;
			}
		}
		#endregion
		#region Privates

		private Theme theme;
		private int borderThickness;
		private Rectangle contentRectangle;
		private Box borderBox;
		private Label title = null;
		private Button closeButton = null;

		#endregion
		public Panel(int X, int Y, int width, int height, Theme theme = null)
		{
			if (theme == null)
				theme = Theme.defaultTheme;
			this.theme = theme;
			borderThickness = 1;
			this.Movable = true;
			this.Closable = true;
			ContentRectangle = new Rectangle(X, Y, width, height);
			closeButton = new Button(ContentRectangle.Right - 24, ContentRectangle.Top + 4, "X");
		}
		private Rectangle GetTitleDrawArea()
		{
			Rectangle r = new Rectangle(contentRectangle.X, contentRectangle.Y, contentRectangle.Width, contentRectangle.Height);
			r.Inflate(-3, -3);
			return r;
		}

		public override bool Update(MouseState mouseState)
		{
			if (title != null)
				title.Update(mouseState);
			if (closeButton != null)
				closeButton.Update(mouseState);
			return false;
		}
		public override void Draw(SpriteBatch sb, Texture2D pixelWhite)
		{
			borderBox.Draw(sb, pixelWhite);
			if(title != null)
				title.Draw(sb, pixelWhite);
			if (closeButton != null)
				closeButton.Draw(sb, pixelWhite);
		}

	}
}
