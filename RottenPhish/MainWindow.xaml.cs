using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        private void btnAnalyze_Click(object sender, RoutedEventArgs e)
        {
            if (!Uri.IsWellFormedUriString(tbURL.Text, UriKind.Absolute)) return;
            var url = new Uri(tbURL.Text);
            var doc = new HtmlDocument();
            doc.Load(new WebClient().OpenRead(url));

            var forms = doc.DocumentNode.SelectNodes("//form").Select(p => new HtmlForm { Node = p });
            lbForms.ItemsSource = forms;
        }
    }
}
