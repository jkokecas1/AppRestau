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
    public partial class SplashCarga : ContentPage
    {
        float maxValue = 1;
        float progressmax = 25;
        bool istimerRunning = true;
        float progress = 0;
        int counter = 1;
        public SplashCarga()
        {
            InitializeComponent();
            Device.StartTimer(TimeSpan.FromSeconds(0.1), () =>
            {
                if (progress >= 1)
                {
                    istimerRunning = false;
                    Navigation.PopAsync(false);
                    
                }
                else
                {
                    progress += maxValue / progressmax;
                    progressbar.ProgressTo(progress, 500, Easing.Linear);
                    progressLabel.Text = $"{counter+75}/{progressmax+75}";
                    counter += 1;
                }

                return istimerRunning;
            });
        }
    }
}