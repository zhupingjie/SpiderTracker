using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MySqlConnector;
using SpiderDomain.Entity;
using SpiderCore.Util;
using SpiderDomain.Data;

namespace SpiderCore.Repository
{
    public class SQLiteDBHelper
    {
        static SQLiteDBHelper()
        {
            
        }
        private SQLiteDBHelper()
        {

        }

        private static readonly SQLiteDBHelper instance = new SQLiteDBHelper();
        //private static readonly string dbName = "../../SQliteDB/SpiderDB.db";
        //private static readonly string strConn = $"Data Source={dbName}";
        public string DBServiceIP { get; set; } = "49.232.192.220";
        public string DBName { get; set; } = "spider";
        public string DBUserID { get; set; } = "sa";
        public string DBPwd { get; set; } = "sa!123456";
        public int DBPort { get; set; } = 3306;

        public string DBConnectionString
        {
            get
            {
                return $"Server={DBServiceIP};Database={DBName};User Id={DBUserID};Password={DBPwd};Connect Timeout=20;";
            }
        }
        public static SQLiteDBHelper Instance
        {
            get
            {
                return instance;
            }
        }

        public bool CreateEntity(BaseEntity entity, string table)
        {
            using (MySqlConnection con = new MySqlConnection(DBConnectionString))
            {
                con.Open();
                var cmd = con.CreateCommand();

                StringBuilder sbCol = new StringBuilder();
                StringBuilder sbVal = new StringBuilder();

                var type = entity.GetType();
                var fields = type.GetProperties();
                foreach (var field in fields)
                {
                    if (field.Name == "id") continue;

                    sbCol.Append($",`{field.Name}`");

                    var value = field.GetValue(entity);
                    if (value != null)
                    {
                        string val = null;
                        if(field.PropertyType == typeof(DateTime))
                        {
                            val = DateTime.Parse(value.ToString()).ToString("s");
                        }
                        else if(field.PropertyType == typeof(string))
                        {
                            val = value.ToString().Replace("'", "''");
                        }
                        else
                        {
                            val = value.ToString();
                        }
                        sbVal.Append($",'{val}'");
                    }
                    else
                    {
                        sbVal.Append(",NULL");
                    }
                }
                cmd.CommandText = $"insert into {table} (ID {sbCol.ToString()}) values (NULL {sbVal.ToString()})";
                try
                {
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    LogUtil.Error(ex);
                    return false;
                }
                finally
                {
                    con.Close();
                }
            }
        }

        public bool UpdateEntity(BaseEntity entity, string table, string col, string val, string[] columns)
        {
            using (MySqlConnection con = new MySqlConnection(DBConnectionString))
            {
                con.Open();
                var cmd = con.CreateCommand();

                StringBuilder sb = new StringBuilder();

                var type = entity.GetType();
                var fields = type.GetProperties();
                foreach (var field in fields)
                {
                    if (field.Name == "id") continue;
                    if (!columns.Contains(field.Name)) continue;

                    var value = field.GetValue(entity);
                    if (value != null)
                    {
                        sb.Append($",`{field.Name}`='{value.ToString().Replace("'","''")}'");
                    }
                }
                cmd.CommandText = $"update {table} set `lastdate`='{DateTime.Now.ToString("s")}' {sb.ToString()} where `{col}`='{val}'";
                try
                {
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    LogUtil.Error(ex);
                    return false;
                }
                finally
                {
                    con.Close();
                }
            }
        }

        public bool UpdateEntitys(string table, string where, string col, object val)
        {
            using (MySqlConnection con = new MySqlConnection(DBConnectionString))
            {
                con.Open();
                var cmd = con.CreateCommand();

                cmd.CommandText = $"update {table} set `lastdate`='{DateTime.Now.ToString("s")}', `{col}`={val} where {where}";
                try
                {
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    LogUtil.Error(ex);
                    return false;
                }
                finally
                {
                    con.Close();
                }
            }
        }

        public bool DeleteEntity(string table, string col, string val)
        {
            using (MySqlConnection con = new MySqlConnection(DBConnectionString))
            {
                con.Open();
                var cmd = con.CreateCommand();

                StringBuilder sb = new StringBuilder();
                cmd.CommandText = $"delete from {table} where `{col}`='{val}'";
                try
                {
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    LogUtil.Error(ex);
                    return false;
                }
                finally
                {
                    con.Close();
                }
            }
        }

        public bool ExistsEntity(string table, string column, string value)
        {
            using (MySqlConnection con = new MySqlConnection(DBConnectionString))
            {
                con.Open();
                var cmd = con.CreateCommand();

                cmd.CommandText = $"select count(*) from {table} where `{column}`='{value}'";
                try
                {
                    var obj = cmd.ExecuteScalar();
                    if(obj != DBNull.Value)
                    {
                        int count = 0;
                        int.TryParse(obj.ToString(), out count);
                        return count > 0 ? true : false;
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    LogUtil.Error(ex);
                    return false;
                }
                finally
                {
                    con.Close();
                }
            }
        }

        public bool ExistsEntity(string table, string where)
        {
            using (MySqlConnection con = new MySqlConnection(DBConnectionString))
            {
                con.Open();
                var cmd = con.CreateCommand();

                cmd.CommandText = $"select count(*) from {table} where {where}";
                try
                {
                    var obj = cmd.ExecuteScalar();
                    if (obj != DBNull.Value)
                    {
                        int count = 0;
                        int.TryParse(obj.ToString(), out count);
                        return count > 0 ? true : false;
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    LogUtil.Error(ex);
                    return false;
                }
                finally
                {
                    con.Close();
                }
            }
        }

        public int GetEntityCount(string table, string where)
        {
            using (MySqlConnection con = new MySqlConnection(DBConnectionString))
            {
                con.Open();
                var cmd = con.CreateCommand();

                cmd.CommandText = $"select count(*) from {table} where {where}";
                try
                {
                    var obj = cmd.ExecuteScalar();
                    if (obj != DBNull.Value)
                    {
                        int count = 0;
                        int.TryParse(obj.ToString(), out count);
                        return count;
                    }
                    return -1;
                }
                catch (Exception ex)
                {
                    LogUtil.Error(ex);
                    return -1;
                }
                finally
                {
                    con.Close();
                }
            }
        }

        public int GetEntitySumCount(string table, string field, string where)
        {
            using (MySqlConnection con = new MySqlConnection(DBConnectionString))
            {
                con.Open();
                var cmd = con.CreateCommand();

                cmd.CommandText = $"select sum({field}) from {table} where {where}";
                try
                {
                    var obj = cmd.ExecuteScalar();
                    if (obj != DBNull.Value)
                    {
                        int count = 0;
                        int.TryParse(obj.ToString(), out count);
                        return count;
                    }
                    return -1;
                }
                catch (Exception ex)
                {
                    LogUtil.Error(ex);
                    return -1;
                }
                finally
                {
                    con.Close();
                }
            }
        }

        public TEntity GetEntity<TEntity>(string table, string where) where TEntity : BaseEntity, new ()
        {
            return GetEntitys<TEntity>(table, where).FirstOrDefault();
        }

        public List<TEntity> GetEntitys<TEntity>(string table, string where)  where TEntity : BaseEntity, new ()
        {
            using (MySqlConnection con = new MySqlConnection(DBConnectionString))
            {
                var lst = new List<TEntity>();
                try
                {
                    con.Open();
                    var cmd = con.CreateCommand();
                    cmd.CommandText = $"select * from {table} where {where}";
                    var ada = new MySqlDataAdapter(cmd);
                    var dt = new DataTable(table);
                    ada.Fill(dt);

                    for(var i=0; i<dt.Rows.Count; i++)
                    {
                        DataRow dr = dt.Rows[i];
                        var item = new TEntity();

                        var type = item.GetType();
                        var fields = type.GetProperties();
                        foreach (var field in fields)
                        {
                            if (dt.Columns.Contains(field.Name))
                            {
                                object obj = null;
                                if(field.PropertyType == typeof(Int32))
                                {
                                    obj = Int32.Parse(dr[field.Name].ToString());
                                }
                                else if(field.PropertyType == typeof(DateTime))
                                {
                                    obj = DateTime.Parse(dr[field.Name].ToString());
                                }
                                else if(dr[field.Name] != DBNull.Value)
                                {
                                    obj = dr[field.Name].ToString();
                                }
                                field.SetValue(item, obj);
                            }
                        }
                        lst.Add(item);
                    }
                    return lst;
                }
                catch (Exception ex)
                {
                    LogUtil.Error(ex);
                    return lst;
                }
                finally
                {
                    con.Close();
                }
            }
        }

        public List<string> GetGroupStrings(string table, string groupColumn, string where)
        {
            using (MySqlConnection con = new MySqlConnection(DBConnectionString))
            {
                try
                {
                    con.Open();
                    var cmd = con.CreateCommand();
                    cmd.CommandText = $"select {groupColumn} from {table} where {where} group by {groupColumn}";
                    var ada = new MySqlDataAdapter(cmd);
                    var dt = new DataTable(table);
                    ada.Fill(dt);

                    var lst = new List<string>();
                    for (var i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow dr = dt.Rows[i];
                        lst.Add(dr[0].ToString());
                    }
                    return lst;
                }
                catch (Exception ex)
                {
                    LogUtil.Error(ex);
                    return null;
                }
                finally
                {
                    con.Close();
                }
            }
        }
    }
}
