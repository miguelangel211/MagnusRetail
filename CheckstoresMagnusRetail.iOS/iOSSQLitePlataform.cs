using System;
using System.IO;
using System.Threading.Tasks;
using CheckstoresMagnusRetail.iOS;
using CheckstoresMagnusRetail.sqlrepo;
using Mono.Data.Sqlite;
using SQLite;
using Xamarin.Essentials;
[assembly:Xamarin.Forms.Dependency(typeof(iOSSQLitePlataform))]
namespace CheckstoresMagnusRetail.iOS
{
    public class iOSSQLitePlataform : ISQLitePlataform
    {
        /*
        private string GetPath() {
            var dbName = "checkstore.db3";
            string personalfolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string librarryFolder = Path.Combine(personalfolder,"..","Library");
            string path = Path.Combine(librarryFolder,dbName);
            return path;
        }*/



        SQLiteAsyncConnection ISQLitePlataform.GetAsyncConnection()
        {
            return new SQLiteAsyncConnection(GetPath(), SQLiteOpenFlags.ReadWrite
     | SQLiteOpenFlags.FullMutex);
        }

        SQLiteConnection ISQLitePlataform.GetConnection()
        {
            return new SQLiteConnection(GetPath(), SQLiteOpenFlags.ReadWrite
     | SQLiteOpenFlags.FullMutex);
        }



        public string GetPath()
        {
            string dbName = "Checkstorev2.db3";
            string path = "";
            var ruta = Task.Run(async () => {
                path = await SecureStorage.GetAsync("rutadb");
            }
              );
            Task.WaitAll(ruta);
            if (path == "" || path == null || !File.Exists(path))
                path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), dbName);
            return path;
        }

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
        public void createdatabase(string command, bool reiniciar)
        {
            string pt = GetPath();

            bool exists = File.Exists(pt);
             
            if (exists == false || reiniciar)
            {
                if (exists || reiniciar)
                {
                    File.Delete(pt);

                }
                SqliteConnection.CreateFile(pt);

                crartablasenBD(command);

            }


        }
    }
}
