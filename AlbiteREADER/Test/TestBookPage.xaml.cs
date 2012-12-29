using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.IO.IsolatedStorage;
using System.IO;
using System.Diagnostics;
using System.Windows.Resources;
using SvetlinAnkov.AlbiteREADER.Layout;
using SvetlinAnkov.AlbiteREADER.Utils;

namespace SvetlinAnkov.AlbiteREADER.Test
{
    public partial class TestBookPage : PhoneApplicationPage
    {
        public TestBookPage()
        {
            InitializeComponent();

            LayoutTemplateResource contentCss = new LayoutTemplateResource("Layout/Content.css");
            LayoutTemplateResource mainXhtml  = new LayoutTemplateResource("Layout/Main.xhtml");
            LayoutTemplateResource stylesCss  = new LayoutTemplateResource("Layout/Styles.css");
            LayoutTemplateResource themeCss   = new LayoutTemplateResource("Layout/Theme.css");

            contentCss["line_height"]   = "140"; //In %
            contentCss["font_size"]     = "28";  //In px
            contentCss["font_family"]   = "Georgia";
            contentCss["text_align"]    = "justify";
            listTemplateNames(contentCss);
            contentCss.SaveToStorage();

            mainXhtml["full_page_width"]= "480"; //In px
            mainXhtml["chapter_title"]  = "Chapter I: Down The Rabbit Hole";
            mainXhtml["chapter_file"]   = "/Test/Book/chapter01.xhtml";
            listTemplateNames(mainXhtml);
            mainXhtml.SaveToStorage();
            
            int fullPageWidth   = 480;
            int fullPageHeight  = 800 - 72; // Leaving place for the application bar?

            int pageMarginTop       = 30;
            int pageMarginBottom    = 30;
            int pageMarginLeft      = 30;
            int pageMarginRight     = 40;

            int pageWidth = fullPageWidth - (pageMarginLeft + pageMarginRight);
            int pageHeight = fullPageHeight - (pageMarginTop + pageMarginBottom);

            stylesCss["page_width_x_3"] = (fullPageWidth * 3).ToString();
            stylesCss["page_margin_top"] = pageMarginTop.ToString();
            stylesCss["page_margin_bottom"] = pageMarginBottom.ToString();
            stylesCss["page_margin_left"] = pageMarginLeft.ToString();
            stylesCss["page_margin_right"] = pageMarginRight.ToString();
            stylesCss["page_width"] = pageWidth.ToString();
            stylesCss["page_height"] = pageHeight.ToString();
            listTemplateNames(stylesCss);
            stylesCss.SaveToStorage();

            themeCss["background_color"] = "white";
            themeCss["text_color"] = "black";
            themeCss["accent_color"] = "#634F3B";
            listTemplateNames(themeCss);
            themeCss.SaveToStorage();

            const string jsEnginePath = "Layout/Albite.js";
            using (AlbiteIsolatedStorage iso = new AlbiteIsolatedStorage(jsEnginePath))
            {
                using (AlbiteResourceStorage res = new AlbiteResourceStorage(jsEnginePath))
                {
                    res.CopyTo(iso);
                }
            }
        }

        private void listTemplateNames(LayoutTemplate t)
        {
            Debug.WriteLine("Number of placeholder names: {0}", t.Count);
            foreach (string name in t.Names)
            {
                Debug.WriteLine("Name: {0} = {1}", name, t[name]);
            }
        }
    }

    public class TestBook : AlbiteREADER.Model.Book
    {
        public override string IsoLocation
        {
            get
            {
                return TestConstants.IsoLocation + "Book/";
            }
        }
    }
}