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


	using Picture = org.activiti.engine.identity.Picture;
	using User = org.activiti.engine.identity.User;
	using Context = org.activiti.engine.impl.context.Context;
	using HasRevision = org.activiti.engine.impl.db.HasRevision;
	using PersistentObject = org.activiti.engine.impl.db.PersistentObject;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Arkadiy Gornovoy
	/// </summary>
	[Serializable]
	public class UserEntity : User, PersistentObject, HasRevision
	{

	  private const long serialVersionUID = 1L;

	  protected internal string id;
	  protected internal int revision;
	  protected internal string firstName;
	  protected internal string lastName;
	  protected internal string email;
	  protected internal string password;

	  protected internal readonly ByteArrayRef pictureByteArrayRef = new ByteArrayRef();

	  public UserEntity()
	  {
	  }

	  public UserEntity(string id)
	  {
		this.id = id;
	  }

	  public virtual void delete()
	  {
		Context.CommandContext.DbSqlSession.delete(this);

		deletePicture();
	  }

	  public virtual object PersistentState
	  {
		  get
		  {
			IDictionary<string, object> persistentState = new Dictionary<string, object>();
			persistentState["firstName"] = firstName;
			persistentState["lastName"] = lastName;
			persistentState["email"] = email;
			persistentState["password"] = password;
			persistentState["pictureByteArrayId"] = pictureByteArrayRef.Id;
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

	  public virtual Picture Picture
	  {
		  get
		  {
			if (pictureByteArrayRef.Id != null)
			{
			  return new Picture(pictureByteArrayRef.Bytes, pictureByteArrayRef.Name);
			}
			return null;
		  }
		  set
		  {
			if (value != null)
			{
			  savePicture(value);
			}
			else
			{
			  deletePicture();
			}
		  }
	  }


	  protected internal virtual void savePicture(Picture picture)
	  {
		pictureByteArrayRef.setValue(picture.MimeType, picture.Bytes);
	  }

	  protected internal virtual void deletePicture()
	  {
		pictureByteArrayRef.delete();
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
	  public virtual string FirstName
	  {
		  get
		  {
			return firstName;
		  }
		  set
		  {
			this.firstName = value;
		  }
	  }
	  public virtual string LastName
	  {
		  get
		  {
			return lastName;
		  }
		  set
		  {
			this.lastName = value;
		  }
	  }
	  public virtual string Email
	  {
		  get
		  {
			return email;
		  }
		  set
		  {
			this.email = value;
		  }
	  }
	  public virtual string Password
	  {
		  get
		  {
			return password;
		  }
		  set
		  {
			this.password = value;
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

	  public virtual bool PictureSet
	  {
		  get
		  {
			return pictureByteArrayRef.Id != null;
		  }
	  }

	}

}