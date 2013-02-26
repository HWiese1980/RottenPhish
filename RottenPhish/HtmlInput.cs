using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace RottenPhish
{
    public enum InputType
    {
        Text,
        Password,
        Boolean,
        Hidden,
        Other
    }

    public class HtmlInput : RottenPhishHtmlBase, INotifyPropertyChanged
    {
        private bool _isEmail;

        public InputType Type
        {
            get
            {
                var v = Node.Attributes["type"].Value;
                switch ((v ?? "").ToLowerInvariant())
                {
                    case "password": return InputType.Password; break;
                    case "text": return InputType.Text;
                    case "checkbox": return InputType.Boolean;
                    case "hidden": return InputType.Hidden;
                    default: return InputType.Other;
                }
            }
        }

        public string Value { get { return Node.Attributes.Contains("value") ? Node.Attributes["value"].Value : null; } }
        public string Name { get { return Node.Attributes["name"].Value; } }

        public bool IsEmail
        {
            get { return _isEmail; }
            set
            {
                _isEmail = value;
                OnPropertyChanged();
            }
        }

        public override string ToString()
        {
            try
            {
                return "Field: " + Node.Attributes["name"].Value + "; Type: " + Type.ToString();
            }
            catch
            {
                return "No Name";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
