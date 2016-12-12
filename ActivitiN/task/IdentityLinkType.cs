namespace org.activiti.engine.task
{


    /// <summary>
    /// 包含所有类型的标识链接的常量，可用于将用户或组与某个任务相关联。
    /// </summary>
    public class IdentityLinkType
    {

        /* Activiti native roles */

        public const string ASSIGNEE = "assignee";

        public const string CANDIDATE = "candidate";

        public const string OWNER = "owner";

        public const string STARTER = "starter";

        public const string PARTICIPANT = "participant";

    }

}