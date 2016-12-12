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

using System;
using bpmn_converter.converter.util;

namespace org.activiti.bpmn.converter{







/**
 * //@author Tijs Rademakers

 */
public class XMLStreamReaderUtil {
  
  protected static  ILog LOGGER = LogManager.GetLogger(typeof(XMLStreamReaderUtil));

  public static String moveDown(XMLStreamReader xtr) {
    try {
      while (xtr.hasNext()) {
        int Event = xtr.next();
        switch ( Event ) {
          case XMLStreamConstants.END_DOCUMENT:
            return null;
          case XMLStreamConstants.START_ELEMENT:
            return xtr.getLocalName();
          case XMLStreamConstants.END_ELEMENT:
            return null;
        }
      }
    } catch (Exception e) {
      LOGGER.warn("Error while moving down in XML document", e);
    }
    return null;
  }
  
  public static bool moveToEndOfElement(XMLStreamReader xtr, String elementName ) {
    try {
      while (xtr.hasNext() ) {
        int Event = xtr.next();
        switch ( Event ) {
          case XMLStreamConstants.END_DOCUMENT:
            return false;
          case XMLStreamConstants.END_ELEMENT:
            if (xtr.getLocalName().Equals(elementName))
              return true;
          break;
        }
      }
    } catch (Exception e) {
      LOGGER.warn("Error while moving to end of element {}", elementName, e);
    }
    return false;
  }
}
