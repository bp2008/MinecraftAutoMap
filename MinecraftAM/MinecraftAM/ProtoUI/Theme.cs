using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ProtoUI
{
	public class Theme
	{
		public static Theme defaultTheme = new Theme(new Color(200, 200, 200, 255), new Color(0, 0, 0, 127), Color.White);
		public static Theme defaultHoverTheme = new Theme(new Color(200, 200, 200, 255), new Color(127, 127, 182, 127), Color.White);
		public static Theme defaultButtonTheme = new Theme(new Color(200, 200, 200, 255), new Color(0, 0, 0, 127), new Color(200, 0, 0, 255));
		public static Theme defaultButtonHoverTheme = new Theme(new Color(200, 200, 200, 255), new Color(45, 45, 45, 127), new Color(200, 0, 0, 255));
		public static Theme defaultButtonDownTheme = new Theme(new Color(200, 200, 200, 255), new Color(63, 63, 91, 127), new Color(200, 0, 0, 255));
		public Color borderColor;
		public Color fillColor;
		public Color textColor;
		public Theme(Color borderColor, Color fillColor, Color textColor)
		{
			this.borderColor = borderColor;
			this.fillColor = fillColor;
			this.textColor = textColor;
		}
		public Theme Copy()
		{
			return new Theme(borderColor, fillColor, textColor);
		}
	}
}
