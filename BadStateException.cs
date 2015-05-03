using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastBitmap
{
	[Serializable]
	public class BadStateException : Exception
	{
		public BadStateException() : base()
		{
		}

		public BadStateException(string message) : base(message)
		{
		}

		public BadStateException(string message,Exception innerException) : base(message,innerException)
		{
		}
	}
}
