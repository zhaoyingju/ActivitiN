using System.Text;

/*
 * Based on JUEL 2.2.1 code, 2006-2009 Odysseus Software GmbH
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace org.activiti.engine.impl.juel
{

	using ELContext = org.activiti.engine.impl.javax.el.ELContext;


	public class AstBinary : AstRightValue
	{
		public interface Operator
		{
			object eval(Bindings bindings, ELContext context, AstNode left, AstNode right);
		}
		public abstract class SimpleOperator : Operator
		{
			public virtual object eval(Bindings bindings, ELContext context, AstNode left, AstNode right)
			{
				return apply(bindings, left.eval(bindings, context), right.eval(bindings, context));
			}

			protected internal abstract object apply(TypeConverter converter, object o1, object o2);
		}
		public static readonly Operator ADD = new SimpleOperatorAnonymousInnerClassHelper();

		private class SimpleOperatorAnonymousInnerClassHelper : SimpleOperator
		{
			public SimpleOperatorAnonymousInnerClassHelper()
			{
			}

			public override object apply(TypeConverter converter, object o1, object o2)
			{
				return NumberOperations.add(converter, o1, o2);
			}
			public override string ToString()
			{
				return "+";
			}
		}
		public static readonly Operator AND = new OperatorAnonymousInnerClassHelper();

		private class OperatorAnonymousInnerClassHelper : Operator
		{
			public OperatorAnonymousInnerClassHelper()
			{
			}

			public virtual object eval(Bindings bindings, ELContext context, AstNode left, AstNode right)
			{
				bool? l = bindings.convert(left.eval(bindings, context), typeof(bool?));
				return true.Equals(l) ? bindings.convert(right.eval(bindings, context), typeof(bool?)) : false;
			}
			public override string ToString()
			{
				return "&&";
			}
		}
		public static readonly Operator DIV = new SimpleOperatorAnonymousInnerClassHelper2();

		private class SimpleOperatorAnonymousInnerClassHelper2 : SimpleOperator
		{
			public SimpleOperatorAnonymousInnerClassHelper2()
			{
			}

			public override object apply(TypeConverter converter, object o1, object o2)
			{
				return NumberOperations.div(converter, o1, o2);
			}
			public override string ToString()
			{
				return "/";
			}
		}
		public static readonly Operator EQ = new SimpleOperatorAnonymousInnerClassHelper3();

		private class SimpleOperatorAnonymousInnerClassHelper3 : SimpleOperator
		{
			public SimpleOperatorAnonymousInnerClassHelper3()
			{
			}

			public override object apply(TypeConverter converter, object o1, object o2)
			{
				return BooleanOperations.eq(converter, o1, o2);
			}
			public override string ToString()
			{
				return "==";
			}
		}
		public static readonly Operator GE = new SimpleOperatorAnonymousInnerClassHelper4();

		private class SimpleOperatorAnonymousInnerClassHelper4 : SimpleOperator
		{
			public SimpleOperatorAnonymousInnerClassHelper4()
			{
			}

			public override object apply(TypeConverter converter, object o1, object o2)
			{
				return BooleanOperations.ge(converter, o1, o2);
			}
			public override string ToString()
			{
				return ">=";
			}
		}
		public static readonly Operator GT = new SimpleOperatorAnonymousInnerClassHelper5();

		private class SimpleOperatorAnonymousInnerClassHelper5 : SimpleOperator
		{
			public SimpleOperatorAnonymousInnerClassHelper5()
			{
			}

			public override object apply(TypeConverter converter, object o1, object o2)
			{
				return BooleanOperations.gt(converter, o1, o2);
			}
			public override string ToString()
			{
				return ">";
			}
		}
		public static readonly Operator LE = new SimpleOperatorAnonymousInnerClassHelper6();

		private class SimpleOperatorAnonymousInnerClassHelper6 : SimpleOperator
		{
			public SimpleOperatorAnonymousInnerClassHelper6()
			{
			}

			public override object apply(TypeConverter converter, object o1, object o2)
			{
				return BooleanOperations.le(converter, o1, o2);
			}
			public override string ToString()
			{
				return "<=";
			}
		}
		public static readonly Operator LT = new SimpleOperatorAnonymousInnerClassHelper7();

		private class SimpleOperatorAnonymousInnerClassHelper7 : SimpleOperator
		{
			public SimpleOperatorAnonymousInnerClassHelper7()
			{
			}

			public override object apply(TypeConverter converter, object o1, object o2)
			{
				return BooleanOperations.lt(converter, o1, o2);
			}
			public override string ToString()
			{
				return "<";
			}
		}
		public static readonly Operator MOD = new SimpleOperatorAnonymousInnerClassHelper8();

		private class SimpleOperatorAnonymousInnerClassHelper8 : SimpleOperator
		{
			public SimpleOperatorAnonymousInnerClassHelper8()
			{
			}

			public override object apply(TypeConverter converter, object o1, object o2)
			{
				return NumberOperations.mod(converter, o1, o2);
			}
			public override string ToString()
			{
				return "%";
			}
		}
		public static readonly Operator MUL = new SimpleOperatorAnonymousInnerClassHelper9();

		private class SimpleOperatorAnonymousInnerClassHelper9 : SimpleOperator
		{
			public SimpleOperatorAnonymousInnerClassHelper9()
			{
			}

			public override object apply(TypeConverter converter, object o1, object o2)
			{
				return NumberOperations.mul(converter, o1, o2);
			}
			public override string ToString()
			{
				return "*";
			}
		}
		public static readonly Operator NE = new SimpleOperatorAnonymousInnerClassHelper10();

		private class SimpleOperatorAnonymousInnerClassHelper10 : SimpleOperator
		{
			public SimpleOperatorAnonymousInnerClassHelper10()
			{
			}

			public override object apply(TypeConverter converter, object o1, object o2)
			{
				return BooleanOperations.ne(converter, o1, o2);
			}
			public override string ToString()
			{
				return "!=";
			}
		}
		public static readonly Operator OR = new OperatorAnonymousInnerClassHelper2();

		private class OperatorAnonymousInnerClassHelper2 : Operator
		{
			public OperatorAnonymousInnerClassHelper2()
			{
			}

			public virtual object eval(Bindings bindings, ELContext context, AstNode left, AstNode right)
			{
				bool? l = bindings.convert(left.eval(bindings, context), typeof(bool?));
				return true.Equals(l) ? true : bindings.convert(right.eval(bindings, context), typeof(bool?));
			}
			public override string ToString()
			{
				return "||";
			}
		}
		public static readonly Operator SUB = new SimpleOperatorAnonymousInnerClassHelper11();

		private class SimpleOperatorAnonymousInnerClassHelper11 : SimpleOperator
		{
			public SimpleOperatorAnonymousInnerClassHelper11()
			{
			}

			public override object apply(TypeConverter converter, object o1, object o2)
			{
				return NumberOperations.sub(converter, o1, o2);
			}
			public override string ToString()
			{
				return "-";
			}
		}

		private readonly Operator @operator;
		private readonly AstNode left, right;

		public AstBinary(AstNode left, AstNode right, Operator @operator)
		{
			this.left = left;
			this.right = right;
			this.@operator = @operator;
		}

		public virtual Operator Operator
		{
			get
			{
				return @operator;
			}
		}

		public override object eval(Bindings bindings, ELContext context)
		{
			return @operator.eval(bindings, context, left, right);
		}

		public override string ToString()
		{
			return "'" + @operator.ToString() + "'";
		}

		public override void appendStructure(StringBuilder b, Bindings bindings)
		{
			left.appendStructure(b, bindings);
			b.Append(' ');
			b.Append(@operator);
			b.Append(' ');
			right.appendStructure(b, bindings);
		}

		public override int Cardinality
		{
			get
			{
				return 2;
			}
		}

		public override AstNode getChild(int i)
		{
			return i == 0 ? left : i == 1 ? right : null;
		}
	}

}