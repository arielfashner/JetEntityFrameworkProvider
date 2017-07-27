﻿using System;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlServerCe;
using System.IO;

namespace JetEntityFrameworkProvider.Test
{
    static class Helpers
    {
        public static int CountRows(JetConnection jetConnection, string sqlStatement)
        {
            DbCommand command = jetConnection.CreateCommand(sqlStatement);
            DbDataReader dataReader = command.ExecuteReader();


            int count = 0;
            while (dataReader.Read())
                count++;

            return count;

        }

        public static void ShowDataReaderContent(DbConnection dbConnection, string sqlStatement)
        {
            DbCommand command = dbConnection.CreateCommand();
            command.CommandText = sqlStatement;
            DbDataReader dataReader = command.ExecuteReader();

            bool first = true;

            for (int i = 0; i < dataReader.FieldCount; i++)
            {
                if (first)
                    first = false;
                else
                    Console.Write("\t");

                Console.Write(dataReader.GetName(i));
            }
            Console.WriteLine();

            while (dataReader.Read())
            {
                first = true;
                for (int i = 0; i < dataReader.FieldCount; i++)
                {
                    if (first)
                        first = false;
                    else
                        Console.Write("\t");

                    Console.Write("{0}", dataReader.GetValue(i));
                }
                Console.WriteLine();
            }

        }

        public static void ShowDataTableContent(DataTable dataTable)
        {

            bool first = true;

            foreach (DataColumn column in dataTable.Columns)
            {
                if (first)
                    first = false;
                else
                    Console.Write("\t");

                Console.Write(column.ColumnName);
            }
            Console.WriteLine();

            foreach (DataRow row in dataTable.Rows)
            {
                first = true;
                foreach (DataColumn column in dataTable.Columns)
                {
                    if (first)
                        first = false;
                    else
                        Console.Write("\t");

                    Console.Write("{0}", row[column]);
                }
                Console.WriteLine();
            }

        }


        public static DbConnection GetJetConnection()
        {
            // Take care because according to this article
            // http://msdn.microsoft.com/en-us/library/dd0w4a2z(v=vs.110).aspx
            // to make the following line work the provider must be installed in the GAC and we also need an entry in machine.config
            /*
            DbProviderFactory providerFactory = System.Data.Common.DbProviderFactories.GetFactory("JetEntityFrameworkProvider");
            
            DbConnection connection = providerFactory.CreateConnection();
            */

            DbConnection connection = new JetConnection();

            connection.ConnectionString = GetJetConnectionString();
            return connection;

        }

        public static string GetJetConnectionString()
        {
            // ReSharper disable once CollectionNeverUpdated.Local
            OleDbConnectionStringBuilder oleDbConnectionStringBuilder = new OleDbConnectionStringBuilder();
            //oleDbConnectionStringBuilder.Provider = "Microsoft.Jet.OLEDB.4.0";
            //oleDbConnectionStringBuilder.DataSource = @".\Empty.mdb";
            oleDbConnectionStringBuilder.Provider = "Microsoft.ACE.OLEDB.12.0";
            oleDbConnectionStringBuilder.DataSource = @".\Empty.accdb";
            return oleDbConnectionStringBuilder.ToString();
        }


        public static DbConnection GetSqlCeConnection()
        {
            // Take care because according to this article
            // http://msdn.microsoft.com/en-us/library/dd0w4a2z(v=vs.110).aspx
            // to make the following line work the provider must be installed in the GAC and we also need an entry in machine.config
            /*
            DbProviderFactory providerFactory = System.Data.Common.DbProviderFactories.GetFactory("JetEntityFrameworkProvider");
            
            DbConnection connection = providerFactory.CreateConnection();
            */

            DbConnection connection = new SqlCeConnection();

            connection.ConnectionString = GetSqlCeConnectionString();
            return connection;

        }

        public static string GetSqlCeConnectionString()
        {
            // ReSharper disable once CollectionNeverUpdated.Local
            SqlCeConnectionStringBuilder sqlCeConnectionStringBuilder = new SqlCeConnectionStringBuilder();
            //oleDbConnectionStringBuilder.Provider = "Microsoft.Jet.OLEDB.4.0";
            //oleDbConnectionStringBuilder.DataSource = @".\Empty.mdb";
            sqlCeConnectionStringBuilder.DataSource = GetSqlCeDatabaseFileName();
            sqlCeConnectionStringBuilder.CaseSensitive = true;
            return sqlCeConnectionStringBuilder.ToString();
        }

        private static string GetSqlCeDatabaseFileName()
        {
            return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase.Replace("file:///", "")) + "\\Data.sdf";
        }

        public static void CreateSqlCeDatabase()
        {
            var sqlCeEngine = new SqlCeEngine(GetSqlCeConnectionString());
            sqlCeEngine.CreateDatabase();
        }

        public static void DeleteSqlCeDatabase()
        {
            if (File.Exists(GetSqlCeDatabaseFileName()))            
                File.Delete(GetSqlCeDatabaseFileName());
        }

        public static DbConnection GetSqlServerConnection()
        {
            DbConnection connection = new System.Data.SqlClient.SqlConnection("Data Source=(local);Initial Catalog=JetEfProviderComparativeTest;Integrated Security=true");
            return connection;
        }
    }
}
