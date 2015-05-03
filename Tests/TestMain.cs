using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Drawing;
using System.IO;
using FastBitmap;

namespace Tests
{
	[TestFixture]
	public class TestMain
	{
		[Test]
		public void TestOneByOne()
		{
			Bitmap bmp = Helpers.GetOneByOne();
			using(var lb = new LockBitmap(bmp))
			{
				lb.LockBits();
				lb.SetPixel(0,0,Color.Red);
				lb.UnlockBits();
			}

			Color c = bmp.GetPixel(0,0);
			Assert.IsTrue(Helpers.AreColorsEqual(c,Color.Red));
		}

		[Test]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void TestOneByOneIndexOOB()
		{
			Bitmap bmp = Helpers.GetOneByOne();
			using(var lb = new LockBitmap(bmp))
			{
				lb.LockBits();
				lb.SetPixel(0,1,Color.Red);
				lb.UnlockBits();
			}
		}
	}
}
