namespace org.activiti.engine.identity
{

    using org.activiti.engine.query;


    /// <summary>
    /// 允许以编程方式查询 <seealso cref="Group"/>s.    
    /// </summary>
    public interface GroupQuery : Query<GroupQuery, Group>
    {

        /// <summary>
        /// Only select <seealso cref="Group"/>s with the given id. </summary>
        GroupQuery groupId(string groupId);

        /// <summary>
        /// Only select <seealso cref="Group"/>s with the given name. </summary>
        GroupQuery groupName(string groupName);

        /// <summary>
        /// Only select <seealso cref="Group"/>s where the name matches the given parameter.
        ///  The syntax to use is that of SQL, eg. %activiti%. 
        /// </summary>
        GroupQuery groupNameLike(string groupNameLike);

        /// <summary>
        /// Only select <seealso cref="Group"/>s which have the given type. </summary>
        GroupQuery groupType(string groupType);

        /// <summary>
        /// Only selects <seealso cref="Group"/>s where the given user is a member of. </summary>
        GroupQuery groupMember(string groupMemberUserId);

        /// <summary>
        /// Only select <seealso cref="Group"/>S that are potential starter for the given process definition. </summary>
        GroupQuery potentialStarter(string procDefId);


        //sorting ////////////////////////////////////////////////////////

        /// <summary>
        /// Order by group id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
        GroupQuery orderByGroupId();

        /// <summary>
        /// Order by group name (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
        GroupQuery orderByGroupName();

        /// <summary>
        /// Order by group type (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
        GroupQuery orderByGroupType();

    }

}