using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
using CSharpCrawler;
using VectorSpaceModel.Components;
namespace SearchEnginesProjectWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static Stopwatch timeToUpdate = new Stopwatch();
        static ThreadStart crawlSite = new ThreadStart(Crawler.CrawlSite);
        static Thread crawlThread = new Thread(crawlSite);
        static ThreadStart switchPage = new ThreadStart(switchPageList);
        static Thread switchPageThread = new Thread(switchPage);
        static Corpus corpus;
        List<KeyValuePair<Document, double>> documentsAndDistances = new List<KeyValuePair<Document, double>>();
        static int pages = 0;
        public MainWindow()
        {
            // crawlThread.Start();
            //timeToUpdate.Start();
            //switchPageThread.Start();
            InitializeComponent();
            Run test = new Run("test");
            Hyperlink testH = new Hyperlink(test);
            testH.NavigateUri = new System.Uri("http://psu.edu.sa");
            testH.Click += new RoutedEventHandler(DynamicClick);
            ResultsLabel.Content = testH;

        }
        public void DynamicClick(object sender, RoutedEventArgs e)
        {
            Hyperlink test = (Hyperlink)sender;
            //Console.WriteLine(test.NavigateUri.AbsolutePath);
            System.Diagnostics.Process.Start(test.NavigateUri.AbsoluteUri);
        }
        static void switchPageList()
        {
            while (true)
            {
                if (timeToUpdate.ElapsedMilliseconds / 3600000 > 6)
                {
                    corpus = new Corpus(Crawler._pages);
                    crawlThread.Abort();
                    Crawler.Clean();
                    crawlSite = new ThreadStart(Crawler.CrawlSite);
                    crawlThread = new Thread(crawlSite);
                    timeToUpdate.Restart();
                }
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            Document query = new Document(queryBox.Text.Split(' '));

            foreach (Document document in corpus.Documents)
            {
                double distance = document.AugumentedSimilarity(query, corpus.Documents);
                KeyValuePair<Document, double> newKeyValuePair = new KeyValuePair<Document, double>(document, distance);
                documentsAndDistances.Add(newKeyValuePair);
            }
            documentsAndDistances.Sort((x, y) => x.Value.CompareTo(y.Value));
            documentsAndDistances = documentsAndDistances.Take(50).ToList();
            pages = (int)Math.Ceiling(documentsAndDistances.Count / 10.0);
            pageList.Items.Clear();
            for (int i = 1; i <= pages; i++)
            {
                pageList.Items.Add(i);
            }
            displayText(1);
        }

        private void displayText(int page)
        {
            for (int i = (page - 1) * 10; i < page * 10; i++)
            {
                /*   Run link = new Run()
                   Hyperlink hl = new Hyperlink("hello");
                   ResultsLabel.Content = hl;*/
            }
        }

        private void pageList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            displayText((int)pageList.SelectedItem);
        }
    }
}
