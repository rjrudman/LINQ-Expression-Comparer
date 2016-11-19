﻿/* Copyright (C) 2007 - 2008  Versant Inc.  http://www.db4o.com */

using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ExpressionComparer
{
	internal class ExpressionEnumeration : ExpressionVisitor, IEnumerable<Expression>
	{
		private readonly List<Expression> _expressions = new List<Expression>();
		
		public override Expression Visit(Expression expression)
		{
			if (expression == null) return null;

			_expressions.Add(expression);
			return base.Visit(expression);
		}

		public IEnumerator<Expression> GetEnumerator()
		{
			return _expressions.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}