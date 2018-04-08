using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastBitmap
{
	// Source: http://www.codeproject.com/Tips/240428/Work-with-bitmap-faster-with-Csharp
	// http://stackoverflow.com/questions/24701703/c-sharp-faster-alternatives-to-setpixel-and-getpixel-for-bitmaps-for-windows-f
	// License: http://www.codeproject.com/info/cpol10.aspx

	public unsafe class LockBitmap : IDisposable
	{
		public int Depth { get; private set; }
		public int Width { get; private set; }
		public int Height { get; private set; }
		public Bitmap Source { get { return source; }}
	
		Bitmap source = null;
		byte* Iptr = null;
		BitmapData bitmapData = null;
		bool isLocked = false;
		long Length = -1;

		public LockBitmap(Bitmap source)
		{
			if (source == null) {
				throw new ArgumentNullException("source");
			}
			this.source = source;
		}
	 
		/// <summary>
		/// Lock bitmap data
		/// </summary>
		public void LockBits()
		{
			// Get width and height of bitmap
			Width = source.Width;
			Height = source.Height;
 
			// get total locked pixels count
			long pixelCount = Width * Height;
 
			// Create rectangle to lock
			Rectangle rect = new Rectangle(0, 0, Width, Height);
 
			// get source bitmap pixel format size
			Depth = Image.GetPixelFormatSize(source.PixelFormat);

			//set the length in bytes (total pixels * bytes/pixel)
			Length = pixelCount * Depth / 8;
 
			// Check if bpp (Bits Per Pixel) is 8, 24, or 32
			if (Depth != 8 && Depth != 24 && Depth != 32)
			{
				throw new ArgumentException("Only 8, 24 and 32 bpp images are supported.");
			}
 
			// Lock bitmap and return bitmap data
			isLocked = true;
			bitmapData = source.LockBits(rect, ImageLockMode.ReadWrite,source.PixelFormat);
 
			// create byte array to copy pixel values
			//int step = Depth / 8;
			//Pixels = new byte[PixelCount * step];
			Iptr = (byte*)bitmapData.Scan0;
 
			// Copy data from pointer to array
			//Marshal.Copy(Iptr, Pixels, 0, Pixels.Length);
		}
	 
		/// <summary>
		/// Unlock bitmap data
		/// </summary>
		public void UnlockBits()
		{
			// Copy data from byte array to pointer
			//Marshal.Copy(Pixels, 0, Iptr, Pixels.Length);
 
			// Unlock bitmap data
			source.UnlockBits(bitmapData);
			isLocked = false;
		}
	 
		/// <summary>
		/// Get the color of the specified pixel
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public Color GetPixel(int x, int y)
		{
			if (!isLocked) {
				throw new BadStateException("Bitmap must be locked first");
			}

			Color clr = Color.Empty;
	 
			// Get color components count
			int cCount = Depth / 8;
	 
			// Get start index of the specified pixel
			int i = ((y * Width) + x) * cCount;
	 
			if (i > Length - cCount) {
				throw new IndexOutOfRangeException();
			}
	 
			if (Depth == 32) // For 32 bpp get Red, Green, Blue and Alpha
			{
				byte b = Iptr[i];
				byte g = Iptr[i + 1];
				byte r = Iptr[i + 2];
				byte a = Iptr[i + 3]; // a
				clr = Color.FromArgb(a, r, g, b);
			}
			if (Depth == 24) // For 24 bpp get Red, Green and Blue
			{
				byte b = Iptr[i];
				byte g = Iptr[i + 1];
				byte r = Iptr[i + 2];
				clr = Color.FromArgb(r, g, b);
			}
			if (Depth == 8) // For 8 bpp get color value (Red, Green and Blue values are the same)
			{
				byte c = Iptr[i];
				clr = Color.FromArgb(c, c, c);
			}
			return clr;
		}
	 
		/// <summary>
		/// Set the color of the specified pixel
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="color"></param>
		public void SetPixel(int x, int y, Color color)
		{
			if (!isLocked) {
				throw new BadStateException("Bitmap must be locked first");
			}

			// Get color components count
			int cCount = Depth / 8;
	 
			// Get start index of the specified pixel
			int i = ((y * Width) + x) * cCount;

			if (i > Length - cCount) {
				throw new IndexOutOfRangeException();
			}
	 
			if (Depth == 32) // For 32 bpp set Red, Green, Blue and Alpha
			{
				Iptr[i] = color.B;
				Iptr[i + 1] = color.G;
				Iptr[i + 2] = color.R;
				Iptr[i + 3] = color.A;
			}
			if (Depth == 24) // For 24 bpp set Red, Green and Blue
			{
				Iptr[i] = color.B;
				Iptr[i + 1] = color.G;
				Iptr[i + 2] = color.R;
			}
			if (Depth == 8) // For 8 bpp set color value (Red, Green and Blue values are the same)
			{
				Iptr[i] = color.B;
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing) 
			{
				if (isLocked) { this.UnlockBits(); }
			}
		}
	}
}
