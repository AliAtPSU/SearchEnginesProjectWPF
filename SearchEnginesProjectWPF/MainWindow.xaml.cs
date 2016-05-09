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
        public MainWindow()
        {
            crawlThread.Start();
            timeToUpdate.Start();
            switchPageThread.Start();
            InitializeComponent();
        }

        static void switchPageList()
        {
            while (true)
            {
                if (timeToUpdate.ElapsedMilliseconds / 3600000 > 6)
                {
                    crawlThread.Abort();
                    Crawler.Clean();
                    crawlSite = new ThreadStart(Crawler.CrawlSite);
                    crawlThread = new Thread(crawlSite);
                    timeToUpdate.Restart();
                }
            }
        }

    }
}
