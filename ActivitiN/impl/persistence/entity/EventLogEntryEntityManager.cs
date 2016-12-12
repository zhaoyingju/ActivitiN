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

namespace org.activiti.engine.impl.persistence.entity
{


	using EventLogEntry = org.activiti.engine.@event.EventLogEntry;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class EventLogEntryEntityManager : AbstractManager
	{

	  public virtual void insert(EventLogEntryEntity eventLogEntryEntity)
	  {
		  DbSqlSession.insert(eventLogEntryEntity);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.activiti.engine.event.EventLogEntry> findAllEventLogEntries()
	  public virtual IList<EventLogEntry> findAllEventLogEntries()
	  {
		  return DbSqlSession.selectList("selectAllEventLogEntries");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.activiti.engine.event.EventLogEntry> findEventLogEntries(long startLogNr, long pageSize)
	  public virtual IList<EventLogEntry> findEventLogEntries(long startLogNr, long pageSize)
	  {
		  IDictionary<string, object> @params = new Dictionary<string, object>(2);
		  @params["startLogNr"] = startLogNr;
		  if (pageSize > 0)
		  {
			  @params["endLogNr"] = startLogNr + pageSize + 1;
		  }
		  return DbSqlSession.selectList("selectEventLogEntries", @params);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.activiti.engine.event.EventLogEntry> findEventLogEntriesByProcessInstanceId(String processInstanceId)
	  public virtual IList<EventLogEntry> findEventLogEntriesByProcessInstanceId(string processInstanceId)
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>(2);
		@params["processInstanceId"] = processInstanceId;
		return DbSqlSession.selectList("selectEventLogEntriesByProcessInstanceId", @params);
	  }

	  public virtual void deleteEventLogEntry(long logNr)
	  {
		  DbSqlSession.SqlSession.delete("deleteEventLogEntry", logNr);
	  }

	}

}