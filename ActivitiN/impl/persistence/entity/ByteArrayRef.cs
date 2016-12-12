using System;

namespace org.activiti.engine.impl.persistence.entity
{

	using Context = org.activiti.engine.impl.context.Context;

	/// <summary>
	/// <para>Encapsulates the logic for transparently working with <seealso cref="ByteArrayEntity"/>.</para>
	/// 
	/// <para>Make sure that instance variables (i.e. fields) of this type are always initialized, 
	/// and thus <strong>never</strong> null.</para>
	/// 
	/// <para>For example:</para>
	/// <pre>
	/// private final ByteArrayRef byteArrayRef = new ByteArrayRef();
	/// </pre>
	/// 
	/// @author Marcus Klimstra (CGI)
	/// </summary>
	[Serializable]
	public sealed class ByteArrayRef
	{

	  private const long serialVersionUID = 1L;

	  private string id;
	  private string name;
	  private ByteArrayEntity entity;
	  protected internal bool deleted = false;

	  public ByteArrayRef()
	  {
	  }

	  // Only intended to be used by ByteArrayRefTypeHandler
	  public ByteArrayRef(string id)
	  {
		this.id = id;
	  }

	  public string Id
	  {
		  get
		  {
			return id;
		  }
	  }

	  public string Name
	  {
		  get
		  {
			return name;
		  }
	  }

	  public sbyte[] Bytes
	  {
		  get
		  {
			ensureInitialized();
			return (entity != null ? entity.Bytes : null);
		  }
		  set
		  {
			if (id == null)
			{
			  if (value != null)
			  {
				entity = ByteArrayEntity.createAndInsert(name, value);
				id = entity.Id;
			  }
			}
			else
			{
			  ensureInitialized();
			  entity.Bytes = value;
			}
		  }
	  }

	  public void setValue(string name, sbyte[] bytes)
	  {
		this.name = name;
		Bytes = bytes;
	  }


	  public ByteArrayEntity Entity
	  {
		  get
		  {
			ensureInitialized();
			return entity;
		  }
	  }

	  public void delete()
	  {
		if (!deleted && id != null)
		{
		  if (entity != null)
		  {
			// if the entity has been loaded already,
			// we might as well use the safer optimistic locking delete.
			Context.CommandContext.ByteArrayEntityManager.deleteByteArray(entity);
		  }
		  else
		  {
			Context.CommandContext.ByteArrayEntityManager.deleteByteArrayById(id);
		  }
		  entity = null;
		  id = null;
		  deleted = true;
		}
	  }

	  private void ensureInitialized()
	  {
		if (id != null && entity == null)
		{
		  entity = Context.CommandContext.ByteArrayEntityManager.findById(id);
		  name = entity.Name;
		}
	  }

	  public bool Deleted
	  {
		  get
		  {
			return deleted;
		  }
	  }

	  public override string ToString()
	  {
		return "ByteArrayRef[id=" + id + ", name=" + name + ", entity=" + entity + (deleted ? ", deleted]" :"]");
	  }
	}

}