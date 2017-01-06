using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace SurlyRoulette.Views
{
    public sealed partial class NumberOval : UserControl
    {
        public string color { get; set; }
        public int index { get; set; }
        public string Content { get; set; }
         
        private SolidColorBrush ovalBrush;
        //private SolidColorBrush gamblingRed = 
            //new SolidColorBrush(Windows.UI.Color.FromArgb(255, 193, 39, 45));
        private int[] colorKey = new int[]{
            1, 0, 1,
            0, 1, 0,
            1, 0, 1,
            0, 0, 1,

            0, 1, 0,
            1, 0, 1,
            1, 0, 1,
            0, 1, 0,

            1, 0, 1,
            0, 0, 1,
            0, 1, 0,
            1, 0, 1
        };

        private string[] colorString = { "black", "red" };
        
        public NumberOval()
        {
            
            this.InitializeComponent();

            this.Loaded += NumberOval_Loaded;
        }

        void NumberOval_Loaded(object sender, RoutedEventArgs e)
        {   
            index = Convert.ToInt16(this.Content);
            int row = Convert.ToInt16(this.GetValue(Grid.RowProperty));
            
            // Set custom margin to adjust for centering along white borders
            if (row == 1)
            {
                OvalWrapper.SetValue(Grid.MarginProperty, new Thickness(0,2,0,0));
            }
            else if (row == 9)
            {
                OvalWrapper.SetValue(Grid.MarginProperty, new Thickness(0,0,0,2));
            }

            numeralText.Text = this.Content;
            int colorKeyIndex = colorKey[index - 1];
            color = colorString[colorKeyIndex];
            if (color == "red")
            {
                ovalBrush = (SolidColorBrush)Application.Current.Resources["FiveChipRed"];
                    //gamblingRed;
            }
            else
            {
                ovalBrush = new SolidColorBrush(Windows.UI.Colors.Black);
            }

            oval.Fill = ovalBrush;
            //oval.Fill = gamblingRed;
                //(SolidColorBrush)this.Resources["GamblingRed"];
        }
    }
}
