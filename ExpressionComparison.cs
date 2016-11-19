/* Copyright (C) 2007 - 2008  Versant Inc.  http://www.db4o.com */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using NUnit.Framework;

namespace ExpressionComparer
{
	internal class ExpressionComparison : ExpressionVisitor
	{
		private readonly Queue<Expression> _candidates;
		private Expression _candidate;

		public bool AreEqual { get; private set; } = true;

		public ExpressionComparison(Expression b)
		{
			var enumeration = new ExpressionEnumeration();
			enumeration.Visit(b);
			_candidates = new Queue<Expression>(enumeration);
		}

		private Expression PeekCandidate()
		{
			return _candidates.Count == 0 ? null : _candidates.Peek();
		}

		private void PopCandidate()
		{
			_candidates.Dequeue();
		}

		private void CheckAreOfSameType(Expression candidate, Expression expression)
		{
			AssertEqual(expression.NodeType, candidate.NodeType);
			AssertEqual(expression.Type, candidate.Type);
		}

		private void Stop()
		{
			AreEqual = false;
		}

		private T CandidateFor<T>() where T : Expression
		{
			return (T)_candidate;
		}

		public void AssertEqual(Expression expression)
		{
			Visit(expression);

			if (_candidates.Count > 0) Stop();
		}

		public override Expression Visit(Expression expression)
		{
			if (expression == null) return null;
			if (!AreEqual) return expression;

			_candidate = PeekCandidate();
			AssertNotNull(_candidate);
			CheckAreOfSameType(_candidate, expression);

			PopCandidate();

			return base.Visit(expression);
		}

		protected override Expression VisitConstant(ConstantExpression constant)
		{
			var candidate = CandidateFor<ConstantExpression>();
			AssertEqual(constant.Value, candidate.Value);
			return base.VisitConstant(constant);
		}

		protected override Expression VisitMember(MemberExpression member)
		{
			var candidate = CandidateFor<MemberExpression>();
			AssertEqual(member.Member, candidate.Member);

			return base.VisitMember(member);
		}

		protected override Expression VisitMethodCall(MethodCallExpression methodCall)
		{
			var candidate = CandidateFor<MethodCallExpression>();
			AssertEqual(methodCall.Method, candidate.Method);

			return base.VisitMethodCall(methodCall);
		}

		protected override Expression VisitParameter(ParameterExpression parameter)
		{
			var candidate = CandidateFor<ParameterExpression>();
			AssertEqual(parameter.Name, candidate.Name);

			return base.VisitParameter(parameter);
		}
		
		protected override Expression VisitTypeBinary(TypeBinaryExpression type)
		{
			var candidate = CandidateFor<TypeBinaryExpression>();
			AssertEqual(type.TypeOperand, candidate.TypeOperand);

			return base.VisitTypeBinary(type);
		}

		protected override Expression VisitBinary(BinaryExpression binary)
		{
			var candidate = CandidateFor<BinaryExpression>();
			AssertEqual(binary.Method, candidate.Method);
			AssertEqual(binary.IsLifted, candidate.IsLifted);
			AssertEqual(binary.IsLiftedToNull, candidate.IsLiftedToNull);

			return base.VisitBinary(binary);
		}

		protected override Expression VisitUnary(UnaryExpression unary)
		{
			var candidate = CandidateFor<UnaryExpression>();
			AssertEqual(unary.Method, candidate.Method);
			AssertEqual(unary.IsLifted, candidate.IsLifted);
			AssertEqual(unary.IsLiftedToNull, candidate.IsLiftedToNull);

			return base.VisitUnary(unary);
		}

		protected override Expression VisitNew(NewExpression nex)
		{
			var candidate = CandidateFor<NewExpression>();
			AssertEqual(nex.Constructor, candidate.Constructor);

			CompareList(nex.Members, candidate.Members);

			return base.VisitNew(nex);
		}

		private void CompareList<T>(ReadOnlyCollection<T> collection, ReadOnlyCollection<T> candidates)
		{
			CompareList(collection, candidates, (item, candidate) => EqualityComparer<T>.Default.Equals(item, candidate));
		}

		private void CompareList<T>(ReadOnlyCollection<T> collection, ReadOnlyCollection<T> candidates, Func<T, T, bool> comparer)
		{
			AssertAreOfSameSize(collection, candidates);

			for (int i = 0; i < collection.Count; i++)
			{
				if (!comparer(collection[i], candidates[i]))
				{
					Stop();
					return;
				}
			}
		}

		private void AssertAreOfSameSize<T>(ReadOnlyCollection<T> collection, ReadOnlyCollection<T> candidate)
		{
			Assert.AreEqual(collection.Count, candidate.Count, $"Expected collection size of {collection.Count}, but found {candidate.Count}.");
		}

		private void AssertNotNull<T>(T t) where T : class
		{
			Assert.NotNull(t, "Expected expression to not be null, but was null");
		}

		private void AssertEqual<T>(T t, T candidate)
		{
			if (!EqualityComparer<T>.Default.Equals(t, candidate))
				Assert.Fail($"Expected {t} but found {candidate}");
		}
	}
}