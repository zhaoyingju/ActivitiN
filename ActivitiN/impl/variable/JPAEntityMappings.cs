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

namespace org.activiti.engine.impl.variable
{


	using Context = org.activiti.engine.impl.context.Context;
	using ReflectUtil = org.activiti.engine.impl.util.ReflectUtil;

	/// <summary>
	/// @author Frederik Heremans
	/// </summary>
	public class JPAEntityMappings
	{

	  private IDictionary<string, EntityMetaData> classMetaDatamap;

	  private JPAEntityScanner enitityScanner;

	  public JPAEntityMappings()
	  {
		classMetaDatamap = new Dictionary<string, EntityMetaData>();
		enitityScanner = new JPAEntityScanner();
	  }

	  public virtual bool isJPAEntity(object value)
	  {
		if (value != null)
		{
		  // EntityMetaData will be added for all classes, even those who are not 
		  // JPA-entities to prevent unneeded annotation scanning  
		  return getEntityMetaData(value.GetType()).JPAEntity;
		}
		return false;
	  }

	  public virtual EntityMetaData getEntityMetaData(Type clazz)
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		EntityMetaData metaData = classMetaDatamap[clazz.FullName];
		if (metaData == null)
		{
		  // Class not present in meta-data map, create metaData for it and add
		  metaData = scanClass(clazz);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  classMetaDatamap[clazz.FullName] = metaData;
		}
		return metaData;
	  }

	  private EntityMetaData scanClass(Type clazz)
	  {
		return enitityScanner.scanClass(clazz);
	  }

	  public virtual string getJPAClassString(object value)
	  {
		if (value == null)
		{
		  throw new ActivitiIllegalArgumentException("null value cannot be saved");
		}

		EntityMetaData metaData = getEntityMetaData(value.GetType());
		if (!metaData.JPAEntity)
		{
		  throw new ActivitiIllegalArgumentException("Object is not a JPA Entity: class='" + value.GetType() + "', " + value);
		}

		// Extract the class from the Entity instance
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		return metaData.EntityClass.FullName;
	  }

	  public virtual string getJPAIdString(object value)
	  {
		EntityMetaData metaData = getEntityMetaData(value.GetType());
		if (!metaData.JPAEntity)
		{
		  throw new ActivitiIllegalArgumentException("Object is not a JPA Entity: class='" + value.GetType() + "', " + value);
		}
		object idValue = getIdValue(value, metaData);
		return getIdString(idValue);
	  }

	  public virtual object getIdValue(object value, EntityMetaData metaData)
	  {
		try
		{
		  if (metaData.IdMethod != null)
		  {
			return metaData.IdMethod.invoke(value);
		  }
		  else if (metaData.IdField != null)
		  {
			return metaData.IdField.get(value);
		  }
		}
		catch (System.ArgumentException iae)
		{
		  throw new ActivitiException("Illegal argument exception when getting value from id method/field on JPAEntity", iae);
		}
		catch (IllegalAccessException iae)
		{
		  throw new ActivitiException("Cannot access id method/field for JPA Entity", iae);
		}
		catch (InvocationTargetException ite)
		{
		  throw new ActivitiException("Exception occured while getting value from id field/method on JPAEntity: " + ite.InnerException.Message, ite.InnerException);
		}

		// Fall trough when no method and field is set
		throw new ActivitiException("Cannot get id from JPA Entity, no id method/field set");
	  }

	  public virtual object getJPAEntity(string className, string idString)
	  {
		Type entityClass = null;
		entityClass = ReflectUtil.loadClass(className);

		EntityMetaData metaData = getEntityMetaData(entityClass);
		if (metaData == null)
		{
		  throw new ActivitiIllegalArgumentException("Class is not a JPA-entity: " + className);
		}

		// Create primary key of right type
		object primaryKey = createId(metaData, idString);
		return findEntity(entityClass, primaryKey);
	  }

	  private object findEntity(Type entityClass, object primaryKey)
	  {
		EntityManager em = Context.CommandContext.getSession(typeof(EntityManagerSession)).EntityManager;

		object entity = em.find(entityClass, primaryKey);
		if (entity == null)
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  throw new ActivitiException("Entity does not exist: " + entityClass.FullName + " - " + primaryKey);
		}
		return entity;
	  }

	  public virtual object createId(EntityMetaData metaData, string @string)
	  {
		Type type = metaData.IdType;
		// According to JPA-spec all primitive types (and wrappers) are supported, String, util.Date, sql.Date,
		// BigDecimal and BigInteger
		if (type == typeof(long?) || type == typeof(long))
		{
		  return Convert.ToInt64(@string);
		}
		else if (type == typeof(string))
		{
		  return @string;
		}
		else if (type == typeof(sbyte?) || type == typeof(sbyte))
		{
		  return Convert.ToByte(@string);
		}
		else if (type == typeof(short?) || type == typeof(short))
		{
		  return Convert.ToInt16(@string);
		}
		else if (type == typeof(int?) || type == typeof(int))
		{
		  return Convert.ToInt32(@string);
		}
		else if (type == typeof(float?) || type == typeof(float))
		{
		  return Convert.ToSingle(@string);
		}
		else if (type == typeof(double?) || type == typeof(double))
		{
		  return Convert.ToDouble(@string);
		}
		else if (type == typeof(char?) || type == typeof(char))
		{
		  return new char?(@string[0]);
		}
		else if (type == typeof(DateTime))
		{
		  return new DateTime(Convert.ToInt64(@string));
		}
		else if (type == typeof(java.sql.Date))
		{
		  return new java.sql.Date(Convert.ToInt64(@string));
		}
		else if (type == typeof(decimal))
		{
		  return new decimal(@string);
		}
		else if (type == typeof(System.Numerics.BigInteger))
		{
		  return new System.Numerics.BigInteger(@string);
		}
		else if (type == typeof(UUID))
		{
			return UUID.fromString(@string);
		}
		else
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  throw new ActivitiIllegalArgumentException("Unsupported Primary key type for JPA-Entity: " + type.FullName);
		}
	  }

	  public virtual string getIdString(object value)
	  {
		if (value == null)
		{
		  throw new ActivitiIllegalArgumentException("Value of primary key for JPA-Entity cannot be null");
		}
		// Only java.sql.date and java.util.date require custom handling, the other types
		// can just use toString()
		if (value is DateTime)
		{
		  return "" + ((DateTime) value).Ticks;
		}
		else if (value is java.sql.Date)
		{
		  return "" + ((java.sql.Date) value).Time;
		}
		else if (value is long? || value is string || value is sbyte? || value is short? || value is int? || value is float? || value is double? || value is char? || value is decimal || value is System.Numerics.BigInteger || value is UUID)
		{
		  return value.ToString();
		}
		else
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  throw new ActivitiIllegalArgumentException("Unsupported Primary key type for JPA-Entity: " + value.GetType().FullName);
		}
	  }
	}

}