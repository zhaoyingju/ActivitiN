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
namespace org.activiti.engine.impl.json
{


	using JSONArray = org.activiti.engine.impl.util.json.JSONArray;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class JsonListConverter<T>
	{

	  internal JsonObjectConverter<T> jsonObjectConverter;

	  public JsonListConverter(JsonObjectConverter<T> jsonObjectConverter)
	  {
		this.jsonObjectConverter = jsonObjectConverter;
	  }

	  public virtual void toJson(IList<T> list, Writer writer)
	  {
		toJsonArray(list).write(writer);
	  }

	  public virtual string toJson(IList<T> list)
	  {
		return toJsonArray(list).ToString();
	  }

	  public virtual string toJson(IList<T> list, int indentFactor)
	  {
		return toJsonArray(list).ToString(indentFactor);
	  }

	  private JSONArray toJsonArray(IList<T> objects)
	  {
		JSONArray jsonArray = new JSONArray();
		foreach (T @object in objects)
		{
		  jsonArray.put(jsonObjectConverter.toJsonObject(@object));
		}
		return jsonArray;
	  }

	  public virtual IList<T> toObject(Reader reader)
	  {
		throw new ActivitiException("not yet implemented");
	  }

	  public virtual JsonObjectConverter<T> getJsonObjectConverter()
	  {
		return jsonObjectConverter;
	  }
	  public virtual void setJsonObjectConverter(JsonObjectConverter<T> jsonObjectConverter)
	  {
		this.jsonObjectConverter = jsonObjectConverter;
	  }
	}

}