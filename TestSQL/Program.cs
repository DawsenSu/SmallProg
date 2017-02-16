using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace TestSQL
{
    class Program
    {
        static void Main(string[] args)
        {
            Program p = new Program();
            p.UseDataReader();
        }
        public void CreateTestdbWithData()
        {
            SQLiteConnection conn = null;
            string dbpath = "Data Source =" + Environment.CurrentDirectory + @"\test.db";
            conn = new SQLiteConnection(dbpath);
            conn.Open();

            string sql = "CREATE TABLE IF NOT EXISTS student(id integer, name varchar(20), sex varchar(2))";
            SQLiteCommand cmdCreateTable = new SQLiteCommand(sql, conn);
            cmdCreateTable.ExecuteNonQuery();

            SQLiteCommand cmdInsert = new SQLiteCommand(conn);
            cmdInsert.CommandText = "INSERT INTO student VALUES(1,'XIAOHONG','M')";
            cmdInsert.ExecuteNonQuery();
            cmdInsert.CommandText = "INSERT INTO student VALUES(2,'XIAOLI','M')";
            cmdInsert.ExecuteNonQuery();
            cmdInsert.CommandText = "INSERT INTO student VALUES(3,'XIAOMING','M')";
            cmdInsert.ExecuteNonQuery();
            conn.Close();
        }
        public void UseDataReader()
        {
            string dbpath = "Data Source =" + Environment.CurrentDirectory + @"\test.db";
            string selectString = "SELECT name,sex FROM student";
            SQLiteConnection m_connection = new SQLiteConnection(dbpath);
            m_connection.Open();
            SQLiteCommand m_select = new SQLiteCommand(selectString, m_connection);
            SQLiteDataReader m_Reader = m_select.ExecuteReader();
            while(m_Reader.Read())
            {
                Console.WriteLine($"{m_Reader.GetString(0)} is {m_Reader.GetString(1)}");
            }
            m_Reader.Close();
            m_connection.Close();
            Console.ReadLine();
        }
    }
}
