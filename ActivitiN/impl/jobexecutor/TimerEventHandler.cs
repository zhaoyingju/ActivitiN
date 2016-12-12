namespace org.activiti.engine.impl.jobexecutor
{
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

	using Expression = org.activiti.engine.@delegate.Expression;
	using JSONException = org.activiti.engine.impl.util.json.JSONException;
	using JSONObject = org.activiti.engine.impl.util.json.JSONObject;

	public class TimerEventHandler
	{

	  public const string PROPERTYNAME_TIMER_ACTIVITY_ID = "activityId";
	  public const string PROPERTYNAME_END_DATE_EXPRESSION = "timerEndDate";
	  public const string PROPERTYNAME_PROCESS_DEFINITION_KEY = "processDefinitionKey";
	  public const string PROPERTYNAME_CALENDAR_NAME_EXPRESSION = "calendarName";

	  public static string createConfiguration(string id, Expression endDate, Expression calendarName)
	  {
		JSONObject cfgJson = new JSONObject();
		cfgJson.put(PROPERTYNAME_TIMER_ACTIVITY_ID, id);
		if (endDate != null)
		{
		  cfgJson.put(PROPERTYNAME_END_DATE_EXPRESSION, endDate.ExpressionText);
		}
		if (calendarName != null)
		{
		  cfgJson.put(PROPERTYNAME_CALENDAR_NAME_EXPRESSION, calendarName.ExpressionText);
		}
		return cfgJson.ToString();
	  }

	  public virtual string setActivityIdToConfiguration(string jobHandlerConfiguration, string activityId)
	  {
		try
		{
		  JSONObject cfgJson = new JSONObject(jobHandlerConfiguration);
		  cfgJson.put(PROPERTYNAME_TIMER_ACTIVITY_ID, activityId);
		  return cfgJson.ToString();
		}
		catch (JSONException)
		{
		  return jobHandlerConfiguration;
		}
	  }

	  public static string getActivityIdFromConfiguration(string jobHandlerConfiguration)
	  {
	   try
	   {
		 JSONObject cfgJson = new JSONObject(jobHandlerConfiguration);
		 return cfgJson.get(PROPERTYNAME_TIMER_ACTIVITY_ID).ToString();
	   }
	   catch (JSONException)
	   {
		return jobHandlerConfiguration;
	   }
	  }

	  public static string geCalendarNameFromConfiguration(string jobHandlerConfiguration)
	  {
		try
		{
		  JSONObject cfgJson = new JSONObject(jobHandlerConfiguration);
		  return cfgJson.get(PROPERTYNAME_CALENDAR_NAME_EXPRESSION).ToString();
		}
		catch (JSONException)
		{
		  // calendar name is not specified
		  return "";
		}
	  }

	  public virtual string setEndDateToConfiguration(string jobHandlerConfiguration, string endDate)
	  {
		JSONObject cfgJson = null;
		try
		{
		  cfgJson = new JSONObject(jobHandlerConfiguration);
		}
		catch (JSONException)
		{
		  //create the json config
		  cfgJson = new JSONObject();
		  cfgJson.put(PROPERTYNAME_TIMER_ACTIVITY_ID, jobHandlerConfiguration);
		}
		if (endDate != null)
		{
		  cfgJson.put(PROPERTYNAME_END_DATE_EXPRESSION, endDate);
		}

		return cfgJson.ToString();
	  }

	  public static string getEndDateFromConfiguration(string jobHandlerConfiguration)
	  {
		try
		{
		  JSONObject cfgJson = new JSONObject(jobHandlerConfiguration);
		  return cfgJson.get(PROPERTYNAME_END_DATE_EXPRESSION).ToString();
		}
		catch (JSONException)
		{
		  return null;
		}
	  }

	  public virtual string setProcessDefinitionKeyToConfiguration(string jobHandlerConfiguration, string activityId)
	  {
		try
		{
		  JSONObject cfgJson = new JSONObject(jobHandlerConfiguration);
		  cfgJson.put(PROPERTYNAME_PROCESS_DEFINITION_KEY, activityId);
		  return cfgJson.ToString();
		}
		catch (JSONException)
		{
		  return jobHandlerConfiguration;
		}
	  }

	  public virtual string getProcessDefinitionKeyFromConfiguration(string jobHandlerConfiguration)
	  {
		try
		{
		  JSONObject cfgJson = new JSONObject(jobHandlerConfiguration);
		  return cfgJson.get(PROPERTYNAME_PROCESS_DEFINITION_KEY).ToString();
		}
		catch (JSONException)
		{
		  return null;
		}
	  }

	  /// <summary>
	  /// Before Activiti 5.21, the jobHandlerConfiguration would have as activityId the process definition key
	  /// (as only one timer start event was supported). In >= 5.21, this changed and in >= 5.21 the activityId 
	  /// is the REAL activity id. It can be recognized by having the 'processDefinitionKey' in the configuration.
	  /// A < 5.21 job would not have that.
	  /// </summary>
	  public static bool hasRealActivityId(string jobHandlerConfiguration)
	  {
		try
		{
		  JSONObject cfgJson = new JSONObject(jobHandlerConfiguration);
		  object processDefinitionKey = cfgJson.get(PROPERTYNAME_PROCESS_DEFINITION_KEY);
		  if (processDefinitionKey != null)
		  {
			return processDefinitionKey.ToString().Length > 0;
		  }
		}
		catch (JSONException)
		{
		  return false;
		}
		return false;
	  }

	}

}