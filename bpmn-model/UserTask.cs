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

namespace org.activiti.bpmn.model
{








/**
 * //@author Tijs Rademakers
 */

    public class UserTask : TaskActivity
    {

        protected String assignee;
        protected String owner;
        protected String priority;
        protected String formKey;
        protected String dueDate;
        protected String category;
        protected List<String> candidateUsers = new List<String>();
        protected List<String> candidateGroups = new List<String>();
        protected List<FormProperty> formProperties = new List<FormProperty>();
        protected List<ActivitiListener> taskListeners = new List<ActivitiListener>();
        protected String skipExpression;

        protected Dictionary<String, List<String>> customUserIdentityLinks = new Dictionary<String, List<string>>();
        protected Dictionary<String, List<String>> customGroupIdentityLinks = new Dictionary<String, List<string>>();

        public String getAssignee()
        {
            return assignee;
        }

        public void setAssignee(String assignee)
        {
            this.assignee = assignee;
        }

        public String getOwner()
        {
            return owner;
        }

        public void setOwner(String owner)
        {
            this.owner = owner;
        }

        public String getPriority()
        {
            return priority;
        }

        public void setPriority(String priority)
        {
            this.priority = priority;
        }

        public String getFormKey()
        {
            return formKey;
        }

        public void setFormKey(String formKey)
        {
            this.formKey = formKey;
        }

        public String getDueDate()
        {
            return dueDate;
        }

        public void setDueDate(String dueDate)
        {
            this.dueDate = dueDate;
        }

        public String getCategory()
        {
            return category;
        }

        public void setCategory(String category)
        {
            this.category = category;
        }

        public List<String> getCandidateUsers()
        {
            return candidateUsers;
        }

        public void setCandidateUsers(List<String> candidateUsers)
        {
            this.candidateUsers = candidateUsers;
        }

        public List<String> getCandidateGroups()
        {
            return candidateGroups;
        }

        public void setCandidateGroups(List<String> candidateGroups)
        {
            this.candidateGroups = candidateGroups;
        }

        public List<FormProperty> getFormProperties()
        {
            return formProperties;
        }

        public void setFormProperties(List<FormProperty> formProperties)
        {
            this.formProperties = formProperties;
        }

        public List<ActivitiListener> getTaskListeners()
        {
            return taskListeners;
        }

        public void setTaskListeners(List<ActivitiListener> taskListeners)
        {
            this.taskListeners = taskListeners;
        }

        public void addCustomUserIdentityLink(String userId, String type)
        {
            List<String> userIdentitySet = customUserIdentityLinks[type];

            if (userIdentitySet == null)
            {
                userIdentitySet = new List<String>();
                customUserIdentityLinks.Add(type, userIdentitySet);
            }

            userIdentitySet.Add(userId);
        }

        public void addCustomGroupIdentityLink(String groupId, String type)
        {
            List<String> groupIdentitySet = customGroupIdentityLinks[type];

            if (groupIdentitySet == null)
            {
                groupIdentitySet = new List<String>();
                customGroupIdentityLinks.Add(type, groupIdentitySet);
            }

            groupIdentitySet.Add(groupId);
        }

        public Dictionary<String, List<String>> getCustomUserIdentityLinks()
        {
            return customUserIdentityLinks;
        }

        public void setCustomUserIdentityLinks(
            Dictionary<String, List<String>> customUserIdentityLinks)
        {
            this.customUserIdentityLinks = customUserIdentityLinks;
        }

        public Dictionary<String, List<String>> getCustomGroupIdentityLinks()
        {
            return customGroupIdentityLinks;
        }

        public void setCustomGroupIdentityLinks(Dictionary<String, List<String>> customGroupIdentityLinks)
        {
            this.customGroupIdentityLinks = customGroupIdentityLinks;
        }

        public String getSkipExpression()
        {
            return skipExpression;
        }

        public void setSkipExpression(String skipExpression)
        {
            this.skipExpression = skipExpression;
        }

        public override Object clone()
        {
            UserTask clone = new UserTask();
            clone.setValues(this);
            return clone;
        }

        public void setValues(UserTask otherElement)
        {
            base.setValues(otherElement);
            setAssignee(otherElement.getAssignee());
            setOwner(otherElement.getOwner());
            setFormKey(otherElement.getFormKey());
            setDueDate(otherElement.getDueDate());
            setPriority(otherElement.getPriority());
            setCategory(otherElement.getCategory());

            setCandidateGroups(new List<String>(otherElement.getCandidateGroups()));
            setCandidateUsers(new List<String>(otherElement.getCandidateUsers()));

            setCustomGroupIdentityLinks(otherElement.customGroupIdentityLinks);
            setCustomUserIdentityLinks(otherElement.customUserIdentityLinks);

            formProperties = new List<FormProperty>();
            if (otherElement.getFormProperties() != null && otherElement.getFormProperties().Any())
            {
                foreach (FormProperty property in otherElement.getFormProperties())
                {
                    formProperties.Add((FormProperty)property.clone());
                }
            }

            taskListeners = new List<ActivitiListener>();
            if (otherElement.getTaskListeners() != null && otherElement.getTaskListeners().Any())
            {
                foreach (ActivitiListener listener in otherElement.getTaskListeners())
                {
                    taskListeners.Add((ActivitiListener)listener.clone());
                }
            }
        }
    }
}