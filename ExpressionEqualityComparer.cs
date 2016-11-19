/* Copyright (C) 2007 - 2008  Versant Inc.  http://www.db4o.com */

using System.Linq.Expressions;
using NUnit.Framework;

namespace ExpressionComparer
{
	public class ExpressionEqualityComparer
	{
		// ReSharper disable UnusedParameter.Local
		public static void AssertExpressionsEqual(Expression expected, Expression actual)
		// ReSharper enable UnusedParameter.Local
		{
			try
			{
				new ExpressionEqualityComparer().AssertEqual(expected, actual);
			}
			catch (AssertionException ex)
			{
				throw new AssertionException($"Expressions are not equal. \n{ex.Message}\n\nExpected: {expected}\nActual  : {actual}", ex);
			}
		}
		
		private void AssertEqual(Expression a, Expression b)
		{
			var comparer = new ExpressionComparison(b);
			comparer.AssertEqual(a);		
		}
	}
}