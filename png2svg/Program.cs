using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using System.Threading.Tasks;

namespace png2svg
{
	internal class Program
	{
		struct countedcolors
		{
			public int counted;
			public Color color;
		} 
		static void Main(string[] args)
		{
			if(args.Length < 2)
			{
				return;
			}
			string inputpng = args[0];
			string outputsvg = args[1];
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			Bitmap bitmap = new Bitmap(Image.FromFile(inputpng));
			List<Color> colors = new List<Color>();
			for (int y = 0; y < bitmap.Height; y++)
			{
				for (int x = 0; x < bitmap.Width; x++)
				{
					if (!colors.Contains(bitmap.GetPixel(x, y)))
					{
						colors.Add(bitmap.GetPixel(x, y));
					}
				}
			}
			countedcolors[] counted = new countedcolors[colors.Count];
			int i = 0;
			foreach (Color item in colors)
			{
				counted[i] = new countedcolors
				{
					color = item,
					counted = 0
				};
				i++;
			}
			for (int y = 0; y < bitmap.Height; y++)
			{
				for (int x = 0; x < bitmap.Width; x++)
				{
					counted[colors.IndexOf(bitmap.GetPixel(x, y))].counted++;
				}
			}
			int max = 0;
			foreach (countedcolors item in counted)
			{
				if (item.counted > max)
				{
					max = item.counted;
				}
			}
			Color col = Color.White;
			foreach (countedcolors item in counted)
			{
				if (item.counted == max)
				{
					col = item.color;
				}
			}
			List<string> strings= new List<string> 
			{
				"<svg height=\"" + bitmap.Height + "\" width=\""+bitmap.Width+"\" style=\"background-color: "+ RGBConverter(col) +";\">"
			};
			for (int y = 0; y < bitmap.Height; y++)
			{
				for (int x = 0; x < bitmap.Width; x++)
				{
					if (bitmap.GetPixel(x, y) != col)
					{
						strings.Add("<rect width=\"1\" x=\"" + x + "\" y=\"" + y + "\" height=\"1\" style=\"fill:" + RGBConverter(bitmap.GetPixel(x,y)) + ";stroke-width:1\" />");
						strings.Add("<!--  -->")
					}
				}
			}
			strings.Add("</svg>");
			File.WriteAllLines(outputsvg,strings);
			Console.WriteLine("task took: " + stopwatch.ElapsedMilliseconds/1000.0);
		}
		private static string RGBConverter(Color c)
		{
			string rtn = string.Empty;
			try
			{
				rtn = "rgb(" + c.R.ToString() + "," + c.G.ToString() + "," + c.B.ToString() + ")";
			}
			catch (Exception ex)
			{
				//doing nothing
			}

			return rtn;
		}
	}
}
