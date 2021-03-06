using System;

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
namespace org.activiti.engine.impl.transformer
{


	using FastDateFormat = org.apache.commons.lang3.time.FastDateFormat;

	/// <summary>
	/// Transforms a <seealso cref="Date"/> to a <seealso cref="String"/>
	/// 
	/// @author Esteban Robles Luna
	/// </summary>
	public class DateToString : AbstractTransformer
	{

	  protected internal Format format = FastDateFormat.getInstance("dd/MM/yyyy");

	  /// <summary>
	  /// {@inheritDoc}
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected Object primTransform(Object anObject) throws Exception
	  protected internal override object primTransform(object anObject)
	  {
		return format.format((DateTime) anObject);
	  }
	}

}