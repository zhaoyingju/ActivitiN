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

namespace org.activiti.engine.impl.pvm.process
{


	/// <summary>
	/// Marks implementing class as having DI-information bounded by a rectangle
	/// at a certain location.
	/// 
	/// @author Frederik Heremans
	/// </summary>
	public interface HasDIBounds
	{

	  int Width {get;set;}
	  int Height {get;set;}
	  int X {get;set;}
	  int Y {get;set;}

	}

}