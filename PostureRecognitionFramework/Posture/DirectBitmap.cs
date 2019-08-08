using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using nuitrack;
using nuitrack.issues;

namespace Posture
{
    public class DirectBitmap : IDisposable
	{
		public DirectBitmap(int width, int height)
		{
			Width      = width;
			Height     = height;
			Bits       = new Int32[width * height];
			BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
			Bitmap     = new Bitmap(width, height, width * 4, PixelFormat.Format32bppPArgb, BitsHandle.AddrOfPinnedObject());
		}

        // ---------------------------------------------------------------------------------------------------- //
        // ---------------------------------------------------------------------------------------------------- //
        // ---------------------------------------------------------------------------------------------------- //

        /// <summary>
        /// Draw a square in the Bitmap
        /// </summary>
        /// <param name="x0"></param>
        /// <param name="y0"></param>
        /// <param name="r0"></param>
        /// <param name="color"></param>
        public void SetSquare(int x0, int y0, int r0, Color color)
        {
            // Limit the coordinate
            int x_s = x0 - r0 > 0 ? x0 - r0 : 0;
            int y_s = y0 - r0 > 0 ? y0 - r0 : 0;
            int x_e = x0 + r0 <  Width ? x0 + r0 :  Width - 1;
            int y_e = y0 + r0 < Height ? y0 + r0 : Height - 1;


            // Set pixels
            for (int x = x_s; x <= x_e; x++)
            {
                for (int y = y_s; y <= y_e; y++)
                {
                    SetPixel(x, y, color);
                }
            }
        }

        // ---------------------------------------------------------------------------------------------------- //
        // ---------------------------------------------------------------------------------------------------- //
        // ---------------------------------------------------------------------------------------------------- //

		public void SetPixel(int x, int y, Color colour)
		{
			int index = x + (y * Width);
			int col = colour.ToArgb();

			Bits[index] = col;
		}

		public Color GetPixel(int x, int y)
		{
			int index = x + (y * Width);
			int col = Bits[index];
			Color result = Color.FromArgb(col);

			return result;
		}

		public void Dispose()
		{
			if (Disposed)
				return;
			Disposed = true;
			Bitmap.Dispose();
			BitsHandle.Free();
		}

        // ---------------------------------------------------------------------------------------------------- //
        // ---------------------------------------------------------------------------------------------------- //
        // ---------------------------------------------------------------------------------------------------- //

		public Bitmap Bitmap { get; set; }

		public Int32[] Bits { get; private set; }

		public bool Disposed { get; private set; }

		public int Height { get; private set; }

		public int Width { get; private set; }

		protected GCHandle BitsHandle { get; private set; }
	}
}
