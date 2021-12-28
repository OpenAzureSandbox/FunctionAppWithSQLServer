﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionAppWithSQLServer.Repository
{
    public interface RowRepository
    {
        public string GetValue(string key);

    }

    public class SQLServerRowRepository : RowRepository
    {
        private readonly SqlConnection _conn;
        private readonly string _keyColumnName;
        private readonly string _valueColumnName;
        private readonly string _tableName;

        private SQLServerRowRepository(SqlConnection conn, string keyConlumnName, string valueColumnName, string tableName)
        {
            this._conn = conn;
            this._keyColumnName = keyConlumnName;
            this._valueColumnName = valueColumnName;
            this._tableName = tableName;
        }

        public static RowRepository OfConnection(SqlConnection conn)
        {
            return new SQLServerRowRepository(conn, "id", "value", "[dbo].[values]"); ;
        }

        private SqlCommand BuildCommand(string key)
        {
            SqlCommand command = new SqlCommand(null, this._conn);
            command.CommandText = $"select {this._valueColumnName} from {this._tableName} where {this._keyColumnName} = @key";

            SqlParameter keyParameter = new SqlParameter("@key", System.Data.SqlDbType.VarChar, 0);
            keyParameter.Value = key;
            command.Parameters.Add(keyParameter);

            return command;

        }

        public string GetValue(string key)
        {
            List<string> results = new List<string>();
            SqlCommand command = this.BuildCommand(key);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while(reader.Read() == true)
                {

                    results.Add((string)reader[this._valueColumnName]);
                }
            }
            return results.First();
           
        }
    }
}