using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MinecraftAM
{
	public static class ColorHelper
	{
		/// <summary>
		/// Given H,S,L in range of 0-1, returns a Color
		/// </summary>
		/// <param name="h"></param>
		/// <param name="s"></param>
		/// <param name="l"></param>
		/// <returns></returns>
		public static Color HSL2RGB(double h, double s, double l)
		{
			return (Color)new HSLColor(h, s, l);
		}

		internal static Color GetPulsingColor()
		{
			return HSL2RGB(Globals.gameTime.TotalGameTime.TotalSeconds - (int)Globals.gameTime.TotalGameTime.TotalSeconds, 1, 0.5);
		}
	}
	public class HSLColor
	{
		// Private data members below are on scale 0-1
		private double hue = 1.0;
		private double saturation = 1.0;
		private double luminosity = 1.0;

		public double Hue
		{
			get { return hue; }
			set { hue = CheckRange(value); }
		}
		public double Saturation
		{
			get { return saturation; }
			set { saturation = CheckRange(value); }
		}
		public double Luminosity
		{
			get { return luminosity; }
			set { luminosity = CheckRange(value); }
		}

		private double CheckRange(double value)
		{
			if (value < 0.0)
				value = 0.0;
			else if (value > 1.0)
				value = 1.0;
			return value;
		}

		public override string ToString()
		{
			return String.Format("H: {0:#0.##} S: {1:#0.##} L: {2:#0.##}", Hue, Saturation, Luminosity);
		}

		public string ToRGBString()
		{
			Color color = (Color)this;
			return String.Format("R: {0:#0.##} G: {1:#0.##} B: {2:#0.##}", color.R, color.G, color.B);
		}

		#region Casts to/from System.Drawing.Color
		public static implicit operator Color(HSLColor hslColor)
		{
			double r = 0, g = 0, b = 0;
			if (hslColor.luminosity != 0)
			{
				if (hslColor.saturation == 0)
					r = g = b = hslColor.luminosity;
				else
				{
					double temp2 = GetTemp2(hslColor);
					double temp1 = 2.0 * hslColor.luminosity - temp2;

					r = GetColorComponent(temp1, temp2, hslColor.hue + 1.0 / 3.0);
					g = GetColorComponent(temp1, temp2, hslColor.hue);
					b = GetColorComponent(temp1, temp2, hslColor.hue - 1.0 / 3.0);
				}
			}
			return new Color((int)(255 * r), (int)(255 * g), (int)(255 * b));
		}

		private static double GetColorComponent(double temp1, double temp2, double temp3)
		{
			temp3 = MoveIntoRange(temp3);
			if (temp3 < 1.0 / 6.0)
				return temp1 + (temp2 - temp1) * 6.0 * temp3;
			else if (temp3 < 0.5)
				return temp2;
			else if (temp3 < 2.0 / 3.0)
				return temp1 + ((temp2 - temp1) * ((2.0 / 3.0) - temp3) * 6.0);
			else
				return temp1;
		}
		private static double MoveIntoRange(double temp3)
		{
			if (temp3 < 0.0)
				temp3 += 1.0;
			else if (temp3 > 1.0)
				temp3 -= 1.0;
			return temp3;
		}
		private static double GetTemp2(HSLColor hslColor)
		{
			double temp2;
			if (hslColor.luminosity < 0.5)  //<=??
				temp2 = hslColor.luminosity * (1.0 + hslColor.saturation);
			else
				temp2 = hslColor.luminosity + hslColor.saturation - (hslColor.luminosity * hslColor.saturation);
			return temp2;
		}
		#endregion

		public HSLColor(double hue, double saturation, double luminosity)
		{
			this.Hue = hue;
			this.Saturation = saturation;
			this.Luminosity = luminosity;
		}
	}
}
