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
        public override string ToString()
        {
            return "Action: " + this.Node.Attributes["action"].Value;
        }

        public IEnumerable<HtmlInput> Inputs
        {
            get
            {
                return Node.SelectNodes("//input").Select(p => new HtmlInput { Node = p });
            }
        }

    }
}
