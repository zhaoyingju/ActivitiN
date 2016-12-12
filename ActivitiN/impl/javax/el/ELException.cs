using System;

/*
 * Based on JUEL 2.2.1 code, 2006-2009 Odysseus Software GmbH
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace org.activiti.engine.impl.javax.el
{

	/// <summary>
	/// Represents any of the exception conditions that can arise during expression evaluation.
	/// </summary>
	public class ELException : Exception
	{
		private const long serialVersionUID = 1L;

		/// <summary>
		/// Creates an ELException with no detail message.
		/// </summary>
		public ELException() : base()
		{
		}

		/// <summary>
		/// Creates an ELException with the provided detail message.
		/// </summary>
		/// <param name="message">
		///            the detail message </param>
		public ELException(string message) : base(message)
		{
		}

		/// <summary>
		/// Creates an ELException with the given cause.
		/// </summary>
		/// <param name="cause">
		///            the originating cause of this exception </param>
		public ELException(Exception cause) : base(cause)
		{
		}

		/// <summary>
		/// Creates an ELException with the given detail message and root cause.
		/// </summary>
		/// <param name="message">
		///            the detail message </param>
		/// <param name="cause">
		///            the originating cause of this exception </param>
		public ELException(string message, Exception cause) : base(message, cause)
		{
		}
	}

}