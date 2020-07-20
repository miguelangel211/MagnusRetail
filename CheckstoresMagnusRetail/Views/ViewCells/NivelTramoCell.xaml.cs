using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckstoresMagnusRetail.DataModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CheckstoresMagnusRetail.Views.ViewCells
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NivelTramoCell : ViewCell
    {
        public NivelTramoCell()
        {
            InitializeComponent();
        }

        public async void DatocambiadoProfundo(object sender, EventArgs args) {
            MessagingCenter.Send<NivelTramoCell, NIvelMedidasvalues>(this,"mensaje",new NIvelMedidasvalues { Valor = this.profucdocell.Text,Medida= NivelPropiedad.Profundo,Nivel=int.Parse(this.nivelnumero.Text) });

        }

        public async void DatocambiadoAlto(object sender, EventArgs args)
        {
            MessagingCenter.Send<NivelTramoCell, NIvelMedidasvalues>(this, "mensaje", new NIvelMedidasvalues { Valor = this.Altocell.Text, Medida = NivelPropiedad.Alto, Nivel = int.Parse(this.nivelnumero.Text) });

        }

        public async void DatocambiadoAncho(object sender, EventArgs args)
        {
            MessagingCenter.Send<NivelTramoCell, NIvelMedidasvalues>(this, "mensaje", new NIvelMedidasvalues { Valor = this.Anchocell.Text, Medida = NivelPropiedad.Ancho, Nivel = int.Parse(this.nivelnumero.Text) });

        }
    }
}
