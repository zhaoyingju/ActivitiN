
namespace org.activiti.engine.task
{

    using GroupQuery = org.activiti.engine.identity.GroupQuery;
    using UserQuery = org.activiti.engine.identity.UserQuery;


    /// <summary>
    /// ����������ڽ�������ĳ����ݹ�����
    /// 
    /// For example:
    /// -�û�����������������ˣ�=����������ͣ�
    /// -�����������ĺ�ѡ�飨=��ʶ��·���ͣ�
    /// </summary>
    public interface IdentityLink
    {

        /// <summary>
        /// Returns the type of link.
        /// See <seealso cref="IdentityLinkType"/> for the native supported types by Activiti. 
        /// </summary>
        string Type { get; }

        /// <summary>
        /// If the identity link involves a user, then this will be a non-null id of a user.
        /// That userId can be used to query for user information through the <seealso cref="UserQuery"/> API.
        /// </summary>
        string UserId { get; }

        /// <summary>
        /// If the identity link involves a group, then this will be a non-null id of a group.
        /// That groupId can be used to query for user information through the <seealso cref="GroupQuery"/> API.
        /// </summary>
        string GroupId { get; }

        /// <summary>
        /// The id of the task associated with this identity link.
        /// </summary>
        string TaskId { get; }

        /// <summary>
        /// The process definition id associated with this identity link.
        /// </summary>
        string ProcessDefinitionId { get; }

        /// <summary>
        /// The process instance id associated with this identity link.
        /// </summary>
        string ProcessInstanceId { get; }

    }

}