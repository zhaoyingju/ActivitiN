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

namespace org.activiti.engine.impl.form
{

	using FormProperty = org.activiti.engine.form.FormProperty;
	using FormType = org.activiti.engine.form.FormType;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	[Serializable]
	public class FormPropertyImpl : FormProperty
	{

	  protected internal string id;
	  protected internal string name;
	  protected internal FormType type;
	  protected internal bool isRequired;
	  protected internal bool isReadable;
	  protected internal bool isWritable;

	  protected internal string value;

	  public FormPropertyImpl(FormPropertyHandler formPropertyHandler)
	  {
		this.id = formPropertyHandler.Id;
		this.name = formPropertyHandler.Name;
		this.type = formPropertyHandler.Type;
		this.isRequired = formPropertyHandler.Required;
		this.isReadable = formPropertyHandler.Readable;
		this.isWritable = formPropertyHandler.Writable;
	  }

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
	  }

	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
	  }

	  public virtual FormType Type
	  {
		  get
		  {
			return type;
		  }
	  }

	  public virtual string Value
	  {
		  get
		  {
			return value;
		  }
		  set
		  {
			this.value = value;
		  }
	  }

	  public virtual bool Required
	  {
		  get
		  {
			return isRequired;
		  }
	  }

	  public virtual bool Readable
	  {
		  get
		  {
			return isReadable;
		  }
	  }


	  public virtual bool Writable
	  {
		  get
		  {
			return isWritable;
		  }
	  }
	}

}