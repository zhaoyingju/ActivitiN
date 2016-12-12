using System;


namespace org.activiti.engine.task
{

    using HistoricData = org.activiti.engine.history.HistoricData;


    /// <summary>
    /// 形成围绕任务的讨论的用户评论。
    /// </summary>
    public interface Comment : HistoricData
    {

        /// <summary>
        /// unique identifier for this comment </summary>
        string Id { get; }

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

        /// <summary>
        /// reference to the type given to the comment </summary>
        string Type { get; }

        /// <summary>
        /// the full comment message the user had related to the task and/or process instance </summary>
        /// <seealso cref= TaskService#getTaskComments(String)  </seealso>
        string FullMessage { get; }
    }

}