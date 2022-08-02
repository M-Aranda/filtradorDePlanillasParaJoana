using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiltradorDePlanillas
{
    internal class Registro
    {


        private String uen;
        private String cd;
        private String centroDeDistribucion;
        private String fletero;
        private String nombre;
        private String camion;
        private String saldoAnterior;
        private String planilla;
        private String valoresAEntregar;
        private String valoresEEntregados;
        private String saldoCredito;
        private String saldoDebito;
        private String diferencia;
        private String fechaPlanilla;
        private String fechaCierre;
        private String observaciones;
        private String referencia;

        public Registro()
        {
        }

        public Registro(string uen, string cd, string centroDeDistribucion, string fletero, string nombre, string camion, string saldoAnterior, string planilla, string valoresAEntregar, string valoresEEntregados, string saldoCredito, string saldoDebito, string diferencia, string fechaPlanilla, string fechaCierre, string observaciones, string referencia)
        {
            this.Uen = uen;
            this.Cd = cd;
            this.CentroDeDistribucion = centroDeDistribucion;
            this.Fletero = fletero;
            this.Nombre = nombre;
            this.Camion = camion;
            this.SaldoAnterior = saldoAnterior;
            this.Planilla = planilla;
            this.ValoresAEntregar = valoresAEntregar;
            this.ValoresEEntregados = valoresEEntregados;
            this.SaldoCredito = saldoCredito;
            this.SaldoDebito = saldoDebito;
            this.Diferencia = diferencia;
            this.FechaPlanilla = fechaPlanilla;
            this.FechaCierre = fechaCierre;
            this.Observaciones = observaciones;
            this.Referencia = referencia;
        }

        public string Uen { get => uen; set => uen = value; }
        public string Cd { get => cd; set => cd = value; }
        public string CentroDeDistribucion { get => centroDeDistribucion; set => centroDeDistribucion = value; }
        public string Fletero { get => fletero; set => fletero = value; }
        public string Nombre { get => nombre; set => nombre = value; }
        public string Camion { get => camion; set => camion = value; }
        public string SaldoAnterior { get => saldoAnterior; set => saldoAnterior = value; }
        public string Planilla { get => planilla; set => planilla = value; }
        public string ValoresAEntregar { get => valoresAEntregar; set => valoresAEntregar = value; }
        public string ValoresEEntregados { get => valoresEEntregados; set => valoresEEntregados = value; }
        public string SaldoCredito { get => saldoCredito; set => saldoCredito = value; }
        public string SaldoDebito { get => saldoDebito; set => saldoDebito = value; }
        public string Diferencia { get => diferencia; set => diferencia = value; }
        public string FechaPlanilla { get => fechaPlanilla; set => fechaPlanilla = value; }
        public string FechaCierre { get => fechaCierre; set => fechaCierre = value; }
        public string Observaciones { get => observaciones; set => observaciones = value; }
        public string Referencia { get => referencia; set => referencia = value; }
    }
}
