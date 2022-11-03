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
        Model.Pagos pago;
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
                    Subtotal.Text = orden.total;//( Int32.Parse(orden.total) + Int32.Parse(propina.Text) + Int32.Parse(Tarjeta.Text) + Int32.Parse(Efectivo.Text)) + "";   
                }
            }

            var monkeyList = new List<string>();
            monkeyList.Add("5");
            monkeyList.Add("10");
            monkeyList.Add("15");
            monkeyList.Add("20");
            monkeyList.Add("25");
            monkeyList.Add("30");
            monkeyList.Add("35");


            piker_propina.ItemsSource = monkeyList;


        }

        void OnPickerSelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = (Picker)sender;
            int selectedIndex = picker.SelectedIndex;
            // Console.WriteLine(selectedIndex);
            if (selectedIndex != -1)
            {
                //(bill * 20)/100 ) 101 * 10 

                propina.Text = ((Int32.Parse(Ordenes.total) * Int32.Parse(picker.SelectedItem.ToString())) / 100) + "";
                total.Text = Ordenes.total;
                Subtotal.Text = Int32.Parse(propina.Text.ToString()) + Int32.Parse(total.Text.ToString()) + "";
                //Console.WriteLine( Ordenes.total);
            }
        }


        void OnClicked_Pagar(object sender, EventArgs e)
        {
            pago = new Model.Pagos();

            if (Efectivo.Text == "" && Tarjeta.Text == "")
            {
                PopupNavigation.Instance.PushAsync(new PrePago());
            }
            else if (Tarjeta.Text != "" && Efectivo.Text == "")
            {
                pago.monto = Double.Parse(Tarjeta.Text + "");
                pago.tipoPago = "TARJETA";
                pago.propina = propina.Text;
                if (pago.monto < Double.Parse(Subtotal.Text))
                {


                    PopupNavigation.Instance.PushAsync(new PrePago());
                }
                else
                {
                    PopupNavigation.Instance.PushAsync(new ConfirmarPago());
                }
            }
            else if (Efectivo.Text != "" && Tarjeta.Text == "")
            {
                pago.monto = Double.Parse(Efectivo.Text + "");
                pago.tipoPago = "EFECTIVO";
                pago.propina = propina.Text;
                if (pago.monto < Double.Parse(Subtotal.Text))
                {
                    PopupNavigation.Instance.PushAsync(new PrePago());
                }
                else
                {
                    PopupNavigation.Instance.PushAsync(new ConfirmarPago());
                }
            }
            else if (Efectivo.Text != "" && Tarjeta.Text != "")
            {
                int monto = Int32.Parse(Efectivo.Text + "") + Int32.Parse(Tarjeta.Text + "");
                Console.WriteLine(monto);
                pago.monto = Double.Parse(monto.ToString());
                pago.tipoPago = "EFECTIVO Y TARJETA";
                pago.propina = propina.Text;
                if (pago.monto <= Double.Parse(Subtotal.Text))
                {
                    PopupNavigation.Instance.PushAsync(new PrePago());
                }
                else
                {
                    PopupNavigation.Instance.PushAsync(new ConfirmarPago());
                }
            }

        }
        bool pagos = false;
        void OnClicked_Num1(object sender, EventArgs e) { _ = pagos == false ? Efectivo.Text += "1" : Tarjeta.Text += "1"; }
        void OnClicked_Num2(object sender, EventArgs e) { _ = pagos == false ? Efectivo.Text += "2" : Tarjeta.Text += "2"; }
        void OnClicked_Num3(object sender, EventArgs e) { _ = pagos == false ? Efectivo.Text += "3" : Tarjeta.Text += "3"; }
        void OnClicked_Num4(object sender, EventArgs e) { _ = pagos == false ? Efectivo.Text += "4" : Tarjeta.Text += "4"; }
        void OnClicked_Num5(object sender, EventArgs e) { _ = pagos == false ? Efectivo.Text += "5" : Tarjeta.Text += "5"; }
        void OnClicked_Num6(object sender, EventArgs e) { _ = pagos == false ? Efectivo.Text += "6" : Tarjeta.Text += "6"; }
        void OnClicked_Num7(object sender, EventArgs e) { _ = pagos == false ? Efectivo.Text += "7" : Tarjeta.Text += "7"; }
        void OnClicked_Num8(object sender, EventArgs e) { _ = pagos == false ? Efectivo.Text += "8" : Tarjeta.Text += "8"; }
        void OnClicked_Num9(object sender, EventArgs e) { _ = pagos == false ? Efectivo.Text += "9" : Tarjeta.Text += "9"; }
        void OnClicked_Num0(object sender, EventArgs e) { _ = pagos == false ? Efectivo.Text += "0" : Tarjeta.Text += "0"; }

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
                Tarjeta.Opacity = 1;

            }
            else
            {

                pagos = false;
                Efectivo.TextColor = Color.Black;
                Efectivo.Opacity = 1;
                Tarjeta.Opacity = 0.3;

            }

        }

    }
}