using System;

namespace org.activiti.engine.impl.util.json
{

	/// <summary>
	/// The JSONException is thrown by the JSON.org classes when things are amiss.
	/// @author JSON.org
	/// @version 2008-09-18
	/// </summary>
	public class JSONException : Exception
	{
		/// 
		private const long serialVersionUID = 0;
		private Exception cause;

		/// <summary>
		/// Constructs a JSONException with an explanatory message. </summary>
		/// <param name="message"> Detail about the reason for the exception. </param>
		public JSONException(string message) : base(message)
		{
		}

		public JSONException(Exception t) : base(t.Message)
		{
			this.cause = t;
		}

		public virtual Exception Cause
		{
			get
			{
				return this.cause;
			}
		}
	}

}