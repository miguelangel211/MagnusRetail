using System;
using System.IO;
using CheckstoresMagnusRetail.Droid;
using CheckstoresMagnusRetail.sqlrepo;
using SQLite;
using Mono.Data.Sqlite;
using System.Diagnostics;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(AndroidSQLitePlataform))]
namespace CheckstoresMagnusRetail.Droid
{
    public class AndroidSQLitePlataform : ISQLitePlataform
    {
        public void crartablasenBD(string comandocreacion)
        {
            string pt = GetPath();

            SqliteConnection connectio = new SqliteConnection("Data Source=" + pt);
            connectio.Open();
            var c = connectio.CreateCommand();
            c.CommandText = comandocreacion;
             c.ExecuteNonQuery();         
            connectio.Close();
            System.Threading.Thread.Sleep(2000);

        }


        public void createdatabase(string command,bool reiniciar)
        {
            string pt = GetPath();

            bool exists = File.Exists(pt);

            if (exists==false || reiniciar)
            {
                if (exists||reiniciar) {
                   File.Delete(pt);

                }
                Mono.Data.Sqlite.SqliteConnection.CreateFile(pt);

                crartablasenBD(command);
                
            }


        }

    

        SQLiteAsyncConnection ISQLitePlataform.GetAsyncConnection()
        {
            return new SQLiteAsyncConnection(GetPath(),SQLiteOpenFlags.ReadWrite);
        }

        SQLiteConnection ISQLitePlataform.GetConnection()
        {
            return new SQLiteConnection(GetPath(),SQLiteOpenFlags.ReadWrite);
        }

         string GetPath() {
            string dbName = "Checkstorev2.db3";
            string path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal),dbName);
            return path;
        }

        string ISQLitePlataform.GetPath()
        {
            string dbName = "Checkstorev2.db3";
            string path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), dbName);
            return path;
        }
    }
}
