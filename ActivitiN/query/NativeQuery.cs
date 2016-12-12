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

namespace org.activiti.engine.query
{


	/// <summary>
	/// Describes basic methods for doing native queries
	/// 
	/// @author Bernd Ruecker (camunda)
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public interface NativeQuery<T extends NativeQuery< ?, ? >, U extends Object>
	public interface NativeQuery<T, U> where T : NativeQuery< ?, ? > where U : Object
	{

	  /// <summary>
	  /// Hand in the SQL statement you want to execute. BEWARE: if you need a count you have to hand in a count() statement
	  /// yourself, otherwise the result will be treated as lost of Activiti entities.
	  /// 
	  /// If you need paging you have to insert the pagination code yourself. We skipped doing this for you
	  /// as this is done really different on some databases (especially MS-SQL / DB2)
	  /// </summary>
	  T sql(string selectClause);

	  /// <summary>
	  /// Add parameter to be replaced in query for index, e.g. :param1, :myParam, ...
	  /// </summary>
	  T parameter(string name, object value);

	  /// <summary>
	  /// Executes the query and returns the number of results </summary>
	  long count();

	  /// <summary>
	  /// Executes the query and returns the resulting entity or null if no
	  /// entity matches the query criteria. </summary>
	  /// <exception cref="ActivitiException"> when the query results in more than one
	  /// entities. </exception>
	  U singleResult();

	  /// <summary>
	  /// Executes the query and get a list of entities as the result. </summary>
	  IList<U> list();

	  /// <summary>
	  /// Executes the query and get a list of entities as the result. </summary>
	  IList<U> listPage(int firstResult, int maxResults);
	}

}