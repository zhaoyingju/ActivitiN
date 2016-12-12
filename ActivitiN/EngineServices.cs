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
namespace org.activiti.engine
{


	/// <summary>
	/// Interface implemented by all classes that expose the Activiti services.
	/// 
	/// @author Joram Barrez 
	/// </summary>
	public interface EngineServices
	{

	  RepositoryService RepositoryService {get;}

	  RuntimeService RuntimeService {get;}

	  FormService FormService {get;}

	  TaskService TaskService {get;}

	  HistoryService HistoryService {get;}

	  IdentityService IdentityService {get;}

	  ManagementService ManagementService {get;}

	  DynamicBpmnService DynamicBpmnService {get;}

	  ProcessEngineConfiguration ProcessEngineConfiguration {get;}
	}

}