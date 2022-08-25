using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Storage;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace FiltradorDePlanillas
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        private void button1_Click(object sender, EventArgs e)
        {

            string sFileName = "";

            OpenFileDialog choofdlog = new OpenFileDialog();
            choofdlog.Filter = "Archivos XLSX (*.xlsx)|*.xlsx";
            choofdlog.FilterIndex = 1;
            choofdlog.Multiselect = true;

            string[] arrAllFiles = new string[] { };

            while (true)
            {


                if (choofdlog.ShowDialog() == DialogResult.OK)
                {
                    sFileName = choofdlog.FileName;
                    arrAllFiles = choofdlog.FileNames; //used when Multiselect = true
                    break;
                }
                else
                {
                    MessageBox.Show("Debe seleccionar un Excel");
                }

            }

            List<Registro> registros = leerExcelDePlanillasDeLaCCU(sFileName);


            if (registros.Count>0)
            {
                string localfolder = ApplicationData.Current.LocalFolder.Path;
                var array = localfolder.Split('\\');
                var username = array[2];
                string downloads = @"C:\Users\" + username + @"\Downloads";


                var archivo = new FileInfo(downloads + @"\RegistrosFiltrados.xlsx");

                SaveExcelFileRegistros(registros, archivo);

                MessageBox.Show("Archivo Excel filtrado, creado en carpeta de descargas!");
            }
            else
            {
                MessageBox.Show("Archivo Excel cargado no contiene duplicados (no se crea ningún archivo)");
            }


          
          
            

        }

        Boolean confirmarPresenciaDePlanillaEnBDDeIntranet(String numeroDePlanilla, String mes, String anio)
        {
            List<String> listadoDePlanillas = new List<String>();
            string URI = "https://intranet.cargopacifico.cl/planillas_WS.php";


            if (mes == "01")
            {
                mes = "12";
                anio = (int.Parse(anio)-1).ToString();
            }
            else
            {
                mes = (int.Parse(mes)-1).ToString();
            }

         

            string myParameters = "planilla=" + numeroDePlanilla+"&mes="+mes+"&anio="+anio;

            

            Boolean planillaPresenteEnIntranet = false;

            using (WebClient wc = new System.Net.WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                string HtmlResult = wc.UploadString(URI, myParameters);

               // Console.WriteLine(HtmlResult);


                String[] datosPartidos = HtmlResult.Split(' ');

                List<String> listado = new List<String>();

                foreach (var item in datosPartidos)
                {
                    listado.Add(item);
                }

                foreach (var item in datosPartidos)
                {
                    Console.WriteLine(item);
                }

                      

                
   

            }


            return planillaPresenteEnIntranet;
        }

        string ReadResponseFromUrl(string url)
        {
            var httpClientHandler = new HttpClientHandler();
            var httpClient = new HttpClient(httpClientHandler)
            {
                BaseAddress = new Uri(url)
            };
            using (var response = httpClient.GetAsync(url))
            {
                string responseBody = response.Result.Content.ReadAsStringAsync().Result;
                return responseBody;
            }
        }




        private List<Registro> leerExcelDePlanillasDeLaCCU( String filePath){
            List<Registro> registros = new List<Registro>();

            List<String> planillas = new List<String>();

            Boolean hayPlanillasDuplicadas = false;


            FileInfo existingFile = new FileInfo(filePath);
            using (ExcelPackage package = new ExcelPackage(existingFile))
            {

                //get the first worksheet in the workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int colCount = worksheet.Dimension.End.Column;  //get Column Count
                int rowCount = worksheet.Dimension.End.Row;     //get row count

                for (int row = 1; row <= rowCount; row++)
                {

                    Registro r = new Registro();
                    r.Uen = worksheet.Cells[row, 1].Value?.ToString().Trim();
                    r.Cd = worksheet.Cells[row, 2].Value?.ToString().Trim();
                    r.CentroDeDistribucion = worksheet.Cells[row, 3].Value?.ToString().Trim();
                    r.Fletero = worksheet.Cells[row, 4].Value?.ToString().Trim();
                    r.Nombre = worksheet.Cells[row, 5].Value?.ToString().Trim();
                    r.Camion = worksheet.Cells[row, 6].Value?.ToString().Trim();
                    r.SaldoAnterior = worksheet.Cells[row, 7].Value?.ToString().Trim();
                    r.Planilla = worksheet.Cells[row, 8].Value?.ToString().Trim();
                    r.ValoresAEntregar = worksheet.Cells[row, 9].Value?.ToString().Trim();
                    r.ValoresEEntregados = worksheet.Cells[row, 10].Value?.ToString().Trim();
                    r.SaldoCredito = worksheet.Cells[row, 11].Value?.ToString().Trim();
                    r.SaldoDebito = worksheet.Cells[row, 12].Value?.ToString().Trim();
                    r.Diferencia = worksheet.Cells[row, 13].Value?.ToString().Trim();
                    r.FechaPlanilla = formatearFecha(worksheet.Cells[row, 14].Value?.ToString().Trim());
                    r.FechaCierre = formatearFecha(worksheet.Cells[row, 15].Value?.ToString().Trim());
                    r.Observaciones = worksheet.Cells[row, 16].Value?.ToString().Trim();
                    r.Referencia = worksheet.Cells[row, 17].Value?.ToString().Trim();

                    registros.Add(r);

                }


                foreach (var r in registros)
                {
                    planillas.Add(r.Planilla);
                }

                //identificar las planillas de la lista, contar cada una solo una vez
                planillas = planillas.Distinct().ToList();

                //diferenciar planillas que aparecen más de una vez, de las que aparecen una
                List<String> planillasUnicas = new List<String>();
                List<String> planillasRepetidas = new List<String>();

                foreach (var p in planillas)
                {

                    int instanciasDeUnaPlanilla = 0;
                    foreach (var r in registros)
                    {
                        if (p == r.Planilla)
                        {
                            instanciasDeUnaPlanilla++;
                        }
                    }

                    if (instanciasDeUnaPlanilla > 1 || p == "Promae sin Dato" || p == "Promae sin Datos")
                    {
                        planillasRepetidas.Add(p);
                    }
                    else
                    {
                        planillasUnicas.Add(p);
                    }

                }

                if (planillasRepetidas.Count > 0)
                {
                    Console.WriteLine("Hay planillas duplicadas");
                    hayPlanillasDuplicadas = true;

                }
                else
                {
                    Console.WriteLine("No hay planillas duplicadas");
                    hayPlanillasDuplicadas = false;
                }




                    List<Registro> registrosAModificar = new List<Registro>();
                    List<Registro> registrosAMantener = new List<Registro>();


                    foreach (var r in registros)
                    {

                        foreach (var pr in planillasRepetidas)
                        {
                            if (pr == r.Planilla)
                            {
                                registrosAModificar.Add(r);
                            }
                        }
                    }



                    foreach (var r in registros)
                    {

                        foreach (var pr in planillasUnicas)
                        {
                            if (pr == r.Planilla)
                            {
                                registrosAMantener.Add(r);
                            }
                        }
                    }


                    //ahora que ambas listas están separadas, tomar la lista de registros a modificar y quitar cargas de Reparto
                    List<Registro> registrosConCargaDeRepartoEnLaObservacion = new List<Registro>();
                    List<Registro> registrosSinCargaDeRepartoEnLaObservacion = new List<Registro>();



                    foreach (var r in registrosAModificar)
                    {
                        String observacion = r.Observaciones;

                        if (observacion != null)
                        {

                            string[] words = observacion.Split(':');
                            //todo registro con una observacion que empiece con "Carga de Reparto", NO debe modificarse
                            if (words[0] != "Carga de Reparto")
                            {
                                registrosSinCargaDeRepartoEnLaObservacion.Add(r);
                            }
                            else
                            {
                                registrosConCargaDeRepartoEnLaObservacion.Add(r);
                            }

                        }

                    }

                    //modificar número de planilla de registros SIN carga de reparto



                    registros = new List<Registro>();

                    //modificar numero de planilla y agregar a listado (planillas duplicadas, sin carga de reparto)


                    string mesActual = DateTime.Now.Month.ToString();
                    string anioActual = DateTime.Now.Year.ToString();

                    switch (mesActual)
                    {
                        case "1":
                            mesActual = "01";
                            break;
                        case "2":
                            mesActual = "02";
                            break;
                        case "3":
                            mesActual = "03";
                            break;
                        case "4":
                            mesActual = "04";
                            break;
                        case "5":
                            mesActual = "05";
                            break;
                        case "6":
                            mesActual = "06";
                            break;
                        case "7":
                            mesActual = "07";
                            break;
                        case "8":
                            mesActual = "08";
                            break;
                        case "9":
                            mesActual = "09";
                            break;
                        default:
                            break;
                    }



                    int numeroDeRegistro = 1;

                    foreach (var item in registrosSinCargaDeRepartoEnLaObservacion)
                    {
                        String numeroDeRegistroComoString = numeroDeRegistro.ToString();

                        switch (numeroDeRegistroComoString.Length)
                        {
                            case 1:
                                numeroDeRegistroComoString = "000" + numeroDeRegistroComoString;
                                break;
                            case 2:
                                numeroDeRegistroComoString = "00" + numeroDeRegistroComoString;
                                break;
                            case 3:
                                numeroDeRegistroComoString = "0" + numeroDeRegistroComoString;
                                break;
                            case 4:
                                //no hacer nada
                                break;
                            default:
                                break;

                        }

                        item.Planilla = anioActual + mesActual + numeroDeRegistroComoString;
                        registros.Add(item);
                        numeroDeRegistro++;
                    }

                    //planillas duplicadas que tengan carga de reparto en la observacion
                    foreach (var item in registrosConCargaDeRepartoEnLaObservacion)
                    {
                        registros.Add(item);
                    }

                    //planillas unicas que no deben ser modificadas
                    foreach (var item in registrosAMantener)
                    {
                        if (item.Planilla != "Planilla")
                        {
                            registros.Add(item);
                        }

                    }



            }

            if (!hayPlanillasDuplicadas)
            {
                registros = new List<Registro>(); 
            }


            confirmarPresenciaDePlanillaEnBDDeIntranet("3333", "08","2022");
            
            //foreach (var item in registros)
            //{

            //    try
            //    {
            //        String[] datosDeFecha = item.FechaPlanilla.Split('/');
                    

            //        String dia = datosDeFecha[1];
            //        String mes = datosDeFecha[0];
            //        String anio = datosDeFecha[2];


            //        if (confirmarPresenciaDePlanillaEnBDDeIntranet(item.Planilla, mes, anio))
            //        {
            //            //se ingresó en un mes anterior
            //            item.PresenteEnMesAnterior = "Sí";
            //        }
            //        else
            //        {
            //            item.PresenteEnMesAnterior = "No";
            //        }


            //    }
            //    catch (Exception)
            //    {


            //    }

            //}

            return registros;

        }




        private static async Task SaveExcelFileRegistros(List<Registro> registros, FileInfo file)
        {
            var package = new ExcelPackage(file);
            var ws = package.Workbook.Worksheets.Add("Registros filtrados");

            var range = ws.Cells["A1"].LoadFromCollection(registros, true);

            range.AutoFitColumns();

            await package.SaveAsync();
        }

        private String formatearFecha(String fecha)
        {
            String fechaCorrecta = "";
            if (!String.IsNullOrEmpty(fecha))
            {
                try
                {
                    String[] words = fecha.Split(' ');
                    //todo registro con una observacion que empiece con "Carga de Reparto", NO debe modificarse

                    String soloLaFecha = words[0];

                    String[] datosDeFecha = soloLaFecha.Split('/');

                    String dia = datosDeFecha[1];
                    String mes = datosDeFecha[0];
                    String anio = datosDeFecha[2];

                    fechaCorrecta = dia + "/" + mes + "/" + anio;

                }
                catch (Exception)
                {
                    Console.WriteLine(fecha);
                    Console.WriteLine("Sin fecha");

                }
            }


            return fechaCorrecta;

        }




    }
}
