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
using bpmn_converter.converter;

namespace org.activiti.bpmn.converter{






/**
 * //@author Tijs Rademakers

 */
abstract class DelegatingXMLStreamWriter : XMLStreamWriter {
  
   private  XMLStreamWriter writer;

   public DelegatingXMLStreamWriter(XMLStreamWriter writer) {
       this.writer = writer;
   }

   public void writeStartElement(String localName) {
       writer.writeStartElement(localName);
   }

   public void writeStartElement(String namespaceURI, String localName) {
       writer.writeStartElement(namespaceURI, localName);
   }

   public void writeStartElement(String prefix, String localName, String namespaceURI) {
       writer.writeStartElement(prefix, localName, namespaceURI);
   }

   public void writeEmptyElement(String namespaceURI, String localName) {
       writer.writeEmptyElement(namespaceURI, localName);
   }

   public void writeEmptyElement(String prefix, String localName, String namespaceURI) {
       writer.writeEmptyElement(prefix, localName, namespaceURI);
   }

   public void writeEmptyElement(String localName) {
       writer.writeEmptyElement(localName);
   }

   public void writeEndElement() {
       writer.writeEndElement();
   }

   public void writeEndDocument() {
       writer.writeEndDocument();
   }

   public void close() {
       writer.close();
   }

   public void flush() {
       writer.flush();
   }

   public void writeAttribute(String localName, String value) {
       writer.writeAttribute(localName, value);
   }

   public void writeAttribute(String prefix, String namespaceURI, String localName, String value) {
       writer.writeAttribute(prefix, namespaceURI, localName, value);
   }

   public void writeAttribute(String namespaceURI, String localName, String value) {
       writer.writeAttribute(namespaceURI, localName, value);
   }

   public void writeNamespace(String prefix, String namespaceURI) {
       writer.writeNamespace(prefix, namespaceURI);
   }

   public void writeDefaultNamespace(String namespaceURI) {
       writer.writeDefaultNamespace(namespaceURI);
   }

   public void writeComment(String data) {
       writer.writeComment(data);
   }

   public void writeProcessingInstruction(String target) {
       writer.writeProcessingInstruction(target);
   }

   public void writeProcessingInstruction(String target, String data) {
       writer.writeProcessingInstruction(target, data);
   }

   public void writeCData(String data) {
       writer.writeCData(data);
   }

   public void writeDTD(String dtd) {
       writer.writeDTD(dtd);
   }

   public void writeEntityRef(String name) {
       writer.writeEntityRef(name);
   }

   public void writeStartDocument() {
       writer.writeStartDocument();
   }

   public void writeStartDocument(String version) {
       writer.writeStartDocument(version);
   }

   public void writeStartDocument(String encoding, String version) {
       writer.writeStartDocument(encoding, version);
   }

   public void writeCharacters(String text) {
       writer.writeCharacters(text);
   }

   public void writeCharacters(char[] text, int start, int len) {
       writer.writeCharacters(text, start, len);
   }

   public String getPrefix(String uri) {
       return writer.getPrefix(uri);
   }

   public void setPrefix(String prefix, String uri) {
       writer.setPrefix(prefix, uri);
   }

   public void setDefaultNamespace(String uri) {
       writer.setDefaultNamespace(uri);
   }

   public void setNamespaceContext(NamespaceContext context) {
       writer.setNamespaceContext(context);
   }

   public NamespaceContext getNamespaceContext() {
       return writer.getNamespaceContext();
   }

   public Object getProperty(String name) {
       return writer.getProperty(name);
   }
}
