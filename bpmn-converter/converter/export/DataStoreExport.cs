using System;
using bpmn_converter.converter;
using org.activiti.bpmn.constants;
using org.activiti.bpmn.model;

namespace org.activiti.bpmn.converter.export
{








    public class DataStoreExport : BpmnXMLConstants
    {

        public static void writeDataStores(BpmnModel model, XMLStreamWriter xtw)
        {

            foreach (DataStore dataStore  in model.getDataStores().Values)
            {
                xtw.writeStartElement(ELEMENT_DATA_STORE);
                xtw.writeAttribute(ATTRIBUTE_ID, dataStore.getId());
                xtw.writeAttribute(ATTRIBUTE_NAME, dataStore.getName());
                if (!String.IsNullOrWhiteSpace(dataStore.getItemSubjectRef()))
                {
                    xtw.writeAttribute(ATTRIBUTE_ITEM_SUBJECT_REF, dataStore.getItemSubjectRef());
                }

                if (!String.IsNullOrWhiteSpace(dataStore.getDataState()))
                {
                    xtw.writeStartElement(ELEMENT_DATA_STATE);
                    xtw.writeCharacters(dataStore.getDataState());
                    xtw.writeEndElement();
                }

                xtw.writeEndElement();
            }
        }
    }
}