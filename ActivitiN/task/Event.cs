using System;
using System.Collections.Generic;


namespace org.activiti.engine.task
{

    /// <summary>
    /// Exposes twitter-like feeds for tasks and process instances.
    /// </summary>
    /// <seealso cref= {@link TaskService#getTaskEvents(String)
    /// @author Tom Baeyens </seealso>
    public interface Event
    {

        /// <summary>
        /// A user identity link was added with following message parts:
        /// [0] userId
        /// [1] identity link type (aka role) 
        /// </summary>

        /// <summary>
        /// A user identity link was added with following message parts:
        /// [0] userId
        /// [1] identity link type (aka role) 
        /// </summary>

        /// <summary>
        /// A group identity link was added with following message parts:
        /// [0] groupId
        /// [1] identity link type (aka role) 
        /// </summary>

        /// <summary>
        /// A group identity link was added with following message parts:
        /// [0] groupId
        /// [1] identity link type (aka role) 
        /// </summary>

        /// <summary>
        /// An user comment was added with the short version of the comment as message. </summary>

        /// <summary>
        /// An attachment was added with the attachment name as message. </summary>

        /// <summary>
        /// An attachment was deleted with the attachment name as message. </summary>

        /// <summary>
        /// Unique identifier for this event </summary>
        string Id { get; }

        /// <summary>
        /// Indicates the type of of action and also indicates the meaning of the parts as exposed in <seealso cref="#getMessageParts()"/> </summary>
        string Action { get; }

        /// <summary>
        /// The meaning of the message parts is defined by the action as you can find in <seealso cref="#getAction()"/> </summary>
        IList<string> MessageParts { get; }

        /// <summary>
        /// The message that can be used in case this action only has a single message part. </summary>
        string Message { get; }

        /// <summary>
        /// reference to the user that made the comment </summary>
        string UserId { get; }

        /// <summary>
        /// time and date when the user made the comment </summary>
        DateTime Time { get; }

        /// <summary>
        /// reference to the task on which this comment was made </summary>
        string TaskId { get; }

        /// <summary>
        /// reference to the process instance on which this comment was made </summary>
        string ProcessInstanceId { get; }

    }

    public static class Event_Fields
    {
        public const string ACTION_ADD_USER_LINK = "AddUserLink";
        public const string ACTION_DELETE_USER_LINK = "DeleteUserLink";
        public const string ACTION_ADD_GROUP_LINK = "AddGroupLink";
        public const string ACTION_DELETE_GROUP_LINK = "DeleteGroupLink";
        public const string ACTION_ADD_COMMENT = "AddComment";
        public const string ACTION_ADD_ATTACHMENT = "AddAttachment";
        public const string ACTION_DELETE_ATTACHMENT = "DeleteAttachment";
    }

}