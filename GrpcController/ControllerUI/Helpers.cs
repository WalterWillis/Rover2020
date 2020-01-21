using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace ControllerUI
{
    public static class Helpers
    {

        public static X509Certificate2 GetClientCertificate()
        {
            string configPath = Path.GetFullPath(Properties.app.Default.ClientCert);
            return new X509Certificate2(configPath, Properties.app.Default.ClientCertPass);
        }

        public static X509Certificate2 GetServerCertificate()
        {
            string configPath = Path.GetFullPath(Properties.app.Default.ServerCert);
            return new X509Certificate2(configPath, Properties.app.Default.ServerCertPass);
        }
    }
}
