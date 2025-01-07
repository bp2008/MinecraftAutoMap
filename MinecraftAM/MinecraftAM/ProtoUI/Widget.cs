using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace ProtoUI
{
	public abstract class Widget : Shape
	{
		/// <summary>
		/// Allows the widget to update its state.  Returns true if the event should be considered "consumed".
		/// </summary>
		/// <param name="mouseState"></param>
		/// <returns></returns>
		public abstract bool Update(MouseState mouseState);
	}
}
