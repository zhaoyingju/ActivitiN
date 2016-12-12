using System;
using System.Threading;

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
namespace org.activiti.engine.impl.util
{


	using ProcessEngineConfigurationImpl = org.activiti.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Context = org.activiti.engine.impl.context.Context;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public abstract class ReflectUtil
	{

	  private static readonly Logger LOG = LoggerFactory.getLogger(typeof(ReflectUtil));

	  private static readonly Pattern GETTER_PATTERN = Pattern.compile("(get|is)[A-Z].*");
	  private static readonly Pattern SETTER_PATTERN = Pattern.compile("set[A-Z].*");

	  public static ClassLoader ClassLoader
	  {
		  get
		  {
			ClassLoader loader = CustomClassLoader;
			if (loader == null)
			{
			  loader = Thread.CurrentThread.ContextClassLoader;
			}
			return loader;
		  }
	  }

	  public static Type loadClass(string className)
	  {
	   Type clazz = null;
	   ClassLoader classLoader = CustomClassLoader;

	   // First exception in chain of classloaders will be used as cause when no class is found in any of them
	   Exception throwable = null;

	   if (classLoader != null)
	   {
		 try
		 {
		   LOG.trace("Trying to load class with custom classloader: {}", className);
		   clazz = loadClass(classLoader, className);
		 }
		 catch (Exception t)
		 {
		   throwable = t;
		 }
	   }
	   if (clazz == null)
	   {
		 try
		 {
		   LOG.trace("Trying to load class with current thread context classloader: {}", className);
		   clazz = loadClass(Thread.CurrentThread.ContextClassLoader, className);
		 }
		 catch (Exception t)
		 {
		   if (throwable == null)
		   {
			 throwable = t;
		   }
		 }
		 if (clazz == null)
		 {
		   try
		   {
			 LOG.trace("Trying to load class with local classloader: {}", className);
			 clazz = loadClass(typeof(ReflectUtil).ClassLoader, className);
		   }
		   catch (Exception t)
		   {
			 if (throwable == null)
			 {
			   throwable = t;
			 }
		   }
		 }
	   }

	   if (clazz == null)
	   {
		 throw new ActivitiClassLoadingException(className, throwable);
	   }
	   return clazz;
	  }

	  public static InputStream getResourceAsStream(string name)
	  {
		InputStream resourceStream = null;
		ClassLoader classLoader = CustomClassLoader;
		if (classLoader != null)
		{
		  resourceStream = classLoader.getResourceAsStream(name);
		}

		if (resourceStream == null)
		{
		  // Try the current Thread context classloader
		  classLoader = Thread.CurrentThread.ContextClassLoader;
		  resourceStream = classLoader.getResourceAsStream(name);
		  if (resourceStream == null)
		  {
			// Finally, try the classloader for this class
			classLoader = typeof(ReflectUtil).ClassLoader;
			resourceStream = classLoader.getResourceAsStream(name);
		  }
		}
		return resourceStream;
	  }

	  public static URL getResource(string name)
	  {
		URL url = null;
		ClassLoader classLoader = CustomClassLoader;
		if (classLoader != null)
		{
		  url = classLoader.getResource(name);
		}
		if (url == null)
		{
		  // Try the current Thread context classloader
		  classLoader = Thread.CurrentThread.ContextClassLoader;
		  url = classLoader.getResource(name);
		  if (url == null)
		  {
			// Finally, try the classloader for this class
			classLoader = typeof(ReflectUtil).ClassLoader;
			url = classLoader.getResource(name);
		  }
		}

		return url;
	  }

	  public static object instantiate(string className)
	  {
		try
		{
		  Type clazz = loadClass(className);
		  return clazz.newInstance();
		}
		catch (Exception e)
		{
		  throw new ActivitiException("couldn't instantiate class " + className, e);
		}
	  }

	  public static object invoke(object target, string methodName, object[] args)
	  {
		try
		{
		  Type clazz = target.GetType();
		  Method method = findMethod(clazz, methodName, args);
		  method.Accessible = true;
		  return method.invoke(target, args);
		}
		catch (Exception e)
		{
		  throw new ActivitiException("couldn't invoke " + methodName + " on " + target, e);
		}
	  }

	  /// <summary>
	  /// Returns the field of the given object or null if it doesnt exist.
	  /// </summary>
	  public static Field getField(string fieldName, object @object)
	  {
		return getField(fieldName, @object.GetType());
	  }

	  /// <summary>
	  /// Returns the field of the given class or null if it doesnt exist.
	  /// </summary>
	  public static Field getField(string fieldName, Type clazz)
	  {
		Field field = null;
		try
		{
		  field = clazz.getDeclaredField(fieldName);
		}
		catch (SecurityException)
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
		  throw new ActivitiException("not allowed to access field " + field + " on class " + clazz.FullName);
		}
		catch (NoSuchFieldException)
		{
		  // for some reason getDeclaredFields doesnt search superclasses
		  // (which getFields() does ... but that gives only public fields)
		  Type superClass = clazz.BaseType;
		  if (superClass != null)
		  {
			return getField(fieldName, superClass);
		  }
		}
		return field;
	  }

	  public static void setField(Field field, object @object, object value)
	  {
		try
		{
		  field.Accessible = true;
		  field.set(@object, value);
		}
		catch (System.ArgumentException e)
		{
		  throw new ActivitiException("Could not set field " + field.ToString(), e);
		}
		catch (IllegalAccessException e)
		{
		  throw new ActivitiException("Could not set field " + field.ToString(), e);
		}
	  }

	  /// <summary>
	  /// Returns the setter-method for the given field name or null if no setter exists.
	  /// </summary>
	  public static Method getSetter(string fieldName, Type clazz, Type fieldType)
	  {
		string setterName = "set" + char.toTitleCase(fieldName[0]) + fieldName.Substring(1, fieldName.Length - 1);
		try
		{
		  // Using getMathods(), getMathod(...) expects exact parameter type
		  // matching and ignores inheritance-tree.
		  Method[] methods = clazz.GetMethods();
		  foreach (Method method in methods)
		  {
			if (method.Name.Equals(setterName))
			{
			  Type[] paramTypes = method.ParameterTypes;
			  if (paramTypes != null && paramTypes.Length == 1 && paramTypes[0].IsAssignableFrom(fieldType))
			  {
				return method;
			  }
			}
		  }
		  return null;
		}
		catch (SecurityException)
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
		  throw new ActivitiException("Not allowed to access method " + setterName + " on class " + clazz.FullName);
		}
	  }

	  private static Method findMethod(Type clazz, string methodName, object[] args)
	  {
		foreach (Method method in clazz.DeclaredMethods)
		{
		  // TODO add parameter matching
		  if (method.Name.Equals(methodName) && matches(method.ParameterTypes, args))
		  {
			return method;
		  }
		}
		Type superClass = clazz.BaseType;
		if (superClass != null)
		{
		  return findMethod(superClass, methodName, args);
		}
		return null;
	  }

	  public static object instantiate(string className, object[] args)
	  {
		Type clazz = loadClass(className);
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Constructor< ? > constructor = findMatchingConstructor(clazz, args);
		Constructor<?> constructor = findMatchingConstructor(clazz, args);
		if (constructor == null)
		{
		  throw new ActivitiException("couldn't find constructor for " + className + " with args " + Arrays.asList(args));
		}
		try
		{
		  return constructor.newInstance(args);
		}
		catch (Exception e)
		{
		  throw new ActivitiException("couldn't find constructor for " + className + " with args " + Arrays.asList(args), e);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({ "unchecked", "rawtypes" }) private static <T> Constructor<T> findMatchingConstructor(Class clazz, Object[] args)
	  private static Constructor<T> findMatchingConstructor<T>(Type clazz, object[] args)
	  {
		foreach (Constructor constructor in clazz.DeclaredConstructors) // cannot use <?> or <T> due to JDK 5/6 incompatibility
		{
		  if (matches(constructor.ParameterTypes, args))
		  {
			return constructor;
		  }
		}
		return null;
	  }

	  private static bool matches(Type[] parameterTypes, object[] args)
	  {
		if ((parameterTypes == null) || (parameterTypes.Length == 0))
		{
		  return ((args == null) || (args.Length == 0));
		}
		if ((args == null) || (parameterTypes.Length != args.Length))
		{
		  return false;
		}
		for (int i = 0; i < parameterTypes.Length; i++)
		{
		  if ((args[i] != null) && (!parameterTypes[i].IsAssignableFrom(args[i].GetType())))
		  {
			return false;
		  }
		}
		return true;
	  }

	  private static ClassLoader CustomClassLoader
	  {
		  get
		  {
			ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;
			if (processEngineConfiguration != null)
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final ClassLoader classLoader = processEngineConfiguration.getClassLoader();
			  ClassLoader classLoader = processEngineConfiguration.ClassLoader;
			  if (classLoader != null)
			  {
				return classLoader;
			  }
			}
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static Class loadClass(ClassLoader classLoader, String className) throws ClassNotFoundException
		private static Type loadClass(ClassLoader classLoader, string className)
		{
			ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;
			bool useClassForName = processEngineConfiguration == null || processEngineConfiguration.UseClassForNameClassLoading;
			return useClassForName ? Type.GetType(className, true, classLoader) : classLoader.loadClass(className);
		}

	  public static bool isGetter(Method method)
	  {
		string name = method.Name;
		Type type = method.ReturnType;
		Type[] @params = method.ParameterTypes;

		if (!GETTER_PATTERN.matcher(name).matches())
		{
		  return false;
		}

		// special for isXXX boolean
		if (name.StartsWith("is"))
		{
		  return @params.Length == 0 && type.Name.Equals("boolean", StringComparison.CurrentCultureIgnoreCase);
		}

		return @params.Length == 0 && !type.Equals(Void.TYPE);
	  }

	  public static bool isSetter(Method method, bool allowBuilderPattern)
	  {
		string name = method.Name;
		Type type = method.ReturnType;
		Type[] @params = method.ParameterTypes;

		if (!SETTER_PATTERN.matcher(name).matches())
		{
		  return false;
		}

		return @params.Length == 1 && (type.Equals(Void.TYPE) || (allowBuilderPattern && type.IsSubclassOf(method.DeclaringClass)));
	  }


	  public static bool isSetter(Method method)
	  {
		return isSetter(method, false);
	  }


	  public static string getGetterShorthandName(Method method)
	  {
		if (!isGetter(method))
		{
		  return method.Name;
		}

		string name = method.Name;
		if (name.StartsWith("get"))
		{
		  name = name.Substring(3);
		  name = name.Substring(0, 1).ToLower(Locale.ENGLISH) + name.Substring(1);
		}
		else if (name.StartsWith("is"))
		{
		  name = name.Substring(2);
		  name = name.Substring(0, 1).ToLower(Locale.ENGLISH) + name.Substring(1);
		}

		return name;
	  }

	  public static string getSetterShorthandName(Method method)
	  {
		if (!isSetter(method))
		{
			return method.Name;
		}

		string name = method.Name;
		if (name.StartsWith("set"))
		{
			name = name.Substring(3);
			name = name.Substring(0, 1).ToLower(Locale.ENGLISH) + name.Substring(1);
		}

		return name;
	  }
	}

}