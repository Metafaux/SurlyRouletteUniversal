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
using System.Diagnostics;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace SurlyRoulette.Views
{
    public sealed partial class Marker : UserControl
    {
        public int BetAmount { get; set; }
        public int[] BetArray { get; set; }
        public int Payout { get; set; }
        public string BetName { get; set; }
        public bool IsRotated { get; set; }
        //public string DebugMsg { get; set; }

        private bool _hasWon = false;

        public Marker(int payoutFactor, int[] newBet, int row, int col, int rowSpan, int colSpan, string mkrName, int initialBet)
        {
            this.InitializeComponent();
            //this.Loaded += Marker_Loaded;

            Payout = payoutFactor;
            BetArray = newBet;
            BetName = mkrName;
            BetAmount = initialBet;
            amountText.Text = BetAmount.ToString();
            Debug.WriteLine("This Marker amount = " + BetAmount.ToString());

            this.SetValue(Grid.RowProperty, row);
            this.SetValue(Grid.ColumnProperty, col);
            this.SetValue(Grid.RowSpanProperty, rowSpan);
            this.SetValue(Grid.ColumnSpanProperty, colSpan);
            this.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);
            this.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
        }

        //void Marker_Loaded(object sender, RoutedEventArgs e)
        //{
        //    ClearBet();
        //}

        public void AddToBet(int addAmt) {
            BetAmount += addAmt;
            if (BetAmount > 9995)
            {
                BetAmount = 9995;
            }
            amountText.Text = BetAmount.ToString();
        }

        public void ClearBet()
        {
            BetAmount = 0;
            amountText.Text = BetAmount.ToString();
        }

        public int GetResult(int wheelIndex)
        {
            int payoutAmount = 0;
            //bool hasWon = false;

            for (int i = 0; i < BetArray.Length; i++)
            {
                string mkrName = this.Name;
                if (BetArray[i] == wheelIndex)
                {
                    payoutAmount += BetAmount * (Payout+1);

                    _hasWon = true;
                    circleBottom.Fill = (Brush)this.Resources["WinIndicatorColor"];
                    circleTop.Fill = (Brush)this.Resources["WinIndicatorColor"];
                    amountText.Text = "+" + payoutAmount.ToString();
                }
            }
            
            return payoutAmount;
        }

        public void ClearWin()
        {
            if (_hasWon)
            {
                _hasWon = false;
                circleBottom.Fill = (Brush)this.Resources["MarkerNormalColor"];
                circleTop.Fill = (Brush)this.Resources["MarkerNormalColor"];
                amountText.Text = BetAmount.ToString();
            }
        }

        public void SetFocus(bool hasFocus)
        {
            if (hasFocus == true)
            {
                circleBottom.Fill = (Brush)this.Resources["FocusIndicatorColor"];
                circleTop.Fill = (Brush)this.Resources["FocusIndicatorColor"];
            }
            else
            {
                circleBottom.Fill = (Brush)this.Resources["MarkerNormalColor"];
                circleTop.Fill = (Brush)this.Resources["MarkerNormalColor"];
            }
        }

    }
}
