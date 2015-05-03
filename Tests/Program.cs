using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
	class Program
	{
		public static void Main(string[] args)
		{
			NUnit.ConsoleRunner.Runner.Main(new string[] {
				System.Reflection.Assembly.GetExecutingAssembly().Location
			});
		}
	}
}
