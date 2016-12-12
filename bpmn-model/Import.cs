using System;
using org.activiti.bpmn.model;

namespace org.activiti.bpmn.model{

public class Import : BaseElement {

  protected String importType;
  protected String location;
  protected String Namespace;
  
  public String getImportType() {
    return importType;
  }
  public void setImportType(String importType) {
    this.importType = importType;
  }
  public String getLocation() {
    return location;
  }
  public void setLocation(String location) {
    this.location = location;
  }
  public String getNamespace() {
    return Namespace;
  }
  public void setNamespace(String Namespace) {
    this.Namespace = Namespace;
  }

  public override Object clone()
  {
      Import clone = new Import();
      clone.setValues(this);
    return clone;
  }
  
  public void setValues(Import otherElement) {
    base.setValues(otherElement);
    setImportType(otherElement.getImportType());
    setLocation(otherElement.getLocation());
    setNamespace(otherElement.getNamespace());
  }
}
}