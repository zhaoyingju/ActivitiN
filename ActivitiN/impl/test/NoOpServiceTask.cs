using System.Collections.Generic;

/* Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace org.activiti.engine.impl.test
{


	using DelegateExecution = org.activiti.engine.@delegate.DelegateExecution;
	using Expression = org.activiti.engine.@delegate.Expression;
	using JavaDelegate = org.activiti.engine.@delegate.JavaDelegate;

	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class NoOpServiceTask : JavaDelegate
	{

		public static AtomicInteger CALL_COUNT = new AtomicInteger(0);
		public static IList<string> NAMES = Collections.synchronizedList(new List<string>());

		protected internal Expression name;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute(org.activiti.engine.delegate.DelegateExecution execution) throws Exception
		public override void execute(DelegateExecution execution)
		{
			CALL_COUNT.incrementAndGet();
			NAMES.Add((string)name.getValue(execution));
		}

		public virtual Expression Name
		{
			get
			{
				return name;
			}
			set
			{
				this.name = value;
			}
		}


		public static void reset()
		{
			CALL_COUNT.set(0);
			NAMES.Clear();
		}

	}

}