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

namespace org.activiti.engine.impl.variable
{

	using ByteArrayEntity = org.activiti.engine.impl.persistence.entity.ByteArrayEntity;


	/// <summary>
	/// Common interface for regular and historic variable entities.
	/// 
	/// @author Tom Baeyens
	/// </summary>
	public interface ValueFields
	{

	  /// <returns> the name of the variable </returns>
	  string Name {get;}

	  /// <returns> the process instance id of the variable </returns>
	  string ProcessInstanceId {get;}

	  /// <returns> the execution id of the variable </returns>
	  string ExecutionId {get;}

	  /// <returns> the task id of the variable </returns>
	  string TaskId {get;}

	  /// <returns> the first text value, if any, or null. </returns>
	  string TextValue {get;set;}


	  /// <returns> the second text value, if any, or null. </returns>
	  string TextValue2 {get;set;}


	  /// <returns> the long value, if any, or null. </returns>
	  long? LongValue {get;set;}


	  /// <returns> the double value, if any, or null. </returns>
	  double? DoubleValue {get;set;}


	  /// <returns> the byte array value, if any, or null. </returns>
	  sbyte[] Bytes {get;set;}


	  /// <returns> the id of the byte array entity value, or null if the byte array value is null. </returns>
	  /// @deprecated should no longer be used 
	  [Obsolete("should no longer be used")]
	  string ByteArrayValueId {get;}

	  /// <returns> the ByteArrayEntity that contains the byte array value, or null if the byte array value is null. </returns>
	  /// @deprecated use getBytes. 
	  [Obsolete("use getBytes.")]
	  ByteArrayEntity ByteArrayValue {get;set;}


	  object CachedValue {get;set;}

	}

}