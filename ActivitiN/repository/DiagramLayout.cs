using System;
using System.Collections.Generic;

namespace org.activiti.engine.repository
{



    /// <summary>
    /// 存储二维图形布局。
    /// </summary>
    [Serializable]
    public class DiagramLayout
    {
        private IDictionary<string, DiagramElement> elements;

        public DiagramLayout(IDictionary<string, DiagramElement> elements)
        {
            this.Elements = elements;
        }

        public virtual DiagramNode getNode(string id)
        {
            DiagramElement element = Elements[id];
            if (element is DiagramNode)
            {
                return (DiagramNode)element;
            }
            else
            {
                return null;
            }
        }

        public virtual DiagramEdge getEdge(string id)
        {
            DiagramElement element = Elements[id];
            if (element is DiagramEdge)
            {
                return (DiagramEdge)element;
            }
            else
            {
                return null;
            }
        }

        public virtual IDictionary<string, DiagramElement> Elements
        {
            get
            {
                return elements;
            }
            set
            {
                this.elements = value;
            }
        }

        public virtual IList<DiagramNode> Nodes
        {
            get
            {
                IList<DiagramNode> nodes = new List<DiagramNode>();
                foreach (KeyValuePair<string, DiagramElement> entry in Elements)
                {
                    DiagramElement element = entry.Value;
                    if (element is DiagramNode)
                    {
                        nodes.Add((DiagramNode)element);
                    }
                }
                return nodes;
            }
        }

    }
}