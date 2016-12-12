using System;
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

namespace org.activiti.engine.impl.bpmn.behavior
{



	using MailServerInfo = org.activiti.engine.cfg.MailServerInfo;
	using DelegateExecution = org.activiti.engine.@delegate.DelegateExecution;
	using Expression = org.activiti.engine.@delegate.Expression;
	using ProcessEngineConfigurationImpl = org.activiti.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Context = org.activiti.engine.impl.context.Context;
	using ActivityExecution = org.activiti.engine.impl.pvm.@delegate.ActivityExecution;
	using Email = org.apache.commons.mail.Email;
	using EmailException = org.apache.commons.mail.EmailException;
	using HtmlEmail = org.apache.commons.mail.HtmlEmail;
	using MultiPartEmail = org.apache.commons.mail.MultiPartEmail;
	using SimpleEmail = org.apache.commons.mail.SimpleEmail;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;

	/// <summary>
	/// @author Joram Barrez
	/// @author Frederik Heremans
	/// @author Tim Stephenson
	/// </summary>
	public class MailActivityBehavior : AbstractBpmnActivityBehavior
	{

	  private const long serialVersionUID = 1L;

	  private static readonly Logger LOG = LoggerFactory.getLogger(typeof(MailActivityBehavior));

	  private static readonly Type[] ALLOWED_ATT_TYPES = new Type[]{typeof(File), typeof(File[]), typeof(string), typeof(string[]), typeof(DataSource), typeof(DataSource[])};

	  protected internal Expression to;
	  protected internal Expression from;
	  protected internal Expression cc;
	  protected internal Expression bcc;
	  protected internal Expression subject;
	  protected internal Expression text;
	  protected internal Expression textVar;
	  protected internal Expression html;
	  protected internal Expression htmlVar;
	  protected internal Expression charset;
	  protected internal Expression ignoreException;
	  protected internal Expression exceptionVariableName;
	  protected internal Expression attachments;

	  public override void execute(ActivityExecution execution)
	  {

		bool doIgnoreException = Convert.ToBoolean(getStringFromField(ignoreException, execution));
		string exceptionVariable = getStringFromField(exceptionVariableName, execution);
		Email email = null;
		try
		{
		  string toStr = getStringFromField(to, execution);
		  string fromStr = getStringFromField(from, execution);
		  string ccStr = getStringFromField(cc, execution);
		  string bccStr = getStringFromField(bcc, execution);
		  string subjectStr = getStringFromField(subject, execution);
		  string textStr = textVar == null ? getStringFromField(text, execution) : getStringFromField(getExpression(execution, textVar), execution);
		  string htmlStr = htmlVar == null ? getStringFromField(html, execution) : getStringFromField(getExpression(execution, htmlVar), execution);
		  string charSetStr = getStringFromField(charset, execution);
		  IList<File> files = new LinkedList<File>();
		  IList<DataSource> dataSources = new LinkedList<DataSource>();
		  getFilesFromFields(attachments, execution, files, dataSources);

		  email = createEmail(textStr, htmlStr, attachmentsExist(files, dataSources));
		  addTo(email, toStr);
		  setFrom(email, fromStr, execution.TenantId);
		  addCc(email, ccStr);
		  addBcc(email, bccStr);
		  setSubject(email, subjectStr);
		  setMailServerProperties(email, execution.TenantId);
		  setCharset(email, charSetStr);
		  attach(email, files, dataSources);

		  email.send();

		}
		catch (ActivitiException e)
		{
		  handleException(execution, e.Message, e, doIgnoreException, exceptionVariable);
		}
		catch (EmailException e)
		{
		  handleException(execution, "Could not send e-mail in execution " + execution.Id, e, doIgnoreException, exceptionVariable);
		}

		leave(execution);
	  }

	  private bool attachmentsExist(IList<File> files, IList<DataSource> dataSources)
	  {
		return !((files == null || files.Count == 0) && (dataSources == null || dataSources.Count == 0));
	  }

	  protected internal virtual Email createEmail(string text, string html, bool attachmentsExist)
	  {
		if (html != null)
		{
		  return createHtmlEmail(text, html);
		}
		else if (text != null)
		{
		  if (!attachmentsExist)
		  {
			return createTextOnlyEmail(text);
		  }
		  else
		  {
			return createMultiPartEmail(text);
		  }
		}
		else
		{
		  throw new ActivitiIllegalArgumentException("'html' or 'text' is required to be defined when using the mail activity");
		}
	  }

	  protected internal virtual HtmlEmail createHtmlEmail(string text, string html)
	  {
		HtmlEmail email = new HtmlEmail();
		try
		{
		  email.HtmlMsg = html;
		  if (text != null) // for email clients that don't support html
		  {
			email.TextMsg = text;
		  }
		  return email;
		}
		catch (EmailException e)
		{
		  throw new ActivitiException("Could not create HTML email", e);
		}
	  }

	  protected internal virtual SimpleEmail createTextOnlyEmail(string text)
	  {
		SimpleEmail email = new SimpleEmail();
		try
		{
		  email.Msg = text;
		  return email;
		}
		catch (EmailException e)
		{
		  throw new ActivitiException("Could not create text-only email", e);
		}
	  }

	  protected internal virtual MultiPartEmail createMultiPartEmail(string text)
	  {
		MultiPartEmail email = new MultiPartEmail();
		try
		{
		  email.Msg = text;
		  return email;
		}
		catch (EmailException e)
		{
		  throw new ActivitiException("Could not create text-only email", e);
		}
	  }

	  protected internal virtual void addTo(Email email, string to)
	  {
		string[] tos = splitAndTrim(to);
		if (tos != null)
		{
		  foreach (string t in tos)
		  {
			try
			{
			  email.addTo(t);
			}
			catch (EmailException e)
			{
			  throw new ActivitiException("Could not add " + t + " as recipient", e);
			}
		  }
		}
		else
		{
		  throw new ActivitiException("No recipient could be found for sending email");
		}
	  }

	  protected internal virtual void setFrom(Email email, string from, string tenantId)
	  {
		string fromAddress = null;

		if (from != null)
		{
		  fromAddress = from;
		} // use default configured from address in process engine config
		else
		{
		  if (tenantId != null && tenantId.Length > 0)
		  {
			IDictionary<string, MailServerInfo> mailServers = Context.ProcessEngineConfiguration.MailServers;
			if (mailServers != null && mailServers.ContainsKey(tenantId))
			{
			  MailServerInfo mailServerInfo = mailServers[tenantId];
			  fromAddress = mailServerInfo.MailServerDefaultFrom;
			}
		  }

		  if (fromAddress == null)
		  {
			fromAddress = Context.ProcessEngineConfiguration.MailServerDefaultFrom;
		  }
		}

		try
		{
		  email.From = fromAddress;
		}
		catch (EmailException e)
		{
		  throw new ActivitiException("Could not set " + from + " as from address in email", e);
		}
	  }

	  protected internal virtual void addCc(Email email, string cc)
	  {
		string[] ccs = splitAndTrim(cc);
		if (ccs != null)
		{
		  foreach (string c in ccs)
		  {
			try
			{
			  email.addCc(c);
			}
			catch (EmailException e)
			{
			  throw new ActivitiException("Could not add " + c + " as cc recipient", e);
			}
		  }
		}
	  }

	  protected internal virtual void addBcc(Email email, string bcc)
	  {
		string[] bccs = splitAndTrim(bcc);
		if (bccs != null)
		{
		  foreach (string b in bccs)
		  {
			try
			{
			  email.addBcc(b);
			}
			catch (EmailException e)
			{
			  throw new ActivitiException("Could not add " + b + " as bcc recipient", e);
			}
		  }
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void attach(org.apache.commons.mail.Email email, java.util.List<java.io.File> files, java.util.List<javax.activation.DataSource> dataSources) throws org.apache.commons.mail.EmailException
	  protected internal virtual void attach(Email email, IList<File> files, IList<DataSource> dataSources)
	  {
		if (!(email is MultiPartEmail && attachmentsExist(files, dataSources)))
		{
		  return;
		}
		MultiPartEmail mpEmail = (MultiPartEmail) email;
		foreach (File file in files)
		{
		  mpEmail.attach(file);
		}
		foreach (DataSource ds in dataSources)
		{
		  if (ds != null)
		  {
			mpEmail.attach(ds, ds.Name, null);
		  }
		}
	  }

	  protected internal virtual void setSubject(Email email, string subject)
	  {
		email.Subject = subject != null ? subject : "";
	  }

	  protected internal virtual void setMailServerProperties(Email email, string tenantId)
	  {
		ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;

		bool isMailServerSet = false;
		if (tenantId != null && tenantId.Length > 0)
		{
		  if (processEngineConfiguration.getMailSessionJndi(tenantId) != null)
		  {
			setEmailSession(email, processEngineConfiguration.getMailSessionJndi(tenantId));
			isMailServerSet = true;

		  }
		  else if (processEngineConfiguration.getMailServer(tenantId) != null)
		  {
			MailServerInfo mailServerInfo = processEngineConfiguration.getMailServer(tenantId);
			string host = mailServerInfo.MailServerHost;
			if (host == null)
			{
			  throw new ActivitiException("Could not send email: no SMTP host is configured for tenantId " + tenantId);
			}
			email.HostName = host;

			email.SmtpPort = mailServerInfo.MailServerPort;

			email.SSLOnConnect = mailServerInfo.MailServerUseSSL;
			email.StartTLSEnabled = mailServerInfo.MailServerUseTLS;

			string user = mailServerInfo.MailServerUsername;
			string password = mailServerInfo.MailServerPassword;
			if (user != null && password != null)
			{
			  email.setAuthentication(user, password);
			}

			isMailServerSet = true;
		  }
		}

		if (!isMailServerSet)
		{
		  string mailSessionJndi = processEngineConfiguration.MailSessionJndi;
		  if (mailSessionJndi != null)
		  {
			setEmailSession(email, mailSessionJndi);

		  }
		  else
		  {
			string host = processEngineConfiguration.MailServerHost;
			if (host == null)
			{
			  throw new ActivitiException("Could not send email: no SMTP host is configured");
			}
			email.HostName = host;

			int port = processEngineConfiguration.MailServerPort;
			email.SmtpPort = port;

			email.SSLOnConnect = processEngineConfiguration.MailServerUseSSL;
			email.StartTLSEnabled = processEngineConfiguration.MailServerUseTLS;

			string user = processEngineConfiguration.MailServerUsername;
			string password = processEngineConfiguration.MailServerPassword;
			if (user != null && password != null)
			{
			  email.setAuthentication(user, password);
			}
		  }
		}
	  }

	  protected internal virtual void setEmailSession(Email email, string mailSessionJndi)
	  {
		try
		{
		  email.MailSessionFromJNDI = mailSessionJndi;
		}
		catch (NamingException e)
		{
		  throw new ActivitiException("Could not send email: Incorrect JNDI configuration", e);
		}
	  }

	  protected internal virtual void setCharset(Email email, string charSetStr)
	  {
		if (charset != null)
		{
		  email.Charset = charSetStr;
		}
	  }

	  protected internal virtual string[] splitAndTrim(string str)
	  {
		if (str != null)
		{
		  string[] splittedStrings = str.Split(",", true);
		  for (int i = 0; i < splittedStrings.Length; i++)
		  {
			splittedStrings[i] = splittedStrings[i].Trim();
		  }
		  return splittedStrings;
		}
		return null;
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

	  private void getFilesFromFields(Expression expression, DelegateExecution execution, IList<File> files, IList<DataSource> dataSources)
	  {
		object value = checkAllowedTypes(expression, execution);
		if (value != null)
		{
		  if (value is File)
		  {
			files.Add((File) value);
		  }
		  else if (value is string)
		  {
			files.Add(new File((string) value));
		  }
		  else if (value is File[])
		  {
			Collections.addAll(files, (File[]) value);
		  }
		  else if (value is string[])
		  {
			string[] paths = (string[]) value;
			foreach (string path in paths)
			{
			  files.Add(new File(path));
			}
		  }
		  else if (value is DataSource)
		  {
			dataSources.Add((DataSource) value);
		  }
		  else if (value is DataSource[])
		  {
			foreach (DataSource ds in (DataSource[]) value)
			{
			  if (ds != null)
			  {
				dataSources.Add(ds);
			  }
			}
		  }
		}
		for (IEnumerator<File> it = files.GetEnumerator(); it.MoveNext();)
		{
		  File file = it.Current;
		  if (!fileExists(file))
		  {
			it.remove();
		  }
		}
	  }

	  private object checkAllowedTypes(Expression expression, DelegateExecution execution)
	  {
		if (expression == null)
		{
		  return null;
		}
		object value = expression.getValue(execution);
		if (value == null)
		{
		  return null;
		}
		foreach (Type allowedType in ALLOWED_ATT_TYPES)
		{
		  if (allowedType.IsInstanceOfType(value))
		  {
			return value;
		  }
		}
		throw new ActivitiException("Invalid attachment type: " + value.GetType());
	  }

	  protected internal virtual bool fileExists(File file)
	  {
		return file != null && file.exists() && file.File && file.canRead();
	  }

	  protected internal virtual Expression getExpression(ActivityExecution execution, Expression @var)
	  {
		string variable = (string) execution.getVariable(@var.ExpressionText);
		return Context.ProcessEngineConfiguration.ExpressionManager.createExpression(variable);
	  }

	  protected internal virtual void handleException(ActivityExecution execution, string msg, Exception e, bool doIgnoreException, string exceptionVariable)
	  {
		if (doIgnoreException)
		{
		  LOG.info("Ignoring email send error: " + msg, e);
		  if (exceptionVariable != null && exceptionVariable.Length > 0)
		  {
			execution.setVariable(exceptionVariable, msg);
		  }
		}
		else
		{
		  if (e is ActivitiException)
		  {
			throw (ActivitiException) e;
		  }
		  else
		  {
			throw new ActivitiException(msg, e);
		  }
		}
	  }
	}

}