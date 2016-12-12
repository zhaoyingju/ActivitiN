using System;

namespace org.activiti.engine.task
{




    /// <summary>
    ///与任务或流程实例相关联的任何类型的内容。
    /// </summary>
    public interface Attachment
    {

        /// <summary>
        /// unique id for this attachment </summary>
        string Id { get; }

        /// <summary>
        /// free user defined short (max 255 chars) name for this attachment </summary>
        string Name { get; set; }


        /// <summary>
        /// long (max 255 chars) explanation what this attachment is about in context of the task and/or process instance it's linked to. </summary>
        string Description { get; set; }


        /// <summary>
        /// indication of the type of content that this attachment refers to. Can be mime type or any other indication. </summary>
        string Type { get; }

        /// <summary>
        /// reference to the task to which this attachment is associated. </summary>
        string TaskId { get; }

        /// <summary>
        /// reference to the process instance to which this attachment is associated. </summary>
        string ProcessInstanceId { get; }

        /// <summary>
        /// the remote URL in case this is remote content.  If the attachment content was 
        /// <seealso cref="TaskService#createAttachment(String, String, String, String, String, java.io.InputStream) uploaded with an input stream"/>, 
        /// then this method returns null and the content can be fetched with <seealso cref="TaskService#getAttachmentContent(String)"/>. 
        /// </summary>
        string Url { get; }

        /// <summary>
        /// reference to the user who created this attachment. </summary>
        string UserId { get; }

        /// <summary>
        /// timestamp when this attachment was created </summary>
        DateTime Time { get; set; }
    }
}