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
namespace org.activiti.engine.runtime
{

    /// <summary>
    /// 帮助启动新的ProcessInstance。
    /// 在调用 Start之前应该设置processDefinitionId或processDefinitionKey。以启动流程实例。   
    /// </summary>
    public interface ProcessInstanceBuilder
    {

        /// <summary>
        /// Set the id of the process definition * </summary>
        ProcessInstanceBuilder processDefinitionId(string processDefinitionId);

        /// <summary>
        /// Set the key of the process definition, latest version of the process
        /// definition with the given key. If processDefinitionId was set this will
        /// be ignored *
        /// </summary>
        ProcessInstanceBuilder processDefinitionKey(string processDefinitionKey);

        /// <summary>
        /// Set the name of process instance * </summary>
        ProcessInstanceBuilder processInstanceName(string processInstanceName);

        /// <summary>
        /// Set the businessKey of process instance * </summary>
        ProcessInstanceBuilder businessKey(string businessKey);

        /// <summary>
        /// Set the tenantId of process instance * </summary>
        ProcessInstanceBuilder tenantId(string tenantId);

        /// <summary>
        /// Add a variable to the process instance * </summary>
        ProcessInstanceBuilder addVariable(string variableName, object value);

        /// <summary>
        /// Start the process instance 
        /// </summary>
        /// <exception cref="ActivitiIllegalArgumentException">
        /// if processDefinitionKey and processDefinitionId are null </exception>
        /// <exception cref="ActivitiObjectNotFoundException">
        /// when no process definition is deployed with the given processDefinitionKey or processDefinitionId
        /// * </exception>
        ProcessInstance start();
    }

}