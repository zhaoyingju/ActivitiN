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

	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;

	using JsonNode = com.fasterxml.jackson.databind.JsonNode;
	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;



	/// <summary>
	/// @author Tijs Rademakers
	/// </summary>
	public class JsonType : VariableType
	{

	  private static readonly Logger logger = LoggerFactory.getLogger(typeof(JsonType));

	  protected internal readonly int maxLength;
	  protected internal ObjectMapper objectMapper = null;

	  public JsonType(int maxLength, ObjectMapper objectMapper)
	  {
		this.maxLength = maxLength;
		this.objectMapper = objectMapper;
	  }

	  public virtual string TypeName
	  {
		  get
		  {
			return "json";
		  }
	  }

	  public virtual bool Cachable
	  {
		  get
		  {
			return true;
		  }
	  }

	  public virtual object getValue(ValueFields valueFields)
	  {
		JsonNode jsonValue = null;
		if (valueFields.TextValue != null && valueFields.TextValue.Length > 0)
		{
		  try
		  {
			jsonValue = objectMapper.readTree(valueFields.TextValue);
		  }
		  catch (Exception e)
		  {
			logger.error("Error reading json variable " + valueFields.Name, e);
		  }
		}
		return jsonValue;
	  }

	  public virtual void setValue(object value, ValueFields valueFields)
	  {
		valueFields.TextValue = value != null ? value.ToString() : null;
	  }

	  public virtual bool isAbleToStore(object value)
	  {
		if (value == null)
		{
		  return true;
		}
		if (value.GetType().IsSubclassOf(typeof(JsonNode)))
		{
		  JsonNode jsonValue = (JsonNode) value;
		  return jsonValue.ToString().Length <= maxLength;
		}
		return false;
	  }
	}

}