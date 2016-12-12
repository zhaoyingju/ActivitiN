using System;

namespace org.activiti.engine
{
    public class ActivitiObjectNotFoundException : ActivitiException
    {

        private Type objectClass;

        public ActivitiObjectNotFoundException(string message) : base(message)
        {
        }

        public ActivitiObjectNotFoundException(string message, Type objectClass) : this(message, objectClass, null)
        {
        }

        public ActivitiObjectNotFoundException(Type objectClass) : this(null, objectClass, null)
        {
        }

        public ActivitiObjectNotFoundException(string message, Type objectClass, Exception cause) : base(message, cause)
        {
            this.objectClass = objectClass;
        }

        /// <summary>
        /// The class of the object that was not found. Contains the 
        /// interface-class of the activiti-object that was not found.
        /// </summary>
        public virtual Type ObjectClass
        {
            get
            {
                return objectClass;
            }
        }
    }

}