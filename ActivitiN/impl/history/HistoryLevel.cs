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

namespace org.activiti.engine.impl.history
{


	/// <summary>
	/// Enum that contains all possible history-levels. 
	/// 
	/// @author Frederik Heremans
	/// </summary>
	public enum HistoryLevel
	{

//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
	  NONE("none"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
	  ACTIVITY("activity"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
	  AUDIT("audit"),
//JAVA TO C# CONVERTER TODO TASK: Enum values cannot be strings in .NET:
	  FULL("full");

//JAVA TO C# CONVERTER TODO TASK: Enums cannot contain fields in .NET:
//	  private String key;

//JAVA TO C# CONVERTER TODO TASK: Enums cannot contain methods in .NET:
//	  private HistoryLevel(String key)
	//  {
	//	this.key = key;
	//  }

	  /// <param name="key"> string representation of level </param>
	  /// <returns> <seealso cref="HistoryLevel"/> for the given key </returns>
	  /// <exception cref="ActivitiException"> when passed in key doesn't correspond to existing level </exception>
//JAVA TO C# CONVERTER TODO TASK: Enums cannot contain methods in .NET:
//	  public static HistoryLevel getHistoryLevelForKey(String key)
	//  {
	//	for(HistoryLevel level : values())
	//	{
	//	  if(level.key.equals(key))
	//	  {
	//		return level;
	//	  }
	//	}
	//	throw new ActivitiIllegalArgumentException("Illegal value for history-level: " + key);
	//  }

	  /// <summary>
	  /// String representation of this history-level.
	  /// </summary>

	  /// <summary>
	  /// Checks if the given level is the same as, or higher in order than the
	  /// level this method is executed on.
	  /// </summary>
	}
	public static partial class EnumExtensionMethods
	{
	  public static string getKey(this HistoryLevel instanceJavaToDotNetTempPropertyGetKey)
	  {
		return key;
	  }
	  public static bool isAtLeast(this HistoryLevel instance, HistoryLevel level)
	  {
		// Comparing enums actually compares the location of values declared in the enum
		return instance.compareTo(level) >= 0;
	  }
	}
}