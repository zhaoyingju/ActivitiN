using System;
using System.Collections.Generic;

namespace org.activiti.bpmn.model
{




/**
 * interface for accessing Element attributes.
 *
 * //@author Martin Grofcik
 */

    public interface HasExtensionAttributes
    {
        /** get element's attributes */
        Dictionary<String, List<ExtensionAttribute>> getAttributes();

        /**
   * return value of the attribute from given Namespace with given name.
   *
   * //@param Namespace
   * //@param name
   * //@return attribute value or null in case when attribute was not found
   */
        String getAttributeValue(String Namespace, String name);

        /** Add attribute to the object */
        void addAttribute(ExtensionAttribute attribute);

        /** set all object's attributes */
        void setAttributes(Dictionary<String, List<ExtensionAttribute>> attributes);
    }
}