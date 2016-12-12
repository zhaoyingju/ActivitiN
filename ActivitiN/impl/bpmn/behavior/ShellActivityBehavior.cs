using System;
using System.Collections.Generic;

namespace org.activiti.engine.impl.bpmn.behavior
{


	using DelegateExecution = org.activiti.engine.@delegate.DelegateExecution;
	using Expression = org.activiti.engine.@delegate.Expression;
	using ActivityExecution = org.activiti.engine.impl.pvm.@delegate.ActivityExecution;

	public class ShellActivityBehavior : AbstractBpmnActivityBehavior
	{

	  protected internal Expression command;
	  protected internal Expression wait;
	  protected internal Expression arg1;
	  protected internal Expression arg2;
	  protected internal Expression arg3;
	  protected internal Expression arg4;
	  protected internal Expression arg5;
	  protected internal Expression outputVariable;
	  protected internal Expression errorCodeVariable;
	  protected internal Expression redirectError;
	  protected internal Expression cleanEnv;
	  protected internal Expression directory;

	  internal string commandStr;
	  internal string arg1Str;
	  internal string arg2Str;
	  internal string arg3Str;
	  internal string arg4Str;
	  internal string arg5Str;
	  internal string waitStr;
	  internal string resultVariableStr;
	  internal string errorCodeVariableStr;
	  internal bool? waitFlag;
	  internal bool? redirectErrorFlag;
	  internal bool? cleanEnvBoolan;
	  internal string directoryStr;

	  private void readFields(ActivityExecution execution)
	  {
		commandStr = getStringFromField(command, execution);
		arg1Str = getStringFromField(arg1, execution);
		arg2Str = getStringFromField(arg2, execution);
		arg3Str = getStringFromField(arg3, execution);
		arg4Str = getStringFromField(arg4, execution);
		arg5Str = getStringFromField(arg5, execution);
		waitStr = getStringFromField(wait, execution);
		resultVariableStr = getStringFromField(outputVariable, execution);
		errorCodeVariableStr = getStringFromField(errorCodeVariable, execution);

		string redirectErrorStr = getStringFromField(redirectError, execution);
		string cleanEnvStr = getStringFromField(cleanEnv, execution);

		waitFlag = waitStr == null || waitStr.Equals("true");
		redirectErrorFlag = redirectErrorStr != null && redirectErrorStr.Equals("true");
		cleanEnvBoolan = cleanEnvStr != null && cleanEnvStr.Equals("true");
		directoryStr = getStringFromField(directory, execution);

	  }

	  public override void execute(ActivityExecution execution)
	  {

		readFields(execution);

		IList<string> argList = new List<string>();
		argList.Add(commandStr);

		if (arg1Str != null)
		{
		  argList.Add(arg1Str);
		}
		if (arg2Str != null)
		{
		  argList.Add(arg2Str);
		}
		if (arg3Str != null)
		{
		  argList.Add(arg3Str);
		}
		if (arg4Str != null)
		{
		  argList.Add(arg4Str);
		}
		if (arg5Str != null)
		{
		  argList.Add(arg5Str);
		}

		ProcessBuilder processBuilder = new ProcessBuilder(argList);

		try
		{
		  processBuilder.redirectErrorStream(redirectErrorFlag);
		  if (cleanEnvBoolan)
		  {
			IDictionary<string, string> env = processBuilder.environment();
			env.Clear();
		  }
		  if (directoryStr != null && directoryStr.Length > 0)
		  {
			processBuilder.directory(new File(directoryStr));
		  }

		  Process process = processBuilder.start();

		  if (waitFlag)
		  {
			int errorCode = process.waitFor();

			if (resultVariableStr != null)
			{
			  string result = convertStreamToStr(process.InputStream);
			  execution.setVariable(resultVariableStr, result);
			}

			if (errorCodeVariableStr != null)
			{
			  execution.setVariable(errorCodeVariableStr, Convert.ToString(errorCode));

			}

		  }
		}
		catch (Exception e)
		{
		  throw new ActivitiException("Could not execute shell command ", e);
		}

		leave(execution);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static String convertStreamToStr(java.io.InputStream is) throws java.io.IOException
	  public static string convertStreamToStr(InputStream @is)
	  {

		if (@is != null)
		{
		  Writer writer = new StringWriter();

		  char[] buffer = new char[1024];
		  try
		  {
			Reader reader = new BufferedReader(new InputStreamReader(@is, "UTF-8"));
			int n;
			while ((n = reader.read(buffer)) != -1)
			{
			  writer.write(buffer, 0, n);
			}
		  }
		  finally
		  {
			@is.close();
		  }
		  return writer.ToString();
		}
		else
		{
		  return "";
		}
	  }

	  protected internal virtual string getStringFromField(Expression expression, DelegateExecution execution)
	  {
		if (expression != null)
		{
		  object value = expression.getValue(execution);
		  if (value != null)
		  {
			return value.ToString();
		  }
		}
		return null;
	  }

	}

}