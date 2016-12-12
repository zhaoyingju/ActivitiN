namespace org.activiti.engine.impl.util
{

	/// <summary>
	/// @author jbarrez
	/// </summary>
	public class JvmUtil
	{

		public static string JavaVersion
		{
			get
			{
				return System.getProperty("java.version");
			}
		}

		public static bool JDK8
		{
			get
			{
				string version = System.getProperty("java.version");
				return version.StartsWith("1.8");
			}
		}

		public static bool JDK7
		{
			get
			{
				string version = System.getProperty("java.version");
				return version.StartsWith("1.7");
			}
		}

		public static bool AtLeastJDK7
		{
			get
			{
				return JDK7 || JDK8;
			}
		}

	}

}