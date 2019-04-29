using System.Collections.Generic;

namespace _crawl0.Domain
{
    public class Page
    {
        public virtual int id { get; set; }
        public virtual string url { get; set; }
        public virtual int w { get; set; }
        public virtual int h { get; set; }
        public virtual string screenshot { get; set; }
        public virtual ISet<Element> elements { get; set; } 
    }
}