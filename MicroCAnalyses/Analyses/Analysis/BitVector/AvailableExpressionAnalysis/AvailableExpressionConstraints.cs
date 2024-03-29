using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Analyses.Helpers;

namespace Analyses.Analysis.BitVector.AvailableExpressionAnalysis
{
    public class AvailableExpressionConstraints : IConstraints
    {
        public HashSet<MicroCTypes.arithmeticExpression> AvailableArithmeticExpressions { get; }
        public HashSet<MicroCTypes.booleanExpression> AvailableBooleanExpressions { get; }
        
        public AvailableExpressionConstraints(HashSet<MicroCTypes.arithmeticExpression> arithmeticSubExpressions, HashSet<MicroCTypes.booleanExpression> booleanSubExpressions)
        {
            AvailableArithmeticExpressions = new HashSet<MicroCTypes.arithmeticExpression>(arithmeticSubExpressions);
            AvailableBooleanExpressions = new HashSet<MicroCTypes.booleanExpression>(booleanSubExpressions);
        }

        /// <summary>
        /// For Cloning purposes
        /// </summary>
        private AvailableExpressionConstraints()
        {
            AvailableArithmeticExpressions= new HashSet<MicroCTypes.arithmeticExpression>();
            AvailableBooleanExpressions = new HashSet<MicroCTypes.booleanExpression>();
        }

        public bool IsSubset(IConstraints other)
        {
            throw new System.NotImplementedException("Not meant to be implemented");
        }

        public bool IsSuperSet(IConstraints other)
        {
            if (!(other is AvailableExpressionConstraints otherAvailableExpressionsConstraint))
            {
                return false;
            }

            if (otherAvailableExpressionsConstraint.AvailableArithmeticExpressions
                .Any(ae => !AvailableArithmeticExpressions.Contains(ae)))
            {
                return false;
            }

            if (otherAvailableExpressionsConstraint.AvailableBooleanExpressions
                .Any(ae => !AvailableBooleanExpressions.Contains(ae)))
            {
                return false;
            }

            return true;
        }

        public IConstraints Clone()
        {
            var clonedConstraints = new AvailableExpressionConstraints();
            foreach (var expression in AvailableArithmeticExpressions)
            {
                clonedConstraints.AvailableArithmeticExpressions.Add(expression);
            }
            foreach (var expression in AvailableBooleanExpressions)
            {
                clonedConstraints.AvailableBooleanExpressions.Add(expression);
            }

            return clonedConstraints;
        }

        public void Join(IConstraints other)
        {
            if (!(other is AvailableExpressionConstraints otherAvailableExpressionsConstraint))
            {
                throw new Exception($"Join operator called with constraint type that was different from {nameof(AvailableExpressionConstraints)}");
            }
            
            //Intersect because constraints are underapproximating
            AvailableArithmeticExpressions.IntersectWith(otherAvailableExpressionsConstraint.AvailableArithmeticExpressions);
            AvailableBooleanExpressions.IntersectWith(otherAvailableExpressionsConstraint.AvailableBooleanExpressions);
        }
        
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is AvailableExpressionConstraints other))
            {
                return false;
            }

            return AvailableArithmeticExpressions.SetEquals(other.AvailableArithmeticExpressions)
                && AvailableBooleanExpressions.SetEquals(other.AvailableBooleanExpressions);
        }
        
        public override int GetHashCode()
        {
            return HashCode.Combine(AvailableArithmeticExpressions.GetHashCode(), AvailableBooleanExpressions.GetHashCode()); //Only used in a non readonly fashion in tests
        }
        
        public override string ToString()
        {

            var sb = new StringBuilder();
            sb.Append("{ ");
            foreach (var availableExpression in AvailableArithmeticExpressions)
            {
                sb.Append(AstExtensions.AstToString(availableExpression));
                sb.Append($", ");
            }
            
            var availableArithmetic = sb.ToString();
            if (availableArithmetic.EndsWith(", "))
            {
                availableArithmetic = availableArithmetic.Substring(0, availableArithmetic.Length - 2);
            }
            
            availableArithmetic += "}";
            
            var sb2 = new StringBuilder();
            sb2.Append("{ ");
            foreach (var availableExpression in AvailableBooleanExpressions)
            {
                sb2.Append(AstExtensions.AstToString(availableExpression));
                sb2.Append($", ");
            }

            var availableBoolean = sb2.ToString();
            if (availableBoolean.EndsWith(", "))
            {
                availableBoolean = availableBoolean.Substring(0, availableBoolean.Length - 2);
            }
            availableBoolean += "}";

            return $"({availableArithmetic}, {availableBoolean})";
        }
    }
}