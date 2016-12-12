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

namespace org.activiti.engine.impl.test
{


	using AssertionFailedError = junit.framework.AssertionFailedError;
	using TestCase = junit.framework.TestCase;

	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public abstract class PvmTestCase : TestCase
	{

	  protected internal const string EMPTY_LINE = "\n";

	  protected internal static Logger log = LoggerFactory.getLogger(typeof(PvmTestCase));

	  protected internal bool isEmptyLinesEnabled = true;


	  /// <summary>
	  /// Asserts if the provided text is part of some text.
	  /// </summary>
	  public virtual void assertTextPresent(string expected, string actual)
	  {
		if ((actual == null) || (!actual.Contains(expected)))
		{
		  throw new AssertionFailedError("expected presence of [" + expected + "], but was [" + actual + "]");
		}
	  }

	  /// <summary>
	  /// Asserts if the provided text is part of some text, ignoring any uppercase characters
	  /// </summary>
	  public virtual void assertTextPresentIgnoreCase(string expected, string actual)
	  {
		assertTextPresent(expected.ToLower(), actual.ToLower());
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void runTest() throws Throwable
	  protected internal override void runTest()
	  {
		if (log.DebugEnabled)
		{
		  if (isEmptyLinesEnabled)
		  {
			log.debug(EMPTY_LINE);
		  }
		  log.debug("#### START {}.{} ###########################################################", this.GetType().Name, Name);
		}

		try
		{

		  base.runTest();

		}
		catch (AssertionFailedError e)
		{
		  log.error(EMPTY_LINE);
		  log.error("ASSERTION FAILED: {}", e, e);
		  throw e;

		}
		catch (Exception e)
		{
		  log.error(EMPTY_LINE);
		  log.error("EXCEPTION: {}", e, e);
		  throw e;

		}
		finally
		{
		  log.debug("#### END {}.{} #############################################################", this.GetType().Name, Name);
		}
	  }

	}

}