using System;

namespace org.activiti.engine.repository
{

    /// <summary>
    /// ��ʾͼ�ڵ㡣
    /// </summary>
    [Serializable]
    public abstract class DiagramElement
    {
        protected internal string id = null;

        public DiagramElement()
        {
        }

        public DiagramElement(string id)
        {
            this.id = id;
        }

        /// <summary>
        /// Id of the diagram element.
        /// </summary>
        public virtual string Id
        {
            get
            {
                return id;
            }
            set
            {
                this.id = value;
            }
        }


        public override string ToString()
        {
            return "id=" + Id;
        }

        public abstract bool Node { get; }
        public abstract bool Edge { get; }

    }
}