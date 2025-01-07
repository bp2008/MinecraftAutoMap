using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ProtoUI
{
	public class Button : Widget
	{
		#region OnClick
		/// <summary>
		/// This event handler handles the OnClick event.
		/// </summary>
		/// <param name="sender">The object which raised the event.</param>
		public delegate void OnClickEventHandler(Button sender);
		/// <summary>
		/// This event is raised on the mouse-up event of the button click.
		/// </summary>
		public event OnClickEventHandler OnClick;
		/// <summary>
		/// This function raises the OnClick event.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void RaiseOnClick()
		{
			if (OnClick != null)
				OnClick(this);
		}
		#endregion
		#region OnMouseDown
		/// <summary>
		/// This event handler handles the OnMouseDown event.
		/// </summary>
		/// <param name="sender">The object which raised the event.</param>
		public delegate void OnMouseDownEventHandler(Button sender);
		/// <summary>
		/// This event is raised when the left mouse button is depressed while the pointer is on the button.
		/// </summary>
		public event OnMouseDownEventHandler OnMouseDown;
		/// <summary>
		/// This function raises the OnMouseDown event.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void RaiseOnMouseDown()
		{
			if (OnMouseDown != null)
				OnMouseDown(this);
		}
		#endregion
		private Label label;
		private Box boxNormal;
		private Box boxHover;
		private Box boxDown;
		private Box boxDraw;
		private bool isPressed = false;
		public Button(int X, int Y, string text, Theme theme = null, Theme hoverTheme = null, Theme downTheme = null)
		{
			if (theme == null)
				theme = Theme.defaultButtonTheme;
			if (hoverTheme == null)
				hoverTheme = Theme.defaultButtonHoverTheme;
			if (downTheme == null)
				downTheme = Theme.defaultButtonDownTheme;
			int padX = 4;
			int padY = 0;
			Vector2 textRequiredSpace = MinecraftAM.Globals.fontDebug.MeasureString(text);
			Rectangle textArea = new Rectangle(X + padX, Y + padY, (int)textRequiredSpace.X + padX, (int)textRequiredSpace.Y + padY);
			Theme labelTheme = new Theme(Color.Transparent, Color.Transparent, theme.textColor);
			label = new Label(text, labelTheme, textArea, HPosition.Left, VPosition.Top);
			Rectangle boxArea = new Rectangle(X, Y, (int)textRequiredSpace.X + padX + padX, (int)textRequiredSpace.Y + padY + padY);
			boxArea.Inflate(1, 1);
			boxDraw = boxNormal = new Box(boxArea, theme);
			boxHover = new Box(boxArea, hoverTheme);
			boxDown = new Box(boxArea, downTheme);
		}

		public override bool Update(MouseState mouseState)
		{
			bool consumeEvent = false;
			bool wasPressed = isPressed;
			isPressed = mouseState.LeftButton == ButtonState.Pressed;
			if (boxNormal.PointIsInBox(mouseState.X, mouseState.Y))
			{
				if (isPressed)
					boxDraw = boxDown;
				else
					boxDraw = boxHover;
			}
			else
				boxDraw = boxNormal;
			if (wasPressed && !isPressed)
			{
				consumeEvent = true;
				// Button was just released
				RaiseOnClick();
			}
			else if (isPressed && !wasPressed)
			{
				consumeEvent = true;
				// Button was just clicked
				RaiseOnMouseDown();
			}
			return consumeEvent;
		}
		public override void Draw(SpriteBatch sb, Texture2D pixelWhite)
		{
			boxDraw.Draw(sb, pixelWhite);
			label.Draw(sb, pixelWhite);
		}
	}
}
