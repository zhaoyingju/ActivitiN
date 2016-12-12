using System;
using System.Xml.Linq;

namespace org.activiti.bpmn.model.parse
{





    public class Warning
    {

        protected String warningMessage;
        protected String resource;
        protected int line;
        protected int column;

        //public Warning(String warningMessage, XElement xtr)
        //{
        //    this.warningMessage = warningMessage;
        //    this.resource = xtr.Name.ToString();
        //    this.line = xtr.getLocation().getLineNumber();
        //    this.column = xtr.getLocation().getColumnNumber();
        //}

        public Warning(String warningMessage, BaseElement element)
        {
            this.warningMessage = warningMessage;
            this.resource = element.getId();
            line = element.getXmlRowNumber();
            column = element.getXmlColumnNumber();
        }

        public String ToString()
        {
            return warningMessage + (resource != null ? " | " + resource : "") + " | line " + line + " | column " +
                   column;
        }
    }
}