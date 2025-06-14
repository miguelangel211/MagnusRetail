﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using CheckstoresMagnusRetail.ApiRepo;
using Newtonsoft.Json;
using SQLite;
using Xamarin.Essentials;
using Xamarin.Forms;
using XF.Material.Forms.UI.Dialogs;
using Color = System.Drawing.Color;

namespace CheckstoresMagnusRetail.sqlrepo
{
    public abstract class ITableoperations<T>:TableOperationsBase
    {
        public ApiRequest repoapi = new ApiRequest();

        public ISQLitePlataform plataform;
        public SQLiteAsyncConnection db;
        public SQLiteConnection dbsincrona;

        private readonly object Lock = new object();

        public resultfromLocalDB<T> resultado;

      public  ITableoperations(){
            plataform = DependencyService.Get<ISQLitePlataform>();
            db = plataform.GetAsyncConnection();
            dbsincrona = plataform.GetConnection();
            resultado = new resultfromLocalDB<T>() { realizado = false, Errores = "" };

        }

       

        public async Task<bool> probarred(string call) {
            var re = await repoapi.Pruebadeconexion();
            if (!re.realizado) {
                await Reportarproceso(call + re.Errores); }
            return !re.realizado;
        }

        public virtual  async Task insertarregistro(T parameter)
        {
            try
            {
                lock (this.Lock)
                {
                     db.InsertAsync(parameter);
                }
                // dbsincrona.Insert(parameter);

                // await  db.CloseAsync();

            }
            catch (Exception ex) {
                Debug.WriteLine(ex.Message);
                await Reportarproceso("Error Guardado: " + ex.Message, true, JsonConvert.SerializeObject(parameter), "Insertar nuevo registro sql");

            }
        }
        public virtual async Task ActualizarDatos(T parameter)
        {
            try
            {

               await db.UpdateAsync(parameter);
             //  await db.UpdateAsync(parameter);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                await Reportarproceso("Error Guardado: " + ex.Message, true, JsonConvert.SerializeObject(parameter), "Actualizar  registro sql");

            }
        }

        public virtual async Task Insertbatch(List<T> parameter) {
            try {
                //  var guardardb = plataform.GetConnection();
                lock (this.Lock)
                {
                     db.InsertAllAsync(parameter);
                }
              }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                await Reportarproceso("Error Guardado: " + ex.Message, true, JsonConvert.SerializeObject(parameter), "Insertar nuevo registro sql");

            }
        }
      //  abstract public Task  insertarregistro(T parameter);
        abstract public Task<resultfromLocalDB<T>> consultarDatoconcisoAsync(T parameter);
        abstract public Task clearData();
        abstract public Task SincronizaciondesdeAPI();
        abstract public Task<resultfromLocalDB<List<T>>> consultarListadodedata(object parameter);

        public async Task insertdata(List<T> datos, ITableoperations<T> operations) {
            try
            {
                await clearData();
                await Task.Delay(100);
                List<Task> tareas = new List<Task>();
                int count = 1;
              
                  await   Insertbatch(datos);
                
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            /*
            foreach (T objeto in datos)
            {
                
                

                if (count == 25)
                {
                    await Task.WhenAll(tareas);
                    tareas.Clear();
                }
               
                    tareas.Add(insertarregistro(objeto));
                if (count == datos.Count)
                    await Task.WhenAll(tareas);
                count++;
                
            }*/

        }

    }
    
}
