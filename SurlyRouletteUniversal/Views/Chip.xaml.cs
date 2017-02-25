using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

namespace SurlyRouletteUniversal.Views
{
    public sealed partial class Chip : UserControl
    {
        public int Denomination { get; set; }
        public string TextContent { get; set; }

        private SolidColorBrush fiveRed = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 169, 6, 65));
        private SolidColorBrush tenBlue = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 102, 137));
        private SolidColorBrush twentyfiveGreen = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 32, 145, 0));
        private SolidColorBrush hundredBlack = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 69, 69, 69));

        private SolidColorBrush chipCodeColor = new SolidColorBrush();
        private int chipAmount;

        public Chip()
        {
            this.InitializeComponent();
            this.Loaded += Chip_Loaded;
            //DefaultStyleKey = typeof(Chip);

        }

        void Chip_Loaded(object sender, RoutedEventArgs e)
        {
            chipAmount = Convert.ToInt16(this.TextContent);

            if (chipAmount == 10)
            {
                chipCodeColor.Color = tenBlue.Color;
            }
            else if (chipAmount == 25)
            {
                chipCodeColor.Color = twentyfiveGreen.Color;
            }
            else if (chipAmount == 100)
            {
                chipCodeColor.Color = hundredBlack.Color;
            }
            else
            {
                chipCodeColor.Color = fiveRed.Color;
            }

            // Find resource from XAML?
            // Or register dependency property?
            //chipCodeColor.Color = twentyfiveGreen.Color;
            SetChipColor();
            amountText.Text = this.TextContent;
        }

        private void SetChipColor()
        {
            colorCircleBottom.Fill = chipCodeColor;
            colorCircleTop.Fill = chipCodeColor;
        }

        //public static readonly DependencyProperty ChipColorProperty =
        //    DependencyProperty.Register("ChipColor",
        //    typeof(SolidColorBrush), typeof(Chip),
        //    new PropertyMetadata(new SolidColorBrush());

        //private static void ChipColorChanged(DependencyObject d, 
        //    DependencyPropertyChangedEventArgs e)
        //{
        //    (d as Chip).
    }
}
