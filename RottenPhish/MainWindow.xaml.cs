using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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
using Path = System.IO.Path;

namespace RottenPhish
{
    using System.Net;

    using HtmlAgilityPack;

    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private Uri _uri;

        private void btnAnalyze_Click(object sender, RoutedEventArgs e)
        {
            if (!Uri.IsWellFormedUriString(tbURL.Text, UriKind.Absolute)) return;
            _uri = new Uri(tbURL.Text);
            var doc = new HtmlDocument();
            doc.Load(new WebClient().OpenRead(_uri));

            var forms = doc.DocumentNode.SelectNodes("//form").Select(p => new HtmlForm(p));
            lbForms.ItemsSource = forms;
        }

        private void btnSingle_Click(object sender, RoutedEventArgs e)
        {
            SinglePhish();
        }

        private async void SinglePhish()
        {
            var form = (HtmlForm)lbForms.SelectedItem;
            var fields = form.Inputs;
            Phish phish = new Phish(fields);
            WebClient wc = new WebClient();

            Uri actionUrl;
            if (!Uri.TryCreate(_uri, form.ActionUrl, out actionUrl)) return;


            await Task.Run(() =>
                         {
                             try
                             {
                                 wc.UploadValuesAsync(actionUrl, phish.NVC);
                             }
                             catch
                             {
                             }
                         });
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            for(int i=0; i < countSlider.Value; i++)
            {
                SinglePhish();
            }
        }
    }

    internal class Phish
    {
        private static readonly string[] EmailHosts = new[]
                                                  {
                                                      "gmx.de",
                                                      "gmx.net",
                                                      "web.de",
                                                      "hotmail.com",
                                                      "outlook.com",
                                                      "gmail.com",
                                                      "googlemail.com",
                                                      "yahoo.de",
                                                      "yahoo.com",
                                                      "about.com",
                                                      "facebook.com",
                                                  };

        private readonly IEnumerable<HtmlInput> _fields;

        public Phish(IEnumerable<HtmlInput> fields)
        {
            _fields = fields;
            LoadWordlists();
        }

        private static List<string> _words;
        private static Random _random;

        private void LoadWordlists()
        {
            if (_words != null) return;

            _words = new List<string>();
            var files = Directory.GetFiles(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Wordlists"));
            foreach (var file in files)
            {
                var lines = File.ReadAllLines(file).AsParallel().Where(p => Regex.IsMatch(p, @"^\w+$"));
                _words.AddRange(lines);
            }
        }


        public NameValueCollection NVC
        {
            get
            {
                var nvc = new NameValueCollection();
                if(_random == null) _random = new Random((int)DateTime.Now.Ticks);
                foreach (var field in _fields)
                {
                    var count = _random.Next(1, 3);
                    var numbered = _random.NextDouble() > 0.5D;
                    var number = _random.Next(1950, 2000);
                    var pwnumber = _random.Next(0, 10000000);
                    var pwnumbered = _random.NextDouble() > 0.5D;
                    var pwspecial = _random.NextDouble() > 0.5D;

                    var output = new StringBuilder();
                    var email = EmailHosts[_random.Next(0, EmailHosts.Count())];
                    if (!(pwspecial && field.Type == InputType.Password) || field.Type != InputType.Password)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var idx = _random.Next(_words.Count);
                            output.Append(_words[idx]);
                        }
                        if (numbered) output.Append(number);
                    }

                    if (field.Type == InputType.Password)
                    {
                        if (pwspecial) output.Append(RandomPassword.Generate(_random.Next(6, 16)));
                        else if (pwnumbered) output.Append(pwnumber);
                    }
                    else if (field.IsEmail) output.Append("@" + email);

                    switch (field.Type)
                    {
                        case InputType.Boolean:
                            nvc.Add(field.Name, field.Value ?? "on");
                            break;
                        case InputType.Password:
                        case InputType.Text:
                        case InputType.Hidden:
                        case InputType.Other:
                            nvc.Add(field.Name, output.ToString());
                            break;
                    }
                }

                return nvc;
            }
        }

    }
}
