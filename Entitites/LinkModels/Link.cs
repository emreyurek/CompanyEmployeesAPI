using System;

namespace Entitites.LinkModels
{
    public class Link
    {
        public string? Href { get; set; }
        public string? Rel { get; set; }
        public string? Method { get; set; }

        public Link() // for xml serialization 
        {

        }

        public Link(string href, string rel, string method)
        {
            Href = href;
            Rel = rel;
            Method = method;
        }
    }
}
