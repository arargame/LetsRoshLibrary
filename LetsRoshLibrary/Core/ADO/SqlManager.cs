﻿using LetsRoshLibrary.Core.Connection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetsRoshLibrary.Core.ADO
{
    public class SqlManager
    {
        public static List<Dictionary<string, object>> ExecuteQuery(string sql, Dictionary<string, object> parameters = null, CustomConnection connection = null)
        {
            var list = new List<Dictionary<string, object>>();

            IDataReader dataReader = null;
            IDbCommand command = null;

            connection = connection ?? new MsSqlConnection();

            using (connection)
            {
                try
                {
                    connection.Connect();

                    command = connection.CreateCommand();

                    command.CommandText = sql;

                    if (parameters != null)
                    {
                        var pKeys = parameters.Select(p => p.Key).ToList();

                        foreach (var key in pKeys)
                        {
                            if (!sql.Contains(key))
                                parameters.Remove(key);
                        }

                        command.AddParameters(parameters);
                    }

                    dataReader = command.ExecuteReader();

                    while (dataReader.Read())
                    {
                        var dictionary = new Dictionary<string, object>();

                        for (int i = 0; i < dataReader.FieldCount; i++)
                        {
                            dictionary.Add(dataReader.GetName(i), dataReader.GetValue(i));
                        }

                        list.Add(dictionary);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw ex;
                }
                finally
                {
                    if (dataReader != null)
                        dataReader.Close();
                }
            }

            return list;

        }

        public static object ExecuteScalar(string sql, Dictionary<string, object> parameters = null, CustomConnection connection = null)
        {
            object result = null;

            connection = connection ?? new MsSqlConnection();

            IDbCommand command = null;

            using (connection)
            {
                try
                {
                    connection.Connect();

                    command = connection.CreateCommand();

                    command.CommandText = sql;

                    if (parameters != null)
                    {
                        var pKeys = parameters.Select(p => p.Key).ToList();

                        foreach (var key in pKeys)
                        {
                            if (!sql.Contains(key))
                                parameters.Remove(key);
                        }

                        command.AddParameters(parameters);
                    }

                    result = command.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw ex;
                }
            }

            return result;
        }

        public static int ExecuteNonQuery(string sql, Dictionary<string, object> parameters = null, CustomConnection connection = null)
        {
            int affectedRowsCount = 0;

            connection = connection ?? new MsSqlConnection();

            IDbCommand command = null;


            using (connection)
            {
                try
                {
                    connection.Connect();

                    command = connection.CreateCommand();


                    command.CommandText = sql;

                    if (parameters != null)
                    {
                        var pKeys = parameters.Select(p => p.Key).ToList();

                        foreach (var key in pKeys)
                        {
                            if (!sql.Contains(key))
                                parameters.Remove(key);
                        }

                        command.AddParameters(parameters);
                    }

                    affectedRowsCount = command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw ex;
                }
            }


            return affectedRowsCount;
        }

        public static bool Any(string sql, Dictionary<string, object> parameters = null, CustomConnection connection = null)
        {
            sql = string.Format("select 1 from ({0}) t", sql);

            return Convert.ToInt32(ExecuteScalar(sql, parameters, connection)) == 1;
        }
    }

    public static class SqlManagerExtensions
    {
        public static void AddParameters(this IDbCommand command, Dictionary<string, object> parameters)
        {
            //if (command.Connection.ToString() == "Oracle.ManagedDataAccess.Client.OracleConnection")
            //    (command as OracleCommand).BindByName = true;

            foreach (var pair in parameters)
            {
                var parameter = command.CreateParameter();

                parameter.ParameterName = pair.Key;
                parameter.Value = pair.Value != null && !string.IsNullOrWhiteSpace(pair.Value.ToString()) ? pair.Value : DBNull.Value;

                command.Parameters.Add(parameter);
            }
        }
    }
}
