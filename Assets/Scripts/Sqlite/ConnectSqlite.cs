using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLiteUnity;

public class ConnectSqlite : MonoBehaviour
{
    private static string _fileName = "test.db";
    // DB
    public static SQLite Database;
    private void Start()
    {

    }
    /// <summary>
    /// SELECT文オンリーの関数。
    /// </summary>
    /// <param name="sql"></param>
    /// <returns></returns>
    public static SQLiteTable GetSqliteQuery(string sql)
    {
        Database = new SQLite(_fileName);

        return Database.ExecuteQuery(sql);


        /*
        try
        {
            //sqliteのdbファイル
            var db = new SqliteDatabase(_fileName);
            var query = db.ExecuteQuery(sql);
            return query;
        }
        catch(Exception ex)
        {
            Debug.Log(ex);
            return null;
        }
        */

        
    }
    public static void SqliteDelete(string sql)
    {
        Database = new SQLite(_fileName);

        Database.ExecuteNonQuery(sql);
        Debug.Log(sql);
    }
    /// <summary>
    /// Insert文のみ
    /// </summary>
    /// <param name="sql"></param>
    public static void SqliteInsert(string sql)
    {
        Database = new SQLite(_fileName);

        Database.ExecuteNonQuery(sql);
        Debug.Log(sql);
        /*
        try
        {
            var db = new SqliteDatabase(_fileName);
            db.ExecuteNonQuery(sql);
        }
        catch
        {
            throw;
        }
        */
    }
    /// <summary>
    /// Update関数のみ
    /// </summary>
    /// <param name="sql"></param>
    public static void SqliteUpdate(string sql)
    {
        try
        {
            var db = new SqliteDatabase(_fileName);
            db.ExecuteQuery(sql);
        }
        catch
        {
            throw;
        }
    }
    public static void sample()
    {
        SQLiteTable s;
        s = GetSqliteQuery("SELECT * FROM Item");
        if (s != null)
        {
            foreach (var row in s.Rows)
            {
                var id = row["id"];
                var name = row["name"];
                var data1 = row["type"];

                var text = $"ID:{id}, Name:{name}, DATA1:{data1}";

                Item item = new Item();
                item.id = (int)id;
                item.name = name.ToString();
                item.item = (Item.ItemEnum)data1;

                Debug.Log($"ID:{item.id}, Name:{item.name}, DATA1:{item.item}");

            }
        }
    }
}
