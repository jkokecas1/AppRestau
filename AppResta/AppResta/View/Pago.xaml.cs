﻿using System;
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
        public Pago(List<Model.Ordenes> orden)
        {
            this.orden = orden;
            InitializeComponent();
            init();
        }

        public void init() 
        {

            
        }
    }
}