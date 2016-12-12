using System;
using System.Collections.Generic;

namespace org.activiti.engine.impl.@event.logger.handler
{


	using ActivitiVariableEvent = org.activiti.engine.@delegate.@event.ActivitiVariableEvent;
	using BooleanType = org.activiti.engine.impl.variable.BooleanType;
	using DateType = org.activiti.engine.impl.variable.DateType;
	using DoubleType = org.activiti.engine.impl.variable.DoubleType;
	using IntegerType = org.activiti.engine.impl.variable.IntegerType;
	using LongStringType = org.activiti.engine.impl.variable.LongStringType;
	using LongType = org.activiti.engine.impl.variable.LongType;
	using SerializableType = org.activiti.engine.impl.variable.SerializableType;
	using ShortType = org.activiti.engine.impl.variable.ShortType;
	using StringType = org.activiti.engine.impl.variable.StringType;
	using UUIDType = org.activiti.engine.impl.variable.UUIDType;
	using VariableType = org.activiti.engine.impl.variable.VariableType;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;

	using JsonProcessingException = com.fasterxml.jackson.core.JsonProcessingException;
	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public abstract class VariableEventHandler : AbstractDatabaseEventLoggerEventHandler
	{

		private static readonly Logger logger = LoggerFactory.getLogger(typeof(VariableEventHandler));

		public const string TYPE_BOOLEAN = "boolean";
		public const string TYPE_STRING = "string";
		public const string TYPE_SHORT = "short";
		public const string TYPE_INTEGER = "integer";
		public const string TYPE_DOUBLE = "double";
		public const string TYPE_LONG = "long";
		public const string TYPE_DATE = "date";
		public const string TYPE_UUID = "uuid";
		public const string TYPE_JSON = "json";

		protected internal virtual IDictionary<string, object> createData(ActivitiVariableEvent variableEvent)
		{
		  IDictionary<string, object> data = new Dictionary<string, object>();
			putInMapIfNotNull(data, Fields_Fields.NAME, variableEvent.VariableName);
			putInMapIfNotNull(data, Fields_Fields.PROCESS_DEFINITION_ID, variableEvent.ProcessDefinitionId);
			putInMapIfNotNull(data, Fields_Fields.PROCESS_INSTANCE_ID, variableEvent.ProcessInstanceId);
			putInMapIfNotNull(data, Fields_Fields.EXECUTION_ID, variableEvent.ExecutionId);
			putInMapIfNotNull(data, Fields_Fields.VALUE, variableEvent.VariableValue);

			VariableType variableType = variableEvent.VariableType;
			if (variableType is BooleanType)
			{

				putInMapIfNotNull(data, Fields_Fields.VALUE_BOOLEAN, (bool?) variableEvent.VariableValue);
				putInMapIfNotNull(data, Fields_Fields.VALUE, variableEvent.VariableValue);
				putInMapIfNotNull(data, Fields_Fields.VARIABLE_TYPE, TYPE_BOOLEAN);

			}
			else if (variableType is StringType || variableType is LongStringType)
			{

				putInMapIfNotNull(data, Fields_Fields.VALUE_STRING, (string) variableEvent.VariableValue);
				putInMapIfNotNull(data, Fields_Fields.VARIABLE_TYPE, TYPE_STRING);

			}
			else if (variableType is ShortType)
			{

				short? value = (short?) variableEvent.VariableValue;
				putInMapIfNotNull(data, Fields_Fields.VALUE_SHORT, value);
				putInMapIfNotNull(data, Fields_Fields.VARIABLE_TYPE, TYPE_SHORT);

				if (value != null)
				{
					putInMapIfNotNull(data, Fields_Fields.VALUE_INTEGER, (int)value);
					putInMapIfNotNull(data, Fields_Fields.VALUE_LONG, (long)value);
					putInMapIfNotNull(data, Fields_Fields.VALUE_DOUBLE, (double)value);
				}

			}
			else if (variableType is IntegerType)
			{

				int? value = (int?) variableEvent.VariableValue;
				putInMapIfNotNull(data, Fields_Fields.VALUE_INTEGER, value);
				putInMapIfNotNull(data, Fields_Fields.VARIABLE_TYPE, TYPE_INTEGER);

				if (value != null)
				{
					putInMapIfNotNull(data, Fields_Fields.VALUE_LONG, (long)value);
					putInMapIfNotNull(data, Fields_Fields.VALUE_DOUBLE, (double)value);
				}

			}
			else if (variableType is LongType)
			{

				long? value = (long?) variableEvent.VariableValue;
				putInMapIfNotNull(data, Fields_Fields.VALUE_LONG, value);
				putInMapIfNotNull(data, Fields_Fields.VARIABLE_TYPE, TYPE_LONG);

				if (value != null)
				{
					putInMapIfNotNull(data, Fields_Fields.VALUE_DOUBLE, (double)value);
				}

			}
			else if (variableType is DoubleType)
			{

				double? value = (double?) variableEvent.VariableValue;
				putInMapIfNotNull(data, Fields_Fields.VALUE_DOUBLE, value);
				putInMapIfNotNull(data, Fields_Fields.VARIABLE_TYPE, TYPE_DOUBLE);

				if (value != null)
				{
					putInMapIfNotNull(data, Fields_Fields.VALUE_INTEGER, (int)value);
					putInMapIfNotNull(data, Fields_Fields.VALUE_LONG, (long)value);
				}

			}
			else if (variableType is DateType)
			{

				DateTime value = (DateTime) variableEvent.VariableValue;
				putInMapIfNotNull(data, Fields_Fields.VALUE_DATE, value != null ? value.Ticks : null);
				putInMapIfNotNull(data, Fields_Fields.VARIABLE_TYPE, TYPE_DATE);

			}
			else if (variableType is UUIDType)
			{

				string value = null;
				if (variableEvent.VariableValue is UUID)
				{
					value = ((UUID) variableEvent.VariableValue).ToString();
				}
				else
				{
					value = (string) variableEvent.VariableValue;
				}

				putInMapIfNotNull(data, Fields_Fields.VALUE_UUID, value);
				putInMapIfNotNull(data, Fields_Fields.VALUE_STRING, value);
				putInMapIfNotNull(data, Fields_Fields.VARIABLE_TYPE, TYPE_UUID);

			}
			else if (variableType is SerializableType || (variableEvent.VariableValue != null && (variableEvent.VariableValue is object)))
			{

				// Last try: serialize it to json
				ObjectMapper objectMapper = new ObjectMapper();
				try
				{
			  string value = objectMapper.writeValueAsString(variableEvent.VariableValue);
				putInMapIfNotNull(data, Fields_Fields.VALUE_JSON, value);
					putInMapIfNotNull(data, Fields_Fields.VARIABLE_TYPE, TYPE_JSON);
					putInMapIfNotNull(data, Fields_Fields.VALUE, value);
				}
		  catch (JsonProcessingException)
		  {
			  // Nothing to do about it
			  logger.debug("Could not serialize variable value " + variableEvent.VariableValue);
		  }

			}

		  return data;
		}


	}

}