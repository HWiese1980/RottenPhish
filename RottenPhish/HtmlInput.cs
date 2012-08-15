using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RottenPhish
{
    public class HtmlInput : RottenPhishHtmlBase
    {
        public override string ToString()
        {
            try
            {
                return "Field: " + this.Node.Attributes["name"].Value;
            }
            catch
            {
                return "No Name";
            }
        }
    }
}
