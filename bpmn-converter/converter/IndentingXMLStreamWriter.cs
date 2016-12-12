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
using System.Collections.Generic;
using System.Text;
using bpmn_converter.converter;

namespace org.activiti.bpmn.converter{






/**
 * //@author Tijs Rademakers

 */
public class IndentingXMLStreamWriter : DelegatingXMLStreamWriter {

  private  static Object SEEN_NOTHING = new Object();
  private  static Object SEEN_ELEMENT = new Object();
  private  static Object SEEN_DATA = new Object();

  private Object state = SEEN_NOTHING;
  private Stack<Object> stateStack = new Stack<Object>();

  private String indentStep = "  ";
  private int depth = 0;

  public IndentingXMLStreamWriter(XMLStreamWriter writer) {
    super(writer);
  }

  /**
   * Return the current indent step.
   * 
   * <p>
   * Return the current indent step: each start tag will be indented by this
   * number of spaces times the number of ancestors that the element has.
   * </p>
   * 
   * //@return The number of spaces in each indentation step, or 0 or less for no

   *         indentation.
   * //@see #setIndentStep(int)

   * 
   * //@deprecated Only return the length of the indent string.

   */
  public int getIndentStep() {
    return indentStep.length();
  }

  /**
   * Set the current indent step.
   * 
   * //@param indentStep

   *          The new indent step (0 or less for no indentation).
   * //@see #getIndentStep()

   * 
   * //@deprecated Should use the version that takes string.

   */
  public void setIndentStep(int indentStep) {
    StringBuilder s = new StringBuilder();
    for (; indentStep > 0; indentStep--)
      s.append(' ');
    setIndentStep(s.ToString());
  }

  public void setIndentStep(String s) {
    this.indentStep = s;
  }

  private void onStartElement() {
    stateStack.push(SEEN_ELEMENT);
    state = SEEN_NOTHING;
    if (depth > 0) {
      super.writeCharacters("\n");
    }
    doIndent();
    depth++;
  }

  private void onEndElement() {
    depth--;
    if (state == SEEN_ELEMENT) {
      super.writeCharacters("\n");
      doIndent();
    }
    state = stateStack.pop();
  }

  private void onEmptyElement() {
    state = SEEN_ELEMENT;
    if (depth > 0) {
      super.writeCharacters("\n");
    }
    doIndent();
  }

  /**
   * Print indentation for the current level.
   * 
   * //@exception org.xml.sax.SAXException

   *              If there is an error writing the indentation characters, or if
   *              a filter further down the chain raises an exception.
   */
  private void doIndent() {
    if (depth > 0) {
      for (int i = 0; i < depth; i++)
        super.writeCharacters(indentStep);
    }
  }

  public void writeStartDocument() {
    super.writeStartDocument();
    super.writeCharacters("\n");
  }

  public void writeStartDocument(String version) {
    super.writeStartDocument(version);
    super.writeCharacters("\n");
  }

  public void writeStartDocument(String encoding, String version)
      {
    super.writeStartDocument(encoding, version);
    super.writeCharacters("\n");
  }

  public void writeStartElement(String localName) {
    onStartElement();
    super.writeStartElement(localName);
  }

  public void writeStartElement(String namespaceURI, String localName)
      {
    onStartElement();
    super.writeStartElement(namespaceURI, localName);
  }

  public void writeStartElement(String prefix, String localName,
      String namespaceURI) {
    onStartElement();
    super.writeStartElement(prefix, localName, namespaceURI);
  }

  public void writeEmptyElement(String namespaceURI, String localName)
      {
    onEmptyElement();
    super.writeEmptyElement(namespaceURI, localName);
  }

  public void writeEmptyElement(String prefix, String localName,
      String namespaceURI) {
    onEmptyElement();
    super.writeEmptyElement(prefix, localName, namespaceURI);
  }

  public void writeEmptyElement(String localName) {
    onEmptyElement();
    super.writeEmptyElement(localName);
  }

  public void writeEndElement() {
    onEndElement();
    super.writeEndElement();
  }

  public void writeCharacters(String text) {
    state = SEEN_DATA;
    super.writeCharacters(text);
  }

  public void writeCharacters(char[] text, int start, int len)
      {
    state = SEEN_DATA;
    super.writeCharacters(text, start, len);
  }

  public void writeCData(String data) {
    state = SEEN_DATA;
    super.writeCData(data);
  }

}
