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
namespace org.activiti.engine.impl.el
{

	using Expression = org.activiti.engine.@delegate.Expression;
	using VariableScope = org.activiti.engine.@delegate.VariableScope;
	using ItemInstance = org.activiti.engine.impl.bpmn.data.ItemInstance;
	using ArrayELResolver = org.activiti.engine.impl.javax.el.ArrayELResolver;
	using BeanELResolver = org.activiti.engine.impl.javax.el.BeanELResolver;
	using CompositeELResolver = org.activiti.engine.impl.javax.el.CompositeELResolver;
	using DynamicBeanPropertyELResolver = org.activiti.engine.impl.javax.el.DynamicBeanPropertyELResolver;
	using ELContext = org.activiti.engine.impl.javax.el.ELContext;
	using ELResolver = org.activiti.engine.impl.javax.el.ELResolver;
	using ExpressionFactory = org.activiti.engine.impl.javax.el.ExpressionFactory;
	using JsonNodeELResolver = org.activiti.engine.impl.javax.el.JsonNodeELResolver;
	using ListELResolver = org.activiti.engine.impl.javax.el.ListELResolver;
	using MapELResolver = org.activiti.engine.impl.javax.el.MapELResolver;
	using ValueExpression = org.activiti.engine.impl.javax.el.ValueExpression;
	using ExpressionFactoryImpl = org.activiti.engine.impl.juel.ExpressionFactoryImpl;
	using VariableScopeImpl = org.activiti.engine.impl.persistence.entity.VariableScopeImpl;


	/// <summary>
	/// <para>
	/// Central manager for all expressions.
	/// </para>
	/// <para>
	/// Process parsers will use this to build expression objects that are stored in
	/// the process definitions.
	/// </para>
	/// <para>
	/// Then also this class is used as an entry point for runtime evaluation of the
	/// expressions.
	/// </para>
	/// 
	/// @author Tom Baeyens
	/// @author Dave Syer
	/// @author Frederik Heremans
	/// </summary>
	public class ExpressionManager
	{

	  protected internal ExpressionFactory expressionFactory;
	  // Default implementation (does nothing)
	  protected internal ELContext parsingElContext = new ParsingElContext();
	  protected internal IDictionary<object, object> beans;


	  public ExpressionManager() : this(null)
	  {
	  }

	  public ExpressionManager(bool initFactory) : this(null, false)
	  {
	  }

	  public ExpressionManager(IDictionary<object, object> beans) : this(beans, true)
	  {
	  }

	  public ExpressionManager(IDictionary<object, object> beans, bool initFactory)
	  {
		// Use the ExpressionFactoryImpl in activiti build in version of juel, with parametrised method expressions enabled
		expressionFactory = new ExpressionFactoryImpl();
		this.beans = beans;
	  }

	  public virtual Expression createExpression(string expression)
	  {
		ValueExpression valueExpression = expressionFactory.createValueExpression(parsingElContext, expression.Trim(), typeof(object));
		return new JuelExpression(valueExpression, expression);
	  }

	  public virtual ExpressionFactory ExpressionFactory
	  {
		  set
		  {
			this.expressionFactory = value;
		  }
	  }

	  public virtual ELContext getElContext(VariableScope variableScope)
	  {
		ELContext elContext = null;
		if (variableScope is VariableScopeImpl)
		{
		  VariableScopeImpl variableScopeImpl = (VariableScopeImpl) variableScope;
		  elContext = variableScopeImpl.CachedElContext;
		}

		if (elContext == null)
		{
		  elContext = createElContext(variableScope);
		  if (variableScope is VariableScopeImpl)
		  {
			((VariableScopeImpl)variableScope).CachedElContext = elContext;
		  }
		}

		return elContext;
	  }

	  protected internal virtual ActivitiElContext createElContext(VariableScope variableScope)
	  {
		ELResolver elResolver = createElResolver(variableScope);
		return new ActivitiElContext(elResolver);
	  }

	  protected internal virtual ELResolver createElResolver(VariableScope variableScope)
	  {
		CompositeELResolver elResolver = new CompositeELResolver();
		elResolver.add(new VariableScopeElResolver(variableScope));

		if (beans != null)
		{
		  // ACT-1102: Also expose all beans in configuration when using standalone activiti, not
		  // in spring-context
		  elResolver.add(new ReadOnlyMapELResolver(beans));
		}

		elResolver.add(new ArrayELResolver());
		elResolver.add(new ListELResolver());
		elResolver.add(new MapELResolver());
		elResolver.add(new JsonNodeELResolver());
		elResolver.add(new DynamicBeanPropertyELResolver(typeof(ItemInstance), "getFieldValue", "setFieldValue")); //TODO: needs verification
		elResolver.add(new BeanELResolver());
		return elResolver;
	  }

		public virtual IDictionary<object, object> Beans
		{
			get
			{
				return beans;
			}
			set
			{
				this.beans = value;
			}
		}


	}

}