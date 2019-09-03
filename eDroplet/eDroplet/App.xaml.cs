/*

 Copyright 2019 Marek Macner, iDiab.dev LLC

 Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
 documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
 the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, 
 and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
 TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
 IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
 WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
 OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


 2019 Marek Macner, iDiab.dev LLC
*/



using Prism;
using Prism.Ioc;
using eDroplet.ViewModels;
using eDroplet.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Acr.UserDialogs;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace eDroplet
{
    public partial class App
    {
        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer) { }

        protected override async void OnInitialized()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTM2NDIwQDMxMzcyZTMyMmUzMEY0K1ZJSUtoaEJCQ1hCU2RBMC9SajJtWVVPNHpQa3VPdFlBWGtWdDFLZTg9");

            InitializeComponent();

            await NavigationService.NavigateAsync("NavigationPage/MainPage");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterInstance<IUserDialogs>(UserDialogs.Instance);

            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
            containerRegistry.RegisterForNavigation<HomePage, HomePageViewModel>();
            containerRegistry.RegisterForNavigation<CgmPage, CgmPageViewModel>();
        }
    }
}
