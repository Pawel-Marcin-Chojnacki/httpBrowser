using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace httpBrowser
{
    /// <summary>
    /// Class responsible for making site (it's one site not website)
    /// </summary>
    public class Site : FindAllImages
    {
        /// <summary>
        /// Web address for one site product
        /// </summary>
        private Uri webSiteAddress;
        public Uri WebSiteAddress
        {
            set 
            { 
                webSiteAddress = value; 
            }
            get 
            { 
                return this.webSiteAddress;
            }
        }


        /// <summary>
        /// HTML Document ripped from site
        /// </summary>
        private string webSiteSourceCode;
        public string WebSiteSourceCode
        {
            //set the website source code
            set 
            { 
                this.webSiteSourceCode = value; 
            }
            //get the website source code
            get 
            { 
                return this.webSiteSourceCode; 
            }
        }

        /// <summary>
        /// Public constructor (default)
        /// </summary>
        public Site()
        {
        }

        /// <summary>
        /// Constructor with known web adress.
        /// </summary>
        /// <param name="adress">It's a web site address with string as a parameter</param>
        public Site(string adress)
        {
            try
            {
                //Try convert string address to Uri object
                if (Uri.TryCreate(adress, UriKind.RelativeOrAbsolute, out webSiteAddress) == false)
                {
                    MessageBox.Show("Podałeś nieprawidłowy format strony. Spróbuj http://www.strona.pl/");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Nie udało się utworzyć poprawnego adresu.");
            }
        }

        /// <summary>
        /// Checks connection with the Internet
        /// Checks availability of website
        /// If all assumptions are granted read source code
        /// Short: Makes connection to web and copy whole source code to webSiteSoureCode
        /// </summary>
        public void GetWebsiteSourceCode()
        {
            if (HasInternetConnection())
            {
                //Has to be null so it can be closed in case of failure
                WebResponse Response = null;
                StreamReader Reader = null;
                try
                {
                    WebRequest Request = WebRequest.Create(WebSiteAddress);
                    Request.Method = "GET";
                    Response = Request.GetResponse();
                    Stream stream = Response.GetResponseStream();
                    Reader = new StreamReader(stream);
                    webSiteSourceCode = Reader.ReadToEnd();
                }
                //Catch Exception
                //Tell user if something is going wrong
                catch (Exception)
                {
                    MessageBox.Show("Mamy tutaj tzw. wyątek.\n" + 
                              "Oznacza to mniej więcej tyle:\n" + 
                    "coś się zepsuło podczas pobierania kodu strony. Przepraszam :(",
                                            "Użytkowniku.");
                }
                //Anyway stream and response has to be closed
                finally
                {
                    //Close them anyway
                    if (Reader != null)
                    {
                        Reader.Close();
                    }
                    if (Response != null)
                    {
                        Response.Close();
                    }
                }
            }
        }
    }
}
