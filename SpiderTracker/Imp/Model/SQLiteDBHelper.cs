using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using SpiderTracker.Imp.Model;
using System.Data;

namespace SpiderTracker.Imp
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
        private static readonly string dbName = "../../SQliteDB/SpiderDB.db";
        private static readonly string strConn = $"Data Source={dbName}";
        public static SQLiteDBHelper Instance
        {
            get
            {
                return instance;
            }
        }

        public void InitSQLiteDB()
        {
            CreateDB(dbName);

            var columns = new List<SQLiteColumn>();
            columns.Add(new SQLiteColumn("groupname", "nvarchar", 20));
            columns.Add(new SQLiteColumn("uid", "nvarchar", 16));
            columns.Add(new SQLiteColumn("name", "nvarchar", 50));
            columns.Add(new SQLiteColumn("avatar", "nvarchar", 250));
            columns.Add(new SQLiteColumn("desc", "nvarchar", 500));
            columns.Add(new SQLiteColumn("profile", "nvarchar", 250));
            columns.Add(new SQLiteColumn("follows", "int"));
            columns.Add(new SQLiteColumn("followers", "int"));
            columns.Add(new SQLiteColumn("statuses", "int"));
            columns.Add(new SQLiteColumn("verified", "varchar", 50));
            columns.Add(new SQLiteColumn("lastdate", "datetime"));
            columns.Add(new SQLiteColumn("focus", "int"));
            columns.Add(new SQLiteColumn("ignore", "int"));
            columns.Add(new SQLiteColumn("getstatuses", "int"));
            columns.Add(new SQLiteColumn("originals", "int"));
            columns.Add(new SQLiteColumn("retweets", "int"));
            CreateTable("sina_user", columns.ToArray());

            columns = new List<SQLiteColumn>();
            columns.Add(new SQLiteColumn("uid", "nvarchar", 16));
            columns.Add(new SQLiteColumn("bid", "nvarchar", 16));
            columns.Add(new SQLiteColumn("mid", "nvarchar", 50));
            columns.Add(new SQLiteColumn("url", "nvarchar", 250));
            columns.Add(new SQLiteColumn("retweeted", "int"));
            columns.Add(new SQLiteColumn("retuid", "nvarchar", 16));
            columns.Add(new SQLiteColumn("retbid", "nvarchar", 16));
            columns.Add(new SQLiteColumn("text", "nvarchar", 4000));
            columns.Add(new SQLiteColumn("pics", "int"));
            columns.Add(new SQLiteColumn("getpics", "int"));
            columns.Add(new SQLiteColumn("lastdate", "datetime"));
            columns.Add(new SQLiteColumn("focus", "int"));
            columns.Add(new SQLiteColumn("ignore", "int"));
            CreateTable("sina_status", columns.ToArray());
        }

        void CreateDB(string dbName)
        {
            SQLiteConnection.CreateFile($"{dbName}");
        }

        /// <summary>
        /// uid varchar(50), name varchar(50), avatar varchar(250), desc varchar(250),profile varchar(250),statuses int,followers int,verified varchar(50)
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columns"></param>
        void CreateTable(string tableName, SQLiteColumn[] columns)
        {
            using (SQLiteConnection con = new SQLiteConnection(strConn))
            {
                con.Open();
                var cmd = con.CreateCommand();
                StringBuilder sb = new StringBuilder();
                sb.Append($"create table {tableName} (id INTEGER PRIMARY KEY AUTOINCREMENT");
                foreach(var column in columns)
                {
                    if (column.Length != 0)
                    {
                        sb.Append($",{column.ColumnName} {column.DataType}({column.Length})");
                    }
                    else
                    {
                        sb.Append($",{column.ColumnName} {column.DataType}");
                    }
                }
                sb.Append(")");
                cmd.CommandText = sb.ToString();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }


        public bool CreateEntity(BaseEntity entity, string table)
        {
            using (SQLiteConnection con = new SQLiteConnection(strConn))
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

                    sbCol.Append($",{field.Name}");

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
                    log4net.LogManager.GetLogger("logAppender").Error(ex);
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
            using (SQLiteConnection con = new SQLiteConnection(strConn))
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
                        sb.Append($",{field.Name}='{value}'");
                    }
                }
                cmd.CommandText = $"update {table} set lastdate='{DateTime.Now.ToString("s")}' {sb.ToString()} where {col}='{val}'";
                try
                {
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    log4net.LogManager.GetLogger("logAppender").Error(ex);
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
            using (SQLiteConnection con = new SQLiteConnection(strConn))
            {
                con.Open();
                var cmd = con.CreateCommand();

                StringBuilder sb = new StringBuilder();
                cmd.CommandText = $"delete from {table} where {col}='{val}'";
                try
                {
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    log4net.LogManager.GetLogger("logAppender").Error(ex);
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
            using (SQLiteConnection con = new SQLiteConnection(strConn))
            {
                con.Open();
                var cmd = con.CreateCommand();

                cmd.CommandText = $"select count(*) from {table} where {column}='{value}'";
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
                    log4net.LogManager.GetLogger("logAppender").Error(ex);
                    return false;
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
            using (SQLiteConnection con = new SQLiteConnection(strConn))
            {
                try
                {
                    con.Open();
                    var cmd = con.CreateCommand();
                    cmd.CommandText = $"select * from {table} where {where}";
                    var ada = new SQLiteDataAdapter(cmd);
                    var dt = new DataTable(table);
                    ada.Fill(dt);

                    var lst = new List<TEntity>();
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
                                else
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
                    log4net.LogManager.GetLogger("logAppender").Error(ex);
                    return null;
                }
                finally
                {
                    con.Close();
                }
            }
        }

        public List<string> GetGroupStrings(string table, string groupColumn, string where)
        {
            using (SQLiteConnection con = new SQLiteConnection(strConn))
            {
                try
                {
                    con.Open();
                    var cmd = con.CreateCommand();
                    cmd.CommandText = $"select {groupColumn} from {table} where {where} group by {groupColumn}";
                    var ada = new SQLiteDataAdapter(cmd);
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
                    log4net.LogManager.GetLogger("logAppender").Error(ex);
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
