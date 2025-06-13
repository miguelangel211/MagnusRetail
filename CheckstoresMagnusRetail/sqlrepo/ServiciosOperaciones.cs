using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using CheckstoresMagnusRetail.ApiRepo;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace CheckstoresMagnusRetail.sqlrepo
{

    public class ServiciosOperaciones : ITableoperations<Servicio>
    {



        public override async Task<resultfromLocalDB<Servicio>> consultarDatoconcisoAsync(Servicio parameter)
        {
            try
            {
                resultado.Result = await db.Table<Servicio>().FirstOrDefaultAsync(x => x.ServicioID == parameter.ServicioID);
                if (resultado.Result != null)
                {
                    resultado.realizado = true;
                }
                else
                {
                    resultado.realizado = false;
                }
                return resultado;
            }
            catch
            {
                resultado.realizado = false;
                return resultado;
            }
        }

        public async Task<resultfromLocalDB<List<Servicio>>> ListadeServiciosporusuario()
        {
            resultfromLocalDB<List<Servicio>> res = new resultfromLocalDB<List<Servicio>>() { realizado = true, Errores = "" };
            List<Servicio> m = new List<Servicio>();
            string userdata = await SecureStorage.GetAsync("User");
            var user = JsonConvert.DeserializeObject<Usuario>(userdata);
            try
            {
                var usuarioservicio =await db.Table<ServicioUsuario>().Where(x=>x.UsuarioID==user.UsuarioID).ToListAsync();
                foreach (var u in usuarioservicio) {
                    var temp = await db.Table<Servicio>().FirstOrDefaultAsync(x => x.ServicioID == u.ServicioID);
                    if(temp!=null)
                        m.Add(temp);
                }
                if (m.Count > 0)
                {
                    res.Result = m;
                    res.realizado = true;

                }
                else
                {
                    res.realizado = false;
                    res.Errores = "No se encontraron Servicios";
                }
                return res;
            }
            catch
            {
                res.realizado = false;
                res.Errores = "No se encontraron Servicios";
                return res;
            }
        }



        public override async Task clearData()
        {
            try
            {
                await db.ExecuteAsync("Delete from Servicio");
            }
            catch(Exception ex) {
                Debug.WriteLine(ex.Message);
            }
        }

        public override async Task insertarregistro(Servicio parameter)
        {
            await Reportarproceso("Guardando Servicio " + parameter.ServicioID);
            parameter.FechaHoraLocal = DateTime.Now;
            parameter.UsuarioLocalID = UsuarioID;
            parameter.DispositivoNombre = DispositivoName;

            await base.insertarregistro(parameter);

        }

        public override async Task SincronizaciondesdeAPI()
        {
            var prueba = await probarred("Descarga de Servicios: ");
            if (prueba)
                return;
            try
            {
                await Reportarproceso("Descargando Servicios " );

                resultfromAPIProgramas datos = await repoapi.DescargaprogramasUsuario();
                if (datos.realizado)
                {
                    if (datos.Programa.Count > 0)
                    {

                        await insertdata(datos.Programa,this);
                    }
                }
                else
                {
                    await Reportarproceso("Error al descargar Servicios " + datos.Errores,
                        true, "", "Descarga de servicios");
                }
            }
            catch (Exception ex)
            {
                await Reportarproceso("Error al descargar Servicios " + ex.Message + ex.StackTrace,
                    true,"", "Descarga de servicios");

                Debug.WriteLine(ex.Message);
            }

        }


        public async Task CargaConcluirServicio() {
            var prueba = await probarred("Concluir servicio: ");
            if (prueba)
                return;
            string usuariodata = await SecureStorage.GetAsync("User");
            var usuario = JsonConvert.DeserializeObject<Usuario>(usuariodata);
            var m = await db.Table<Servicio>().Where(x => x.Sincronizado == false).ToListAsync();

            if (m.Count > 0)
            {
                foreach (var i in m)
                {
                    i.DispositivoID = DispositivoID;
                    i.DispositivoNombre = DispositivoName;
                    i.UsuarioID = usuario.UsuarioID;
                    i.FechaHoraLocal = DateTime.Now;
                    await Reportarproceso("Concluir Servicio " + i.ServicioID);

                    var r = await repoapi.ConcluirServicio(i);
                    if (r.realizado)
                    {
                        try
                        {
                            var tempm = await db.Table<Servicio>().FirstOrDefaultAsync(x => x.ServicioID == i.ServicioID);
                            tempm.Sincronizado = true;
                            await db.UpdateAsync(tempm);
                        }
                        catch { }
                    }
                    else
                    {
                        await Reportarproceso("Error al concluir servicio " + resultado.Errores, true, JsonConvert.SerializeObject(i), "Conclusion de servicio");

                    }
                }
            }
           
        }
        public async Task ConcluirServicio(Servicio servicio) {
          //  await db.ExecuteAsync("update Servicio set ServicioEstatusID=3 and Sincronizado=0 where ServicioID = "+ servicio.ServicioID);
            var temps=await db.Table<Servicio>().FirstOrDefaultAsync(x=>x.ServicioID == servicio.ServicioID);
      temps.ServicioEstatusID = 3;
            temps.Sincronizado = false;
            await db.UpdateAsync(temps);
        }

        public override Task<resultfromLocalDB<List<Servicio>>> consultarListadodedata(object parameter)
        {
            throw new NotImplementedException();
        }
    }
}
