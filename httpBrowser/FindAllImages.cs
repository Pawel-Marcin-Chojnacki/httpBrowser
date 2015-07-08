using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Windows;

namespace httpBrowser
{
    /// <summary>
    /// Searches for  images on website
    /// Has ability to check connection before work
    /// </summary>
    public class FindAllImages : Connections
    {
        /// <summary>
        /// Static Collection of matches
        /// Has list of all links found on website
        /// </summary>
        public static MatchCollection Matches 
        { 
            get; 
            private set; 
        }

        /// <summary>
        /// Get all images from website
        /// </summary>
        /// <param name="webPageAddress"></param>
        public void RetrieveImageLinks( Uri webPageAddress )
        {
            GetAllImagesUrls(RetrieveContent(webPageAddress));
        }

        /// <summary>
        /// Get file\date whatever is given to the streamReader.
        /// </summary>
        /// <param name="webPageAddress">Link to file</param>
        /// <returns>Whole data in string format</returns>
        private string RetrieveContent( Uri webPageAddress )
        {
            HttpWebResponse Response = null;
            StreamReader ResponseStream = null;
            try
            {
                HttpWebRequest Request = (HttpWebRequest)WebRequest.Create(webPageAddress);
                Request.Timeout = 3000;

                Response = (HttpWebResponse)Request.GetResponse();

                ResponseStream = new StreamReader(Response.GetResponseStream());
                return ResponseStream.ReadToEnd(); 
            }
            //There is problem with streaming or request.
            catch (Exception ex)
            { 
                throw ex; 
            } 
            finally 
            { 
                if (Response != null)
                {
                    Response.Close(); 
                }
                //close down descriptors
                if (ResponseStream != null)
                {
                    ResponseStream.Close(); 
                }
            }

        }

        /// <summary>
        /// using a regular expression, find all of the href or urls 
        /// in the content of the page 
        /// </summary>
        /// <param name="content"></param>
        private void GetAllImagesUrls( string content ) 
        { 
            //regular expression Responsible for finding all images on html document.
            string Pattern = @"(http://)[A-Za-z0-9\-\.]+\.[A-Za-z]{2,3}(:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*[^\.\,\)\(\s].(jpg|.png|.gif|.tiff)";
            Regex Regex = new Regex(Pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            Matches = Regex.Matches(content);
            ListOfDistinctMatches = Matches.Cast<Match>().Select(m => m.Value).Distinct().ToList();
        }
        public static List<string> ListOfDistinctMatches;
    }
}
