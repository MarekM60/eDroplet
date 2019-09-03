using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace eDroplet.Views
{
    public partial class MainPage : TabbedPage
    {
        public MainPage()
        {
            InitializeComponent();
            BarBackgroundColor = Color.Black;
            BarTextColor = Color.White;
            if (Device.RuntimePlatform == Device.Android) UnselectedTabColor = Color.DarkGray;
        }
    }
}