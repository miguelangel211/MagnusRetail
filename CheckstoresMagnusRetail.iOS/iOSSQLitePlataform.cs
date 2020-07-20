using System;
using System.IO;
using CheckstoresMagnusRetail.iOS;
using CheckstoresMagnusRetail.sqlrepo;
using SQLite;

[assembly:Xamarin.Forms.Dependency(typeof(iOSSQLitePlataform))]
namespace CheckstoresMagnusRetail.iOS
{
    public class iOSSQLitePlataform:ISQLitePlataform
    {
        private string GetPath() {
            var dbName = "checkstore.db3";
            string personalfolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string librarryFolder = Path.Combine(personalfolder,"..","Library");
            string path = Path.Combine(librarryFolder,dbName);
            return path;
        }


        public SQLiteAsyncConnection GetAsyncConnection()
        {
            return new SQLiteAsyncConnection(GetPath());
        }

        public SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(GetPath());
        }

        public void createdatabase(string command, bool reiniciar)
        {
            throw new NotImplementedException();
        }

        string ISQLitePlataform.GetPath()
        {
            throw new NotImplementedException();
        }

        public void crartablasenBD(string comandocreacion)
        {
            throw new NotImplementedException();
        }
    }
}
