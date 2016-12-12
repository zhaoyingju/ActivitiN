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

using System;

namespace org.activiti.bpmn.model{


/**
 * Element for defining an Event listener to hook in to the global Event-mechanism.
 * 
 * //@author Frederik Heremans
 */
public class EventListener : BaseElement {

  protected String events;
  protected String implementationType;
  protected String implementation;
  protected String entityType;

  public String getEvents() {
    return events;
  }
  public void setEvents(String events) {
    this.events = events;
  }
  public String getImplementationType() {
    return implementationType;
  }
  public void setImplementationType(String implementationType) {
    this.implementationType = implementationType;
  }
  public String getImplementation() {
    return implementation;
  }
  public void setImplementation(String implementation) {
    this.implementation = implementation;
  }
  public void setEntityType(String entityType) {
	  this.entityType = entityType;
  }
  public String getEntityType() {
	  return entityType;
  }

  public override Object clone()
  {
    EventListener clone = new EventListener();
    clone.setValues(this);
    return clone;
  }
  
  public void setValues(EventListener otherListener) {
    setEvents(otherListener.getEvents());
    setImplementation(otherListener.getImplementation());
    setImplementationType(otherListener.getImplementationType());
    setEntityType(otherListener.getEntityType());
  }
}
}