namespace org.activiti.engine.repository
{

	using ProcessInstance = org.activiti.engine.runtime.ProcessInstance;

    /// <summary>
    ///表示由活动和转换组成的可执行过程的对象结构。
    /// </summary>
    public interface ProcessDefinition
	{

	  /// <summary>
	  /// unique identifier </summary>
	  string Id {get;}

	  /// <summary>
	  /// category name which is derived from the targetNamespace attribute in the definitions element </summary>
	  string Category {get;}

	  /// <summary>
	  /// label used for display purposes </summary>
	  string Name {get;}

	  /// <summary>
	  /// unique name for all versions this process definitions </summary>
	  string Key {get;}

	  /// <summary>
	  /// description of this process * </summary>
	  string Description {get;}

	  /// <summary>
	  /// version of this process definition </summary>
	  int Version {get;}

	  /// <summary>
	  /// name of <seealso cref="RepositoryService#getResourceAsStream(String, String) the resource"/> 
	  /// of this process definition. 
	  /// </summary>
	  string ResourceName {get;}

	  /// <summary>
	  /// The deployment in which this process definition is contained. </summary>
	  string DeploymentId {get;}

	  /// <summary>
	  /// The resource name in the deployment of the diagram image (if any). </summary>
	  string DiagramResourceName {get;}

	  /// <summary>
	  /// Does this process definition has a <seealso cref="FormService#getStartFormData(String) start form key"/>. </summary>
	  bool hasStartFormKey();

	  /// <summary>
	  /// Does this process definition has a graphical notation defined (such that a diagram can be generated)? </summary>
	  bool hasGraphicalNotation();

	  /// <summary>
	  /// Returns true if the process definition is in suspended state. </summary>
	  bool Suspended {get;}

	  /// <summary>
	  /// The tenant identifier of this process definition </summary>
	  string TenantId {get;}

	}

}