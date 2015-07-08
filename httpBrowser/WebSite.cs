using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace httpBrowser
{

    /// <summary>
    /// Class Responsible for holding all informations about website
    /// </summary>
    public class WebSite : Site
    {

        /// <summary>
        /// Public constructor with one parameter
        /// </summary>
        /// <param name="address">Web site address in string format.</param>
        public WebSite(string address) 
        {
            WebSiteAddress = new Uri(address);
        }

        /// <summary>
        /// Public constructor (default)
        /// </summary>
        public WebSite()
        {
        }

        /// <summary>
        /// Attribute to store domain of website
        /// </summary>
        private string domain;
        public string Domain
        {
            //set the domain name
            set { domain = value; }
            //get the domain name
            get { return domain; }
        }
    }
}
