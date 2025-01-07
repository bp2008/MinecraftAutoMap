using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ProtoUI
{
	class GUI
	{
		/// <summary>
		/// A white pixel texture used to draw the lines and rectangles that make up much of the UI.
		/// </summary>
		protected Texture2D pixelWhite;
		protected List<Widget> widgets;
		protected GraphicsDevice gd;
		public bool Enabled = true;
		public GUI(GraphicsDevice gd)
		{
			this.gd = gd;
			byte[] whiteBytes = new byte[]{255, 255, 255, 255};
			pixelWhite = new Texture2D(gd, 1, 1, false, SurfaceFormat.Color);
			pixelWhite.SetData<byte>(whiteBytes);
			widgets = new List<Widget>();
		}
		/// <summary>
		/// Returns true if the event should be consumed.
		/// </summary>
		/// <param name="gameTime"></param>
		/// <returns></returns>
		public bool Update(GameTime gameTime)
		{
			if (!Enabled)
				return false;
			foreach (Widget w in widgets)
				if(w.Update(Mouse.GetState()))
					return true;
			return false;
		}
		public void Draw(SpriteBatch spriteBatch)
		{
			if (!Enabled)
				return;
			foreach (Widget w in widgets)
				w.Draw(spriteBatch, pixelWhite);
		}
		public void AddWidget(Widget w)
		{
			widgets.Add(w);
		}
	}
}
