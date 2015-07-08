using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Threading;

namespace httpBrowser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initialize whole main windows for application
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Adds new information to small infobox
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="statement"></param>
        public static void ChangeBoxValue(MainWindow obj, string statement)
        {
            obj.InfoBoxLabel.AppendText(obj.InfoBoxLabel.Text + statement + "\n");
            obj.InfoBoxLabel.ScrollToEnd();
        }

        /// <summary>
        /// Attribute that holds information about all cells needed to create\edit\destroy
        /// </summary>
        static int Cells = 0;

        /// <summary>
        /// Dynamically create number of columns
        /// </summary>
        static int NumberOfColums = 5;

        static void DownThemAlles()
        {

        }

        /// <summary>
        /// Most important method in application
        /// Is responsible for coordinating every class
        /// 1. Prepares field for making web request.
        /// 2. Constantly updates itself in infobox
        /// 3. Creates new instance of WebSite and serves requests
        /// 4. If there is internet connection takes source code and gives it to Parser to find all correct images links
        /// 5. Makes grid to show images
        /// 6. Allows images to be downloaded from WEB.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WebRequest(object sender, RoutedEventArgs e)
        {
            //Prepare application windows for new request.
            ClearGrid(Cells);

            //Inform about start of process.
            InfoBoxLabel.Background = System.Windows.Media.Brushes.White;
            InfoBoxLabel.AppendText("Wysyłam żądanie...\n");
            InfoBoxLabel.ScrollToEnd();

            //Take address from address bar.
            WebSite WebSite = new WebSite(SiteAdressTextBox.Text);
            //Verify correctness of web address
            Parser.ValidateWebAddress(WebSite.WebSiteAddress.ToString());
            InfoBoxLabel.AppendText("Adres internetowy został zweryfikowany...\n");
            InfoBoxLabel.ScrollToEnd();

            //When there is internet connection. 
            if (Connections.HasInternetConnection())
            {
                InfoBoxLabel.AppendText("Wykryto połączenie internetowe...\n");
                InfoBoxLabel.ScrollToEnd();

                //Check if website is available (can I ping it?)
                if (WebSite.IsWebSiteAvailable(WebSite.WebSiteAddress) == true)
                {
                    //If there was response from website...
                    InfoBoxLabel.AppendText("Nawiązano połączenie ze stroną...\n");
                    InfoBoxLabel.ScrollToEnd();

                    //Get Source Code of website (HTML document only)...
                    WebSite.GetWebsiteSourceCode();
                    //...and inform user about success
                    InfoBoxLabel.AppendText("Kod strony pobrano pomyślnie...\n");
                    InfoBoxLabel.ScrollToEnd();

                    //Having document, Parse it and tame all image links from it.
                    WebSite.RetrieveImageLinks(WebSite.WebSiteAddress);

                    //Update information about needed cells to dynamically create in GRID
                    Cells = FindAllImages.ListOfDistinctMatches.Count;
                    if (Cells > 0)
                    {
                        //Prepare GRID layout to further display
                        MakeGrid(Cells);
                        //Take all links and fill grid with it's content
                        FillGrid(Cells);
                        //Enable second button
                        DownloadAllButon.IsEnabled = true;
                    }

                    
                }

                //Can't ping website
                else
                {
                    //Inform user about failure
                    InfoBoxLabel.AppendText("Brak odpowiedzi od serwera\n");
                    InfoBoxLabel.ScrollToEnd();
                    //Change box color to draw attention of user
                    InfoBoxLabel.Background = System.Windows.Media.Brushes.Crimson;
                }
            }

            else
            {
                //Very angry color to force user to buy the internet (or correct it's connection)
                InfoBoxLabel.Background = System.Windows.Media.Brushes.Crimson;
                //Short info about "obstacle"
                InfoBoxLabel.AppendText("Brak połączenia internetowego...\n");
                InfoBoxLabel.ScrollToEnd();
            }
        }

        /// <summary>
        /// Clear Grid in MainWindow to remove all elements
        /// Remove all Children(buttons)
        /// Then destroy Rows and Columns
        /// </summary>
        /// <param name="cells">Number of cells to clear</param>
        private void ClearGrid(int cells)
        {
            try
            {
                Photos.Children.Clear();
                Photos.RowDefinitions.Clear();
                Photos.ColumnDefinitions.Clear();
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show("Uruchom ponownie program.");
                this.Close();
            }
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Method to inform user when ALL downloads are completed.
        /// Don't use it for individual downloads it will get users angry.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            System.Windows.MessageBox.Show("Download completed!");
        }

        /// <summary>
        /// Changes text color with every keystroke.
        /// Notifies user that address box is changing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SiteAdressTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SiteAdressTextBox.Foreground == System.Windows.Media.Brushes.DarkGray)
            {
                SiteAdressTextBox.Foreground = System.Windows.Media.Brushes.Black;
            }
            else
            {
                SiteAdressTextBox.Foreground = System.Windows.Media.Brushes.DarkGray;
            }
        }

        /// <summary>
        /// Downloads all image files found on the web site in appropriate file format
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DownThemAll(object sender, RoutedEventArgs e)
        {
            //Get show on grid out of this method
            WebClient WebClient;
            //Microsoft.Win32.SaveFileDialog savefile = new Microsoft.Win32.SaveFileDialog();
            //savefile.Title = "Wybierz folder zapisu";
            FolderBrowserDialog Folder = new FolderBrowserDialog();
            Uri Uri;
            string FileName;
            if (Folder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                for (int Ctr = 0; Ctr < FindAllImages.ListOfDistinctMatches.Count; Ctr++)
                {
                    WebClient = new WebClient();
                    Uri = new Uri(FindAllImages.Matches[Ctr].Value.ToString());
                    FileName = System.IO.Path.GetFileName(Uri.AbsolutePath);
                    WebClient.DownloadFileAsync(Uri, Folder.SelectedPath + "\\" + FileName);
                }
            }
        }

        /// <summary>
        /// Makes grid for all images.
        /// If grid is not clears it will double current amount of cells
        /// </summary>
        /// <param name="cells">How many images will be to show.</param>
        protected void MakeGrid(int cells)
        {
            ColumnDefinition ColDef1 = new ColumnDefinition();
            RowDefinition RowDef1 = new RowDefinition();
            if (cells < NumberOfColums)
            {
                for (int i = 0; i < cells; i++)
                {
                    ColDef1.Width = new System.Windows.GridLength(200);
                    Photos.ColumnDefinitions.Add(ColDef1);
                    ColDef1 = new ColumnDefinition();
                }
            }
            else
            {
                for (int i = 0; i < NumberOfColums; i++)
                {
                    ColDef1.Width = new System.Windows.GridLength(200);
                    Photos.ColumnDefinitions.Add(ColDef1);
                    ColDef1 = new ColumnDefinition();
                }
            }
            for (int i = 0; i < cells; i++)
            {
                if (i % 10 == 0)
                {
                    RowDef1.Height = new System.Windows.GridLength(200);
                    Photos.RowDefinitions.Add(RowDef1);
                    RowDef1 = new RowDefinition();
                }
            }
        }

        /// <summary>
        /// Dictionary for holding connection (relation) between image and button
        /// </summary>
        private Dictionary<System.Windows.Controls.Button, Uri> Przyciski = new Dictionary<System.Windows.Controls.Button, Uri>();

        /// <summary>
        /// Fills main Grid with cells by 5 columns each row
        /// </summary>
        /// <param name="cells"></param>
        protected void FillGrid(int cells)
        {
            System.Windows.Controls.Button PhotoButton;
            BitmapImage Bitimg;
            Image Img;
            Przyciski = new Dictionary<System.Windows.Controls.Button, Uri>();
            for (int i = 0; i < cells; i++)
            {
                Bitimg = new BitmapImage();
                Bitimg.BeginInit();
                Bitimg.UriSource = new Uri((FindAllImages.Matches[i].Value.ToString()));
                //System.Windows.MessageBox.Show(bitimg.Height.ToString());
                Bitimg.EndInit();

                Img = new Image();
                Img.Stretch = Stretch.Uniform;
                Img.Source = Bitimg;
                PhotoButton = new System.Windows.Controls.Button();

                // Set Button.Content
                PhotoButton.Content = Img;

                PhotoButton.Click += PhotoButton_Click;

                PhotoButton.Background = System.Windows.Media.Brushes.White;/*new ImageBrush(bitimg)*/;

                Przyciski.Add(PhotoButton, Bitimg.UriSource);

                Grid.SetColumn(PhotoButton, i % NumberOfColums);
                Grid.SetRow(PhotoButton, i / NumberOfColums);
                Photos.Children.Add(PhotoButton);
            }
        }

        /// <summary>
        /// Download async single clicked button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PhotoButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button Przycisk = (System.Windows.Controls.Button)sender;
            Uri uri = Przyciski[Przycisk];
            Microsoft.Win32.SaveFileDialog Savefile = new Microsoft.Win32.SaveFileDialog();
            Savefile.DefaultExt = Parser.CheckFileExtension(uri);
            Savefile.Filter = Parser.SetFilter(Savefile.DefaultExt);
            Savefile.FileName = System.IO.Path.GetFileName(uri.AbsolutePath);
            if (Savefile.ShowDialog() == true)
            {
                BitmapImage webImage = new BitmapImage(uri);
                WebClient webClient = new WebClient();
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(CompletedOneImage);
                webClient.DownloadFileAsync(uri, Savefile.FileName);
                //Modify filename attributes to save with original filename.extension
            }
            else
            {
                System.Windows.MessageBox.Show("Nie zapisano pliku.");
            }
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Inform user about completed download
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CompletedOneImage(object sender, AsyncCompletedEventArgs e)
        {
            InfoBoxLabel.AppendText("Plik został pobrany...\n");
            InfoBoxLabel.ScrollToEnd();
            //throw new NotImplementedException();
        }

    }
}
