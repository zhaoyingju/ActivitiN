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
using bpmn_converter.converter;
using bpmn_converter.converter.util;
using org.activiti.bpmn.converter.child;
using org.activiti.bpmn.converter.util;
using org.activiti.bpmn.model;

namespace org.activiti.bpmn.converter
{






















/**
 * //@author Tijs Rademakers, Saeid Mirzaei

 */

    public class UserTaskXMLConverter : BaseBpmnXMLConverter
    {

        protected Dictionary<String, BaseChildElementParser> childParserMap =
            new Dictionary<String, BaseChildElementParser>();

        /** default attributes taken from bpmn spec and from activiti extension */

        protected static List<ExtensionAttribute> defaultUserTaskAttributes = Arrays.asList(
            new ExtensionAttribute(ACTIVITI_EXTENSIONS_NAMESPACE, ATTRIBUTE_FORM_FORMKEY),
            new ExtensionAttribute(ACTIVITI_EXTENSIONS_NAMESPACE, ATTRIBUTE_TASK_USER_DUEDATE),
            new ExtensionAttribute(ACTIVITI_EXTENSIONS_NAMESPACE, ATTRIBUTE_TASK_USER_ASSIGNEE),
            new ExtensionAttribute(ACTIVITI_EXTENSIONS_NAMESPACE, ATTRIBUTE_TASK_USER_PRIORITY),
            new ExtensionAttribute(ACTIVITI_EXTENSIONS_NAMESPACE, ATTRIBUTE_TASK_USER_CANDIDATEUSERS),
            new ExtensionAttribute(ACTIVITI_EXTENSIONS_NAMESPACE, ATTRIBUTE_TASK_USER_CANDIDATEGROUPS),
            new ExtensionAttribute(ACTIVITI_EXTENSIONS_NAMESPACE, ATTRIBUTE_TASK_USER_CATEGORY),
            new ExtensionAttribute(ACTIVITI_EXTENSIONS_NAMESPACE, ATTRIBUTE_TASK_USER_SKIP_EXPRESSION)
            );

        public UserTaskXMLConverter()
        {
            HumanPerformerParser humanPerformerParser = new HumanPerformerParser();
            childParserMap.Add(humanPerformerParser.getElementName(), humanPerformerParser);
            PotentialOwnerParser potentialOwnerParser = new PotentialOwnerParser();
            childParserMap.Add(potentialOwnerParser.getElementName(), potentialOwnerParser);
            CustomIdentityLinkParser customIdentityLinkParser = new CustomIdentityLinkParser();
            childParserMap.Add(customIdentityLinkParser.getElementName(), customIdentityLinkParser);
        }

        protected override Type getBpmnElementType()
        {
            return typeof (UserTask);
        }

        //@Override

        protected override String getXMLElementName()
        {
            return ELEMENT_TASK_USER;
        }

        //@Override

        //@SuppressWarnings("unchecked")

        protected BaseElement convertXMLToElement(XMLStreamReader xtr, BpmnModel model)
        {
            String formKey = xtr.getAttributeValue(ACTIVITI_EXTENSIONS_NAMESPACE, ATTRIBUTE_FORM_FORMKEY);
            UserTask userTask = null;
            if (!String.IsNullOrWhiteSpace(formKey))
            {
                if (model.getUserTaskFormTypes() != null && model.getUserTaskFormTypes().contains(formKey))
                {
                    userTask = new AlfrescoUserTask();
                }
            }
            if (userTask == null)
            {
                userTask = new UserTask();
            }
            BpmnXMLUtil.addXMLLocation(userTask, xtr);
            userTask.setDueDate(xtr.getAttributeValue(ACTIVITI_EXTENSIONS_NAMESPACE, ATTRIBUTE_TASK_USER_DUEDATE));
            userTask.setCategory(xtr.getAttributeValue(ACTIVITI_EXTENSIONS_NAMESPACE, ATTRIBUTE_TASK_USER_CATEGORY));
            userTask.setFormKey(formKey);
            userTask.setAssignee(xtr.getAttributeValue(ACTIVITI_EXTENSIONS_NAMESPACE, ATTRIBUTE_TASK_USER_ASSIGNEE));
            userTask.setOwner(xtr.getAttributeValue(ACTIVITI_EXTENSIONS_NAMESPACE, ATTRIBUTE_TASK_USER_OWNER));
            userTask.setPriority(xtr.getAttributeValue(ACTIVITI_EXTENSIONS_NAMESPACE, ATTRIBUTE_TASK_USER_PRIORITY));

            if (
                !String.IsNullOrWhiteSpace(xtr.getAttributeValue(ACTIVITI_EXTENSIONS_NAMESPACE,
                    ATTRIBUTE_TASK_USER_CANDIDATEUSERS)))
            {
                String expression = xtr.getAttributeValue(ACTIVITI_EXTENSIONS_NAMESPACE,
                    ATTRIBUTE_TASK_USER_CANDIDATEUSERS);
                userTask.getCandidateUsers().AddRange(parseDelimitedList(expression));
            }

            if (
                !String.IsNullOrWhiteSpace(xtr.getAttributeValue(ACTIVITI_EXTENSIONS_NAMESPACE,
                    ATTRIBUTE_TASK_USER_CANDIDATEGROUPS)))
            {
                String expression = xtr.getAttributeValue(ACTIVITI_EXTENSIONS_NAMESPACE,
                    ATTRIBUTE_TASK_USER_CANDIDATEGROUPS);
                userTask.getCandidateGroups().AddRange(parseDelimitedList(expression));
            }

            if (
                !String.IsNullOrWhiteSpace(xtr.getAttributeValue(ACTIVITI_EXTENSIONS_NAMESPACE,
                    ATTRIBUTE_TASK_USER_SKIP_EXPRESSION)))
            {
                String expression = xtr.getAttributeValue(ACTIVITI_EXTENSIONS_NAMESPACE,
                    ATTRIBUTE_TASK_USER_SKIP_EXPRESSION);
                userTask.setSkipExpression(expression);
            }

            BpmnXMLUtil.addCustomAttributes(xtr, userTask, defaultElementAttributes,
                defaultActivityAttributes, defaultUserTaskAttributes);

            parseChildElements(getXMLElementName(), userTask, childParserMap, model, xtr);

            return userTask;
        }

        //@Override

        //@SuppressWarnings("unchecked")

        protected void writeAdditionalAttributes(BaseElement element, BpmnModel model, XMLStreamWriter xtw)
        {
            UserTask userTask = (UserTask) element;
            writeQualifiedAttribute(ATTRIBUTE_TASK_USER_ASSIGNEE, userTask.getAssignee(), xtw);
            writeQualifiedAttribute(ATTRIBUTE_TASK_USER_OWNER, userTask.getOwner(), xtw);
            writeQualifiedAttribute(ATTRIBUTE_TASK_USER_CANDIDATEUSERS,
                convertToDelimitedString(userTask.getCandidateUsers()), xtw);
            writeQualifiedAttribute(ATTRIBUTE_TASK_USER_CANDIDATEGROUPS,
                convertToDelimitedString(userTask.getCandidateGroups()), xtw);
            writeQualifiedAttribute(ATTRIBUTE_TASK_USER_DUEDATE, userTask.getDueDate(), xtw);
            writeQualifiedAttribute(ATTRIBUTE_TASK_USER_CATEGORY, userTask.getCategory(), xtw);
            writeQualifiedAttribute(ATTRIBUTE_FORM_FORMKEY, userTask.getFormKey(), xtw);
            if (userTask.getPriority() != null)
            {
                writeQualifiedAttribute(ATTRIBUTE_TASK_USER_PRIORITY, userTask.getPriority().ToString(), xtw);
            }
            if (userTask.getSkipExpression() != null)
            {
                writeQualifiedAttribute(ATTRIBUTE_TASK_USER_SKIP_EXPRESSION, userTask.getSkipExpression(), xtw);
            }
            // write custom attributes
            BpmnXMLUtil.writeCustomAttributes(userTask.getAttributes().Values, xtw, defaultElementAttributes,
                defaultActivityAttributes, defaultUserTaskAttributes);
        }

        //@Override

        protected bool writeExtensionChildElements(BaseElement element, bool didWriteExtensionStartElement,
            XMLStreamWriter xtw)
        {
            UserTask userTask = (UserTask) element;
            didWriteExtensionStartElement = writeFormProperties(userTask, didWriteExtensionStartElement, xtw);
            didWriteExtensionStartElement = writeCustomIdentities(element, didWriteExtensionStartElement, xtw);
            return didWriteExtensionStartElement;
        }

        protected bool writeCustomIdentities(BaseElement element, bool didWriteExtensionStartElement,
            XMLStreamWriter xtw)
        {
            UserTask userTask = (UserTask) element;
            if (userTask.getCustomUserIdentityLinks().isEmpty() && userTask.getCustomGroupIdentityLinks().isEmpty())
                return didWriteExtensionStartElement;


            if (didWriteExtensionStartElement == false)
            {
                xtw.writeStartElement(ELEMENT_EXTENSIONS);
                didWriteExtensionStartElement = true;
            }
            List<String> identityLinkTypes = new List<String>();
            identityLinkTypes.AddRange(userTask.getCustomUserIdentityLinks().Keys);
            identityLinkTypes.AddRange(userTask.getCustomGroupIdentityLinks().Keys);
            foreach (String identityType  in identityLinkTypes)
            {
                writeCustomIdentities(userTask, identityType, userTask.getCustomUserIdentityLinks().get(identityType),
                    userTask.getCustomGroupIdentityLinks().get(identityType), xtw);
            }

            return didWriteExtensionStartElement;
        }

        protected void writeCustomIdentities(UserTask userTask, String identityType, List<String> users,
            List<String> groups, XMLStreamWriter xtw)
        {
            xtw.writeStartElement(ACTIVITI_EXTENSIONS_PREFIX, ELEMENT_CUSTOM_RESOURCE, ACTIVITI_EXTENSIONS_NAMESPACE);
            writeDefaultAttribute(ATTRIBUTE_NAME, identityType, xtw);

            List<String> identityList = new List<String>();

            if (users != null)
            {
                foreach (String userId in users)
                {
                    identityList.Add("user(" + userId + ")");
                }
            }

            if (groups != null)
            {
                foreach (String groupId in groups)
                {
                    identityList.Add("group(" + groupId + ")");
                }
            }

            String delimitedString = convertToDelimitedString(identityList);

            xtw.writeStartElement(ELEMENT_RESOURCE_ASSIGNMENT);
            xtw.writeStartElement(ELEMENT_FORMAL_EXPRESSION);
            xtw.writeCharacters(delimitedString);
            xtw.writeEndElement(); // End ELEMENT_FORMAL_EXPRESSION
            xtw.writeEndElement(); // End ELEMENT_RESOURCE_ASSIGNMENT

            xtw.writeEndElement(); // End ELEMENT_CUSTOM_RESOURCE
        }

        //@Override

        protected override void writeAdditionalChildElements(BaseElement element, BpmnModel model, XMLStreamWriter xtw)
        {
        }

        public class HumanPerformerParser : BaseChildElementParser
        {

            public override String getElementName()
            {
                return "humanPerformer";
            }

            public override void parseChildElement(XMLStreamReader xtr, BaseElement parentElement, BpmnModel model)
            {
                String resourceElement = XMLStreamReaderUtil.moveDown(xtr);
                if (!String.IsNullOrWhiteSpace(resourceElement) && ELEMENT_RESOURCE_ASSIGNMENT.Equals(resourceElement))
                {
                    String expression = XMLStreamReaderUtil.moveDown(xtr);
                    if (!String.IsNullOrWhiteSpace(expression) && ELEMENT_FORMAL_EXPRESSION.Equals(expression))
                    {
                        ((UserTask) parentElement).setAssignee(xtr.getElementText());
                    }
                }
            }
        }



        public class PotentialOwnerParser : BaseChildElementParser
        {

            public override String getElementName()
            {
                return "potentialOwner";
            }



            public override void parseChildElement(XMLStreamReader xtr, BaseElement parentElement, BpmnModel model)
            {
                String resourceElement = XMLStreamReaderUtil.moveDown(xtr);
                if (!String.IsNullOrWhiteSpace(resourceElement) && ELEMENT_RESOURCE_ASSIGNMENT.Equals(resourceElement))
                {
                    String expression = XMLStreamReaderUtil.moveDown(xtr);
                    if (!String.IsNullOrWhiteSpace(expression) && ELEMENT_FORMAL_EXPRESSION.Equals(expression))
                    {

                        List<String> assignmentList = CommaSplitter.splitCommas(xtr.getElementText());

                        foreach (String assignmentValue  in assignmentList)
                        {
                            if (assignmentValue == null)
                            {
                                continue;
                            }

                            assignmentValue = assignmentValue.Trim();

                            if (assignmentValue.length() == 0)
                            {
                                continue;
                            }

                            String userPrefix = "user(";
                            String groupPrefix = "group(";
                            if (assignmentValue.StartsWith(userPrefix))
                            {
                                assignmentValue =
                                    assignmentValue.Substring(userPrefix.length(), assignmentValue.length() - 1).Trim();
                                ((UserTask) parentElement).getCandidateUsers().Add(assignmentValue);
                            }
                            else if (assignmentValue.StartsWith(groupPrefix))
                            {
                                assignmentValue =
                                    assignmentValue.Substring(groupPrefix.length(), assignmentValue.length() - 1).Trim();
                                ((UserTask) parentElement).getCandidateGroups().Add(assignmentValue);
                            }
                            else
                            {
                                ((UserTask) parentElement).getCandidateGroups().Add(assignmentValue);
                            }
                        }
                    }
                }
            }
        }

        public class CustomIdentityLinkParser : BaseChildElementParser
        {

            public override String getElementName()
            {
                return ELEMENT_CUSTOM_RESOURCE;
            }

            public override void parseChildElement(XMLStreamReader xtr, BaseElement parentElement, BpmnModel model)
            {
                String identityLinkType = xtr.getAttributeValue(ACTIVITI_EXTENSIONS_NAMESPACE, ATTRIBUTE_NAME);

                // the attribute value may be unqualified
                if (identityLinkType == null)
                {
                    identityLinkType = xtr.getAttributeValue(null, ATTRIBUTE_NAME);
                }

                if (identityLinkType == null) return;

                String resourceElement = XMLStreamReaderUtil.moveDown(xtr);
                if (!String.IsNullOrWhiteSpace(resourceElement) && ELEMENT_RESOURCE_ASSIGNMENT.Equals(resourceElement))
                {
                    String expression = XMLStreamReaderUtil.moveDown(xtr);
                    if (!String.IsNullOrWhiteSpace(expression) && ELEMENT_FORMAL_EXPRESSION.Equals(expression))
                    {

                        List<String> assignmentList = CommaSplitter.splitCommas(xtr.getElementText());

                        foreach (String assignmentValue  in assignmentList)
                        {
                            if (assignmentValue == null)
                            {
                                continue;
                            }

                            assignmentValue = assignmentValue.Trim();

                            if (assignmentValue.length() == 0)
                            {
                                continue;
                            }

                            String userPrefix = "user(";
                            String groupPrefix = "group(";
                            if (assignmentValue.StartsWith(userPrefix))
                            {
                                assignmentValue =
                                    assignmentValue.Substring(userPrefix.length(), assignmentValue.length() - 1).Trim();
                                ((UserTask) parentElement).addCustomUserIdentityLink(assignmentValue, identityLinkType);
                            }
                            else if (assignmentValue.StartsWith(groupPrefix))
                            {
                                assignmentValue =
                                    assignmentValue.Substring(groupPrefix.length(), assignmentValue.length() - 1).Trim();
                                ((UserTask) parentElement).addCustomGroupIdentityLink(assignmentValue, identityLinkType);
                            }
                            else
                            {
                                ((UserTask) parentElement).addCustomGroupIdentityLink(assignmentValue, identityLinkType);
                            }
                        }
                    }
                }
            }
        }
    }
}