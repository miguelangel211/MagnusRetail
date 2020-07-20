using System.IO;
using SQLite;
using Xamarin.Forms;

namespace CheckstoresMagnusRetail.sqlrepo
{
    public class SQLitemethods
    {
        ISQLitePlataform plataform;
        string commands = @"PRAGMA foreign_keys=OFF;
BEGIN TRANSACTION;
CREATE TABLE [Usuario] (
  [UsuarioID] int PRIMARY KEY  NOT NULL
, [RoleID] int NOT NULL
, [Nombre] nvarchar(200) NOT NULL COLLATE NOCASE
, [ApPaterno] nvarchar(200) NOT NULL COLLATE NOCASE
, [ApMaterno] nvarchar(200) NOT NULL COLLATE NOCASE
, [Foto] image NULL
, [FotoRuta] nvarchar(2048) NULL COLLATE NOCASE
, [CorreoElectronico] nvarchar(100) NOT NULL COLLATE NOCASE
, [Telefono] nvarchar(20) NOT NULL COLLATE NOCASE
, [Activo] bit NOT NULL
, [FechaHoraCreacion] datetime  NULL
, [FechaHoraActualiza] datetime NULL
, [NombreUsuario] nvarchar(100) NOT NULL COLLATE NOCASE
, [Password] nvarchar(100) NOT NULL COLLATE NOCASE
);
CREATE TABLE [ServicioEstatus] (
  [ServicioEstatusID] int PRIMARY KEY  NOT NULL
, [Descripcion] nvarchar(100) NOT NULL COLLATE NOCASE
, [FechaHoraCreacion] datetime  NULL
, [FechaHoraModifica] datetime NULL
);
CREATE TABLE [Servicio] (
  [ServicioID] int PRIMARY KEY  NOT NULL
, [ServicioTipo] nvarchar(50) NULL COLLATE NOCASE
, [Nombre] nvarchar(100) NOT NULL COLLATE NOCASE
, [Descripcion] nvarchar(200) NOT NULL COLLATE NOCASE
, [ServicioEstatusID] int NOT NULL
, [ServicioFechaHora] datetime NOT NULL
, [NombreCadena] nvarchar(250) NULL COLLATE NOCASE
, [NombreTienda] nvarchar(250) NULL COLLATE NOCASE
, [Ubicacion] nvarchar(250) NULL COLLATE NOCASE
, [DispositivoID] nvarchar(50) NULL COLLATE NOCASE
, [DispositivoNombre] nvarchar(50) NULL COLLATE NOCASE
, [FechaHoraLocal] datetime NULL
, [UsuarioLocalID] int NULL
, [Sincronizado] bit NULL
, [SincronizadoFecha] datetime NULL
);
CREATE TABLE [ServicioUsuario] (
  [ServicioID] int NOT NULL
, [UsuarioID] int NOT NULL
);
CREATE TABLE [ServicioLayout] (
  [ServicioID] int NOT NULL
, [ServicioLayoutLocalID] INTEGER PRIMARY KEY NOT NULL
, [ServicioLayoutID] int NULL
, [LayoutImagen] image NULL
, [LayoutRuta] nvarchar(512) NULL COLLATE NOCASE
, [DispositivoID] nvarchar(50) NULL COLLATE NOCASE
, [DispositivoNombre] nvarchar(50) NULL COLLATE NOCASE
, [FechaHoraLocal] datetime NULL
, [UsuarioLocalID] int NULL
, [Sincronizado] bit NULL
, [SincronizadoFecha] datetime NULL
);
CREATE TABLE [Producto] (
  [ProductoLocalID] INTEGER PRIMARY KEY  NOT NULL
, [ProductoID] int NULL
, [UPC] nvarchar(50) NULL COLLATE NOCASE
, [Nombre] nvarchar(150) NULL COLLATE NOCASE
, [Marca] nvarchar(150) NULL COLLATE NOCASE
, [Fabricante] nvarchar(150) NULL COLLATE NOCASE
, [Alto] nchar(10) NULL COLLATE NOCASE
, [Ancho] nchar(10) NULL COLLATE NOCASE
, [Gramaje] nchar(10) NULL COLLATE NOCASE
, [Profundo] nchar(10) NULL COLLATE NOCASE
, [DispositivoID] nvarchar(50) NULL COLLATE NOCASE
, [DispositivoNombre] nvarchar(50) NULL COLLATE NOCASE
, [FechaHoraLocal] datetime NULL
, [UsuarioLocalID] int NULL
, [Sincronizado] bit NULL
, [SincronizadoFecha] datetime NULL
, [CategoriaID] int NULL
);
CREATE TABLE [ProductoImagen] (
  [ProductoImagenLocalID] INTEGER PRIMARY KEY  NOT NULL
, [ProductoImagenID] int NULL
, [ProductoID] int NULL
, [ProductoImagen] image NULL
, [ProductImageRuta] nvarchar(512) NULL COLLATE NOCASE
, [ProductoLocalID] int NULL
, [DispositivoID] nvarchar(50) NULL COLLATE NOCASE
, [DispositivoNombre] nvarchar(50) NULL COLLATE NOCASE
, [FechaHoraLocal] datetime NULL
, [UsuarioLocalID] int NULL
, [Sincronizado] bit NULL
, [SincronizadoFecha] datetime NULL
);
CREATE TABLE [MuebleTipo] (
  [MuebleTipoID] int NOT NULL
, [Descripcion] nvarchar(200) NOT NULL COLLATE NOCASE
, [Activo] bit NOT NULL
, [FechaHoraCreacion] datetime DEFAULT current_timestamp NOT NULL
, [FechaHoraModifica] datetime NULL
, CONSTRAINT [PK__furnitur__CCF985BECEB4CBE2] PRIMARY KEY ([MuebleTipoID])
);
CREATE TABLE [ServicioMueble] (
  [ServicioID] int NOT NULL
, [ServicioMuebleLocalID] INTEGER NOT NULL PRIMARY KEY 
, [MuebleTipoID] int NOT NULL
, [ServicioMuebleID] int NULL
, [MuebleCara] nvarchar(5) NOT NULL COLLATE NOCASE
, [MueblePasillo] nvarchar(5) NOT NULL COLLATE NOCASE
, [MuebleTramoInicial] nvarchar(5) NOT NULL COLLATE NOCASE
, [MuebleComentario] nvarchar(100) COLLATE NOCASE
, [MuebleAltura] decimal NOT NULL
, [MuebleTramos] int NOT NULL
, [MuebleMedidasIguales] bit NOT NULL
, [MuebleNivelMinimo] int NOT NULL
, [MuebleNivelMaximo] int NOT NULL
, [DispositivoID] nvarchar(50) NULL COLLATE NOCASE
, [DispositivoNombre] nvarchar(50) NULL COLLATE NOCASE
, [FechaHoraLocal] datetime NULL
, [UsuarioLocalID] int NULL
, [Sincronizado] bit NULL
, [SincronizadoFecha] datetime NULL
);
CREATE TABLE [ServicioMuebleTramo] (
  [ServicioID] int NOT NULL
, [ServicioMuebleTramoLocalID] INTEGER PRIMARY KEY NOT NULL
, [ServicioMuebleLocalID] int 
, [ServicioMuebleTramoID] int NULL
, [ServicioMuebleID] int NULL
, [TramoNumero] int NULL
, [DispositivoID] nvarchar(50) NULL COLLATE NOCASE
, [DispositivoNombre] nvarchar(50) NULL COLLATE NOCASE
, [FechaHoraLocal] datetime NULL
, [UsuarioLocalID] int NULL
, [Sincronizado] bit NULL
, [SincronizadoFecha] datetime NULL
);
CREATE TABLE [ServicioMuebleTramoNivel] (
  [ServicioMuebleTramoNivelLocalID] INTEGER PRIMARY KEY NOT NULL 
, [ServicioMuebleTramoNivelID] int NULL
, [ServicioID] int  NULL
, [ServicioMuebleTramoLocalID] int NOT NULL
, [ServicioMuebleLocalID] int  NULL
, [ServicioMuebleTramoID] int NULL
, [ServicioMuebleID] int NULL
, [Profundo] int  NULL
, [Ancho] int  NULL
, [Alto] int  NULL
, [DispositivoID] nvarchar(50) NULL COLLATE NOCASE
, [DispositivoNombre] nvarchar(50) NULL COLLATE NOCASE
, [FechaHoraLocal] datetime NULL
, [UsuarioLocalID] int NULL
, [Sincronizado] bit NULL
, [SincronizadoFecha] datetime NULL
);
CREATE TABLE [ServicioMuebleProductoNivel] (
  [ServicioMuebleProductoNivelLocalID] INTEGER PRIMARY KEY NOT NULL
, [ServicioMuebleTramoNivelLocalID] int NOT NULL
, [ServicioMuebleTramoNivelID] int NOT NULL
, [ServicioMuebleProductoNivelID] int NULL
, [Frente] int NULL
, [Profundo] int NULL
, [Posicion] int NULL
, [ProductoLocalID] int NOT NULL
, [ProductoID] int NULL
, [DispositivoID] nvarchar(50) NULL COLLATE NOCASE
, [DispositivoNombre] nvarchar(50) NULL COLLATE NOCASE
, [FechaHoraLocal] datetime NULL
, [UsuarioLocalID] int NULL
, [Sincronizado] bit NULL
, [SincronizadoFecha] datetime NULL
, [ServicioMuebleTramoNivelCategoriaLocalID] int NOT NULL
, [ServicioMuebleTramoNivelCategoriaID] int  NULL
, [CategoriaLocalID] int NOT NULL
, [CategoriaID] int  NULL
);
CREATE TABLE [ServicioMuebleImagen] (
  [ServicioID] int NOT NULL
, [ServicioMuebleImagenLocalID] INTEGER PRIMARY KEY  NOT NULL
, [ServicioMuebleLocalID] int NULL
, [ServicioMuebleImagenID] int NULL
, [ServicioMuebleID] int NULL
, [MuebleImagen] image NULL
, [MuebleImagenRuta] nvarchar(512) NULL COLLATE NOCASE
, [DispositivoID] nvarchar(50) NULL COLLATE NOCASE
, [DispositivoNombre] nvarchar(50) NULL COLLATE NOCASE
, [FechaHoraLocal] datetime NULL
, [UsuarioLocalID] int NULL
, [Sincronizado] bit NULL
, [SincronizadoFecha] datetime NULL
);
CREATE TABLE [Categoria] (
  [CategoriaLocalID] INTEGER NOT NULL PRIMARY KEY 
, [CategoriaID] int NULL
, [CategoriaNombre] nvarchar(200) NOT NULL COLLATE NOCASE
, [CategoriaImagen] image NULL
, [CategoriaImagenRuta] nvarchar(512) NULL COLLATE NOCASE
, [Activo] bit NOT NULL
, [DispositivoID] nvarchar(50) NULL COLLATE NOCASE
, [DispositivoNombre] nvarchar(50) NULL COLLATE NOCASE
, [FechaHoraLocal] datetime NULL
, [UsuarioLocalID] int NULL
, [Sincronizado] bit NULL
, [SincronizadoFecha] datetime NULL
);
CREATE TABLE [ServicioMuebleTramoNivelCategoria] (
  [CategoriaLocalID] int NULL
, [ServicioMuebleTramoNivelCategoriaLocalID] INTEGER NOT NULL PRIMARY KEY 
, [ServicioMuebleTramoNivelCategoriaID] int NULL
, [ServicioMuebleTramoID] int NULL
,[ServicioMuebleTramoLocalID] int NULL
, [ServicioMuebleTramoNivelLocalID] int NULL
, [ServicioMuebleTramoNivelID] int NULL
, [CategoriaID] int NULL
, [DispositivoID] nvarchar(50) NULL COLLATE NOCASE
, [DispositivoNombre] nvarchar(50) NULL COLLATE NOCASE
, [FechaHoraLocal] datetime NULL
, [UsuarioLocalID] int NULL
, [Sincronizado] bit NULL
, [SincronizadoFecha] datetime NULL
);
CREATE TABLE [ServicioMuebleCategoria] (
  [ServicioID] int NOT NULL
, [ServicioMuebleLocalID] int NOT NULL
, [CategoriaLocalID] int NOT NULL
, [CategoriaID] int NULL
, [ServicioMuebleID] int NULL
);



COMMIT;

"
;

        string path;
        public SQLitemethods()
        {
            plataform = DependencyService.Get<ISQLitePlataform>();


        }

        public void reiniciardb() {
            plataform.createdatabase(commands, true);

        }

        public void createdb() {
            //   SQLiteConnection db = plataform.GetConnection();

         
           plataform.createdatabase(commands,false);
        }

        
    }
}
