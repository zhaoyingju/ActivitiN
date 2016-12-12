using System;
using System.Collections.Generic;

namespace org.activiti.engine.impl.variable
{


	using Context = org.activiti.engine.impl.context.Context;

	/// <summary>
	/// Variable type capable of storing a list of reference to JPA-entities. Only JPA-Entities which
	/// are configured by annotations are supported. Use of compound primary keys is not supported.
	/// <br>
	/// The variable value should be of type <seealso cref="List"/> and can only contain objects of the same type.
	/// 
	/// @author Frederik Heremans
	/// </summary>
	public class JPAEntityListVariableType : VariableType, CacheableVariable
	{

	  public const string TYPE_NAME = "jpa-entity-list";

	  protected internal JPAEntityMappings mappings;

	  protected internal bool forceCachedValue = false;

	  public JPAEntityListVariableType()
	  {
		mappings = new JPAEntityMappings();
	  }

	  public override bool ForceCacheable
	  {
		  set
		  {
			this.forceCachedValue = value;
		  }
	  }

	  public override string TypeName
	  {
		  get
		  {
			return TYPE_NAME;
		  }
	  }

	  public override bool Cachable
	  {
		  get
		  {
			return forceCachedValue;
		  }
	  }

	  public override bool isAbleToStore(object value)
	  {
		bool canStore = false;

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: if(value instanceof java.util.List<?>)
		if (value is IList<?>)
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.List<?> list = (java.util.List<?>) value;
		  IList<?> list = (IList<?>) value;
		  if (list.Count > 0)
		  {
			// We can only store the list if we are sure it's actually a list of JPA entities. In case the 
			// list is empty, we don't store it.
			canStore = true;
			Type entityClass = mappings.getEntityMetaData(list[0].GetType()).EntityClass;

			foreach (object entity in list)
			{
			  canStore = entity != null && mappings.isJPAEntity(entity) && mappings.getEntityMetaData(entity.GetType()).EntityClass.Equals(entityClass);
			  if (!canStore)
			  {
				// In case the object is not a JPA entity or the class doesn't match, we can't store the list
				break;
			  }
			}
		  }
		}
		return canStore;
	  }

	  public override void setValue(object value, ValueFields valueFields)
	  {
		EntityManagerSession entityManagerSession = Context.CommandContext.getSession(typeof(EntityManagerSession));
		if (entityManagerSession == null)
		{
		  throw new ActivitiException("Cannot set JPA variable: " + typeof(EntityManagerSession) + " not configured");
		}
		else
		{
		  // Before we set the value we must flush all pending changes from the entitymanager
		  // If we don't do this, in some cases the primary key will not yet be set in the object
		  // which will cause exceptions down the road.
		  entityManagerSession.flush();
		}

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: if(value instanceof java.util.List<?> && ((java.util.List<?>) value).size() > 0)
		if (value is IList<?> && ((IList<?>) value).Count > 0)
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.List<?> list = (java.util.List<?>) value;
		  IList<?> list = (IList<?>) value;
		  IList<string> ids = new List<string>();

		  string type = mappings.getJPAClassString(list[0]);
		  foreach (object entry in list)
		  {
			ids.Add(mappings.getJPAIdString(entry));
		  }

		  // Store type in text field and the ID's as a serialized array
		  valueFields.Bytes = serializeIds(ids);
		  valueFields.TextValue = type;

		}
		else if (value == null)
		{
		  valueFields.Bytes = null;
		  valueFields.TextValue = null;
		}
		else
		{
		  throw new ActivitiIllegalArgumentException("Value is not a list of JPA entities: " + value);
		}

	  }

	  public override object getValue(ValueFields valueFields)
	  {
		sbyte[] bytes = valueFields.Bytes;
		if (valueFields.TextValue != null && bytes != null)
		{
		  string entityClass = valueFields.TextValue;

		  IList<object> result = new List<object>();
		  string[] ids = deserializeIds(bytes);

		  foreach (string id in ids)
		  {
			result.Add(mappings.getJPAEntity(entityClass, id));
		  }

		  return result;
		}
		return null;
	  }


	  /// <returns> a bytearray containing all ID's in the given string serialized as an array. </returns>
	  protected internal virtual sbyte[] serializeIds(IList<string> ids)
	  {
		try
		{
		  string[] toStore = ids.ToArray();
		  ByteArrayOutputStream baos = new ByteArrayOutputStream();
		  ObjectOutputStream @out = new ObjectOutputStream(baos);

		  @out.writeObject(toStore);
		  return baos.toByteArray();
		}
		catch (IOException ioe)
		{
		  throw new ActivitiException("Unexpected exception when serializing JPA id's", ioe);
		}
	  }

	  protected internal virtual string[] deserializeIds(sbyte[] bytes)
	  {
		try
		{
		  ByteArrayInputStream bais = new ByteArrayInputStream(bytes);
		  ObjectInputStream @in = new ObjectInputStream(bais);

		  object read = @in.readObject();
		  if (!(read is string[]))
		  {
			throw new ActivitiIllegalArgumentException("Deserialized value is not an array of ID's: " + read);
		  }

		  return (string[]) read;
		}
		catch (IOException ioe)
		{
		  throw new ActivitiException("Unexpected exception when deserializing JPA id's", ioe);
		}
		catch (ClassNotFoundException e)
		{
		  throw new ActivitiException("Unexpected exception when deserializing JPA id's", e);
		}
	  }

	}

}