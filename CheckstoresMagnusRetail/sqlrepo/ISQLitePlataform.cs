using System;
using SQLite;
namespace CheckstoresMagnusRetail.sqlrepo
{
    
        public interface ISQLitePlataform
        {
            SQLiteConnection GetConnection();
            SQLiteAsyncConnection GetAsyncConnection();
            void createdatabase(string command, bool reiniciar);
            string GetPath();
            void crartablasenBD(string comandocreacion);
        }
    
}
