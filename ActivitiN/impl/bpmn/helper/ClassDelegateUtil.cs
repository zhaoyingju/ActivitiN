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
namespace org.activiti.engine.impl.bpmn.helper
{


	using FieldDeclaration = org.activiti.engine.impl.bpmn.parser.FieldDeclaration;
	using ReflectUtil = org.activiti.engine.impl.util.ReflectUtil;

	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class ClassDelegateUtil
	{

	  public static object instantiateDelegate(Type clazz, IList<FieldDeclaration> fieldDeclarations)
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		return instantiateDelegate(clazz.FullName, fieldDeclarations);
	  }

	  public static object instantiateDelegate(string className, IList<FieldDeclaration> fieldDeclarations)
	  {
		object @object = ReflectUtil.instantiate(className);
		applyFieldDeclaration(fieldDeclarations, @object);
		return @object;
	  }

	  public static void applyFieldDeclaration(IList<FieldDeclaration> fieldDeclarations, object target)
	  {
		if (fieldDeclarations != null)
		{
		  foreach (FieldDeclaration declaration in fieldDeclarations)
		  {
			applyFieldDeclaration(declaration, target);
		  }
		}
	  }

	  public static void applyFieldDeclaration(FieldDeclaration declaration, object target)
	  {
		Method setterMethod = ReflectUtil.getSetter(declaration.Name, target.GetType(), declaration.Value.GetType());

		if (setterMethod != null)
		{
		  try
		  {
			setterMethod.invoke(target, declaration.Value);
		  }
		  catch (System.ArgumentException e)
		  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			throw new ActivitiException("Error while invoking '" + declaration.Name + "' on class " + target.GetType().FullName, e);
		  }
		  catch (IllegalAccessException e)
		  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			throw new ActivitiException("Illegal acces when calling '" + declaration.Name + "' on class " + target.GetType().FullName, e);
		  }
		  catch (InvocationTargetException e)
		  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			throw new ActivitiException("Exception while invoking '" + declaration.Name + "' on class " + target.GetType().FullName, e);
		  }
		}
		else
		{
		  Field field = ReflectUtil.getField(declaration.Name, target);
		  if (field == null)
		  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			throw new ActivitiIllegalArgumentException("Field definition uses unexisting field '" + declaration.Name + "' on class " + target.GetType().FullName);
		  }
		  // Check if the delegate field's type is correct
		 if (!fieldTypeCompatible(declaration, field))
		 {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		   throw new ActivitiIllegalArgumentException("Incompatible type set on field declaration '" + declaration.Name + "' for class " + target.GetType().FullName + ". Declared value has type " + declaration.Value.GetType().FullName + ", while expecting " + field.Type.Name);
		 }
		 ReflectUtil.setField(field, target, declaration.Value);
		}
	  }

	  public static bool fieldTypeCompatible(FieldDeclaration declaration, Field field)
	  {
		if (declaration.Value != null)
		{
		  return declaration.Value.GetType().IsSubclassOf(field.Type);
		}
		else
		{
		  // Null can be set any field type
		  return true;
		}
	  }

	}

}