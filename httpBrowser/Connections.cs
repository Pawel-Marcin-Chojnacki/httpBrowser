using System;
using System.Collections.Generic;
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
    /// Checks connection type and internet availability
    /// </summary>
    public class Connections
    {
        /// <summary>
        /// Checks if website is online.
        /// Attribute is IPv4 address.
        /// </summary>
        /// <returns>Description of WebSite availability status</returns>
        public bool IsWebSiteAvailable(Uri keyword)
        {
            Ping pingSender = new Ping();
            if (pingSender.Send(keyword.Host).Status == IPStatus.Success)
                return true;
            else return false;
        }

        /// <summary>
        /// Converts host from http://domain/ to www.domain
        /// Attribute is host with Hypertext Transder Protocol
        /// </summary>
        /// <returns>returns domain in form of www.site.xx or site.xx</returns>
        private string CleanAddress(string host)
        {
            host = host.Substring(7, host.Length - 8);
            if (host.Substring(0, 4) == "www.")
            {
                host = host.Substring(4, host.Length - 4);
            }
            return host;
        }

        /// <summary>
        /// This method checks if there is internet connection
        /// </summary>
        /// <returns>True when internet connection was found</returns>
        public static bool HasInternetConnection()
        {
            //Instance of our ConnectionStatusEnum
            ConnectionStatusEnum state = 0;

            //Call the API
            InternetGetConnectedState(ref state, 0);

            //Check the status, if not offline and the returned state
            //Is 0 then we have a connection
            if (((int)ConnectionStatusEnum.INTERNET_CONNECTION_OFFLINE & (int)state) == 0)
            {
                //Return true, we have a connection
                return true;
            }
            //Return false, no connection available
            return false;
        }

        /// <summary>
        /// ENUM - holds possible internet connection states.
        /// 0x1 - Internet connected thrugh modem or phone
        /// 0x2 - Internet connected thrugh LAN or Wi-Fi
        /// 0x4 - Internet connected thrugh PROXY or Parental Control Software
        /// 0x10 - Connected to Remote Access Service
        /// 0x20 - Lack of connection (wrong configuration\lack of service running\mom didn't pay the bills
        /// 0x40 - Local system has a valid connection to the Internet, but it might or might not be currently connected.
        /// </summary>
        [Flags]
        enum ConnectionStatusEnum : int
        {
            INTERNET_CONNECTION_MODEM = 0x1,
            INTERNET_CONNECTION_LAN = 0x2,
            INTERNET_CONNECTION_PROXY = 0x4,
            INTERNET_RAS_INSTALLED = 0x10,
            INTERNET_CONNECTION_OFFLINE = 0x20,
            INTERNET_CONNECTION_CONFIGURED = 0x40
        }

        /// <summary>
        /// Call for unmanaged code
        /// To import Windows Internet API
        /// </summary>
        /// <param name="flags"></param>
        /// <param name="dw"></param>
        /// <returns></returns>
        [DllImport("wininet", CharSet = CharSet.Auto)]
        
        /// <summary>
        /// Retrieves the connected state of the local system.
        /// </summary>
        static extern bool InternetGetConnectedState(ref ConnectionStatusEnum flags, int dw);

        /// <summary>
        /// Check if link exists in the internet 
        /// </summary>
        /// <param name="webSiteAddress"></param>
        /// <returns></returns>
        public static bool CheckLink(string webSiteAddress)
        {
            Uri UrlCheck = new Uri(webSiteAddress);
            HttpWebRequest Request = (HttpWebRequest)WebRequest.Create(UrlCheck);
            //Loooong time for request.
            Request.Timeout = 1500;
            HttpWebResponse Response;
            try
            {
                Response = (HttpWebResponse)Request.GetResponse();
                return Response.StatusCode == HttpStatusCode.Found;
            }
            catch (Exception)
            {
                return false; //could not connect to the internet (maybe) 
            }
        }
    }
}