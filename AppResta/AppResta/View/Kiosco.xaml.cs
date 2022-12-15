using AppResta.Model;
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
    public partial class Kiosco : ContentPage
    {
        List<Model.Platillos> platillo;
        List<Model.Categorias> categorias;
        Model.Empleado emp;
        public static List<Model.Cart> cart;
        bool banderaExpander = false;

        public Kiosco(List<Model.Platillos> platillo, List<Model.Categorias> categorias, Model.Empleado emp)
        {
            
            InitializeComponent();
            this.platillo = platillo;
            this.categorias = categorias;
            this.emp = emp;
            platillosListView.ItemsSource = platillo;
            Model.Categorias cate = new Model.Categorias();
            cate.nombre = "TODO";
            this.categorias.Add(cate);
            categoriasListView.ItemsSource = this.categorias;
            cart = new List<Cart>();
            cantidadCarrito.Text = cart.Count.ToString();
        }

        private void btnOrdenar_Clicked(object sender, EventArgs e)
        {
            foreach (Model.Cart c in cart) { 
                Console.WriteLine(c.platillo+" "+c.idItem);
            }
            PopupNavigation.Instance.PushAsync(new PagoKiosko(cart));
            
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void categoriasListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Model.Categorias categoria =e.CurrentSelection.FirstOrDefault() as Model.Categorias;
            List<Model.Platillos> pla = new List<Platillos>();

            if (categoria.nombre == "TODO") {
                platillosListView.ItemsSource = platillo;
            }
            else {

                foreach (Model.Platillos s in platillo)
                {
                    if (s.categoria == categoria.nombre)
                    {
                        pla.Add(s);
                    }
                }
                platillosListView.ItemsSource = pla;
            }
               
        }

        private void platillosListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Model.Platillos platillo = e.CurrentSelection.FirstOrDefault() as Model.Platillos;
            
            PopupNavigation.Instance.PushAsync(new ItemKiosco(platillo:platillo,
                                                              caritoListView: caritoListView, cart: cart, cantidadCarrito));
            //cantidadCarrito.Text = cart.Count.ToString();
            //ItemPlatillo(platillo,"",bandera:1,cart:cart, caritoListView,empleado: emp, size: array));
        }
        private async void Expandir_Clicked(object sender, EventArgs e)
        {
            if (banderaExpander == false)
            {
                carrito.IsVisible = true;
                await carrito.TranslateTo(0, 0, 500); //Scale to half its size
                banderaExpander = true;
            }
            else
            {
                await carrito.TranslateTo(0, 400, 500);
                //carrito.Animate();
                carrito.IsVisible = false;
                banderaExpander = false;
            }
        }
    }
}