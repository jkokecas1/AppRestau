using Newtonsoft.Json.Linq;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppResta.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Pago : ContentPage
    {
        List<Model.Ordenes> orden;
        Model.Pagos pago = new Model.Pagos();
        int id = 0;
        Model.Ordenes Ordenes;
        ListView ordenesListView;

        public Pago(List<Model.Ordenes> orden, int id, ListView ordenesListView)
        {
            this.id = id;
            this.orden = orden;
            this.ordenesListView = ordenesListView;
            InitializeComponent();

            BindingContext = new ViewModel.PagoViewModel(Navigation);

            Device.StartTimer(TimeSpan.FromSeconds(1.2), () => {
                cargar.IsEnabled = false;
                cargar.IsRunning = false;
                cargar.IsVisible = false;
                grild.IsVisible = true;
                return false;
            });
            init();
        }

        public void init()
        {
            Efectivo.Text = "$ 0";
            Tarjeta.Text = "$ 0";
            TarjetaGrild.IsVisible = false;
            tiketOrden.Text = "TICKET # :" + id;
            double[] totoales = new double[3];

            foreach (Model.Ordenes orden in orden)
            {
                if (orden.id == id)
                {
                    
                    totoales = obtenerPagoFinal(orden.id);
                    orden.totoalExtras = totoales[1].ToString();
                    orden.total = totoales[0].ToString();
                    Ordenes = new Model.Ordenes();
                    Ordenes.id = id;
                    Ordenes.mesero = orden.mesero;
                    Ordenes.total = orden.total;
                    Ordenes.mesa = orden.mesa;
                    Ordenes.pago = orden.pago;
                    Ordenes.totoalExtras = orden.totoalExtras;
                    Ordenes.fecha_cerada = orden.fecha_cerada;
                    Ordenes.fecha_orden = orden.fecha_orden;

                    // total.Text = orden.total;
                    // extras.Text =orden.totoalExtras;
                    if (orden.totoalExtras != null)
                        Subtotal.Text = (Double.Parse(Ordenes.total) + Double.Parse(Ordenes.totoalExtras)) + "";//( Int32.Parse(orden.total) + Int32.Parse(propina.Text) + Int32.Parse(Tarjeta.Text) + Int32.Parse(Efectivo.Text)) + "";   
                    else
                        Subtotal.Text = Ordenes.total;
                }
            }


        }
        public static double[] obtenerPagoFinal(int id)
        {
            double[] total = new double[2];

            var client1 = new HttpClient();


            client1.BaseAddress = new Uri(("http://192.168.1.112/resta/admin/mysql/orden/index.php?op=ObtenerPrecioItems&idCart=" + id));

            HttpResponseMessage response1 = client1.GetAsync(client1.BaseAddress).Result;
            if (response1.IsSuccessStatusCode)
            {
                var content1 = response1.Content.ReadAsStringAsync().Result;
                string json1 = content1.ToString();

                var jsonArray1 = JArray.Parse(json1.ToString());

                //userInfo = JsonConvert.DeserializeObject<List<Model.Categorias>>(content);
                foreach (var item in jsonArray1)
                {
                    total[0] += Double.Parse(item["precio"].ToString().Replace(",", "."));
                    total[1] += Double.Parse(ExtrasItem(item["item"].ToString()) + "");
                }


            }
            return total;
        }
        public static double ExtrasItem(string id)
        {
            double totoal = 0.0;
            var client = new HttpClient();

            client.BaseAddress = new Uri("http://192.168.1.112/resta/admin/mysql/platillo/index.php?op=obtenerExtrasAsItem&iditem=" + id);

            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                string json = content.ToString();
                if (json.Equals("[]"))
                {
                    totoal = 0.0;

                }
                else
                {
                    var jsonArray = JArray.Parse(json.ToString());

                    foreach (var item in jsonArray)
                    {
                        totoal += Double.Parse(item["precio"].ToString().Replace(",", "."));
                    }

                }

            }
            return totoal;
        }
        
        double propina  = 0.0;
        public void calcularPropona(int p)
        {      
               // Console.WriteLine();
                propina= ((Double.Parse(Ordenes.total) * Double.Parse(p + "")) / 100) ;
            // total.Text = Ordenes.total;
            if (Ordenes.totoalExtras != null)
                Subtotal.Text = propina + Double.Parse(Ordenes.total) + Double.Parse(Ordenes.totoalExtras) + "";
            else if (propina > 0.0)
                Subtotal.Text = propina + Ordenes.total + "";
            else
                Subtotal.Text = Ordenes.total;

            if (radioTarjeta.IsChecked)
                    Tarjeta.Text ="$"+ Subtotal.Text;

            
            //Console.WriteLine( Ordenes.total);
        }


        void OnClicked_Pagar(object sender, EventArgs e)
        {


            if (Efectivo.Text == "" && Tarjeta.Text.Replace("$ ","") == "")
            {
                PopupNavigation.Instance.PushAsync(new PopError("VERIFICA LOS CAMPOS"));
            }
            else if (Tarjeta.Text.Replace("$ ", "") != "" && Efectivo.Text == "0")
            {
                pago.monto = Double.Parse(Tarjeta.Text.Replace("$ ", "") + "");
                pago.idcart = Ordenes.id;
                pago.tipoPago = "2"; // TARJETA
                pago.propina = propina+"";
                pago.total = Subtotal.Text;

                if (pago.monto < Double.Parse(Subtotal.Text))
                {
                    PopupNavigation.Instance.PushAsync(new PopError("VERIFICA LOS CAMPOS"));
                }
                else
                {
                    Console.WriteLine(pago.tipoPago + "Tarjeta");
                    PopupNavigation.Instance.PushAsync(new ConfirmarPago(pago, this, ordenesListView));
                }
            }
            else if (Efectivo.Text != "" && Tarjeta.Text.Replace("$ ", "") == "0")
            {
                pago.monto = Double.Parse(Efectivo.Text + "");
                pago.idcart = Ordenes.id;
                pago.tipoPago = "1"; // EFECTIVO
                pago.propina = propina+"";
                pago.total =  Subtotal.Text;
                if (pago.monto < Double.Parse(Subtotal.Text))
                {
                    PopupNavigation.Instance.PushAsync(new PopError("VERIFICA LOS CAMPOS"));
                }
                else
                {
                    Console.WriteLine(pago.tipoPago + "EFECTIVO");
                    PopupNavigation.Instance.PushAsync(new ConfirmarPago(pago, this, ordenesListView));
                }
            }
            else if (Efectivo.Text != "" && Tarjeta.Text.Replace("$ ", "") != "")
            {
                int monto = Int32.Parse(Efectivo.Text + "") + Int32.Parse(Tarjeta.Text.Replace("$ ", "") + "");
                //Console.WriteLine(monto);
                pago.monto = Double.Parse(monto.ToString());
                pago.idcart = Ordenes.id;
                pago.tipoPago = "3"; // EFECTIVO Y TARJETA
                pago.propina = propina+"";
                pago.total = Subtotal.Text;
                if (pago.monto < Double.Parse(Subtotal.Text))
                {
                    PopupNavigation.Instance.PushAsync(new PopError("VERIFICA LOS CAMPOS"));

                }
                else
                {
                   // Console.WriteLine(pago.tipoPago + "AMBOS");
                    PopupNavigation.Instance.PushAsync(new ConfirmarPago(pago, this, ordenesListView));
                }
            }

        }
        bool pagos = false;
        void OnClicked_Num1(object sender, EventArgs e)
        {
            verificar0();
            _ = pagos == false ? Efectivo.Text += "1" : Tarjeta.Text += "1";
        }
        void OnClicked_Num2(object sender, EventArgs e) { verificar0(); _ = pagos == false ? Efectivo.Text += "2" : Tarjeta.Text += "2"; }
        void OnClicked_Num3(object sender, EventArgs e) { verificar0(); _ = pagos == false ? Efectivo.Text += "3" : Tarjeta.Text += "3"; }
        void OnClicked_Num4(object sender, EventArgs e) { verificar0(); _ = pagos == false ? Efectivo.Text += "4" : Tarjeta.Text += "4"; }
        void OnClicked_Num5(object sender, EventArgs e) { verificar0(); _ = pagos == false ? Efectivo.Text += "5" : Tarjeta.Text += "5"; }
        void OnClicked_Num6(object sender, EventArgs e) { verificar0(); _ = pagos == false ? Efectivo.Text += "6" : Tarjeta.Text += "6"; }
        void OnClicked_Num7(object sender, EventArgs e) { verificar0(); _ = pagos == false ? Efectivo.Text += "7" : Tarjeta.Text += "7"; }
        void OnClicked_Num8(object sender, EventArgs e) { verificar0(); _ = pagos == false ? Efectivo.Text += "8" : Tarjeta.Text += "8"; }
        void OnClicked_Num9(object sender, EventArgs e) { verificar0(); _ = pagos == false ? Efectivo.Text += "9" : Tarjeta.Text += "9"; }
        void OnClicked_Num0(object sender, EventArgs e) { verificar0(); _ = pagos == false ? Efectivo.Text += "0" : Tarjeta.Text += "0"; }

        void verificar0()
        {
            if (pagos == false)
            { _ = Efectivo.Text == "0" ? Efectivo.Text = "" : Efectivo.Text; }
            else
            {
                _ = Tarjeta.Text == "0" ? Tarjeta.Text = "" : Tarjeta.Text;
            }


        }

        void OnClicked_NumBorrar(object sender, EventArgs e)
        {
            if (pagos == false)
            {
                if (Efectivo.Text.Length < 0 || Efectivo.Text != "")
                {
                    Efectivo.Text = Efectivo.Text.Remove(Efectivo.Text.Length - 1, 1);
                }
            }
            else
            {
                if (Tarjeta.Text.Length < 0 || Tarjeta.Text != "")
                {
                    Tarjeta.Text = Tarjeta.Text.Remove(Tarjeta.Text.Length - 1, 1);
                }
            }


        }
        void OnClicked_NumNext(object sender, EventArgs e)
        {
            if (pagos == false)
            {

                pagos = true;
                Tarjeta.TextColor = Color.Black;
                Efectivo.Opacity = 0.3;
                //Tarjeta.Opacity = 1;

            }
            else
            {

                pagos = false;
                Efectivo.TextColor = Color.Black;
                Efectivo.Opacity = 1;
                // Tarjeta.Opacity = 0.3;

            }

        }


        private void radioEfectivo_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            pagos = false;
            Tarjeta.Text = "0";
            btnSiguiente.IsEnabled = false;
            TarjetaGrild.IsVisible = false;
            EfectivoGrild.IsVisible = true;
        }

        private void radioTarjeta_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            pagos = true;
            Efectivo.Text = "0";
            btnSiguiente.IsEnabled = false;
            Tarjeta.Text = Subtotal.Text;
            TarjetaGrild.IsVisible = true;
            EfectivoGrild.IsVisible = false;
        }

        private void radioEfecTarj_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            pagos = false;
            Tarjeta.Text = "0";
            Efectivo.Text = "0";
            btnSiguiente.IsEnabled = true;
            EfectivoGrild.IsVisible = true;
            TarjetaGrild.IsVisible = true;
        }



        private void propina0_Clicked(object sender, EventArgs e)
        {
            propina0.BorderColor = Color.Green;
            propina5.BorderColor = Color.Black;
            propina10.BorderColor = Color.Black;
            propina15.BorderColor = Color.Black;
            propina20.BorderColor = Color.Black;
            propinaOtro.BorderColor = Color.Black;

            propina0.BorderWidth = 3;
            propina5.BorderWidth = 1;
            propina10.BorderWidth = 1;
            propina15.BorderWidth = 1;
            propina20.BorderWidth = 1;
            propinaOtro.BorderWidth = 1;

            calcularPropona(0);

        }

        private void propina5_Clicked(object sender, EventArgs e)
        {
            propina0.BorderColor = Color.Black;
            propina5.BorderColor = Color.Green;
            propina10.BorderColor = Color.Black;
            propina15.BorderColor = Color.Black;
            propina20.BorderColor = Color.Black;
            propinaOtro.BorderColor = Color.Black;

            propina0.BorderWidth = 1;
            propina5.BorderWidth = 3;
            propina10.BorderWidth = 1;
            propina15.BorderWidth = 1;
            propina20.BorderWidth = 1;
            propinaOtro.BorderWidth = 1;
            calcularPropona(5);
        }

        private void propina10_Clicked(object sender, EventArgs e)
        {
            propina0.BorderColor = Color.Black;
            propina5.BorderColor = Color.Black;
            propina10.BorderColor = Color.Green;
            propina15.BorderColor = Color.Black;
            propina20.BorderColor = Color.Black;
            propinaOtro.BorderColor = Color.Black;

            propina0.BorderWidth = 1;
            propina5.BorderWidth = 1;
            propina10.BorderWidth = 3;
            propina15.BorderWidth = 1;
            propina20.BorderWidth = 1;
            propinaOtro.BorderWidth = 1;
            calcularPropona(10);
        }

        private void propina15_Clicked(object sender, EventArgs e)
        {
            propina0.BorderColor = Color.Black;
            propina5.BorderColor = Color.Black;
            propina10.BorderColor = Color.Black;
            propina15.BorderColor = Color.Green;
            propina20.BorderColor = Color.Black;
            propinaOtro.BorderColor = Color.Black;

            propina0.BorderWidth = 1;
            propina5.BorderWidth = 1;
            propina10.BorderWidth = 1;
            propina15.BorderWidth = 3;
            propina20.BorderWidth = 1;
            propinaOtro.BorderWidth = 1;
            calcularPropona(15);
        }

        private void propina20_Clicked(object sender, EventArgs e)
        {
            propina0.BorderColor = Color.Black;
            propina5.BorderColor = Color.Black;
            propina10.BorderColor = Color.Black;
            propina15.BorderColor = Color.Black;
            propina20.BorderColor = Color.Green;
            propinaOtro.BorderColor = Color.Black;

            propina0.BorderWidth = 1;
            propina5.BorderWidth = 1;
            propina10.BorderWidth = 1;
            propina15.BorderWidth = 1;
            propina20.BorderWidth = 3;
            propinaOtro.BorderWidth = 1;

            calcularPropona(20);
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PushAsync(new Ticket(orden,id));
        }

        private void propinaOtro_Clicked(object sender, EventArgs e)
        {
            propina0.BorderColor = Color.Black;
            propina5.BorderColor = Color.Black;
            propina10.BorderColor = Color.Black;
            propina15.BorderColor = Color.Black;
            propina20.BorderColor = Color.Black;
            propinaOtro.BorderColor = Color.Green;

            propina0.BorderWidth = 1;
            propina5.BorderWidth = 1;
            propina10.BorderWidth = 1;
            propina15.BorderWidth = 1;
            propina20.BorderWidth = 1;
            propinaOtro.BorderWidth = 3;
        }
    }
}