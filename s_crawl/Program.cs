using System;
using System.Drawing;

namespace _crawl0
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = @"http://localhost/some_site";
            using (Scraper scraper = new Scraper())
            {
                scraper.LoadPage(url);
                scraper.WindowSize = new Size(1280, 960);
                scraper.StorePage();
                scraper.WindowSize = new Size(20, 20);
                scraper.StorePage();
                string screenshotPath = @"C:\junk";
                scraper.ScreenshotToDisk(screenshotPath, url);
            }
            Console.WriteLine("teh end");
        }
    }
}
