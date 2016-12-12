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




	/// <summary>
	/// Scans class and creates <seealso cref="EntityMetaData"/> based on it. 
	/// 
	/// @author Frederik Heremans
	/// </summary>
	public class JPAEntityScanner
	{

	  public virtual EntityMetaData scanClass(Type clazz)
	  {
		EntityMetaData metaData = new EntityMetaData();
		// in case with JPA Enhancement
		// method should iterate over superclasses list
		// to find @Entity and @Id annotations
		while (clazz != null && !clazz.Equals(typeof(object)))
		{

		  // Class should have @Entity annotation
		  bool isEntity = isEntityAnnotationPresent(clazz);

		  if (isEntity)
		  {
			metaData.EntityClass = clazz;
			metaData.JPAEntity = true;
			// Try to find a field annotated with @Id
			Field idField = getIdField(clazz);
			if (idField != null)
			{
			  metaData.IdField = idField;
			}
			else
			{
			  // Try to find a method annotated with @Id
			  Method idMethod = getIdMethod(clazz);
			  if (idMethod != null)
			  {
				metaData.IdMethod = idMethod;
			  }
			  else
			  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				throw new ActivitiException("Cannot find field or method with annotation @Id on class '" + clazz.FullName + "', only single-valued primary keys are supported on JPA-enities");
			  }
			}
			break;
		  }
		  clazz = clazz.BaseType;
		}
		return metaData;
	  }

	  private Method getIdMethod(Type clazz)
	  {
		Method idMethod = null;
		// Get all public declared methods on the class. According to spec, @Id should only be 
		// applied to fields and property get methods
		Method[] methods = clazz.GetMethods();
		Id idAnnotation = null;
		foreach (Method method in methods)
		{
		  idAnnotation = method.getAnnotation(typeof(Id));
		  if (idAnnotation != null && !method.Bridge)
		  {
			idMethod = method;
			break;
		  }
		}
		return idMethod;
	  }

	  private Field getIdField(Type clazz)
	  {
	   Field idField = null;
	   Field[] fields = clazz.DeclaredFields;
	   Id idAnnotation = null;
	   foreach (Field field in fields)
	   {
		 idAnnotation = field.getAnnotation(typeof(Id));
		 if (idAnnotation != null)
		 {
		   idField = field;
		   break;
		 }
	   }

	   if (idField == null)
	   {
		 // Check superClass for fields with @Id, since getDeclaredFields does
		 // not return superclass-fields.
		 Type superClass = clazz.BaseType;
		 if (superClass != null && !superClass.Equals(typeof(object)))
		 {
		   // Recursively go up class hierarchy
		   idField = getIdField(superClass);
		 }
	   }
	   return idField;
	  }

	  private bool isEntityAnnotationPresent(Type clazz)
	  {
		return (clazz.getAnnotation(typeof(Entity)) != null);
	  }
	}

}