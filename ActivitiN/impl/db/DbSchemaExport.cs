using System;
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
namespace org.activiti.engine.impl.db
{



	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class DbSchemaExport
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void main(String[] args) throws Exception
	  public static void Main(string[] args)
	  {
		if (args == null || args.Length != 1)
		{
		  Console.Error.WriteLine("Syntax: java -cp ... org.activiti.engine.impl.db.DbSchemaExport <path-to-properties-file> <path-to-export-file>");
		  return;
		}
		File propertiesFile = new File(args[0]);
		if (!propertiesFile.exists())
		{
		  Console.Error.WriteLine("File '" + args[0] + "' doesn't exist \n" + "Syntax: java -cp ... org.activiti.engine.impl.db.DbSchemaExport <path-to-properties-file> <path-to-export-file>\n");
		  return;
		}
		Properties properties = new Properties();
		properties.load(new FileInputStream(propertiesFile));

		string jdbcDriver = properties.getProperty("jdbc.driver");
		string jdbcUrl = properties.getProperty("jdbc.url");
		string jdbcUsername = properties.getProperty("jdbc.username");
		string jdbcPassword = properties.getProperty("jdbc.password");

		Type.GetType(jdbcDriver);
		Connection connection = DriverManager.getConnection(jdbcUrl, jdbcUsername, jdbcPassword);
		try
		{
		  DatabaseMetaData meta = connection.MetaData;

		  SortedSet<string> tableNames = new SortedSet<string>();
		  ResultSet tables = meta.getTables(null, null, null, null);
		  while (tables.next())
		  {
			string tableName = tables.getString(3);
			tableNames.add(tableName);
		  }

		  Console.WriteLine("TABLES");
		  foreach (string tableName in tableNames)
		  {
			IDictionary<string, string> columnDescriptions = new Dictionary<string, string>();
			ResultSet columns = meta.getColumns(null, null, tableName, null);
			while (columns.next())
			{
			  string columnName = columns.getString(4);
			  string columnTypeAndSize = columns.getString(6) + " " + columns.getInt(7);
			  columnDescriptions[columnName] = columnTypeAndSize;
			}

			Console.WriteLine(tableName);
			foreach (string columnName in new SortedSet<string>(columnDescriptions.Keys))
			{
			  Console.WriteLine("  " + columnName + " " + columnDescriptions[columnName]);
			}

			Console.WriteLine("INDEXES");
			SortedSet<string> indexNames = new SortedSet<string>();
			ResultSet indexes = meta.getIndexInfo(null, null, tableName, false, true);
			while (indexes.next())
			{
			  string indexName = indexes.getString(6);
			  indexNames.add(indexName);
			}
			foreach (string indexName in indexNames)
			{
			  Console.WriteLine(indexName);
			}
			Console.WriteLine();
		  }


		}
		catch (Exception e)
		{
		  Console.WriteLine(e.ToString());
		  Console.Write(e.StackTrace);
		  connection.close();
		}
	  }
	}

}