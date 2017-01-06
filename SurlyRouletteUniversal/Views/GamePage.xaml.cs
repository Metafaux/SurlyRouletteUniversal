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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Popups;
using SurlyRoulette.Models;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace SurlyRoulette.Views
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class GamePage : SurlyRoulette.Common.LayoutAwarePage
    {
        public static readonly DependencyProperty TotalBetProperty = DependencyProperty.Register("TotalBet", typeof(int), typeof(RouletteTable), null);
        public int TotalBet
        {
            get
            {
                return (int)GetValue(TotalBetProperty);
            }
            set
            {
                base.SetValue(TotalBetProperty, value);
            }
        }
        
        private readonly Game _currentGame = new Game();
        private const double numberSlotDegrees = 360 / 38;
        private const double slotQty = 38;
        private Random spinGen = new Random();
        private int TotalPayout;
        private int NewWalletAmount;

        public GamePage()
        {
            this.InitializeComponent();

            this.Loaded += GamePage_Loaded;
            this.ApplicationViewStates.CurrentStateChanged += ApplicationViewStates_CurrentStateChanged;
            tableControl.Tapped += tableControl_Tapped;
        }

        void ApplicationViewStates_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            int markerListLength = tableControl.MarkerList.Count();
            if (this.ApplicationViewStates.CurrentState == FullScreenPortrait)
            {
                // rotate markers
                // rotate number ovals
                
                for (int i = 0; i < markerListLength; i++)
                {
                    tableControl.MarkerList[i].IsRotated = true;
                }
            }
            else
            {
                for (int i = 0; i < markerListLength; i++)
                {
                    tableControl.MarkerList[i].IsRotated = false;
                }
            }
        }

        void GamePage_Loaded(object sender, RoutedEventArgs e)
        {
            // somehow, bet gets set to zero
            _currentGame.PlaceBet();

            WalletAmount.Text = _currentGame.Wallet.ToString();
            ReplenishButton.Visibility = Visibility.Collapsed;

            TotalBet = 0;
        }

        void tableControl_Tapped(object sender, TappedRoutedEventArgs e)
        {
            UpdateScores();
        }

        void UpdateScores()
        {
            TotalBetAmount.Text = tableControl.BetSum.ToString();
            PayoutAmount.Text = TotalPayout.ToString();
            NewWalletAmount = _currentGame.Wallet - tableControl.BetSum;
            WalletAmount.Text = NewWalletAmount.ToString();

            if (NewWalletAmount < 0)
            {
                ReplenishButton.Visibility = Visibility.Visible;
            }
            else
            {
                ReplenishButton.Visibility = Visibility.Collapsed;
            }
            //BetPayoutText.Text = tableControl.CurrentOdds;
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            //if (pageState != null && pageState.ContainsKey("walletAmountText"))
            //{
            //    WalletAmount.Text = pageState["walletAmountText"].ToString();
            //}

            Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            if (roamingSettings.Values.ContainsKey("userWalletAmount"))
            {
                _currentGame.Wallet = Convert.ToInt16( roamingSettings.Values["userWalletAmount"]);
                WalletAmount.Text = roamingSettings.Values["userWalletAmount"].ToString();
            }
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            //pageState["walletAmountText"] = _currentGame.Wallet.ToString();
        }

        public void ShowRowAndColumn()
        {
            rowText.Text = tableControl.RowIndex.ToString();
            colText.Text = tableControl.ColumnIndex.ToString();
            betText.Text = _currentGame.BetIndex.ToString();
        }

        private void ButtonSpin_Click(object sender, RoutedEventArgs e)
        {
            SpinRouletteWheel();
        }

        private void SpinRouletteWheel()
        {
            TotalPayout = 0;
            double rotationTime = 1800; // milliseconds per degree of rotation
            for (int i = 0; i < tableControl.MarkerList.Count; i++)
            {
                tableControl.MarkerList[i].ClearWin();
            }
            UpdateScores();

            rouletteBall.SetValue(Canvas.TopProperty, 102);

            Storyboard spinStoryboard = new Storyboard();
            Storyboard centerStoryboard = new Storyboard();
            Storyboard ballSpinStoryboard = new Storyboard();
            Storyboard ballBounceStoryboard = new Storyboard();
            DoubleAnimation spinAnimation = new DoubleAnimation();
            DoubleAnimation centerAnimation = new DoubleAnimation();
            DoubleAnimation ballSpinAnimation = new DoubleAnimation();
            DoubleAnimation ballBounceAnimation = new DoubleAnimation();
              
            // Generate a random number
            int randomGen = spinGen.Next(37);
            double spinRandom = (randomGen / slotQty) * 360;
            double spinAmount = spinRandom + 360;

            ballBounceAnimation.From = 102;
            ballBounceAnimation.To = 307;
            ballBounceAnimation.BeginTime = TimeSpan.FromMilliseconds(rotationTime / 2);
            ballBounceAnimation.Duration = TimeSpan.FromMilliseconds(rotationTime/2);
            BounceEase bounceDef = new BounceEase();
            bounceDef.Bounces = 2;
            bounceDef.Bounciness = 2;
            ballBounceAnimation.EasingFunction = bounceDef;
            ballBounceAnimation.EasingFunction.EasingMode = EasingMode.EaseOut;
            Storyboard.SetTargetProperty(ballBounceAnimation, "(Canvas.Top)");
            Storyboard.SetTarget(ballBounceStoryboard, rouletteBall);
            
            spinAnimation.By = spinAmount;
            spinAnimation.Duration = TimeSpan.FromMilliseconds(rotationTime);
            spinAnimation.EasingFunction = new QuadraticEase();
            spinAnimation.EasingFunction.EasingMode = EasingMode.EaseOut;
            Storyboard.SetTargetProperty(spinAnimation, "(UIElement.RenderTransform).(CompositeTransform.Rotation)");
            Storyboard.SetTarget(spinStoryboard, numberwang);

            centerAnimation.By = spinAmount;
            centerAnimation.Duration = TimeSpan.FromMilliseconds(rotationTime);
            centerAnimation.EasingFunction = new QuadraticEase();
            centerAnimation.EasingFunction.EasingMode = EasingMode.EaseOut;
            Storyboard.SetTargetProperty(centerAnimation, "(UIElement.RenderTransform).(CompositeTransform.Rotation)");
            Storyboard.SetTarget(centerStoryboard, wheelCenter);

            ballSpinAnimation.From = 0;
            ballSpinAnimation.To = -720;
            ballSpinAnimation.Duration = TimeSpan.FromMilliseconds(rotationTime);
            ballSpinAnimation.EasingFunction = new QuadraticEase();
            ballSpinAnimation.EasingFunction.EasingMode = EasingMode.EaseOut;
            Storyboard.SetTargetProperty(ballSpinAnimation, "(UIElement.RenderTransform).(CompositeTransform.Rotation)");
            Storyboard.SetTarget(ballSpinStoryboard, ballContainer);

            ballBounceStoryboard.Children.Add(ballBounceAnimation);
            ballBounceStoryboard.Begin();

            spinStoryboard.Children.Add(spinAnimation);
            spinStoryboard.Completed += spinStoryboard_Completed;
            spinStoryboard.Begin();

            centerStoryboard.Children.Add(centerAnimation);
            centerStoryboard.Begin();

            ballSpinStoryboard.Children.Add(ballSpinAnimation);
            ballSpinStoryboard.Begin();
        }

        void spinStoryboard_Completed(object sender, object e)
        {
            CompositeTransform stopAngleTransform = new CompositeTransform();
            stopAngleTransform = (CompositeTransform)numberwang.RenderTransform;
            
            double stopAngle = stopAngleTransform.Rotation;

            double angleRemainder = stopAngle % 360;

            stopAngleTransform.Rotation = angleRemainder;
            numberwang.RenderTransform = stopAngleTransform;

            decimal positionIndexDecimal = (decimal)(angleRemainder / 360) * 38;
            int positionIndex = (int)Math.Round(positionIndexDecimal);
            positionIndex = positionIndex % 38;

            // Use Game.cs to determine winner
            int winnerIndex = _currentGame.numeralSequence[positionIndex];

            // GET MARKER LIST AND ITERATE RESULTS
            for (int i = 0; i < tableControl.MarkerList.Count; i++)
            {
                TotalPayout += tableControl.MarkerList[i].GetResult(winnerIndex);
            }

            _currentGame.Wallet = NewWalletAmount;
            _currentGame.Wallet += TotalPayout;
            PayoutAmount.Text = TotalPayout.ToString();
            WalletAmount.Text = _currentGame.Wallet.ToString();
            Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            roamingSettings.Values["userWalletAmount"] = _currentGame.Wallet;
            if (_currentGame.Wallet < 0)
            {
                ReplenishButton.Visibility = Visibility.Visible;
            }
            else
            {
                ReplenishButton.Visibility = Visibility.Collapsed;
            }
        }

        private void BetChip_Click(object sender, RoutedEventArgs e)
        {
            tableControl.BetChip_Click(sender, e);
            UpdateScores();
        }

        private void ClearBet_Click(object sender, RoutedEventArgs e)
        {
            tableControl.ClearBet();
            UpdateScores();
        }

        private void ResetBet_Click(object sender, RoutedEventArgs e)
        {
            tableControl.ResetBet();
            UpdateScores();
        }

        private void ReplenishWallet_Click(object sender, RoutedEventArgs e)
        {
            if (_currentGame.Wallet < 500)
            {
                _currentGame.Wallet = 500;
                WalletAmount.Text = _currentGame.Wallet.ToString();
            }
        }

        //protected override void SaveState(Dictionary<String, Object> pageState)
        //{
        //    pageState["greetingOutputText"] =
        //}
    }
}
