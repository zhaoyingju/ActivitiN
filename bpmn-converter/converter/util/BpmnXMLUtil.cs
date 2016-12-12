using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using bpmn_converter.converter;
using bpmn_converter.converter.util;
using org.activiti.bpmn.constants;
using org.activiti.bpmn.converter.child;
using org.activiti.bpmn.model;

namespace org.activiti.bpmn.converter.util{

public class BpmnXMLUtil : BpmnXMLConstants {
  
  private static Dictionary<String, BaseChildElementParser> genericChildParserMap = new Dictionary<String, BaseChildElementParser>();
  
  //static {
  //  addGenericParser(new ActivitiEventListenerParser());
  //  addGenericParser(new CancelEventDefinitionParser());
  //  addGenericParser(new CompensateEventDefinitionParser());
  //  addGenericParser(new ConditionExpressionParser());
  //  addGenericParser(new DataInputAssociationParser());
  //  addGenericParser(new DataOutputAssociationParser());
  //  addGenericParser(new DataStateParser());
  //  addGenericParser(new DocumentationParser());
  //  addGenericParser(new ErrorEventDefinitionParser());
  //  addGenericParser(new ExecutionListenerParser());
  //  addGenericParser(new FieldExtensionParser());
  //  addGenericParser(new FormPropertyParser());
  //  addGenericParser(new IOSpecificationParser());
  //  addGenericParser(new MessageEventDefinitionParser());
  //  addGenericParser(new MultiInstanceParser());
  //  addGenericParser(new SignalEventDefinitionParser());
  //  addGenericParser(new TaskListenerParser());
  //  addGenericParser(new TerminateEventDefinitionParser());
  //  addGenericParser(new TimerEventDefinitionParser());
  //  addGenericParser(new TimeDateParser());
  //  addGenericParser(new TimeCycleParser());
  //  addGenericParser(new TimeDurationParser());
  //  addGenericParser(new FlowNodeRefParser());
  //  addGenericParser(new ActivitiFailedjobRetryParser());
  //}
  
  private static void addGenericParser(BaseChildElementParser parser) {
    genericChildParserMap.Add(parser.getElementName(), parser);
  }

  public static void addXMLLocation(BaseElement element, XMLStreamReader xtr) {
    Location location = xtr.getLocation();
    element.setXmlRowNumber(location.getLineNumber());
    element.setXmlColumnNumber(location.getColumnNumber());
  }
  
  public static void addXMLLocation(GraphicInfo graphicInfo, XMLStreamReader xtr) {
    Location location = xtr.getLocation();
    graphicInfo.setXmlRowNumber(location.getLineNumber());
    graphicInfo.setXmlColumnNumber(location.getColumnNumber());
  }
  
  public static void parseChildElements(String elementName, BaseElement parentElement, XMLStreamReader xtr, BpmnModel model) {
    parseChildElements(elementName, parentElement, xtr, null, model); 
  }
  
  public static void parseChildElements(String elementName, BaseElement parentElement, XMLStreamReader xtr, 
      Dictionary<String, BaseChildElementParser> childParsers, BpmnModel model) {
    
    if (childParsers == null) {
      childParsers = new Dictionary<String, BaseChildElementParser>();
    }
    childParsers.putAll(genericChildParserMap);
    
    bool inExtensionElements = false;
    bool readyWithChildElements = false;
    while (readyWithChildElements == false && xtr.hasNext()) {
      xtr.next();
      if (xtr.isStartElement()) {
        if (ELEMENT_EXTENSIONS.Equals(xtr.getLocalName())) {
          inExtensionElements = true;
        } else if (childParsers.containsKey(xtr.getLocalName())) {
          childParsers.get(xtr.getLocalName()).parseChildElement(xtr, parentElement, model);
        } else if (inExtensionElements) {
          ExtensionElement extensionElement = BpmnXMLUtil.parseExtensionElement(xtr);
          parentElement.addExtensionElement(extensionElement);
        }

      } else if (xtr.isEndElement()) {
        if (ELEMENT_EXTENSIONS.Equals(xtr.getLocalName())) {
          inExtensionElements = false;
        } else if (elementName.equalsIgnoreCase(xtr.getLocalName())) {
          readyWithChildElements = true;
        }
      }
    }
  }
  
  public static ExtensionElement parseExtensionElement(XMLStreamReader xtr) {
    ExtensionElement extensionElement = new ExtensionElement();
    extensionElement.setName(xtr.getLocalName());
    if (!String.IsNullOrWhiteSpace(xtr.getNamespaceURI())) {
      extensionElement.setNamespace(xtr.getNamespaceURI());
    }
    if (!String.IsNullOrWhiteSpace(xtr.getPrefix())) {
      extensionElement.setNamespacePrefix(xtr.getPrefix());
    }
    
    for (int i = 0; i < xtr.getAttributeCount(); i++) {
      ExtensionAttribute extensionAttribute = new ExtensionAttribute();
      extensionAttribute.setName(xtr.getAttributeLocalName(i));
      extensionAttribute.setValue(xtr.getAttributeValue(i));
      if (!String.IsNullOrWhiteSpace(xtr.getAttributeNamespace(i))) {
        extensionAttribute.setNamespace(xtr.getAttributeNamespace(i));
      }
      if (!String.IsNullOrWhiteSpace(xtr.getAttributePrefix(i))) {
        extensionAttribute.setNamespacePrefix(xtr.getAttributePrefix(i));
      }
      extensionElement.addAttribute(extensionAttribute);
    }
    
    bool readyWithExtensionElement = false;
    while (readyWithExtensionElement == false && xtr.hasNext()) {
      xtr.next();
      if (xtr.isCharacters() || XMLStreamReader.CDATA == xtr.getEventType()) {
        if (!String.IsNullOrWhiteSpace(xtr.getText().Trim())) {
          extensionElement.setElementText(xtr.getText().Trim());
        }
      } else if (xtr.isStartElement()) {
        ExtensionElement childExtensionElement = parseExtensionElement(xtr);
        extensionElement.addChildElement(childExtensionElement);
      } else if (xtr.isEndElement() && extensionElement.getName().equalsIgnoreCase(xtr.getLocalName())) {
        readyWithExtensionElement = true;
      }
    }
    return extensionElement;
  }
  
  public static void writeDefaultAttribute(String attributeName, String value, XMLStreamWriter xtw) {
    if (!String.IsNullOrWhiteSpace(value) && "null".equalsIgnoreCase(value) == false) {
      xtw.writeAttribute(attributeName, value);
    }
  }
  
  public static void writeQualifiedAttribute(String attributeName, String value, XMLStreamWriter xtw) {
    if (!String.IsNullOrWhiteSpace(value)) {
      xtw.writeAttribute(ACTIVITI_EXTENSIONS_PREFIX, ACTIVITI_EXTENSIONS_NAMESPACE, attributeName, value);
    }
  }
  
  public static bool writeExtensionElements(BaseElement baseElement, bool didWriteExtensionStartElement, XMLStreamWriter xtw) {
    return didWriteExtensionStartElement = writeExtensionElements(baseElement, didWriteExtensionStartElement, null, xtw);
  }
 
  public static bool writeExtensionElements(BaseElement baseElement, bool didWriteExtensionStartElement, Dictionary<String, String> namespaceMap, XMLStreamWriter xtw) {
    if (baseElement.getExtensionElements().Any()) {
      if (didWriteExtensionStartElement == false) {
        xtw.writeStartElement(ELEMENT_EXTENSIONS);
        didWriteExtensionStartElement = true;
      }
      
      if (namespaceMap == null) {
        namespaceMap = new Dictionary<String, String>();
      }
      
      foreach (List<ExtensionElement> extensionElements  in baseElement.getExtensionElements().Values) {
        foreach (ExtensionElement extensionElement  in extensionElements) {
          writeExtensionElement(extensionElement, namespaceMap, xtw);
        }
      }
    }
    return didWriteExtensionStartElement;
  }
  
  protected static void writeExtensionElement(ExtensionElement extensionElement, Dictionary<String, String> namespaceMap, XMLStreamWriter xtw) {
    if (!String.IsNullOrWhiteSpace(extensionElement.getName())) {
      Dictionary<String, String> localNamespaceMap = new Dictionary<String, String>();
      if (!String.IsNullOrWhiteSpace(extensionElement.getNamespace())) {
        if (!String.IsNullOrWhiteSpace(extensionElement.getNamespacePrefix())) {
          xtw.writeStartElement(extensionElement.getNamespacePrefix(), extensionElement.getName(), extensionElement.getNamespace());
          
          if (namespaceMap.containsKey(extensionElement.getNamespacePrefix()) == false ||
              namespaceMap.get(extensionElement.getNamespacePrefix()).Equals(extensionElement.getNamespace()) == false) {
            
            xtw.writeNamespace(extensionElement.getNamespacePrefix(), extensionElement.getNamespace());
            namespaceMap.Add(extensionElement.getNamespacePrefix(), extensionElement.getNamespace());
            localNamespaceMap.Add(extensionElement.getNamespacePrefix(), extensionElement.getNamespace());
          }
        } else {
          xtw.writeStartElement(extensionElement.getNamespace(), extensionElement.getName());
        }
      } else {
        xtw.writeStartElement(extensionElement.getName());
      }
      
      foreach (List<ExtensionAttribute> attributes  in extensionElement.getAttributes().Values) {
        foreach (ExtensionAttribute attribute  in attributes) {
          if (!String.IsNullOrWhiteSpace(attribute.getName()) && attribute.getValue() != null) {
            if (!String.IsNullOrWhiteSpace(attribute.getNamespace())) {
              if (!String.IsNullOrWhiteSpace(attribute.getNamespacePrefix())) {
                
                if (namespaceMap.containsKey(attribute.getNamespacePrefix()) == false ||
                    namespaceMap.get(attribute.getNamespacePrefix()).Equals(attribute.getNamespace()) == false) {
                  
                  xtw.writeNamespace(attribute.getNamespacePrefix(), attribute.getNamespace());
                  namespaceMap.Add(attribute.getNamespacePrefix(), attribute.getNamespace());
                }
                
                xtw.writeAttribute(attribute.getNamespacePrefix(), attribute.getNamespace(), attribute.getName(), attribute.getValue());
              } else {
                xtw.writeAttribute(attribute.getNamespace(), attribute.getName(), attribute.getValue());
              }
            } else {
              xtw.writeAttribute(attribute.getName(), attribute.getValue());
            }
          }
        }
      }
      
      if (extensionElement.getElementText() != null) {
        xtw.writeCharacters(extensionElement.getElementText());
      } else {
        foreach (List<ExtensionElement> childElements  in extensionElement.getChildElements().Values) {
          foreach (ExtensionElement childElement  in childElements) {
            writeExtensionElement(childElement, namespaceMap, xtw);
          }
        }
      }
      
      foreach (String prefix  in localNamespaceMap.Keys) {
        namespaceMap.remove(prefix);
      }
      
      xtw.writeEndElement();
    }
  }
  
  public static List<String> parseDelimitedList(String s) {
    List<String> result = new List<String>();
    if (!String.IsNullOrWhiteSpace(s)) {

      StringCharacterIterator iterator = new StringCharacterIterator(s);
      char c = iterator.first();

      StringBuilder strb = new StringBuilder();
      bool insideExpression = false;

      while (c != StringCharacterIterator.DONE) {
        if (c == '{' || c == '$') {
          insideExpression = true;
        } else if (c == '}') {
          insideExpression = false;
        } else if (c == ',' && !insideExpression) {
          result.Add(strb.ToString().Trim());
          strb.delete(0, strb.length());
        }

        if (c != ',' || (insideExpression)) {
          strb.append(c);
        }

        c = iterator.next();
      }

      if (strb.length() > 0) {
        result.Add(strb.ToString().Trim());
      }

    }
    return result;
  }
  
  public static String convertToDelimitedString(List<String> stringList) {
    StringBuilder resultString = new StringBuilder();
    
    if(stringList != null) {
    	foreach (String result  in stringList) {
    		if (resultString.length() > 0) {
    			resultString.append(",");
    		}
    		resultString.append(result);
    	}
    }
    return resultString.ToString();
  }

  /**
   * add all attributes from XML to element extensionAttributes (except blackListed).
   *
   * //@param xtr

   * //@param element

   * //@param blackList

   */
  public static void addCustomAttributes(XMLStreamReader xtr, BaseElement element,params List<ExtensionAttribute>[] blackLists) {
    for (int i = 0; i < xtr.getAttributeCount(); i++) {
      ExtensionAttribute extensionAttribute = new ExtensionAttribute();
      extensionAttribute.setName(xtr.getAttributeLocalName(i));
      extensionAttribute.setValue(xtr.getAttributeValue(i));
      if (!String.IsNullOrWhiteSpace(xtr.getAttributeNamespace(i))) {
        extensionAttribute.setNamespace(xtr.getAttributeNamespace(i));
      }
      if (!String.IsNullOrWhiteSpace(xtr.getAttributePrefix(i))) {
        extensionAttribute.setNamespacePrefix(xtr.getAttributePrefix(i));
      }
      if (!isBlacklisted(extensionAttribute, blackLists)) {
        element.addAttribute(extensionAttribute);
      }
    }
  }

  public static void writeCustomAttributes(Collection<List<ExtensionAttribute>> attributes, XMLStreamWriter xtw, List<ExtensionAttribute>... blackLists) {
    writeCustomAttributes(attributes, xtw, new LinkedHashMap<String, String>(), blackLists);
  }
  
  /**
   * write attributes to xtw (except blacklisted)
   * //@param attributes

   * //@param xtw

   * //@param blackList

   */
  public static void writeCustomAttributes(Collection<List<ExtensionAttribute>> attributes, XMLStreamWriter xtw, Dictionary<String, String> namespaceMap,
     params List<ExtensionAttribute>[] blackLists) {
    
    foreach (List<ExtensionAttribute> attributeList  in attributes) {
      if (attributeList != null && !attributeList.isEmpty()) {
        foreach (ExtensionAttribute attribute  in attributeList) {
          if (!isBlacklisted(attribute, blackLists)) {
            if (attribute.getNamespacePrefix() == null) {
              if (attribute.getNamespace() == null)
                xtw.writeAttribute(attribute.getName(), attribute.getValue());
              else {
                xtw.writeAttribute(attribute.getNamespace(), attribute.getName(), attribute.getValue());
              }
            } else {
              if (!namespaceMap.ContainsKey(attribute.getNamespacePrefix())) {
                namespaceMap.Add(attribute.getNamespacePrefix(), attribute.getNamespace());
                xtw.writeNamespace(attribute.getNamespacePrefix(), attribute.getNamespace());
              }
              xtw.writeAttribute(attribute.getNamespacePrefix(), attribute.getNamespace(),
                  attribute.getName(), attribute.getValue());
            }
          }
        }
      }
    }
  }

  public static bool isBlacklisted(ExtensionAttribute attribute, params List<ExtensionAttribute>[] blackLists) {
    if (blackLists != null) {
      foreach (var blackList  in blackLists) {
        foreach (ExtensionAttribute blackAttribute  in blackList) {
          if (blackAttribute.getName().Equals(attribute.getName())) {
            if ( blackAttribute.getNamespace() != null && attribute.getNamespace() != null
                && blackAttribute.getNamespace().Equals(attribute.getNamespace()))
              return true;
            if (blackAttribute.getNamespace() == null && attribute.getNamespace() == null)
              return true;
          }
        }
      }
    }
    return false;
  }
}
