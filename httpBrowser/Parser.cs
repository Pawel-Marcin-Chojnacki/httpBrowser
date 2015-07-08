using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Windows;


namespace httpBrowser
{
    /// <summary>
    /// Class responsible for Parsing website
    /// </summary>
    public class Parser
    {

        /// <summary>
        /// Code - source of website to work with
        /// </summary>
        private string code;
        /// <summary>
        /// Code - source of website to work with (setter and getter)
        /// </summary>
        public string Code
        {
            //set the website source code to parse
            set { this.code = value; }
            //get the website source code to parse
            get { return this.code; }
        }

        /// <summary>
        /// Attribute responsible for holding Title attribute
        /// </summary>
        private string title;
        /// <summary>
        /// Attribute responsible for holding Title attribute (setter and getter)
        /// </summary>
        public string Title
        {
            //set the website title if available
            set { this.title = value; }
            //set the website title if available
            get { return this.title; }
        }

        /// <summary>
        /// Public constructor (default)
        /// </summary>
        public Parser()
        {
        }

        /// <summary>
        /// Public constructor which takse one parameter and sets source code
        /// </summary>
        /// <param name="source">Source code of website in string type</param>
        public Parser(string source)
        {
            Code = source;
        }

        /// <summary>
        /// Method to clean code from obfuscations if needed.
        /// Pre-parsing
        /// </summary>
        /// <param name="codeToClean">obfuscated source code</param>
        /// <returns>Clean HTML code ready to correct parsing</returns>
        public string CodeCleaner(string codeToClean)
        {
            return codeToClean;
        }

        /// <summary>
        /// Validates correctness of given web address
        /// Assuming that correct address for HTTP protocol is
        /// http://www.subdomain.domain/
        /// http://www.subdomain.domain//
        /// http://subdomain.domain/
        /// http://subdomain.domain//
        /// </summary>
        /// <param name="address">Website Address in string format</param>
        /// <returns>Is correct http address? True or False</returns>
        public static bool ValidateWebAddress(string address)
        {
            //if true then it's too short for valid http address
            if (address.Length < 10)
            {
                return false;
            }
            //if true then it's valid internet address
            if (address.Substring(0, 7) == "http://" && address[address.Length - 1] == '/')
            {
                return true;
            }
            //Anything other has to be incorrect
            else return true;
        }

        /// <summary>
        /// Method which uses regular expression to extract all correct links frm website
        /// </summary>
        /// <param name="sourceCode">Gets website source code in form of plain string.</param>
        public static void GetAllLinksFromSite(string sourceCode)
        {
            string Pattern = @"((ht|f)tp(s?)\:\/\/|~/|/)?([w]{2}([\w\-]+\.)+([\w]{2,5}))(:[\d]{1,5})?";
            Regex Regex = new Regex(Pattern);
            MatchCollection Matches = Regex.Matches(sourceCode);
        }

        /// <summary>
        /// Checks type of image found on Website
        /// </summary>
        /// <param name="filePath">Gets Uri and takes just filename</param>
        /// <returns>File Extension</returns>
        public static string CheckFileExtension(Uri filePath)
        {
            if (filePath.LocalPath.Substring(filePath.LocalPath.Length - 4) == ".png")
            {
                return "png";
            }
            else if (filePath.LocalPath.Substring(filePath.LocalPath.Length - 4) == ".jpg")
	        {
		        return "jpg";
	        }
            else if (filePath.LocalPath.Substring(filePath.LocalPath.Length - 4) == ".gif")
	        {
		        return "gif";
	        }
            else return "tiff";
        }

        /// <summary>
        /// Sets filter for SaveFile class property
        /// </summary>
        /// <param name="extension">Parameter of file</param>
        /// <returns>Returns string to set filter in SaveFileDialog</returns>
        public static string SetFilter(string extension)
        {
            if (extension == "png")
            {
                return "Image File (PNG)|*.png;";
            }
            else if(extension == "jpg")
            {
                return "Image File (JPG)|*.jpg";
            }
            else if (extension == "gif")
            {
                return "Image File (GIF)|*.gif";
            }
            return "Image File (TIFF)|*.tiff;";
        }
    }
}