using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace SvetlinAnkov.Albite.READER.BrowserEngine
{
    /// <summary>
    /// Represents a LayoutTemplate
    /// 
    /// The input is text data with placeholders of the kind: %{placeholder_name}
    /// 
    /// Once all placeholders have been set, call GetOutput() to get the
    /// processed content
    /// </summary>
    public class Template
    {
        private const string PlaceholderRegex = "%{([^}]*)}";
        private const string PlaceholderLeftSide = "%{";
        private const string PlaceholderRightSide = "}";

        private string template;
        private Dictionary<string, string> placeholders = new Dictionary<string,string>();

        protected Template() { }

        public Template(string template)
        {
            setTemplate(template);
        }

        protected void setTemplate(string template)
        {
            this.template = template;
            parsePlaceholders();
        }

        public string this[string placeholderName] {
            get
            {
                return placeholders[placeholderName];
            }

            set
            {
                if (!placeholders.ContainsKey(placeholderName))
                {
                    throw new ArgumentException("Placeholder not found in template");
                }

                placeholders[placeholderName] = value;
            }
        }

        /// <summary>
        /// Placeholder names
        /// </summary>
        public string[] Names
        {
            get
            {
                Dictionary<string, string>.KeyCollection keys = placeholders.Keys;
                string[] names = new string[keys.Count];
                int i = 0;
                foreach (string name in keys)
                {
                    names[i++] = name;
                }

                return names;
            }
        }

        /// <summary>
        /// Number of placeholder names
        /// </summary>
        public int Count
        {
            get
            {
                return placeholders.Count;
            }
        }

        private void parsePlaceholders()
        {
            placeholders.Clear();
            
            MatchCollection matches = Regex.Matches(template, PlaceholderRegex, RegexOptions.None);
            foreach (Match match in matches)
            {
                placeholders[match.Groups[1].Value] = null;
            }
        }

        /// <summary>
        /// Returns the content of the template with all placeholders replaced
        /// </summary>
        /// <throws>An InvalidOperationException if a placeholder hasn't been set</throws>
        /// <returns></returns>
        public string GetOutput()
        {
            StringBuilder buffer = new StringBuilder(template, (int) (template.Length * 1.2));

            foreach (KeyValuePair<string, string> placeholder in placeholders)
            {
                if (placeholder.Value == null)
                {
                    throw new InvalidOperationException("Placeholder " + placeholder.Key + " has not been set");
                }

                buffer.Replace(PlaceholderLeftSide + placeholder.Key + PlaceholderRightSide, placeholder.Value);
            }

            return buffer.ToString();
        }
    }
}
