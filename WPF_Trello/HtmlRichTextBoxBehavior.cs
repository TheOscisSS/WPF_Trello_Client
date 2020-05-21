using HTMLConverter;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;

namespace WPF_Trello
{
    public class HtmlRichTextBoxBehavior : DependencyObject
    {
        public static readonly DependencyProperty TextProperty =
          DependencyProperty.RegisterAttached("Text", typeof(string),
          typeof(HtmlRichTextBoxBehavior), new UIPropertyMetadata(null, OnValueChanged));

        public static string GetText(RichTextBox o) { return (string)o.GetValue(TextProperty); }

        public static void SetText(RichTextBox o, string value) { o.SetValue(TextProperty, value); }

        private static void OnValueChanged(DependencyObject dependencyObject,
          DependencyPropertyChangedEventArgs e)
        {
            try
            {
                var richTextBox = (RichTextBox)dependencyObject;
                var text = (e.NewValue ?? string.Empty).ToString();
                var xaml = HtmlToXamlConverter.ConvertHtmlToXaml(text, true);
                var flowDocument = XamlReader.Parse(xaml) as FlowDocument;
                HyperlinksSubscriptions(flowDocument);
                richTextBox.Document = flowDocument;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private static void HyperlinksSubscriptions(FlowDocument flowDocument)
        {
            try
            {
                if (flowDocument == null) return;
                    GetVisualChildren(flowDocument).OfType<Hyperlink>().ToList()
                        .ForEach(i => i.RequestNavigate += HyperlinkNavigate);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            
        }

        private static IEnumerable<DependencyObject> GetVisualChildren(DependencyObject root)
        {
            foreach (var child in LogicalTreeHelper.GetChildren(root).OfType<DependencyObject>())
            {
                yield return child;
                foreach (var descendants in GetVisualChildren(child)) yield return descendants;
            }
        }

        private static void HyperlinkNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            try
            {
                if (Uri.IsWellFormedUriString(e.Uri.ToString(), UriKind.Absolute))
                {
                    Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
                    e.Handled = true;
                }
                else
                {
                    e.Handled = true;
                    throw new Exception("Invalid URL");
                }
            }
            catch (Exception ex)
            {
                e.Handled = true;
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
