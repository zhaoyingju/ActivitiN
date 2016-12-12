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
namespace org.activiti.engine.impl.variable
{


	using Context = org.activiti.engine.impl.context.Context;
	using VariableInstanceEntity = org.activiti.engine.impl.persistence.entity.VariableInstanceEntity;
	using IoUtil = org.activiti.engine.impl.util.IoUtil;
	using ReflectUtil = org.activiti.engine.impl.util.ReflectUtil;

	/// <summary>
	/// @author Tom Baeyens
	/// @author Marcus Klimstra (CGI)
	/// </summary>
	public class SerializableType : ByteArrayType
	{

	  public const string TYPE_NAME = "serializable";

	  public override string TypeName
	  {
		  get
		  {
			return TYPE_NAME;
		  }
	  }

	  public override object getValue(ValueFields valueFields)
	  {
		object cachedObject = valueFields.CachedValue;
		if (cachedObject != null)
		{
		  return cachedObject;
		}

		sbyte[] bytes = (sbyte[]) base.getValue(valueFields);
		if (bytes != null)
		{
			object deserializedObject = deserialize(bytes, valueFields);

		  valueFields.CachedValue = deserializedObject;

		  if (valueFields is VariableInstanceEntity)
		  {
			// we need to register the deserialized object for dirty checking, 
			// so that it can be serialized again if it was changed. 
			Context.CommandContext.DbSqlSession.addDeserializedObject(new DeserializedObject(this, deserializedObject, bytes, (VariableInstanceEntity) valueFields));
		  }

		  return deserializedObject;
		}
		return null; // byte array is null
	  }

	  public override void setValue(object value, ValueFields valueFields)
	  {
		sbyte[] byteArray = serialize(value, valueFields);
		valueFields.CachedValue = value;

		if (valueFields.Bytes == null)
		{
		  // TODO why the null check? won't this cause issues when setValue is called the second this with a different object?
		  if (valueFields is VariableInstanceEntity)
		  {
			// register the deserialized object for dirty checking.
			Context.CommandContext.DbSqlSession.addDeserializedObject(new DeserializedObject(this, valueFields.CachedValue, byteArray, (VariableInstanceEntity)valueFields));
		  }
		}

		base.setValue(byteArray, valueFields);
	  }

	  public virtual sbyte[] serialize(object value, ValueFields valueFields)
	  {
		if (value == null)
		{
		  return null;
		}
		ByteArrayOutputStream baos = new ByteArrayOutputStream();
		ObjectOutputStream oos = null;
		try
		{
		  oos = createObjectOutputStream(baos);
		  oos.writeObject(value);
		}
		catch (Exception e)
		{
		  throw new ActivitiException("Couldn't serialize value '" + value + "' in variable '" + valueFields.Name + "'", e);
		}
		finally
		{
		  IoUtil.closeSilently(oos);
		}
		return baos.toByteArray();
	  }

	  public virtual object deserialize(sbyte[] bytes, ValueFields valueFields)
	  {
		ByteArrayInputStream bais = new ByteArrayInputStream(bytes);
		try
		{
		  ObjectInputStream ois = createObjectInputStream(bais);
		  object deserializedObject = ois.readObject();

		  return deserializedObject;
		}
		catch (Exception e)
		{
		  throw new ActivitiException("Couldn't deserialize object in variable '" + valueFields.Name + "'", e);
		}
		finally
		{
		  IoUtil.closeSilently(bais);
		}
	  }

	  public override bool isAbleToStore(object value)
	  {
		// TODO don't we need null support here?
		return value is Serializable;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected java.io.ObjectInputStream createObjectInputStream(java.io.InputStream is) throws java.io.IOException
	  protected internal virtual ObjectInputStream createObjectInputStream(InputStream @is)
	  {
		return new ObjectInputStreamAnonymousInnerClassHelper(this, @is);
	  }

	  private class ObjectInputStreamAnonymousInnerClassHelper : ObjectInputStream
	  {
		  private readonly SerializableType outerInstance;

		  public ObjectInputStreamAnonymousInnerClassHelper(SerializableType outerInstance, InputStream @is) : base(@is)
		  {
			  this.outerInstance = outerInstance;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected Class resolveClass(java.io.ObjectStreamClass desc) throws java.io.IOException, ClassNotFoundException
		  protected internal virtual Type resolveClass(ObjectStreamClass desc)
		  {
			return ReflectUtil.loadClass(desc.Name);
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected java.io.ObjectOutputStream createObjectOutputStream(java.io.OutputStream os) throws java.io.IOException
		protected internal virtual ObjectOutputStream createObjectOutputStream(OutputStream os)
		{
		return new ObjectOutputStream(os);
		}
	}

}