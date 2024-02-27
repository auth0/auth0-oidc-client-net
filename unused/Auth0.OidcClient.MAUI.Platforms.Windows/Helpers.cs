using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Auth0.OidcClient.Platforms.Windows
{
    internal interface IHelpers
    {
        bool IsAppPackaged { get; }
        bool IsUriProtocolDeclared(string scheme);
        void OpenBrowser(Uri uri);
    }

    internal class Helpers : IHelpers
    {
#pragma warning disable SA1203 // Constants should appear before fields
        private const long AppModelErrorNoPackage = 15700L;
#pragma warning restore SA1203 // Constants should appear before fields

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int GetCurrentPackageFullName(ref int packageFullNameLength, System.Text.StringBuilder packageFullName);

        /// <summary>
        /// Helper property to verify the application is packaged.
        /// </summary>
        /// <remarks>
        /// Original source: https://github.com/dotMorten/WinUIEx
        /// </remarks>
        /// <returns>A boolean indicate whether or not the app is packaged.</returns>
        public bool IsAppPackaged
        {
            get
            {
                try
                {
                    // Application is MSIX packaged if it has an identity: https://learn.microsoft.com/en-us/windows/msix/detect-package-identity
                    int length = 0;
                    var sb = new StringBuilder(0);
                    int result = GetCurrentPackageFullName(ref length, sb);
                    return result != AppModelErrorNoPackage;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Helper method to verify the scheme is defined as a protocol in the AppxManifest.xml files
        /// </summary>
        /// <remarks>
        /// Original source: https://github.com/dotMorten/WinUIEx
        /// </remarks>
        /// <param name="scheme">The scheme expected to be declared.</param>
        /// <returns>A boolean indicate whether or not the scheme is declared as an Uri protocol.</returns>
        public bool IsUriProtocolDeclared(string scheme)
        {
            if (global::Windows.ApplicationModel.Package.Current is null)
                return false;
            var docPath = Path.Combine(global::Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "AppxManifest.xml");
            var doc = XDocument.Load(docPath, LoadOptions.None);
            var reader = doc.CreateReader();
            var namespaceManager = new XmlNamespaceManager(reader.NameTable);
            namespaceManager.AddNamespace("x", "http://schemas.microsoft.com/appx/manifest/foundation/windows10");
            namespaceManager.AddNamespace("uap", "http://schemas.microsoft.com/appx/manifest/uap/windows10");

            // Check if the protocol was declared
            var decl = doc.Root?.XPathSelectElements($"//uap:Extension[@Category='windows.protocol']/uap:Protocol[@Name='{scheme}']", namespaceManager);

            return decl != null && decl.Any();
        }

        /// <summary>
        /// Helper method to open the browser through the url.dll.
        /// </summary>
        /// <param name="uri">The Uri to open</param>
        public void OpenBrowser(Uri uri)
        {
            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = uri.ToString(),
                UseShellExecute = true
            };
            System.Diagnostics.Process.Start(psi);
        }

        public static string Encode(string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(bytes);
        }

        public static string Decode(string value)
        {
            var bytes = Convert.FromBase64String(value);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
