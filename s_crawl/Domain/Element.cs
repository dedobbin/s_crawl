using OpenQA.Selenium;
using System;
using System.Collections.Generic;

namespace _crawl0.Domain
{
    public class Element
    {
        public virtual int id { get; set; }
        public virtual string elementId { get; set; }
        public virtual string tagName { get; set; }
        public virtual int x { get; set; }
        public virtual int y { get; set; }
        public virtual int w { get; set; }
        public virtual int h { get; set; }
        public virtual int depth { get; set; }
        public virtual Page page { get; set; }
        public virtual ISet<Element> elements { get; set; }
    }
}