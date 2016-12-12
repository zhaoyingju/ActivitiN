using System.Collections;
using System.Collections.Generic;

/* Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace org.activiti.engine.impl.persistence.entity
{


	using Context = org.activiti.engine.impl.context.Context;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class IdentityInfoEntityManager : AbstractManager
	{

	  public virtual void deleteUserInfoByUserIdAndKey(string userId, string key)
	  {
		IdentityInfoEntity identityInfoEntity = findUserInfoByUserIdAndKey(userId, key);
		if (identityInfoEntity != null)
		{
		  deleteIdentityInfo(identityInfoEntity);
		}
	  }

	  public virtual void deleteIdentityInfo(IdentityInfoEntity identityInfo)
	  {
		DbSqlSession.delete(identityInfo);
	  }

	  protected internal virtual IList<IdentityInfoEntity> findIdentityInfoDetails(string identityInfoId)
	  {
		return Context.CommandContext.DbSqlSession.SqlSession.selectList("selectIdentityInfoDetails", identityInfoId);
	  }

	  public virtual void setUserInfo(string userId, string userPassword, string type, string key, string value, string accountPassword, IDictionary<string, string> accountDetails)
	  {
		sbyte[] storedPassword = null;
		if (accountPassword != null)
		{
		  storedPassword = encryptPassword(accountPassword, userPassword);
		}

		IdentityInfoEntity identityInfoEntity = findUserInfoByUserIdAndKey(userId, key);
		if (identityInfoEntity != null)
		{
		  // update
		  identityInfoEntity.Value = value;
		  identityInfoEntity.PasswordBytes = storedPassword;

		  if (accountDetails == null)
		  {
			accountDetails = new Dictionary<string, string>();
		  }

		  IDictionary<string, string>.KeyCollection newKeys = new HashSet<string>(accountDetails.Keys);
		  IList<IdentityInfoEntity> identityInfoDetails = findIdentityInfoDetails(identityInfoEntity.Id);
		  foreach (IdentityInfoEntity identityInfoDetail in identityInfoDetails)
		  {
			string detailKey = identityInfoDetail.Key;
			newKeys.remove(detailKey);
			string newDetailValue = accountDetails[detailKey];
			if (newDetailValue == null)
			{
			  deleteIdentityInfo(identityInfoDetail);
			}
			else
			{
			  // update detail
			  identityInfoDetail.Value = newDetailValue;
			}
		  }
		  insertAccountDetails(identityInfoEntity, accountDetails, newKeys);


		}
		else
		{
		  // insert
		  identityInfoEntity = new IdentityInfoEntity();
		  identityInfoEntity.UserId = userId;
		  identityInfoEntity.Type = type;
		  identityInfoEntity.Key = key;
		  identityInfoEntity.Value = value;
		  identityInfoEntity.PasswordBytes = storedPassword;
		  DbSqlSession.insert(identityInfoEntity);
		  if (accountDetails != null)
		  {
			insertAccountDetails(identityInfoEntity, accountDetails, accountDetails.Keys);
		  }
		}
	  }

	  private void insertAccountDetails(IdentityInfoEntity identityInfoEntity, IDictionary<string, string> accountDetails, Set<string> keys)
	  {
		foreach (string newKey in keys)
		{
		  // insert detail
		  IdentityInfoEntity identityInfoDetail = new IdentityInfoEntity();
		  identityInfoDetail.ParentId = identityInfoEntity.Id;
		  identityInfoDetail.Key = newKey;
		  identityInfoDetail.Value = accountDetails[newKey];
		  DbSqlSession.insert(identityInfoDetail);
		}
	  }

	  public virtual sbyte[] encryptPassword(string accountPassword, string userPassword)
	  {
		// TODO
		return accountPassword.GetBytes();
	  }

	  public virtual string decryptPassword(sbyte[] storedPassword, string userPassword)
	  {
		// TODO
		return StringHelperClass.NewString(storedPassword);
	  }

	  public virtual IdentityInfoEntity findUserInfoByUserIdAndKey(string userId, string key)
	  {
		IDictionary<string, string> parameters = new Dictionary<string, string>();
		parameters["userId"] = userId;
		parameters["key"] = key;
		return (IdentityInfoEntity) DbSqlSession.selectOne("selectIdentityInfoByUserIdAndKey", parameters);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<String> findUserInfoKeysByUserIdAndType(String userId, String type)
	  public virtual IList<string> findUserInfoKeysByUserIdAndType(string userId, string type)
	  {
		IDictionary<string, string> parameters = new Dictionary<string, string>();
		parameters["userId"] = userId;
		parameters["type"] = type;
		return (IList) DbSqlSession.SqlSession.selectList("selectIdentityInfoKeysByUserIdAndType", parameters);
	  }
	}

}