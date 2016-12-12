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
namespace org.activiti.engine.impl.pvm.runtime
{



	/// <summary>
	/// @author Tom Baeyens
	/// @author Daniel Meyer
	/// </summary>
	public interface AtomicOperation
	{

	  void execute(InterpretableExecution execution);

	  bool isAsync(InterpretableExecution execution);
	}

	public static class AtomicOperation_Fields
	{
	  public static readonly AtomicOperation PROCESS_START = new AtomicOperationProcessStart();
	  public static readonly AtomicOperation PROCESS_START_INITIAL = new AtomicOperationProcessStartInitial();
	  public static readonly AtomicOperation PROCESS_END = new AtomicOperationProcessEnd();
	  public static readonly AtomicOperation ACTIVITY_START = new AtomicOperationActivityStart();
	  public static readonly AtomicOperation ACTIVITY_EXECUTE = new AtomicOperationActivityExecute();
	  public static readonly AtomicOperation ACTIVITY_END = new AtomicOperationActivityEnd();
	  public static readonly AtomicOperation TRANSITION_NOTIFY_LISTENER_END = new AtomicOperationTransitionNotifyListenerEnd();
	  public static readonly AtomicOperation TRANSITION_DESTROY_SCOPE = new AtomicOperationTransitionDestroyScope();
	  public static readonly AtomicOperation TRANSITION_NOTIFY_LISTENER_TAKE = new AtomicOperationTransitionNotifyListenerTake();
	  public static readonly AtomicOperation TRANSITION_CREATE_SCOPE = new AtomicOperationTransitionCreateScope();
	  public static readonly AtomicOperation TRANSITION_NOTIFY_LISTENER_START = new AtomicOperationTransitionNotifyListenerStart();
	  public static readonly AtomicOperation DELETE_CASCADE = new AtomicOperationDeleteCascade();
	  public static readonly AtomicOperation DELETE_CASCADE_FIRE_ACTIVITY_END = new AtomicOperationDeleteCascadeFireActivityEnd();
	}

}