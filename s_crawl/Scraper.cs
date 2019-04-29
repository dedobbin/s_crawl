namespace _crawl0
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using _crawl0.Domain;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Firefox;

    class Scraper : IDisposable
    {
        private string url = null;
        private IWebDriver browser = null;
        private Size windowSize;
        
        public void LoadPage(string url, bool headless = true)
        {
            this.url = url;
            FirefoxOptions options = new FirefoxOptions();
            if (headless)
            {
                options.AddArgument("--headless");
            }
            browser = new FirefoxDriver(options);
            browser.Navigate().GoToUrl(url);
            if (windowSize.Height == 0 && windowSize.Width == 0)
            {
                windowSize = browser.Manage().Window.Size;
            }
            else
            {
                browser.Manage().Window.Size = windowSize;
            }
        }

        public void Dispose()
        {
            if (browser != null)
            {
                browser.Quit();
                browser.Dispose();
            }
        }

        public Size WindowSize
        {
            get
            {
                return windowSize;
            }
            set
            {
                windowSize = value;
                browser.Manage().Window.Size = windowSize;
            }
        }

        public HashSet<Element> parseDomTree(IWebElement rootWebElement, Page page, int depth = 0)
        {
            // Because all child elements of an element should have foreign key to same page, 
            // but are not direct members of page object, we need to set associated page of every element.
            // This allows for a single node to be retrieved from table, and knowing what page it is from.
            // Not sure if this is ever useful, would be easier to only let root element know about page, but eh.
            string elementId = rootWebElement.GetAttribute("id");
            string selector = (string.Format("{0}{1}>*", rootWebElement.TagName, string.IsNullOrEmpty(elementId) ? "" : "#" + elementId));
            IReadOnlyCollection<IWebElement> childWebElements = rootWebElement.FindElements(By.CssSelector(selector));
            var childs = new HashSet<Element>();
            foreach (IWebElement childWebElement in childWebElements)
            {
                var child = new Element
                {
                    elementId = childWebElement.GetAttribute("id"),
                    tagName = childWebElement.TagName,
                    x = Convert.ToInt32(childWebElement.Location.X),
                    y = Convert.ToInt32(childWebElement.Location.Y),
                    w = Convert.ToInt32(childWebElement.Size.Width),
                    h = Convert.ToInt32(childWebElement.Size.Height),
                    depth = depth,
                    page = page,
                    elements = parseDomTree(childWebElement, page, depth + 1)
                };
                childs.Add(child);
            }
            return childs;
       }

        public void StorePage()
        {
            Screenshot screenshot = ((ITakesScreenshot)browser).GetScreenshot();
            //TODO: this is broken??? can't insert
            string b64Screenshot = screenshot.AsBase64EncodedString;
            IWebElement rootWebElement = browser.FindElement(By.TagName("body"));

            var page = new Page
            {
                url = url,
                w = windowSize.Width,
                h = windowSize.Height,
                screenshot = b64Screenshot,
            };

            //In code a page only has 1 element as direct member, 
            //but needs to be set as collection/one-to-many relationship because element has child elements
            //that needs to be associated to page.
            var root = new Element
            {
                elementId = rootWebElement.GetAttribute("id"),
                tagName = rootWebElement.TagName,
                x = Convert.ToInt32(rootWebElement.Location.X),
                y = Convert.ToInt32(rootWebElement.Location.Y),
                w = Convert.ToInt32(rootWebElement.Size.Width),
                h = Convert.ToInt32(rootWebElement.Size.Height),
                depth = 0,
                page = page,
                elements = parseDomTree(rootWebElement, page, 1)
            };
            var elements = new HashSet<Element>();
            elements.Add(root);
            page.elements = elements;

            using (var session = Database.OpenSession)
            using (var tx = session.BeginTransaction())
            {
                session.SaveOrUpdate(page);
                tx.Commit();
            }
        }

        /**
         * Looks for screenshots in database by url and saves them to disk. 
         */
        public bool ScreenshotToDisk(string folder, string url, int limit = 0)
        {
            return false;
        }

    }

}