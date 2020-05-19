using System;
using System.ServiceModel.Web;

namespace TTServer {
    class Program {
        static void Main() {
            // For a REST WCF service host notice the use of WebServiceHost class instead of ServiceHost
            WebServiceHost host = new WebServiceHost(typeof(TTService.TTService));
            host.Open();
            Console.WriteLine("TT service running");
            Console.WriteLine("Press ENTER to stop the service");
            Console.ReadLine();
            host.Close();
        }
    }
}
