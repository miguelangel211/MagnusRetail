using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CheckstoresMagnusRetail.DataModels;
using CheckstoresMagnusRetail.sqlrepo;
using CheckstoresMagnusRetail.Views.ViewCells;
using Xamarin.Forms;

namespace CheckstoresMagnusRetail.ViewModels
{
    public class CrearNuevoMuebleModel:BaseViewModel
    {
        #region colecciones
        private ObservableCollection<Categoria> categorias;
        public ObservableCollection<Categoria> Categorias { get { return categorias; } set { categorias = value; OnPropertyChanged(); } }
        public ObservableCollection<MuebleTipo> tiposMuebles;
        public ObservableCollection<MuebleTipo> TiposMuebles { get { return tiposMuebles; } set { tiposMuebles = value;OnPropertyChanged(); } }
        public ObservableCollection<string> carasmueble;
        public ObservableCollection<string> CarasMueble { get { return carasmueble; } set { carasmueble = value;OnPropertyChanged(); } }

        public ObservableCollection<ServicioMuebleTramo> tramos;
        public ObservableCollection<ServicioMuebleTramo> Tramos { get { return tramos; } set { tramos = value;OnPropertyChanged(); } }
        public ObservableCollection<ServicioMuebleTramoNivel> nivelesdb;
        public ObservableCollection<ServicioMuebleTramoNivel> NIvelesDB { get { return nivelesdb; } set { nivelesdb = value; OnPropertyChanged(); } }
        public ObservableCollection<Opcionboleana> opciones;
        public ObservableCollection<Opcionboleana> Opciones { get { return opciones; } set { opciones = value;OnPropertyChanged(); } }
        #endregion  colecciones
        public FormaDeMuebles formdata;
        public FormaDeMuebles FormData { get { return formdata; }set { formdata = value;OnPropertyChanged(); } }

        public MuebleTipo tipoSeleccionado;
        public MuebleTipo TipoSeleccionado { get { return tipoSeleccionado; } set { tipoSeleccionado = value;OnPropertyChanged(); } }
        public Opcionboleana medidaigualseleccionada;
        public Opcionboleana MedidaIgualSeleccionada { get { return medidaigualseleccionada; } set { medidaigualseleccionada = value; OnPropertyChanged(); } }
        #region comandos
        public Command cambioentramos;
        public Command Cambioentramos { get { return cambioentramos; }set { cambioentramos = value;OnPropertyChanged(); } }
        public Command cambioennivelesminimo;
        public Command Cambioennivelesminimo { get { return cambioennivelesminimo; } set { cambioennivelesminimo = value; OnPropertyChanged(); } }
        public Command cambioennivelesmaximo;
        public Command Cambioennivelesmaximo { get { return cambioennivelesmaximo; } set { cambioennivelesmaximo = value; OnPropertyChanged(); } }
        public Command guardadoDedatos;
        public Command GuardadoDeDatos { get { return guardadoDedatos; } set { guardadoDedatos = value;OnPropertyChanged(); } }
        public Command cambioentramoinicial;
        public Command CambioentramoInicial { get { return cambioentramoinicial; } set { cambioentramoinicial = value; OnPropertyChanged();} }
        #endregion comandos
        public ServicioMueble muebledata;
        public ServicioMueble MuebleData { get { return muebledata; } set { muebledata = value;OnPropertyChanged(); } }
        bool medidaigual=false;

        public bool modf;
       public bool modoforma { get { return modf; } set { modf = value;OnPropertyChanged(); } }
        public bool camposeditables;
        public bool Camposeditables { get { return camposeditables; } set { camposeditables = value; OnPropertyChanged(); } }
        #region repos
        CategoriasOperaciones catrepo = new CategoriasOperaciones();
        ServicioMueblesOperaciones mueblerepo = new ServicioMueblesOperaciones();
        MueblesTiposoperaciones tiposmueblesrepo = new MueblesTiposoperaciones();
        TramosOperaciones tramorepo = new TramosOperaciones();
        TramoNivelesOperaciones nivelesrepo = new TramoNivelesOperaciones();
        MuebleTramoNivelCategoriaOperaciones categoriasnivelesrepo = new MuebleTramoNivelCategoriaOperaciones();
        #endregion repos
        public CrearNuevoMuebleModel(bool editando,MuebleModel mueble)
        {
            CarasMueble = new ObservableCollection<string>();
            modoforma = editando;
            MuebleData = new ServicioMueble { ServicioMuebleID = mueble.MuebleID,
            ServicioID=mueble.ServicioID,ServicioMuebleLocalID=mueble.ServicioMuebleLocalID};
            Categorias = new ObservableCollection<Categoria>();
            TiposMuebles = new ObservableCollection<MuebleTipo>();
            Tramos = new ObservableCollection<ServicioMuebleTramo>();
            Opciones = new ObservableCollection<Opcionboleana>();
            Cambioentramos = new Command<string>(async (string parameter) => await GenerarTramosporforma(parameter));
            CambioentramoInicial = new Command<string>(async (string parameter) => await generartramosdesdetramoinicial(parameter));
            GuardadoDeDatos = new Command(async () =>await AlmacenarDatos()) ;
            FormData = new FormaDeMuebles() {Tramos=null };
            CarasMueble.Add("A");
            CarasMueble.Add("B");
            CarasMueble.Add("C");
            CarasMueble.Add("D");
            CarasMueble.Add("A1");
            CarasMueble.Add("B1");
            CarasMueble.Add("C1");
            CarasMueble.Add("D1");
            CarasMueble.Add("A2");
            CarasMueble.Add("B2");
            CarasMueble.Add("C2"); CarasMueble.Add("D2");
            Opciones.Add(new Opcionboleana { Activo = true, DisplayName = "SI" });
            Opciones.Add(new Opcionboleana { Activo = false, DisplayName = "NO" });

            MessagingCenter.Subscribe<NivelTramoCell, NIvelMedidasvalues>(this, "mensaje", async (sender, arg) =>
            {
                if (medidaigual) {
                    await Task.Run(async () =>
                    {
                        int valor;
                        NIvelMedidasvalues dato = (NIvelMedidasvalues)arg;
                        if (int.TryParse(dato.Valor, out valor))
                        {
                            try
                            {
                                switch (dato.Medida)
                                {
                                    case NivelPropiedad.Profundo: await cambiarprofundonivel(valor, dato.Nivel); break;
                                    case NivelPropiedad.Alto: await cambiaraltonivel(valor, dato.Nivel); break;
                                    case NivelPropiedad.Ancho: await cambiaranchonivel(valor, dato.Nivel); break;
                                }
                            }
                            catch { }
                        }
                    });
                } }


                );
        }


        public async Task cambiarprofundonivel(int value,int numeronivel) {
            foreach (var t in Tramos)
            {
                t.Niveles[numeronivel].Profundo = value;

            }
        }
        public async Task cambiaraltonivel(int value,int numeronivel)
        {
            foreach (var t in Tramos)
            {

                t.Niveles[numeronivel].Alto = value;
            }
        }

        public async Task cambiaranchonivel(int value,int numeronivel)
        {
            foreach (var t in Tramos)
            {

                t.Niveles[numeronivel].Ancho = value;

            }
        }

        public override Task CargarDatos () {
            return  cargadatas();

        }
        public async Task datocanbiadoenmedidas(bool activo) {
            medidaigual = activo;
            if (medidaigual) {
                await Task.Run(() => {
                    if (Tramos.Count > 0) {
                        try
                        {
                            var primernivel = Tramos.FirstOrDefault().Niveles;
                            foreach (var t in Tramos)
                            {
                                int index = 0;
                                foreach (var n in (t.Niveles ?? new ObservableCollection<ServicioMuebleTramoNivel>()))
                                {
                                    n.Alto = primernivel[index].Alto;
                                    n.Ancho = primernivel[index].Ancho;
                                    n.Profundo = primernivel[index].Profundo;
                                    index++;
                                }
                            }

                        }
                        catch { }
                    } }
                );
            }
        }


        public async Task cargadatas() {
            
            var categoriaslist=await  catrepo.ListadeCategorias();
            var mubles = await tiposmueblesrepo.ListadeTiposMuebles();
            if (mubles.realizado)
                TiposMuebles = new ObservableCollection<MuebleTipo>(mubles.Result);

            if (modoforma) {

                var categoriasdelmueble = await catrepo.ListadeCategoriasdelmuebleformaseleccionados(MuebleData);
                if (categoriasdelmueble.realizado)
                {
                    // foreach (var categoria in categoriasdelmueble.Result)
                    //   categoriaslist.Result.FirstOrDefault(x => x.CategoriaID == categoria.CategoriaID).Activo = true;
                    foreach (var c in categoriasdelmueble.Result) {
                        c.Activo = true;
                        c.Editable = false;
                    }
                    categoriaslist.Result = categoriasdelmueble.Result;
                }
                var t = await mueblerepo.consultarDatoconcisoAsync(muebledata);
                if (t.realizado)
                    MuebleData = t.Result;
                TipoSeleccionado = TiposMuebles.FirstOrDefault(x=>x.MuebleTipoID==MuebleData.MuebleTipoID);
                MedidaIgualSeleccionada = Opciones.FirstOrDefault(x=>x.Activo==MuebleData.MuebleMedidasIguales);
                var tramostemp = await tramorepo.consultarListadodedata(MuebleData);
                if (tramostemp.realizado)
                    Tramos = new ObservableCollection<ServicioMuebleTramo>(tramostemp.Result);
                var nivelestemp = await nivelesrepo.consultarListadodedata(muebledata);
                if (nivelestemp.realizado)
                    NIvelesDB = new ObservableCollection<ServicioMuebleTramoNivel>(nivelestemp.Result);
                await GenerarNivelesmodoeditando();
            }

            if (categoriaslist.realizado)
                Categorias = new ObservableCollection<Categoria>(categoriaslist.Result);
            Cambioennivelesminimo = new Command<string>(async (string parameter) => await cambiodeNivelMinimo(parameter));
            Cambioennivelesmaximo = new Command<string>(async (string parameter) => await cambiodeNivelMaximo(parameter));
        }

        public async Task cambiodeNivelMinimo(string parameter) {
            if (int.TryParse(parameter, out int result))
            {
                FormData.NivelMinimo = result;
                await GenerarNiveles();
            }
        }

        public async Task cambiodeNivelMaximo(string parameter)
        {
            if (int.TryParse(parameter, out int result))
            {
                FormData.NivelMaximo = result;
                await GenerarNiveles();
            }
        }

        public async Task GenerarNiveles() {
            //Tramos.Clear();

            if (FormData.NivelMinimo > formdata.NivelMaximo)
                return;

            int numeroniveles = FormData.NivelMaximo - FormData.NivelMinimo;
            if (FormData.NivelMaximo == 0 || FormData.NivelMinimo == 0)
            {

            }
            else
            {
                foreach (var tramo in Tramos)
                {
                    tramo.Niveles = new ObservableCollection<ServicioMuebleTramoNivel>();
                   int  indexer = 0;
                    for (int o = FormData.NivelMinimo; o <= FormData.NivelMaximo; o++)
                    {
                        
                            tramo.Niveles.Add(new ServicioMuebleTramoNivel { NombreNivel = "Nivel " + o,NumeroNivel=indexer
                    });
                        indexer++;
                       
                    }
                }
            }

       }


        public async Task GenerarNivelesmodoeditando()
        {
            if (MuebleData.MuebleNivelMinimo > MuebleData.MuebleNivelMaximo)
                return;
            int numeroniveles = (MuebleData.MuebleNivelMaximo??0) - (MuebleData.MuebleNivelMinimo??0);
            if (MuebleData.MuebleNivelMaximo == 0 || MuebleData.MuebleNivelMinimo == 0 || MuebleData.MuebleNivelMaximo == null
                || MuebleData.MuebleNivelMinimo == null)
            {

            }
            else
            {
                foreach (var tramo in Tramos)
                {
                    List<ServicioMuebleTramoNivel> nivs;
                    if (NIvelesDB != null)
                    {
                        nivs = NIvelesDB.Where(x => (x.ServicioMuebleTramoID == tramo.ServicioMuebleTramoID && tramo.ServicioMuebleTramoID!=null && tramo.ServicioMuebleTramoID!=0 )|| (x.ServicioMuebleTramoLocalID == tramo.ServicioMuebleTramoLocalID && tramo.ServicioMuebleTramoLocalID != 0)).ToList();
                    }
                    else {
                        nivs = new List<ServicioMuebleTramoNivel>();
                    }
                    int cantidaddeniveles = ((MuebleData.MuebleNivelMaximo??0) -( MuebleData.MuebleNivelMinimo??0)) + 1;
                    if (nivs.Count() < cantidaddeniveles)
                    {
                        int index = 0;
                        for (int o = (MuebleData.MuebleNivelMinimo??0); o <= MuebleData.MuebleNivelMaximo; o++)
                        {
                            if (nivs.Count < index + 1)
                            {
                                nivs.Add(new ServicioMuebleTramoNivel { NombreNivel = "Nivel " + o ,NumeroNivel=index, Editable = modoforma });
                            }
                            else
                            {
                                try
                                {
                                    var tempooralnivel = tramo.Niveles.ElementAt(index);
                                    tempooralnivel.NombreNivel = "Nivel " + o;
                                    tempooralnivel.NumeroNivel = index;
                                    tempooralnivel.Editable = modoforma;
                                    nivs.Add(tempooralnivel);
                                }
                                catch(Exception ex) {
                                    Debug.WriteLine(ex.Message);
                                }
                            }
                            index++;
                        }
                    }
                    if (nivs.Count() >= cantidaddeniveles)
                    {
                        int diferencia = nivs.Count() - cantidaddeniveles;
                         
                        int index = nivs.Count() - 1;
                        nivs.RemoveRange(0, diferencia);
                    
                        index = 0;
                        for (int o = (MuebleData.MuebleNivelMinimo??0); o <= (MuebleData.MuebleNivelMaximo??0); o++)
                        {
                            nivs[index].NombreNivel = "Nivel " + o;
                            nivs[index].NumeroNivel = index;
                            nivs[index].Editable = modoforma;
                            index++;
                        }
                    }
                    tramo.Niveles = new ObservableCollection<ServicioMuebleTramoNivel>(nivs);

                }
            }

        }

        public async Task generartramosdesdetramoinicial(string para) {

            try
            {
                int numerodetramosinicial = int.Parse(para);
                FormData.Tramoinicial = para;
                if(FormData.Tramos.HasValue)
                    tramogeneration(numerodetramosinicial,int.Parse(FormData.Tramos.ToString()));
                await GenerarNiveles();
            }

            catch { }
        }

        public async Task GenerarTramosporforma(string para)
        {
            try
            {
                FormData.Tramos = int.Parse(para);
                int numerodetramos = int.Parse(para);
                tramogeneration(int.Parse(FormData.Tramoinicial),numerodetramos);
                await GenerarNiveles();

            }

            catch { }

        }

        public void tramogeneration(int inicio,int final) {

           var Tramostemp = Tramos.Where(x=>x.TramoNumero>=inicio && x.TramoNumero<=(inicio+(final-1))).ToList();
            for (int i = inicio; i <= (inicio + (final - 1)); i++)
            {
                if (!Tramostemp.Any(x=>x.TramoNumero==i))
                    Tramostemp.Add(new ServicioMuebleTramo{TramoNumero = i,TramoName = "Tramo " + i});
            }
            Tramos = new ObservableCollection<ServicioMuebleTramo>(Tramostemp.OrderBy(x=>x.TramoNumero)) ;

        }

        public async Task<bool> Verificardatacompleta()
        {
            if (modoforma == false)
            {
                if (String.IsNullOrEmpty(MuebleData.MuebleCara))
                {
                    await mensajetoast("Ingrese la cara");

                    MuebleData.CaraError = true; return false;
                }
                if (String.IsNullOrEmpty(MuebleData.MueblePasillo))
                {
                    await mensajetoast("Ingrese el pasillo");

                    MuebleData.PasilloError = true; return false;
                }
                if (String.IsNullOrEmpty(MuebleData.MuebleTramoInicial))
                {
                    await mensajetoast("Ingrese el tramo inicial");

                    MuebleData.PasilloError = true; return false;
                }
                if (String.IsNullOrEmpty(MuebleData.MuebleTramoInicial))
                {
                    await mensajetoast("Ingrese el tramo inicial");

                    MuebleData.PasilloError = true;
                    return false;
                }
                if (MuebleData.MuebleAltura == 0 || MuebleData.MuebleAltura==null)
                {
                    await mensajetoast("Ingrese la Altura");
                    return false;
                }
                if (MuebleData.MuebleTramos == 0|| MuebleData.MuebleTramos == null)
                {
                    await mensajetoast("Ingrese los tramos");

                    // MuebleData.PasilloError = true;
                    return false;
                }
                if (MuebleData.MuebleNivelMinimo == 0 || MuebleData.MuebleNivelMinimo == null)
                {
                    await mensajetoast("Ingrese el nivel minimo");

                    // MuebleData.PasilloError = true;
                    return false;
                }
                if (MuebleData.MuebleNivelMaximo == 0 || MuebleData.MuebleNivelMaximo == null)
                {
                    await mensajetoast("Ingrese el nivel maximo");

                    // MuebleData.PasilloError = true
                    return false;
                }
                if (MedidaIgualSeleccionada == null)
                {
                    await mensajetoast("Eliga una opcion MEDIDAS IGUALES");

                    return false;
                }
                if (TipoSeleccionado == null)
                {
                    await mensajetoast("Eliga un tipo de Mueble");

                    return false;
                }
                
                if (!await mueblerepo.verificarminimodefotos(MuebleData))
                {
                    return false;
                }

                    return true;
            }
            else {
                return true;
            }
        }

        public async Task<bool> AlmacenarDatos() {
            if (modoforma == false)
            {
                MuebleData.MuebleTipoID = TipoSeleccionado.MuebleTipoID;
                MuebleData.MuebleMedidasIguales = MedidaIgualSeleccionada.Activo;
              
                    await mueblerepo.insertarregistro(MuebleData);
                    foreach (var t in Tramos)
                    {
                        t.ServicioID = MuebleData.ServicioID;
                        t.ServicioMuebleLocalID = MuebleData.ServicioMuebleLocalID;
                        t.Sincronizado = false;
                        await tramorepo.insertarregistro(t);
                        foreach (var niv in t.Niveles)
                        {
                            niv.ServicioID = MuebleData.ServicioID;
                            niv.ServicioMuebleID = MuebleData.ServicioMuebleID;
                            niv.ServicioMuebleLocalID = MuebleData.ServicioMuebleLocalID ?? 0;
                            niv.ServicioMuebleTramoLocalID = t.ServicioMuebleTramoLocalID;
                            await nivelesrepo.insertarregistro(niv);
                            foreach (var c in Categorias)
                            {
                                if (c.Activo)
                                {
                                    var nuevacategorianivel = new ServicioMuebleTramoNivelCategoria
                                    {
                                        CategoriaID = c.CategoriaID,
                                        CategoriaLocalID = c.CategoriaLocalID,
                                        ServicioMuebleTramoID = t.ServicioMuebleTramoID,
                                        ServicioMuebleTramoLocalID = t.ServicioMuebleTramoLocalID,
                                        ServicioMuebleTramoNivelID = niv.ServicioMuebleTramoNivelID,
                                        ServicioMuebleTramoNivelLocalID = niv.ServicioMuebleTramoNivelLocalID,
                                    };
                                    await categoriasnivelesrepo.insertarregistro(nuevacategorianivel);
                                }
                            }
                        }
                    }
                    await mueblerepo.guardarimagenesdelmuebles(MuebleData);
                    MessagingCenter.Send(this, "Hi", "actualizar");
                return true;
              
            }
            else {
                return true;
            }
        }
    
    }
}
