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
namespace org.activiti.engine.impl.persistence.entity
{


	using Context = org.activiti.engine.impl.context.Context;
	using HasRevision = org.activiti.engine.impl.db.HasRevision;
	using PersistentObject = org.activiti.engine.impl.db.PersistentObject;
	using ObjectUtils = org.apache.commons.lang3.ObjectUtils;

	/// <summary>
	/// @author Tom Baeyens
	/// @author Marcus Klimstra (CGI)
	/// </summary>
	[Serializable]
	public class ByteArrayEntity : PersistentObject, HasRevision
	{

	  private const long serialVersionUID = 1L;

	  protected internal string id;
	  protected internal int revision;
	  protected internal string name;
	  protected internal sbyte[] bytes;
	  protected internal string deploymentId;

	  // Default constructor for SQL mapping
	  protected internal ByteArrayEntity()
	  {
	  }

	  public ByteArrayEntity(string name, sbyte[] bytes)
	  {
		this.name = name;
		this.bytes = bytes;
	  }

	  public ByteArrayEntity(sbyte[] bytes)
	  {
		this.bytes = bytes;
	  }

	  public static ByteArrayEntity createAndInsert(string name, sbyte[] bytes)
	  {
		ByteArrayEntity byteArrayEntity = new ByteArrayEntity(name, bytes);

		Context.CommandContext.DbSqlSession.insert(byteArrayEntity);

		return byteArrayEntity;
	  }

	  public static ByteArrayEntity createAndInsert(sbyte[] bytes)
	  {
		return createAndInsert(null, bytes);
	  }

	  public virtual sbyte[] Bytes
	  {
		  get
		  {
			return bytes;
		  }
		  set
		  {
			this.bytes = value;
		  }
	  }

	  public virtual object PersistentState
	  {
		  get
		  {
			return new PersistentState(name, bytes);
		  }
	  }

	  public virtual int RevisionNext
	  {
		  get
		  {
			return revision + 1;
		  }
	  }

	  // getters and setters //////////////////////////////////////////////////////

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
	  public virtual string DeploymentId
	  {
		  get
		  {
			return deploymentId;
		  }
		  set
		  {
			this.deploymentId = value;
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

	  public override string ToString()
	  {
		return "ByteArrayEntity[id=" + id + ", name=" + name + ", size=" + (bytes != null ? bytes.Length : 0) + "]";
	  }

	  // Wrapper for a byte array, needed to do byte array comparisons
	  // See https://activiti.atlassian.net/browse/ACT-1524
	  private class PersistentState
	  {

		internal readonly string name;
		internal readonly sbyte[] bytes;

		public PersistentState(string name, sbyte[] bytes)
		{
		  this.name = name;
		  this.bytes = bytes;
		}

		public override bool Equals(object obj)
		{
		  if (obj is PersistentState)
		  {
			PersistentState other = (PersistentState) obj;
			return ObjectUtils.Equals(this.name, other.name) && Arrays.Equals(this.bytes, other.bytes);
		  }
		  return false;
		}

		public override int GetHashCode()
		{
		  throw new System.NotSupportedException();
		}

	  }

	}

}