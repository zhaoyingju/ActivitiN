using System;
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


	using HasRevision = org.activiti.engine.impl.db.HasRevision;
	using PersistentObject = org.activiti.engine.impl.db.PersistentObject;
	using Attachment = org.activiti.engine.task.Attachment;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	[Serializable]
	public class AttachmentEntity : Attachment, PersistentObject, HasRevision
	{

	  private const long serialVersionUID = 1L;

	  protected internal string id;
	  protected internal int revision;
	  protected internal string name;
	  protected internal string description;
	  protected internal string type;
	  protected internal string taskId;
	  protected internal string processInstanceId;
	  protected internal string url;
	  protected internal string contentId;
	  protected internal ByteArrayEntity content;
	  protected internal string userId;
	  protected internal DateTime time;

	  public AttachmentEntity()
	  {
	  }

	  public virtual object PersistentState
	  {
		  get
		  {
			IDictionary<string, object> persistentState = new Dictionary<string, object>();
			persistentState["name"] = name;
			persistentState["description"] = description;
			return persistentState;
		  }
	  }

	  public virtual int RevisionNext
	  {
		  get
		  {
			return revision + 1;
		  }
	  }


	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
		  set
		  {
			this.id = value;
		  }
	  }




	  public virtual int Revision
	  {
		  get
		  {
			return revision;
		  }
		  set
		  {
			this.revision = value;
		  }
	  }




	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
		  set
		  {
			this.name = value;
		  }
	  }




	  public virtual string Description
	  {
		  get
		  {
			return description;
		  }
		  set
		  {
			this.description = value;
		  }
	  }




	  public virtual string Type
	  {
		  get
		  {
			return type;
		  }
		  set
		  {
			this.type = value;
		  }
	  }




	  public virtual string TaskId
	  {
		  get
		  {
			return taskId;
		  }
		  set
		  {
			this.taskId = value;
		  }
	  }




	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return processInstanceId;
		  }
		  set
		  {
			this.processInstanceId = value;
		  }
	  }




	  public virtual string Url
	  {
		  get
		  {
			return url;
		  }
		  set
		  {
			this.url = value;
		  }
	  }




	  public virtual string ContentId
	  {
		  get
		  {
			return contentId;
		  }
		  set
		  {
			this.contentId = value;
		  }
	  }


	  public virtual ByteArrayEntity getContent()
	  {
		return content;
	  }

	  public virtual void setContent(ByteArrayEntity content)
	  {
		this.content = content;
	  }

	  public virtual string UserId
	  {
		  set
		  {
			this.userId = value;
		  }
		  get
		  {
			return userId;
		  }
	  }


	  public virtual DateTime Time
	  {
		  get
		  {
			return time;
		  }
		  set
		  {
			this.time = value;
		  }
	  }


	}

}