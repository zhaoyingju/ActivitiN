using System.Collections.Generic;

namespace org.activiti.engine.impl.db
{


	/// <summary>
	/// This class is used for auto-upgrade purposes.
	/// 
	/// The idea is that instances of this class are put in a sequential order,
	/// and that the current version is determined from the ACT_GE_PROPERTY table.
	/// 
	/// Since sometimes in the past, a version is ambiguous (eg. 5.12 => 5.12, 5.12.1, 5.12T) this class act as a wrapper
	/// with a smarter matches() method.
	/// 
	/// @author jbarrez
	/// </summary>
	public class ActivitiVersion
	{

		protected internal string mainVersion;
		protected internal IList<string> alternativeVersionStrings;

		public ActivitiVersion(string mainVersion)
		{
			this.mainVersion = mainVersion;
			this.alternativeVersionStrings = Arrays.asList(mainVersion);
		}

		public ActivitiVersion(string mainVersion, IList<string> alternativeVersionStrings)
		{
			this.mainVersion = mainVersion;
			this.alternativeVersionStrings = alternativeVersionStrings;
		}

		public virtual string MainVersion
		{
			get
			{
				return mainVersion;
			}
		}

		public virtual bool matches(string version)
		{
			if (version.Equals(mainVersion))
			{
				return true;
			}
			else if (alternativeVersionStrings.Count > 0)
			{
				return alternativeVersionStrings.Contains(version);
			}
			else
			{
				return false;
			}
		}

		public override bool Equals(object obj)
		{
			if (!(obj is ActivitiVersion))
			{
				return false;
			}
			ActivitiVersion other = (ActivitiVersion) obj;
			bool mainVersionEqual = mainVersion.Equals(other.mainVersion);
			if (!mainVersionEqual)
			{
				return false;
			}
			else
			{
				if (alternativeVersionStrings != null)
				{
					return alternativeVersionStrings.Equals(other.alternativeVersionStrings);
				}
				else
				{
					return other.alternativeVersionStrings == null;
				}
			}
		}

	}

}