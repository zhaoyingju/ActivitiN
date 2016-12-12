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
namespace org.activiti.engine.impl.bpmn.webservice
{


	/// <summary>
	/// Represents an implementation of a <seealso cref="Operation"/>
	/// 
	/// @author Esteban Robles Luna
	/// </summary>
	public interface OperationImplementation
	{

	  /// <returns> the id of this implementation </returns>
	  string Id {get;}

	  /// <returns> the name of this implementation </returns>
	  string Name {get;}

	  /// <summary>
	  /// Sends the message on behalf of operation
	  /// </summary>
	  /// <param name="message"> the message to be sent </param>
	  /// <param name="operation"> the operation that is interested on sending the message </param>
	  /// <param name="overridenEndpointAddresses"> a not null map of overriden enpoint addresses. The key is the endpoint qualified name. </param>
	  /// <returns> the resulting message </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: MessageInstance sendFor(MessageInstance message, Operation operation, final java.util.concurrent.ConcurrentMap<javax.xml.namespace.QName, java.net.URL> overridenEndpointAddresses) throws Exception;
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
	  MessageInstance sendFor(MessageInstance message, Operation operation, ConcurrentMap<QName, URL> overridenEndpointAddresses);
	}

}