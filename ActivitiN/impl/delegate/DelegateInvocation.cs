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
namespace org.activiti.engine.impl.@delegate
{

	using DelegateInterceptor = org.activiti.engine.impl.interceptor.DelegateInterceptor;

	/// <summary>
	/// Provides context about the invocation of usercode and handles the actual
	/// invocation
	/// 
	/// @author Daniel Meyer </summary>
	/// <seealso cref= DelegateInterceptor </seealso>
	public abstract class DelegateInvocation
	{

	  protected internal object invocationResult;
	  protected internal object[] invocationParameters;

	  /// <summary>
	  /// make the invocation proceed, performing the actual invocation of the user
	  /// code.
	  /// </summary>
	  /// <exception cref="Exception">
	  ///           the exception thrown by the user code </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void proceed() throws Exception
	  public virtual void proceed()
	  {
		invoke();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract void invoke() throws Exception;
	  protected internal abstract void invoke();

	  /// <returns> the result of the invocation (can be null if the invocation does
	  ///         not return a result) </returns>
	  public virtual object InvocationResult
	  {
		  get
		  {
			return invocationResult;
		  }
	  }

	  /// <returns> an array of invocation parameters (null if the invocation takes no
	  ///         parameters) </returns>
	  public virtual object[] InvocationParameters
	  {
		  get
		  {
			return invocationParameters;
		  }
	  }

	  /// <summary>
	  /// returns the target of the current invocation, ie. JavaDelegate, ValueExpression ... 
	  /// </summary>
	  public abstract object Target {get;}

	}

}