using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace parseLambda
{
	class Program
	{
		static public int F(int x)
		{
			x = x * 3;
			return x;
		}

		static void Main(string[] args)
		{

			var t = 5;
			//	Func<int, int, int> lambda = (x, y) => F(x) > F(y) ? F(x) : (F(x) < F(2 * y) ? F(2 * y) : F(y));
	
			Expression<Func<int, int, int>> expLambda = (x, y) => F(x) > F(y) ? F(x) : (F(x) < F(2 * y) ? F(2 * y) : F(y));

			var lambda = new Lambda(expLambda);
			var optlambda = lambda.OptimizeLambda(lambda.LambdaExpression);
			object[] lambdaArg = {2, 5};
			var res = lambda.OptimizeCalculation(lambda.LambdaExpression, lambdaArg);

			Console.WriteLine(expLambda.ToString());
		}
	}
}
