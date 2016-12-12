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
using System.Linq;
using bpmn_converter.converter.util;
using org.activiti.bpmn.constants;
using org.activiti.bpmn.model;

namespace org.activiti.bpmn.converter.parser
{

    public class PotentialStarterParser : BpmnXMLConstants
    {

        public void parse(XMLStreamReader xtr, Process activeProcess)
        {
            String resourceElement = XMLStreamReaderUtil.moveDown(xtr);
            if (!String.IsNullOrWhiteSpace(resourceElement) && "resourceAssignmentExpression".Equals(resourceElement))
            {
                String expression = XMLStreamReaderUtil.moveDown(xtr);
                if (!String.IsNullOrWhiteSpace(expression) && "formalExpression".Equals(expression))
                {
                    List<String> assignmentList = new List<String>();
                    String assignmentText = xtr.getElementText();
                    if (assignmentText.Contains(","))
                    {
                        String[] assignmentArray = assignmentText.Split(',');
                        assignmentList = assignmentArray.ToList();
                    }
                    else
                    {
                        assignmentList.Add(assignmentText);
                    }
                    foreach (String assignmentValue  in assignmentList)
                    {
                        if (assignmentValue == null)
                            continue;
                        var value = assignmentValue.Trim();
                        if (value.length() == 0)
                            continue;

                        String userPrefix = "user(";
                        String groupPrefix = "group(";
                        if (value.StartsWith(userPrefix))
                        {
                            value = value.Substring(userPrefix.length(), value.length() - 1).Trim();
                            activeProcess.getCandidateStarterUsers().Add(value);
                        }
                        else if (value.StartsWith(groupPrefix))
                        {
                            value = value.Substring(groupPrefix.length(), value.length() - 1).Trim();
                            activeProcess.getCandidateStarterGroups().Add(value);
                        }
                        else
                        {
                            activeProcess.getCandidateStarterGroups().Add(value);
                        }
                    }
                }
            }
        }
    }
}