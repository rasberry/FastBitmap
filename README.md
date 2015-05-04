# FastBitmap
A wrapper for C# Bitmap that uses the raw data to get / set pixels fast

## Usage example
```C#
Bitmap bmp = new Bitmap(100,100,System.Drawing.Imaging.PixelFormat.Format32bppArgb);
using(var lb = new LockBitmap(bmp))
{
	lb.LockBits();
	for(int y=0; y<lb.Height; y++) {
		for(int x=0; x<lb.Width; x++) {
			lb.SetPixel(x,y,Color.Red);
		}
	}
	lb.UnlockBits();
}
```