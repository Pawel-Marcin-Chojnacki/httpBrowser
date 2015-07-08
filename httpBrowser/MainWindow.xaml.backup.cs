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
        }

        /// <summary>
        /// Attribute that holds information about all cells needed to create\edit\destroy
        /// </summary>
        static int cells = 0;

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
        private void webRequest(object sender, RoutedEventArgs e)
        {
            //Prepare application windows for new request.
            clearGrid(cells);
            
            //Inform about start of process.
            InfoBoxLabel.Background = System.Windows.Media.Brushes.White;
            InfoBoxLabel.AppendText("Wysyłam żądanie...\n");
            
            //Take address from address bar.
            Site webSite = new Site(siteAdressTextBox.Text);
            
            //Verify correctness of web address
            Parser.validateWebAddress(webSite.webSiteAddress.ToString());
            InfoBoxLabel.AppendText("Adres internetowy został zweryfikowany...\n");

            //When there is internet connection. 
            if (Connections.HasInternetConnection())
            {
                InfoBoxLabel.AppendText("Wykryto połączenie internetowe...\n");
                
                //Check if website is available (can I ping it?)
                if(Connections.isWebSiteAvailable(webSite.webSiteAddress) == true)
                {
                    //If there was response from website...
                    InfoBoxLabel.AppendText("Nawiązano połączenie ze stroną...\n");

                    //Get Source Code of website (HTML document only)...
                    webSite.getWebsiteSourceCode();
                    //...and inform user about success
                    InfoBoxLabel.AppendText("Kod strony pobrano pomyślnie...\n");

                    //Having document, Parse it and tame all image links from it.
                    webSite.RetrieveImageLinks(webSite.webSiteAddress);
                    
                    //Update information about needed cells to dynamically create in GRID
                    cells = FindAllImages.Matches.Count;
                    //Prepare GRID layout to further display
                    makeGrid(cells);
                    //Take all links and fill grid with it's content
                    fillGrid(cells);
                }

                //Can't ping website
                else
                {
                    //Inform user about failure
                    InfoBoxLabel.AppendText("Brak odpowiedzi od serwera\n");
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
            }             
        }

        /// <summary>
        /// Clear Grid in MainWindow to remove all elements
        /// Remove all Children(buttons)
        /// Then destroy Rows and Columns
        /// </summary>
        /// <param name="cells">Number of cells to clear</param>
        private void clearGrid(int cells)
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
        

        //private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        //{
        //    progressBar.Value = e.ProgressPercentage;
        //}

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
        private void siteAdressTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(siteAdressTextBox.Foreground == System.Windows.Media.Brushes.DarkGray)
            {
                siteAdressTextBox.Foreground = System.Windows.Media.Brushes.Black;
            }
            else
            {
                siteAdressTextBox.Foreground = System.Windows.Media.Brushes.DarkGray;
            }
        }

        /// <summary>
        /// Downloads all image files found on the web site in appropriate file format
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void downThemAll(object sender, RoutedEventArgs e)
        {
            
            //Get show on grid out of this method
            WebClient webClient;
            //Microsoft.Win32.SaveFileDialog savefile = new Microsoft.Win32.SaveFileDialog();
            //savefile.Title = "Wybierz folder zapisu";
            FolderBrowserDialog folder = new FolderBrowserDialog();
            Uri uri;
            string FileName;
            if (folder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                for (int ctr = 0; ctr < FindAllImages.Matches.Count; ctr++)
                {
                    webClient = new WebClient();
                    uri = new Uri(FindAllImages.Matches[ctr].Value.ToString());
                    FileName = System.IO.Path.GetFileName(uri.AbsolutePath);
                    System.Windows.MessageBox.Show(folder.SelectedPath + FileName);
                    webClient.DownloadFileAsync(uri, folder.SelectedPath + uri.LocalPath);
                }
            }  
        }

        /// <summary>
        /// Makes grid for all images.
        /// If grid is not clears it will double current amount of cells
        /// </summary>
        /// <param name="cells">How many images will be to show.</param>
        protected void makeGrid(int cells)
        {
            ColumnDefinition colDef1 = new ColumnDefinition();
            RowDefinition rowDef1 = new RowDefinition();
            if (cells < 5)
            {
                for (int i = 0; i < cells; i++)
                {
                    colDef1.Width = new System.Windows.GridLength(200);
                    Photos.ColumnDefinitions.Add(colDef1);
                    Photos.ColumnDefinitions.Clear();
                    colDef1 = new ColumnDefinition();
                }
            }
            else
            {
                for (int i = 0; i < 5; i++)
                {
                    colDef1.Width = new System.Windows.GridLength(200);
                    Photos.ColumnDefinitions.Add(colDef1);
                    colDef1 = new ColumnDefinition();
                }
            }
            for (int i = 0; i < cells; i++)
            {
                if(i%10 == 0)
                {
                    rowDef1.Height = new System.Windows.GridLength(200);
                    Photos.RowDefinitions.Add(rowDef1);
                    rowDef1 = new RowDefinition();
                }
            }
        }

        /// <summary>
        /// Dictionary for holding connection between image and button
        /// </summary>
        private Dictionary<System.Windows.Controls.Button, Uri> przyciski = new Dictionary<System.Windows.Controls.Button, Uri>();

        /// <summary>
        /// Fills main Grid with cells by 5 columns each row
        /// </summary>
        /// <param name="cells"></param>
        protected void fillGrid(int cells)
        {
            System.Windows.Controls.Button photoButton;
            BitmapImage bitimg;
            Image img;
            przyciski = new Dictionary<System.Windows.Controls.Button,Uri>();
            for (int i = 0; i < cells; i++)
            {
                bitimg = new BitmapImage();
                bitimg.BeginInit();
                bitimg.UriSource = new Uri((FindAllImages.Matches[i].Value.ToString()));
                //System.Windows.MessageBox.Show(bitimg.Height.ToString());
                bitimg.EndInit();

                img = new Image();
                img.Stretch = Stretch.Uniform;
                img.Source = bitimg;
                photoButton = new System.Windows.Controls.Button();

                // Set Button.Content
                photoButton.Content = img;

                photoButton.Click += photoButton_Click;
                // Set Button.Background
                photoButton.Background = System.Windows.Media.Brushes.White;/*new ImageBrush(bitimg)*/;
                //photoButton.Background = ne

                przyciski.Add(photoButton, bitimg.UriSource);

                Grid.SetColumn(photoButton, i%5);
                Grid.SetRow(photoButton, i/5);
                Photos.Children.Add(photoButton);
                
                
            }
        }

        /// <summary>
        /// Download async single clicked button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void photoButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button przycisk = (System.Windows.Controls.Button)sender;
            Uri uri = przyciski[przycisk];
            Microsoft.Win32.SaveFileDialog savefile = new Microsoft.Win32.SaveFileDialog();
            savefile.DefaultExt = Parser.checkFileExtension(uri);
            savefile.Filter = Parser.setFilter(savefile.DefaultExt);
            if(savefile.ShowDialog() == true)
            {
                BitmapImage webImage = new BitmapImage(uri);
                WebClient webClient = new WebClient();
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(CompletedOneImage);
                webClient.DownloadFileAsync(uri, savefile.FileName);
                //Modify filename attributes to save with original filename.extension
            }
            else
            {
                System.Windows.MessageBox.Show("Nie mogę zapisać pliku");
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
            InfoBoxLabel.AppendText("\nPlik został pobrany...\n");
            //throw new NotImplementedException();
        }

    }
}
