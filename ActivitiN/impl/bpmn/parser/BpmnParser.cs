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
namespace org.activiti.engine.impl.bpmn.parser
{

	using ActivityBehaviorFactory = org.activiti.engine.impl.bpmn.parser.factory.ActivityBehaviorFactory;
	using ListenerFactory = org.activiti.engine.impl.bpmn.parser.factory.ListenerFactory;
	using BpmnParseFactory = org.activiti.engine.impl.cfg.BpmnParseFactory;
	using ExpressionManager = org.activiti.engine.impl.el.ExpressionManager;


	/// <summary>
	/// Parser for BPMN 2.0 process models.
	/// 
	/// There is only one instance of this parser in the process engine.
	/// This <seealso cref="Parser"/> creates <seealso cref="BpmnParse"/> instances that 
	/// can be used to actually parse the BPMN 2.0 XML process definitions.
	/// 
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// </summary>
	public class BpmnParser
	{

	  /// <summary>
	  /// The namepace of the BPMN 2.0 diagram interchange elements.
	  /// </summary>
	  public const string BPMN_DI_NS = "http://www.omg.org/spec/BPMN/20100524/DI";

	  /// <summary>
	  /// The namespace of the BPMN 2.0 diagram common elements.
	  /// </summary>
	  public const string BPMN_DC_NS = "http://www.omg.org/spec/DD/20100524/DC";

	  /// <summary>
	  /// The namespace of the generic OMG DI elements (don't ask me why they didnt use the BPMN_DI_NS ...)
	  /// </summary>
	  public const string OMG_DI_NS = "http://www.omg.org/spec/DD/20100524/DI";

	  protected internal ExpressionManager expressionManager;
	  protected internal ActivityBehaviorFactory activityBehaviorFactory;
	  protected internal ListenerFactory listenerFactory;
	  protected internal BpmnParseFactory bpmnParseFactory;
	  protected internal BpmnParseHandlers bpmnParserHandlers;

	  /// <summary>
	  /// Creates a new <seealso cref="BpmnParse"/> instance that can be used
	  /// to parse only one BPMN 2.0 process definition.
	  /// </summary>
	  public virtual BpmnParse createParse()
	  {
		return bpmnParseFactory.createBpmnParse(this);
	  }

	  public virtual ActivityBehaviorFactory ActivityBehaviorFactory
	  {
		  get
		  {
			return activityBehaviorFactory;
		  }
		  set
		  {
			this.activityBehaviorFactory = value;
		  }
	  }


	  public virtual ListenerFactory ListenerFactory
	  {
		  get
		  {
			return listenerFactory;
		  }
		  set
		  {
			this.listenerFactory = value;
		  }
	  }


	  public virtual BpmnParseFactory BpmnParseFactory
	  {
		  get
		  {
			return bpmnParseFactory;
		  }
		  set
		  {
			this.bpmnParseFactory = value;
		  }
	  }


	  public virtual ExpressionManager ExpressionManager
	  {
		  get
		  {
			return expressionManager;
		  }
		  set
		  {
			this.expressionManager = value;
		  }
	  }


	  public virtual BpmnParseHandlers getBpmnParserHandlers()
	  {
		return bpmnParserHandlers;
	  }

	  public virtual void setBpmnParserHandlers(BpmnParseHandlers bpmnParserHandlers)
	  {
		this.bpmnParserHandlers = bpmnParserHandlers;
	  }
	}

}