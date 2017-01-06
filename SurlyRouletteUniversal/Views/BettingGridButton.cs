using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace SurlyRoulette.Views
{
    public class BettingGridButton : Button
    {
        public bool IsChecked { get; set; }
        public int MarkerID { get; set; }
        //public Marker BetMarker { get; set; }

        public BettingGridButton()
        {
            this.Loaded += BettingGridButton_Loaded;
        }

        void BettingGridButton_Loaded(object sender, RoutedEventArgs e)
        {
            IsChecked = false;
        }
    }
}
