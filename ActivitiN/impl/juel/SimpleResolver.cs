using System;
using System.Collections.Generic;

/*
 * Based on JUEL 2.2.1 code, 2006-2009 Odysseus Software GmbH
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace org.activiti.engine.impl.juel
{


	using ItemInstance = org.activiti.engine.impl.bpmn.data.ItemInstance;
	using ArrayELResolver = org.activiti.engine.impl.javax.el.ArrayELResolver;
	using BeanELResolver = org.activiti.engine.impl.javax.el.BeanELResolver;
	using CompositeELResolver = org.activiti.engine.impl.javax.el.CompositeELResolver;
	using DynamicBeanPropertyELResolver = org.activiti.engine.impl.javax.el.DynamicBeanPropertyELResolver;
	using ELContext = org.activiti.engine.impl.javax.el.ELContext;
	using ELResolver = org.activiti.engine.impl.javax.el.ELResolver;
	using JsonNodeELResolver = org.activiti.engine.impl.javax.el.JsonNodeELResolver;
	using ListELResolver = org.activiti.engine.impl.javax.el.ListELResolver;
	using MapELResolver = org.activiti.engine.impl.javax.el.MapELResolver;
	using ResourceBundleELResolver = org.activiti.engine.impl.javax.el.ResourceBundleELResolver;

	/// <summary>
	/// Simple resolver implementation. This resolver handles root properties (top-level identifiers).
	/// Resolving "real" properties (<code>base != null</code>) is delegated to a resolver specified at
	/// construction time.
	/// 
	/// @author Christoph Beck
	/// </summary>
	public class SimpleResolver : ELResolver
	{
		private static readonly ELResolver DEFAULT_RESOLVER_READ_ONLY = new CompositeELResolverAnonymousInnerClassHelper();

		private class CompositeELResolverAnonymousInnerClassHelper : CompositeELResolver
		{
			public CompositeELResolverAnonymousInnerClassHelper()
			{
			}

			{
				add(new ArrayELResolver(true));
				add(new ListELResolver(true));
				add(new MapELResolver(true));
				add(new JsonNodeELResolver(true));
				add(new ResourceBundleELResolver());
		  add(new DynamicBeanPropertyELResolver(true, typeof(ItemInstance), "getFieldValue", "setFieldValue"));
				add(new BeanELResolver(true));
			}
		}
		private static final ELResolver DEFAULT_RESOLVER_READ_WRITE = new CompositeELResolverAnonymousInnerClassHelper2(this);

		private final RootPropertyResolver root;
		private final CompositeELResolver @delegate;

		/// <summary>
		/// Create a resolver capable of resolving top-level identifiers. Everything else is passed to
		/// the supplied delegate.
		/// </summary>
		public SimpleResolver(ELResolver resolver, bool readOnly)
		{
			@delegate = new CompositeELResolver();
			@delegate.add(root = new RootPropertyResolver(readOnly));
			@delegate.add(resolver);
		}

		/// <summary>
		/// Create a read/write resolver capable of resolving top-level identifiers. Everything else is
		/// passed to the supplied delegate.
		/// </summary>
		public SimpleResolver(ELResolver resolver)
		{
			this(resolver, false);
		}

		/// <summary>
		/// Create a resolver capable of resolving top-level identifiers, array values, list values, map
		/// values, resource values and bean properties.
		/// </summary>
		public SimpleResolver(bool readOnly)
		{
			this(readOnly ? DEFAULT_RESOLVER_READ_ONLY : DEFAULT_RESOLVER_READ_WRITE, readOnly);
		}

		/// <summary>
		/// Create a read/write resolver capable of resolving top-level identifiers, array values, list
		/// values, map values, resource values and bean properties.
		/// </summary>
		public SimpleResolver()
		{
			this(DEFAULT_RESOLVER_READ_WRITE, false);
		}

		/// <summary>
		/// Answer our root resolver which provides an API to access top-level properties.
		/// </summary>
		/// <returns> root property resolver </returns>
		public RootPropertyResolver RootPropertyResolver
		{
			return root;
		}

		public Type getCommonPropertyType(ELContext context, object @base)
		{
			return @delegate.getCommonPropertyType(context, @base);
		}

		public IEnumerator<FeatureDescriptor> getFeatureDescriptors(ELContext context, object @base)
		{
			return @delegate.getFeatureDescriptors(context, @base);
		}

		public Type getType(ELContext context, object @base, object property)
		{
			return @delegate.getType(context, @base, property);
		}

		public object getValue(ELContext context, object @base, object property)
		{
			return @delegate.getValue(context, @base, property);
		}

		public bool isReadOnly(ELContext context, object @base, object property)
		{
			return @delegate.isReadOnly(context, @base, property);
		}

		public void setValue(ELContext context, object @base, object property, object value)
		{
			@delegate.setValue(context, @base, property, value);
		}

		public object invoke(ELContext context, object @base, object method, Class[] paramTypes, Object[] @params)
		{
			return @delegate.invoke(context, @base, method, paramTypes, @params);
		}
	}


	private class CompositeELResolverAnonymousInnerClassHelper2 : CompositeELResolver
	{
		private readonly SimpleResolver outerInstance;

		public CompositeELResolverAnonymousInnerClassHelper2(SimpleResolver outerInstance)
		{
			this.outerInstance = outerInstance;
		}

		{
			add(new ArrayELResolver(false));
			add(new ListELResolver(false));
			add(new MapELResolver(false));
			add(new JsonNodeELResolver(false));
			add(new ResourceBundleELResolver());
	  add(new DynamicBeanPropertyELResolver(false, typeof(ItemInstance), "getFieldValue", "setFieldValue"));
			add(new BeanELResolver(false));
		}
	}
}