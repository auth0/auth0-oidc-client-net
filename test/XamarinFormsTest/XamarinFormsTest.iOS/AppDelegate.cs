using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

using Auth0.OidcClient;
using Auth0.OidcClient.Core;
using Autofac;
using Autofac.Extras.CommonServiceLocator;
using CommonServiceLocator;

namespace XamarinFormsTest.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.Register(context => new Auth0Client(new Auth0ClientOptions
            {
                Domain = "jerrie.auth0.com",
                ClientId = "vV9twaySQzfGesS9Qs6gOgqDsYDdgoKE"
            })).As<IAuth0Client>();

            IContainer container = builder.Build();

            AutofacServiceLocator asl = new AutofacServiceLocator(container);
            ServiceLocator.SetLocatorProvider(() => asl);

            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
    }
}
