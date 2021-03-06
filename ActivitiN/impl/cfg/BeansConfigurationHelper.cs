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

namespace org.activiti.engine.impl.cfg
{

	using DefaultListableBeanFactory = org.springframework.beans.factory.support.DefaultListableBeanFactory;
	using XmlBeanDefinitionReader = org.springframework.beans.factory.xml.XmlBeanDefinitionReader;
	using ClassPathResource = org.springframework.core.io.ClassPathResource;
	using InputStreamResource = org.springframework.core.io.InputStreamResource;
	using Resource = org.springframework.core.io.Resource;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class BeansConfigurationHelper
	{

	  public static ProcessEngineConfiguration parseProcessEngineConfiguration(Resource springResource, string beanName)
	  {
		DefaultListableBeanFactory beanFactory = new DefaultListableBeanFactory();
		XmlBeanDefinitionReader xmlBeanDefinitionReader = new XmlBeanDefinitionReader(beanFactory);
		xmlBeanDefinitionReader.ValidationMode = XmlBeanDefinitionReader.VALIDATION_XSD;
		xmlBeanDefinitionReader.loadBeanDefinitions(springResource);
		ProcessEngineConfigurationImpl processEngineConfiguration = (ProcessEngineConfigurationImpl) beanFactory.getBean(beanName);
		processEngineConfiguration.Beans = new SpringBeanFactoryProxyMap(beanFactory);
		return processEngineConfiguration;
	  }

	  public static ProcessEngineConfiguration parseProcessEngineConfigurationFromInputStream(InputStream inputStream, string beanName)
	  {
		Resource springResource = new InputStreamResource(inputStream);
		return parseProcessEngineConfiguration(springResource, beanName);
	  }

	  public static ProcessEngineConfiguration parseProcessEngineConfigurationFromResource(string resource, string beanName)
	  {
		Resource springResource = new ClassPathResource(resource);
		return parseProcessEngineConfiguration(springResource, beanName);
	  }

	}

}