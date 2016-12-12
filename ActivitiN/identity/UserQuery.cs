namespace org.activiti.engine.identity
{

    using org.activiti.engine.query;

    public interface UserQuery : Query<UserQuery, User>
    {

        /// <summary>
        /// Only select <seealso cref="User"/>s with the given id/ </summary>
        UserQuery userId(string id);

        /// <summary>
        /// Only select <seealso cref="User"/>s with the given firstName. </summary>
        UserQuery userFirstName(string firstName);

        /// <summary>
        /// Only select <seealso cref="User"/>s where the first name matches the given parameter.
        /// The syntax is that of SQL, eg. %activivi%.
        /// </summary>
        UserQuery userFirstNameLike(string firstNameLike);

        /// <summary>
        /// Only select <seealso cref="User"/>s with the given lastName. </summary>
        UserQuery userLastName(string lastName);

        /// <summary>
        /// Only select <seealso cref="User"/>s where the last name matches the given parameter.
        /// The syntax is that of SQL, eg. %activivi%.
        /// </summary>
        UserQuery userLastNameLike(string lastNameLike);

        /// <summary>
        /// Only select <seealso cref="User"/>s where the full name matches the given parameters.
        /// Both the first name and last name will be tried, ie in semi-sql:
        /// where firstName like xxx or lastname like xxx
        /// </summary>
        UserQuery userFullNameLike(string fullNameLike);

        /// <summary>
        /// Only those <seealso cref="User"/>s with the given email addres. </summary>
        UserQuery userEmail(string email);

        /// <summary>
        /// Only select <seealso cref="User"/>s where the email matches the given parameter.
        /// The syntax is that of SQL, eg. %activivi%.
        /// </summary>
        UserQuery userEmailLike(string emailLike);

        /// <summary>
        /// Only select <seealso cref="User"/>s that belong to the given group. </summary>
        UserQuery memberOfGroup(string groupId);

        /// <summary>
        /// Only select <seealso cref="User"/>S that are potential starter for the given process definition. </summary>
        UserQuery potentialStarter(string procDefId);

        //sorting ////////////////////////////////////////////////////////

        /// <summary>
        /// Order by user id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
        UserQuery orderByUserId();

        /// <summary>
        /// Order by user first name (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
        UserQuery orderByUserFirstName();

        /// <summary>
        /// Order by user last name (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
        UserQuery orderByUserLastName();

        /// <summary>
        /// Order by user email  (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
        UserQuery orderByUserEmail();
    }

}