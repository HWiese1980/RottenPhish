using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RottenPhish
{
    using System.Collections;

    using HtmlAgilityPack;

    public class HtmlForm : RottenPhishHtmlBase
    {
        private readonly HtmlInput[] _htmlInputs;

        public override string ToString()
        {
            return "Action: " + this.Node.Attributes["action"].Value;
        }

        public HtmlForm(HtmlNode n)
        {
            Node = n;
            ActionUrl = n.Attributes["action"].Value;
            _htmlInputs = Node.SelectNodes("//input").Select(p => new HtmlInput { Node = p }).ToArray();
        }

        public string ActionUrl { get; set; }


        public IEnumerable<HtmlInput> Inputs
        {
            get
            {
                return _htmlInputs;
            }
        }

    }
}
