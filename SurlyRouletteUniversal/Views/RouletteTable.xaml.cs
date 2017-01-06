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

/* ROULETTE TABLE USERCONTROL
 * UserControl betting table for Roulette game
 * Takes user tap input, reveals tap wheel control
 * Exposes properties of Bet IDs and payouts
 * 
 * Author Alexander D. Wilson
 * Copyright 2012 Surly Industries LLC
 */

namespace SurlyRoulette.Views
{   
    public sealed partial class RouletteTable : UserControl
    {
        public static readonly DependencyProperty CurrentOddsProperty = DependencyProperty.Register(
            "CurrentOdds", typeof(string), typeof(RouletteTable), new PropertyMetadata("", OnCurrentOddsPropertyChange)
        );

        public string CurrentOdds
        {
            get { return (string)GetValue(CurrentOddsProperty); }
            set { SetValue(CurrentOddsProperty, value); }
        }

        private static void OnCurrentOddsPropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Debug.WriteLine("Property changed: {0}", e.NewValue);
        }

        //public int TotalBet { get; set; }
        public int BetSum { get; set; }
        
        public int Payout { get; set; }
        public string PayoutString { get; set; }

        public int RowIndex { get; set; }
        public int ColumnIndex { get; set; }

        private int LastBetAmount { get; set; }
        
        private BettingGridButton CurrentButton;
        public Marker CurrentMarker;

        public List<Marker> MarkerList
        {
            get
            {
                return markerList;
            }
        }

        private List<Marker> markerList = new List<Marker>();
        private int MarkerVisualIndex;

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
 
        // CONSTRUCTOR
        public RouletteTable()
        {
            this.InitializeComponent();
            this.Loaded += RouletteTable_Loaded;
        }

        void RouletteTable_Loaded(object sender, RoutedEventArgs e)
        {
            BetSum = 0;
            //BetDrawer.Visibility = Visibility.Collapsed;
            MarkerVisualIndex = tableGrid.Children.IndexOf(FirstBetBtn);
            TableDebugOutput.Text = "MARKER INDEX =" + MarkerVisualIndex.ToString();
        }

        private void StraightBet_Click(object sender, RoutedEventArgs e)
        {
            // Straight bet (single number)
            TranslateCoordinates(sender, e);

            int betPayout = Models.Game.straight;
            string betName;

            int rowPos = RowIndex - 1;
            int colPos = ColumnIndex - 1;

            int betIndex = ColumnIndex;
            if (RowIndex == (int)this.Resources["NumberBtnRow2"])
            {
                betIndex = ColumnIndex - 1;
            }
            else if (RowIndex == (int)this.Resources["NumberBtnRow1"])
            {
                betIndex = ColumnIndex - 2;
            }
            int[] betArray = new int[] {betIndex};

            betName = "Straight " + betIndex.ToString();
            AddMarker(betPayout, betArray, rowPos, colPos, 4, 3, betName);
        }

        private void StreetBet_Click(object sender, RoutedEventArgs e)
        {
            // Street bet
            TranslateCoordinates(sender, e);
            int betPayout = Models.Game.street;

            int rowPos = RowIndex;
            int colPos = ColumnIndex - 1;
            int rowSpan = 3;
            int colSpan = 3;
            if (RowIndex == (int)this.Resources["StreetRowInside"])
            {
                rowPos = 9;
                rowSpan = 6;
            }

            int[] betArray = new int[] { ColumnIndex - 2, ColumnIndex - 1, ColumnIndex };
            string betName = "Street " + betArray[0].ToString();
            AddMarker(betPayout, betArray, rowPos, colPos, rowSpan, colSpan, betName);
        }

        private void SplitBet_Click(object sender, RoutedEventArgs e)
        {
            // Split bet
            TranslateCoordinates(sender, e);
            int betPayout = Models.Game.split;
            int colSpan = 4;

            int[] betArray = new int[] { ColumnIndex - 3, ColumnIndex };
            if (RowIndex == (int)this.Resources["NumberBtnRow2"])
            {
                betArray = new int[] { ColumnIndex - 2, ColumnIndex + 1 };
            }
            else if (RowIndex == (int)this.Resources["NumberBtnRow3"])
            {
                betArray = new int[] { ColumnIndex - 1, ColumnIndex + 2 };
            }
            else if (RowIndex == (int)this.Resources["SplitBtnRow12"])
            {
                betArray = new int[] { ColumnIndex, ColumnIndex + 1 };
                colSpan = 3;
            }
            else if (RowIndex == (int)this.Resources["SplitBtnRow23"])
            {
                betArray = new int[] {ColumnIndex+1, ColumnIndex+2};
                colSpan = 3;
            }

            int rowPos = RowIndex - 1;
            int colPos = ColumnIndex - 1;
            string betName = "Split " + betArray[0].ToString() +"/"+ betArray[1].ToString();
            AddMarker(betPayout, betArray, rowPos, colPos, 4, colSpan, betName);
        }

        private void DoubleStreet_Click(object sender, RoutedEventArgs e)
        {
            // Double street bet
            TranslateCoordinates(sender, e);
            int betPayout = Models.Game.doubleStreet;

            int rowPos = RowIndex;
            int colPos = ColumnIndex - 1;
            int rowSpan = 3;
            int colSpan = 4;
            if (RowIndex == (int)this.Resources["StreetRowInside"])
            {
                rowPos = 9;
                rowSpan = 6;
            }

            int[] betArray = new int[] { ColumnIndex - 3, ColumnIndex - 2, ColumnIndex - 1, ColumnIndex, ColumnIndex + 1, ColumnIndex + 2 };
            string betName = "Double Street " + betArray[0].ToString() +"/"+ betArray[3].ToString();
            AddMarker(betPayout, betArray, rowPos, colPos, rowSpan, colSpan, betName);
        }

        private void SquareBet_Click(object sender, RoutedEventArgs e)
        {
            // Square bet (4 numbers)
            TranslateCoordinates(sender, e);
            int betPayout = Models.Game.square;

            int[] betArray = new int[] { ColumnIndex - 3, ColumnIndex - 2, ColumnIndex, ColumnIndex + 1 };
            if (RowIndex == (int)this.Resources["SplitBtnRow23"])
            {
                betArray = new int[] { ColumnIndex - 2, ColumnIndex - 1, ColumnIndex+1, ColumnIndex + 2 };
            }

            int rowPos = RowIndex - 1;
            int colPos = ColumnIndex - 1;
            string betName = "Square" + betArray[0].ToString() +"/"+betArray[1].ToString() +"/"+betArray[2].ToString() +"/"+betArray[3].ToString();
            AddMarker(betPayout, betArray, rowPos, colPos, 4, 4, betName);
        }

        private void TranslateCoordinates(object sender, RoutedEventArgs e)
        {
            //BetDrawer.Visibility = Visibility.Visible;
            
            //CurrentButton = e.OriginalSource as ToggleButton;
            CurrentButton = (BettingGridButton)sender;
            int br = Grid.GetRow(CurrentButton);
            int bc = Grid.GetColumn(CurrentButton);

            RowIndex = br;
            ColumnIndex = bc;
        }

        public void AddMarker(int pay, int[] bet, int mkrRow, int mkrCol, int mkrRowSpan, int mkrColSpan, string betName)
        {
            string mkrId = "Marker" + mkrRow.ToString() + mkrCol.ToString();
            this.CurrentOdds = pay.ToString() + " to 1";
            
            if (CurrentMarker != null)
            {
                CurrentMarker.SetFocus(false);
                RemoveZeroMarker();
            }

            if (CurrentButton.IsChecked == true)
            {
                CurrentMarker = (Marker)this.FindName(mkrId);
                CurrentMarker.Visibility = Visibility.Visible;
            }
            else
            {
                Marker newMarker = new Marker(pay, bet, mkrRow, mkrCol, mkrRowSpan, mkrColSpan, betName, LastBetAmount);
                newMarker.SetValue(NameProperty, mkrId);

                // UNHANDLED EXCEPTION ON STREET BET AFTER RESET
                BetSum += newMarker.BetAmount;
                //Debug.WriteLine("LAST BET AMOUNT = " + LastBetAmount.ToString());
                //Debug.WriteLine("New Marker amount = " + newMarker.BetAmount);
                tableGrid.Children.Insert(MarkerVisualIndex, newMarker);
                CurrentMarker = newMarker;
                markerList.Add(newMarker);
                CurrentButton.IsChecked = true;
                
                
            }

            if (CurrentMarker != null)
            {
                CurrentMarker.SetFocus(true);
            }
        }

        public void BetChip_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentMarker != null)
            {
                // GET DENOMINATION FROM TARGET'S CONTENT
                Button b = e.OriginalSource as Button;
                string textContent = b.Content.ToString();
                int chipValue = Convert.ToInt16(textContent);

                BetSum += chipValue;
                CurrentMarker.AddToBet(chipValue);
                LastBetAmount = CurrentMarker.BetAmount;
            }
        }

        public void ClearBet()
        {
            if (CurrentMarker != null)
            {
                BetSum -= CurrentMarker.BetAmount;
                CurrentMarker.ClearBet();
            }
        }

        private void RemoveZeroMarker()
        {
            if (CurrentMarker.BetAmount <= 0)
            {
                CurrentMarker.Visibility = Visibility.Collapsed;
            }
        }

        public void ResetBet()
        {
            for (int i = 0; i < MarkerList.Count; i++)
            {
                MarkerList[i].ClearBet();
                MarkerList[i].Visibility = Visibility.Collapsed;
            }
            BetSum = 0;
            CurrentMarker = null;
        }

        private void ZeroStraight_Click(object sender, RoutedEventArgs e)
        {
            TranslateCoordinates(sender, e);
            int betPayout = Models.Game.straight;

            int rowPos = RowIndex;
            int colPos = ColumnIndex;

            string betName = "Straight 00";
            int betIndex = 37;
            if (RowIndex == 7)
            {
                betIndex = 0;
                betName = "Straight 0";
            }
            int[] betArray = new int[] { betIndex };

            AddMarker(betPayout, betArray, rowPos, colPos, 6, 2, betName);
        }

        private void ColumnBet_Click(object sender, RoutedEventArgs e)
        {
            TranslateCoordinates(sender, e);
            int betPayout = Models.Game.dozen;
            int rowPos = RowIndex;
            int colPos = ColumnIndex;

            int startIndex = 1;
            if (RowIndex == 5)
            {
                startIndex = 2;
            }
            else if (RowIndex == 1)
            {
                startIndex = 3;
            }

            int[] betArray = new int[12];
            for (int i = 0; i < 12; i++)
            {
                betArray[i] = startIndex + (i * 3);
                //Debug.WriteLine("BET ARRAY ITEM" + i.ToString() + " INDEX = " + betArray[i].ToString());
            }
            string betName = "Column " + betArray[0].ToString();
            AddMarker(betPayout, betArray, rowPos, colPos, 4, 2, betName);
        }

        private void DozenBet_Click(object sender, RoutedEventArgs e)
        {
            TranslateCoordinates(sender, e);
            int betPayout = Models.Game.dozen;
            int rowPos = RowIndex;
            int colPos = ColumnIndex;

            int startIndex = 1;
            if (ColumnIndex == 14)
            {
                startIndex = 13;
            }
            else if (ColumnIndex == 26)
            {
                startIndex = 25;
            }

            int[] betArray = new int[12];
            for (int i = 0; i < 12; i++)
            {
                betArray[i] = startIndex + i;
                //Debug.WriteLine("BET ARRAY ITEM " + i.ToString() + " INDEX = " + betArray[i].ToString());
            }
            string betName = "Dozen " + betArray[0].ToString() +"-"+ betArray[11].ToString();
            AddMarker(betPayout, betArray, rowPos, colPos, 1, 12, betName);
        }

        private void HalfBet_Click(object sender, RoutedEventArgs e)
        {
            TranslateCoordinates(sender, e);
            int betPayout = 1;
            int rowPos = RowIndex;
            int colPos = ColumnIndex;

            // Column Positions:
            // 1 to 18: 2
            // 19 to 36: 32
            // Even: 8
            // Odd: 26
            // Red: 14
            // Black: 20

            int[] betArray = new int[18];
            string betName = string.Empty;
            if (colPos == 2)
            {
                // 1 to 18
                for (int i = 0; i < 18; i++)
                {
                    betArray[i] = i+1;
                    //Debug.WriteLine("BET ARRAY ITEM " + i.ToString() + " INDEX = " + betArray[i].ToString());
                }
                betName = "1 to 18";
            }
            else if (colPos == 32)
            {
                // 19 to 36
                for (int i = 0; i < 18; i++)
                {
                    betArray[i] = i + 19;
                    //Debug.WriteLine("BET ARRAY ITEM " + i.ToString() + " INDEX = " + betArray[i].ToString());
                }
                betName = "19 to 36";
            }
            else if (colPos == 8)
            {
                // Even
                for (int i = 0; i < 18; i++)
                {
                    betArray[i] = (i + 1) * 2;
                    //Debug.WriteLine("BET ARRAY ITEM " + i.ToString() + " INDEX = " + betArray[i].ToString());
                }
                betName = "Even";
            }
            else if (colPos == 26)
            {
                // Odd
                for (int i = 0; i < 18; i++)
                {
                    betArray[i] = 1 + (i * 2);
                    //Debug.WriteLine("BET ARRAY ITEM " + i.ToString() + " INDEX = " + betArray[i].ToString());
                }
                betName = "Odd";
            }
            else if (colPos == 14)
            {
                // 0=black, 1=red
                int ovalIndex = 0;

                //Red
                for (int i = 0; i < 36; i++)
                {
                    if (colorKey[i] == 1)
                    {
                        betArray[ovalIndex] = i+1;
                        //Debug.WriteLine("BET ARRAY ITEM " + ovalIndex.ToString() + " INDEX = " + betArray[ovalIndex].ToString());
                        ovalIndex++;
                    }
                }
                betName = "Red";
            }
            else if (colPos == 20)
            {
                int ovalIndex = 0;
                // Black
                for (int i = 0; i < 36; i++)
                {
                    if (colorKey[i] == 0)
                    {
                        betArray[ovalIndex] = i + 1;
                        //Debug.WriteLine("BET ARRAY ITEM " + ovalIndex.ToString() + " INDEX = " + betArray[ovalIndex].ToString());
                        ovalIndex++;
                    }
                }
                betName = "Black";
            }

            AddMarker(betPayout, betArray, rowPos, colPos, 2, 6, betName);
        }
    }
}
