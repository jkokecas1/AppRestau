using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public Pago(List<Model.Ordenes> orden, int id)
        {
            this.id = id;
            this.orden = orden;
            InitializeComponent();

            BindingContext = new ViewModel.PagoViewModel(Navigation);
            init();
        }

        public void init()
        {
            Efectivo.Text = "0";
            Tarjeta.Text = "0";
            foreach (Model.Ordenes orden in orden)
            {
                if (orden.id == id)
                {
                    Ordenes = new Model.Ordenes();
                    Ordenes.id = id;
                    Ordenes.mesero = orden.mesero;
                    Ordenes.total = orden.total;
                    Ordenes.mesa = orden.mesa;
                    Ordenes.pago = orden.pago;
                    Ordenes.fecha_cerada = orden.fecha_cerada;
                    Ordenes.fecha_orden = orden.fecha_orden;

                    total.Text = orden.total;
                    Subtotal.Text =  orden.total;//( Int32.Parse(orden.total) + Int32.Parse(propina.Text) + Int32.Parse(Tarjeta.Text) + Int32.Parse(Efectivo.Text)) + "";   
                }
            }


        }

        public void calcularPropona(int p)
        {
            if (p == 0)
            {
                propina.Text = "0";
            }
            {
                Console.WriteLine();
                propina.Text = ((Double.Parse(Ordenes.total) * Double.Parse(p + "")) / 100) + "";
                total.Text = Ordenes.total;
                Subtotal.Text = Double.Parse(propina.Text.ToString()) + Double.Parse(total.Text.ToString()) + "";
                if (radioTarjeta.IsChecked)
                    Tarjeta.Text = Subtotal.Text;

            }
            //Console.WriteLine( Ordenes.total);

        }


        void OnClicked_Pagar(object sender, EventArgs e)
        {


            if (Efectivo.Text == "" && Tarjeta.Text == "")
            {
                PopupNavigation.Instance.PushAsync(new PopError("VERIFICA LOS CAMPOS"));
            }
            else if (Tarjeta.Text != "" && Efectivo.Text == "0")
            {
                pago.monto = Double.Parse(Tarjeta.Text + "");
                pago.idcart = Ordenes.id;
                pago.tipoPago = "2"; // TARJETA
                pago.propina = propina.Text;
                pago.total = Subtotal.Text;

                if (pago.monto < Double.Parse(Subtotal.Text))
                {
                    PopupNavigation.Instance.PushAsync(new PopError("VERIFICA LOS CAMPOS"));
                }
                else
                {
                    Console.WriteLine(pago.tipoPago + "Tarjeta");
                    PopupNavigation.Instance.PushAsync(new ConfirmarPago(pago, this));
                }
            }
            else if (Efectivo.Text != "" && Tarjeta.Text == "0")
            {
                pago.monto = Double.Parse(Efectivo.Text + "");
                pago.idcart = Ordenes.id;
                pago.tipoPago = "1"; // EFECTIVO
                pago.propina = propina.Text;
                pago.total =  Subtotal.Text;
                if (pago.monto < Double.Parse(Subtotal.Text))
                {
                    PopupNavigation.Instance.PushAsync(new PopError("VERIFICA LOS CAMPOS"));
                }
                else
                {
                    Console.WriteLine(pago.tipoPago + "EFECTIVO");
                    PopupNavigation.Instance.PushAsync(new ConfirmarPago(pago, this));
                }
            }
            else if (Efectivo.Text != "" && Tarjeta.Text != "")
            {
                int monto = Int32.Parse(Efectivo.Text + "") + Int32.Parse(Tarjeta.Text + "");
                //Console.WriteLine(monto);
                pago.monto = Double.Parse(monto.ToString());
                pago.idcart = Ordenes.id;
                pago.tipoPago = "3"; // EFECTIVO Y TARJETA
                pago.propina = propina.Text;
                pago.total = Subtotal.Text;
                if (pago.monto < Double.Parse(Subtotal.Text))
                {
                    PopupNavigation.Instance.PushAsync(new PopError("VERIFICA LOS CAMPOS"));

                }
                else
                {
                   // Console.WriteLine(pago.tipoPago + "AMBOS");
                    PopupNavigation.Instance.PushAsync(new ConfirmarPago(pago, this));
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

            propina0.BorderWidth = 3;
            propina5.BorderWidth = 1;
            propina10.BorderWidth = 1;
            propina15.BorderWidth = 1;
            propina20.BorderWidth = 1;

            calcularPropona(0);

        }

        private void propina5_Clicked(object sender, EventArgs e)
        {
            propina0.BorderColor = Color.Black;
            propina5.BorderColor = Color.Green;
            propina10.BorderColor = Color.Black;
            propina15.BorderColor = Color.Black;
            propina20.BorderColor = Color.Black;

            propina0.BorderWidth = 1;
            propina5.BorderWidth = 3;
            propina10.BorderWidth = 1;
            propina15.BorderWidth = 1;
            propina20.BorderWidth = 1;
            calcularPropona(5);
        }

        private void propina10_Clicked(object sender, EventArgs e)
        {
            propina0.BorderColor = Color.Black;
            propina5.BorderColor = Color.Black;
            propina10.BorderColor = Color.Green;
            propina15.BorderColor = Color.Black;
            propina20.BorderColor = Color.Black;

            propina0.BorderWidth = 1;
            propina5.BorderWidth = 1;
            propina10.BorderWidth = 3;
            propina15.BorderWidth = 1;
            propina20.BorderWidth = 1;
            calcularPropona(10);
        }

        private void propina15_Clicked(object sender, EventArgs e)
        {
            propina0.BorderColor = Color.Black;
            propina5.BorderColor = Color.Black;
            propina10.BorderColor = Color.Black;
            propina15.BorderColor = Color.Green;
            propina20.BorderColor = Color.Black;

            propina0.BorderWidth = 1;
            propina5.BorderWidth = 1;
            propina10.BorderWidth = 1;
            propina15.BorderWidth = 3;
            propina20.BorderWidth = 1;
            calcularPropona(15);
        }

        private void propina20_Clicked(object sender, EventArgs e)
        {
            propina0.BorderColor = Color.Black;
            propina5.BorderColor = Color.Black;
            propina10.BorderColor = Color.Black;
            propina15.BorderColor = Color.Black;
            propina20.BorderColor = Color.Green;

            propina0.BorderWidth = 1;
            propina5.BorderWidth = 1;
            propina10.BorderWidth = 1;
            propina15.BorderWidth = 1;
            propina20.BorderWidth = 3;

            calcularPropona(20);
        }
    }
}