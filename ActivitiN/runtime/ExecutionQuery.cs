using System;

namespace org.activiti.engine.runtime
{


	using org.activiti.engine.query;



	/// <summary>
	/// Allows programmatic querying of <seealso cref="Execution"/>s.
	/// 
	/// @author Joram Barrez
	/// @author Frederik Heremans
	/// </summary>
	public interface ExecutionQuery : Query<ExecutionQuery, Execution>
	{

	  /// <summary>
	  /// Only select executions which have the given process definition key. * </summary>
	  ExecutionQuery processDefinitionKey(string processDefinitionKey);

	  /// <summary>
	  /// Only select executions which have process definitions with the given keys. * </summary>
	  ExecutionQuery processDefinitionKeys(Set<string> processDefinitionKeys);

	  /// <summary>
	  /// Only select executions which have the given process definition id. * </summary>
	  ExecutionQuery processDefinitionId(string processDefinitionId);

	  /// <summary>
	  /// Only select executions which have the given process definition category. </summary>
	  ExecutionQuery processDefinitionCategory(string processDefinitionCategory);

	  /// <summary>
	  /// Only select executions which have the given process definition name. </summary>
	  ExecutionQuery processDefinitionName(string processDefinitionName);

	  /// <summary>
	  /// Only select executions which have the given process definition version.
	  /// Particulary useful when used in combination with <seealso cref="#processDefinitionKey(String)"/>
	  /// </summary>
	  ExecutionQuery processDefinitionVersion(int? processDefinitionVersion);

	  /// <summary>
	  /// Only select executions which have the given process instance id. * </summary>
	  ExecutionQuery processInstanceId(string processInstanceId);

	  /// <summary>
	  /// Only executions with the given business key.
	  /// 
	  /// Note that only process instances have a business key and as such, child executions
	  /// will NOT be returned. If you want to return child executions of the process instance with
	  /// the given business key too, use the <seealso cref="#processInstanceBusinessKey(String, boolean)"/> method
	  /// with a boolean value of <i>true</i> instead.
	  /// </summary>
	  ExecutionQuery processInstanceBusinessKey(string processInstanceBusinessKey);

	  /// <summary>
	  /// Only executions with the given business key. 
	  /// Similar to <seealso cref="#processInstanceBusinessKey(String)"/>, but allows to choose
	  /// whether child executions are returned or not.
	  /// </summary>
	  ExecutionQuery processInstanceBusinessKey(string processInstanceBusinessKey, bool includeChildExecutions);

	  /// <summary>
	  /// Only select executions with the given id. * </summary>
	  ExecutionQuery executionId(string executionId);

	  /// <summary>
	  /// Only select executions which contain an activity with the given id. * </summary>
	  ExecutionQuery activityId(string activityId);

	  /// <summary>
	  /// Only select executions which are a direct child-execution of the execution with the given id. * </summary>
	  ExecutionQuery parentId(string parentId);

		/// <summary>
		/// Only select process instances that have the given tenant id.
		/// </summary>
	  ExecutionQuery executionTenantId(string tenantId);

		/// <summary>
		/// Only select process instances with a tenant id like the given one.
		/// </summary>
	  ExecutionQuery executionTenantIdLike(string tenantIdLike);

		/// <summary>
		/// Only select process instances that do not have a tenant id.
		/// </summary>
	  ExecutionQuery executionWithoutTenantId();

	  /// <summary>
	  /// Only select executions which have a local variable with the given value. The type
	  /// of variable is determined based on the value, using types configured in 
	  /// <seealso cref="ProcessEngineConfiguration#getVariableTypes()"/>. 
	  /// Byte-arrays and <seealso cref="Serializable"/> objects (which are not primitive type wrappers)
	  /// are not supported. </summary>
	  /// <param name="name"> name of the variable, cannot be null. </param>
	  ExecutionQuery variableValueEquals(string name, object value);

	  /// <summary>
	  /// Only select executions which have a local string variable with the given value, 
	  /// case insensitive.
	  /// <para>
	  /// This method only works if your database has encoding/collation that supports case-sensitive
	  /// queries. For example, use "collate UTF-8" on MySQL and for MSSQL, select one of the case-sensitive Collations 
	  /// available (<a href="http://msdn.microsoft.com/en-us/library/ms144250(v=sql.105).aspx">MSDN Server Collation Reference</a>).
	  /// </para>
	  /// </summary>
	  /// <param name="name"> name of the variable, cannot be null. </param>
	  /// <param name="value"> value of the variable, cannot be null. </param>
	  ExecutionQuery variableValueEqualsIgnoreCase(string name, string value);

	  /// <summary>
	  /// Only select executions which have at least one local variable with the given value. The type
	  /// of variable is determined based on the value, using types configured in 
	  /// <seealso cref="ProcessEngineConfiguration#getVariableTypes()"/>. 
	  /// Byte-arrays and <seealso cref="Serializable"/> objects (which are not primitive type wrappers)
	  /// are not supported.
	  /// </summary>
	  ExecutionQuery variableValueEquals(object value);

	  /// <summary>
	  /// Only select executions which have a local variable with the given name, but
	  /// with a different value than the passed value.
	  /// Byte-arrays and <seealso cref="Serializable"/> objects (which are not primitive type wrappers)
	  /// are not supported. </summary>
	  /// <param name="name"> name of the variable, cannot be null. </param>
	  ExecutionQuery variableValueNotEquals(string name, object value);

	  /// <summary>
	  /// Only select executions which have a local string variable which is not the given value, 
	  /// case insensitive.
	  /// <para>
	  /// This method only works if your database has encoding/collation that supports case-sensitive
	  /// queries. For example, use "collate UTF-8" on MySQL and for MSSQL, select one of the case-sensitive Collations 
	  /// available (<a href="http://msdn.microsoft.com/en-us/library/ms144250(v=sql.105).aspx">MSDN Server Collation Reference</a>).
	  /// </para>
	  /// </summary>
	  /// <param name="name"> name of the variable, cannot be null. </param>
	  /// <param name="value"> value of the variable, cannot be null. </param>
	  ExecutionQuery variableValueNotEqualsIgnoreCase(string name, string value);


	  /// <summary>
	  /// Only select executions which have a local variable value greater than the passed value.
	  /// Booleans, Byte-arrays and <seealso cref="Serializable"/> objects (which are not primitive type wrappers)
	  /// are not supported. </summary>
	  /// <param name="name"> variable name, cannot be null. </param>
	  /// <param name="value"> variable value, cannot be null. </param>
	  ExecutionQuery variableValueGreaterThan(string name, object value);

	  /// <summary>
	  /// Only select executions which have a local variable value greater than or equal to
	  /// the passed value. Booleans, Byte-arrays and <seealso cref="Serializable"/> objects (which 
	  /// are not primitive type wrappers) are not supported. </summary>
	  /// <param name="name"> variable name, cannot be null. </param>
	  /// <param name="value"> variable value, cannot be null. </param>
	  ExecutionQuery variableValueGreaterThanOrEqual(string name, object value);

	  /// <summary>
	  /// Only select executions which have a local variable value less than the passed value.
	  /// Booleans, Byte-arrays and <seealso cref="Serializable"/> objects (which are not primitive type wrappers)
	  /// are not supported. </summary>
	  /// <param name="name"> variable name, cannot be null. </param>
	  /// <param name="value"> variable value, cannot be null. </param>
	  ExecutionQuery variableValueLessThan(string name, object value);

	  /// <summary>
	  /// Only select executions which have a local variable value less than or equal to the passed value.
	  /// Booleans, Byte-arrays and <seealso cref="Serializable"/> objects (which are not primitive type wrappers)
	  /// are not supported. </summary>
	  /// <param name="name"> variable name, cannot be null. </param>
	  /// <param name="value"> variable value, cannot be null. </param>
	  ExecutionQuery variableValueLessThanOrEqual(string name, object value);

	  /// <summary>
	  /// Only select executions which have a local variable value like the given value.
	  /// This be used on string variables only. </summary>
	  /// <param name="name"> variable name, cannot be null. </param>
	  /// <param name="value"> variable value, cannot be null. The string can include the
	  /// wildcard character '%' to express like-strategy: 
	  /// starts with (string%), ends with (%string) or contains (%string%). </param>
	  ExecutionQuery variableValueLike(string name, string value);

	  /// <summary>
	  /// Only select executions which have a local variable value like the given value (case insensitive).
	  /// This be used on string variables only. </summary>
	  /// <param name="name"> variable name, cannot be null. </param>
	  /// <param name="value"> variable value, cannot be null. The string can include the
	  /// wildcard character '%' to express like-strategy: 
	  /// starts with (string%), ends with (%string) or contains (%string%). </param>
	  ExecutionQuery variableValueLikeIgnoreCase(string name, string value);

	  /// <summary>
	  /// Only select executions which are part of a process that have a variable
	  /// with the given name set to the given value.
	  /// Byte-arrays and <seealso cref="Serializable"/> objects (which are not primitive type wrappers)
	  /// are not supported.
	  /// </summary>
	  ExecutionQuery processVariableValueEquals(string variableName, object variableValue);

	  /// <summary>
	  /// Only select executions which are part of a process that have at least one variable
	  /// with the given value.
	  /// Byte-arrays and <seealso cref="Serializable"/> objects (which are not primitive type wrappers)
	  /// are not supported.
	  /// </summary>
	  ExecutionQuery processVariableValueEquals(object variableValue);

	  /// <summary>
	  /// Only select executions which are part of a process that have a variable  with the given name, but
	  /// with a different value than the passed value.
	  /// Byte-arrays and <seealso cref="Serializable"/> objects (which are not primitive type wrappers)
	  /// are not supported.
	  /// </summary>
	  ExecutionQuery processVariableValueNotEquals(string variableName, object variableValue);

	  /// <summary>
	  /// Only select executions which are part of a process that have a local string variable with 
	  /// the given value, case insensitive.
	  /// <para>
	  /// This method only works if your database has encoding/collation that supports case-sensitive
	  /// queries. For example, use "collate UTF-8" on MySQL and for MSSQL, select one of the case-sensitive Collations 
	  /// available (<a href="http://msdn.microsoft.com/en-us/library/ms144250(v=sql.105).aspx">MSDN Server Collation Reference</a>).
	  /// </para>
	  /// </summary>
	  /// <param name="name"> name of the variable, cannot be null. </param>
	  /// <param name="value"> value of the variable, cannot be null. </param>
	  ExecutionQuery processVariableValueEqualsIgnoreCase(string name, string value);

	  /// <summary>
	  /// Only select executions which are part of a process that have a local string variable which is not 
	  /// the given value, case insensitive.
	  /// <para>
	  /// This method only works if your database has encoding/collation that supports case-sensitive
	  /// queries. For example, use "collate UTF-8" on MySQL and for MSSQL, select one of the case-sensitive Collations 
	  /// available (<a href="http://msdn.microsoft.com/en-us/library/ms144250(v=sql.105).aspx">MSDN Server Collation Reference</a>).
	  /// </para>
	  /// </summary>
	  /// <param name="name"> name of the variable, cannot be null. </param>
	  /// <param name="value"> value of the variable, cannot be null. </param>
	  ExecutionQuery processVariableValueNotEqualsIgnoreCase(string name, string value);

	  /// <summary>
	  /// Only select executions which are part of a process that have at least one variable like the given value.
	  /// Byte-arrays and <seealso cref="Serializable"/> objects (which are not primitive type wrappers) are not supported.
	  /// </summary>
	  ExecutionQuery processVariableValueLike(string name, string value);

	  /// <summary>
	  /// Only select executions which are part of a process that have at least one variable like the given value (case insensitive).
	  /// Byte-arrays and <seealso cref="Serializable"/> objects (which are not primitive type wrappers) are not supported.
	  /// </summary>
	  ExecutionQuery processVariableValueLikeIgnoreCase(string name, string value);

	  // event subscriptions //////////////////////////////////////////////////

	  /// <seealso cref= #signalEventSubscriptionName(String) </seealso>
	  [Obsolete]
	  ExecutionQuery signalEventSubscription(string signalName);

	  /// <summary>
	  /// Only select executions which have a signal event subscription 
	  /// for the given signal name.
	  /// 
	  /// (The signalName is specified using the 'name' attribute of the signal element 
	  /// in the BPMN 2.0 XML.)
	  /// </summary>
	  /// <param name="signalName"> the name of the signal the execution has subscribed to </param>
	  ExecutionQuery signalEventSubscriptionName(string signalName);

	  /// <summary>
	  /// Only select executions which have a message event subscription 
	  /// for the given messageName. 
	  /// 
	  /// (The messageName is specified using the 'name' attribute of the message element 
	  /// in the BPMN 2.0 XML.)
	  /// </summary>
	  /// <param name="messageName"> the name of the message the execution has subscribed to </param>
	  ExecutionQuery messageEventSubscriptionName(string messageName);

	  /// <summary>
	  /// Localize execution name and description to specified locale.
	  /// </summary>
	  ExecutionQuery locale(string locale);

	  /// <summary>
	  /// Instruct localization to fallback to more general locales including the default locale of the JVM if the specified locale is not found. 
	  /// </summary>
	  ExecutionQuery withLocalizationFallback();

	  //ordering //////////////////////////////////////////////////////////////

	  /// <summary>
	  /// Order by id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  ExecutionQuery orderByProcessInstanceId();

	  /// <summary>
	  /// Order by process definition key (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  ExecutionQuery orderByProcessDefinitionKey();

	  /// <summary>
	  /// Order by process definition id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  ExecutionQuery orderByProcessDefinitionId();

		/// <summary>
		/// Order by tenant id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  ExecutionQuery orderByTenantId();

	}

}