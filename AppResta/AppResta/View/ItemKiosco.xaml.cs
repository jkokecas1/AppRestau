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
    public partial class ItemKiosco : Rg.Plugins.Popup.Pages.PopupPage
    {
        Model.Platillos platillo; // 2020/12/05
        Model.Cart cartItem = new Model.Cart();
        public List<Model.Cart> cart;
        ListView caritoListView;
        int cantsteper = 0;
        Button cantidadCarrito;


        public ItemKiosco(Model.Platillos platillo = null, ListView caritoListView = null, List<Model.Cart> cart= null, Button cantidadCarrito = null)
        {
            InitializeComponent();

            this.platillo = platillo;
            this.caritoListView = caritoListView;
            this.cart = cart;
            this.cantidadCarrito = cantidadCarrito;
            init();


        }

        private void init()
        {
            nombPlatillo.Text = platillo.nombre;
            descPlatillo.Text = platillo.descrip;
            extrasListView.ItemsSource = Services.CartService.Extras(platillo.id);
        }

        public void cantidadPlatillo(object sender, ValueChangedEventArgs e)
        {
            var value = e.NewValue;
            valCantidad.Text = value.ToString();
        }

        public void agregarItemCart(object sender, EventArgs e)
        {
            int valSteper = Int32.Parse(stepper.Value + "");
            int index = 0;

            int cantidad = Int32.Parse(valCantidad.Text);
            var hora = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss").Replace(" ", "-");
            var fecha = DateTime.Now.ToString("MM-dd-yy");
            string comentario = "SIN COMENTARIOS";


            if (comentTxt.Text != null)
            {
                comentario = comentTxt.Text;
            }

            double total = cantidad + Convert.ToDouble(platillo.precio.Replace(",", "."));
            cartItem = new Model.Cart();
            if (cart.Count == 0 && cart != null) // Caso 1: Carrito vacio
            {
                // Console.WriteLine("Si entras");
                cartItem.id = platillo.id;
                cartItem.platillo = platillo.nombre;
                cartItem.cantidad = cantidad;
                cartItem.precio = Convert.ToDouble(platillo.precio.Replace(",", "."));
                cartItem.total = (int)(cartItem.precio * cartItem.cantidad);
                cartItem.comentario = comentario;
                cart.Add(cartItem);



            }
            else // Caso 2: Cariito con al menuos un platillo
            {
                int band = 0;
                for (int i = 0; i < cart.Count; i++)
                {
                    if (cart[i].id == platillo.id) // Caso 2.1: El platillo existe
                    {
                        if (cantsteper != 0)
                        {
                            if (valSteper > cart[i].cantidad || valSteper == cart[i].cantidad)
                            {
                                cart[i].cantidad = valSteper;
                            }
                            else
                            {
                                int aux = cart[i].cantidad - valSteper;
                                cart[i].cantidad -= aux;
                            }
                        }
                        else
                        {
                            //Console.WriteLine("00000");
                            cart[i].cantidad += valSteper;
                        }

                        // Console.WriteLine("-----" + cart[i].cantidad);
                        cart[i].total = (double)(cart[i].precio * cart[i].cantidad);
                        index = i;
                        band = 1;
                        break;
                    }
                }
                if (band == 0)
                {

                    cartItem.id = platillo.id;

                    cartItem.platillo = platillo.nombre;
                    cartItem.cantidad = cantidad;
                    cartItem.comentario = comentario;
                    cartItem.precio = Convert.ToDouble(platillo.precio.Replace(",", "."));
                    cartItem.total = (double)(cartItem.precio * cartItem.cantidad);
                    cart.Add(cartItem);


                }

            }


            caritoListView.ItemsSource = null;
            caritoListView.ItemsSource = cart;
            int articulos= 0;
            foreach (Model.Cart c in cart) {
                articulos += c.cantidad;  
            }
            cantidadCarrito.Text = articulos+"";
            Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();
        }



        private void cerrarPop(object sender, EventArgs e)
        {
            //this.IsVisible = false;
            Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();
        }
    }
}