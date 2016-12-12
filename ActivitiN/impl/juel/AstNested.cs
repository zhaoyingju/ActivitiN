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


	public sealed class AstNested : AstRightValue
	{
		private readonly AstNode child;

		public AstNested(AstNode child)
		{
			this.child = child;
		}

		public override object eval(Bindings bindings, ELContext context)
		{
			return child.eval(bindings, context);
		}

		public override string ToString()
		{
			return "(...)";
		}

		public override void appendStructure(StringBuilder b, Bindings bindings)
		{
			b.Append("(");
			child.appendStructure(b, bindings);
			b.Append(")");
		}

		public override int Cardinality
		{
			get
			{
				return 1;
			}
		}

		public override AstNode getChild(int i)
		{
			return i == 0 ? child : null;
		}
	}

}