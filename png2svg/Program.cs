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
	// png2svg by Maciej Cichocki
	internal class Program
	{
		struct countedcolors
		{
			public int counted;
			public Color color;
		}
		static void Main(string[] args)
		{
			if (args.Length < 3)
			{
				return;
			}
			string inputpng = args[0];
			string outputsvg = args[1];
			string format = args[2];
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			Bitmap bitmap = new Bitmap(Image.FromFile(inputpng));
			int oheight = int.Parse(args[2].Split(':')[0]), owidth = int.Parse(args[2].Split(':')[1]);


			List<string> strings = new List<string>
			{
				$"<svg xmlns=\"http://www.w3.org/2000/svg\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" width=\"100%\" height=\"100%\" viewBox=\"0 0 {bitmap.Width} {bitmap.Height}\">"
			};
			int[] ints = { owidth, oheight };
			Color[,] bgcols = new Color[owidth, oheight];
			foreach (var item in printAllDivisors(ints))
			{
				int width = owidth / item;
				int height = oheight / item;
				bgcols = new Color[owidth, oheight];
				Bitmap[,] bitmaps = DevideImage(bitmap, width, height);
				for (int x = 0; x < width; x++)
				{
					for (int y = 0; y < height; y++)
					{
						Console.Write($"Analising image [{x}:{y}] Col: ");
						bgcols[x, y] = CalculateAverageSector(bitmaps[x, y]);
						strings.Add($"<rect x=\"{bitmap.Width / width * x}\" y=\"{bitmap.Height / height * y}\" width=\"{bitmap.Width / width}\" height=\"{bitmap.Height / height}\" style=\"fill:{RGBConverter(bgcols[x, y])};stroke-width:1\" />");
					}
				}
			}
			Console.WriteLine($"Filling image");
			for (int y = 0; y < bitmap.Height; y++)
			{
				for (int x = 0; x < bitmap.Width; x++)
				{
					if (bitmap.GetPixel(x, y) != bgcols[x / (bitmap.Width / owidth) , y / (bitmap.Height / oheight)])
					{
						strings.Add("<rect width=\"1\" x=\"" + x + "\" y=\"" + y + "\" height=\"1\" style=\"fill:" + RGBConverter(bitmap.GetPixel(x, y)) + ";stroke-width:1\" />");
					}
				}
			}
			strings.Add("</svg>");
			File.WriteAllLines(outputsvg, strings);
			Console.WriteLine("task took: " + stopwatch.ElapsedMilliseconds / 1000.0);
		}
		private static Bitmap[,] DevideImage(Bitmap bitmap, int width, int height)
		{
			Bitmap[,] bit = new Bitmap[width, height];
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					bit[x, y] = new Bitmap(bitmap.Width/width, bitmap.Height/height);
				}
			}
			for (int x = 0; x < bitmap.Width; x++) {
				for (int y = 0; y < bitmap.Height; y++) {

					int bitmapx = x / (bitmap.Width / width);
					int bitmapy = y / (bitmap.Height / height);
					//Console.WriteLine($"Bitmap @ [{x}, {y}]");
					Bitmap correctone = bit[bitmapx, bitmapy];
					//Console.WriteLine($"Bitmap info:  Width: {correctone.Width} Height: {correctone.Height}");
					int bitx = x % (bitmap.Width / width);
					int bity = y % (bitmap.Height / height);
					correctone.SetPixel(bitx, bity, bitmap.GetPixel(x,y));
					bit[bitmapx, bitmapy] = correctone;
				}
			}
			return bit;
		}
		private static Color CalculateAverageSector(Bitmap bitmap)
		{
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
			Console.WriteLine(RGBConverter(col));
			return col;
		}
		private static string RGBConverter(Color c)
		{
			return $"#{c.R:X2}{c.G:X2}{c.B:X2}";
		}

		static int gcd(int a, int b)
		{
			if (a == 0)
				return b;
			return gcd(b % a, a);
		}

		// Function to print all the
		// common divisors
		static int[] printAllDivisors(int[] arr)
		{
			// Variable to find the gcd
			// of N numbers
			int g = arr[0];

			// Set to store all the
			// common divisors
			HashSet<int> divisors = new HashSet<int>();

			// Finding GCD of the given
			// N numbers
			for (int i = 1; i < arr.Length; i++)
			{
				g = gcd(arr[i], g);
			}

			// Finding divisors of the
			// HCF of n numbers
			for (int i = 1; i * i <= g; i++)
			{
				if (g % i == 0)
				{
					divisors.Add(i);
					if (g / i != i)
						divisors.Add(g / i);
				}
			}

			// Print all the divisors
			return divisors.ToArray();
		}
	}
}
