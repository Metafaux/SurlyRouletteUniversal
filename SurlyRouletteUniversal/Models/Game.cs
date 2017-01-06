using System;
using System.Collections.Generic;

namespace SurlyRoulette.Models
{
    class Game
    {
       /* Inside Bets:
        * Straight 35:1
        * Street 11:1
        * Corner 8:1
        * Six line (or Double Street) 5:1
        * Basket 11:1
        * Top line 6:1
        * 
        * Outside bets:
        * 1 to 18 (Manque) 1:1
        * 19 to 36 (Passe) 1:1
        * Red or black 1:1
        * Even or Odd 1:1
        * Dozen bets 2:1
        * Column bet 2:1 
       */

        /* Bet Index IDs
         * 0-36         face
         * 37           00
         * 38-40        column
         * 41-43        1st 12, 2nd 12, 3rd 12
         * 44, 45       red, black
         * 46, 47       even, odd
         * 49, 50       1 to 18, 19 to 36
         * 
         * Make flags for each ID, then have dynamic payout factors
         * Attach payouts to button Types, not IDs
         * ID only determines win match
         * 
         * Inside space properties: index, color
         * Outside space properties: range of indexes
        */

        /* Board button types
         * Straight (one index)
         * Split (two adjacent indexes)
         * 
        */

        // BET PAYOUT FACTORS
        public const int straight = 35;
        public const int street = 11;
        public const int split = 17;
        public const int square = 8;
        public const int doubleStreet = 5;
        public const int topline = 6;
        public const int dozen = 2;

        // double zero is represented by index 37
        //public readonly int[] numeralSequence = new int[] { 0, 28, 9, 26, 30, 11, 7, 20, 32, 17, 5, 22, 34, 15, 3, 24, 36, 13, 1, 37, 27, 10, 25, 29, 12, 8, 19, 31, 18, 6, 21, 33, 16, 4, 23, 35, 14, 2 };
        //counter clockwise
        public readonly int[] numeralSequence = new int[] {37, 1, 13, 36, 24, 3, 15, 34, 22, 5, 17, 32, 20, 7, 11, 30, 26, 9, 28, 0, 2, 14, 35, 23, 4, 16, 33, 21, 6, 18, 31, 19, 8, 12, 29, 25, 10, 27  };

        private const int boardQty = 36;
        private Random spinGen = new Random();
        private int winIndex;

        // This maps the payout factors to the indexes on the board
        // Initialized in constructor
        private int[] payouts = new int[50];
        
        // This will be broken out into its own class
        // when multiplayer can be introduced on a later version
        //public Player player1;

        // Player properties
        // To be broken out into separate class in later version
        private const int defaultWallet = 500;
        public int Wallet { get; set; }
        public int BetPayout { get; set; }
        // to be replaced with List property
        public int BetIndex { get; set; }
        public int BetAmount { get; set; }
        // list of arrays containing bet info
        public List<int[]> BetList;

        public Game()
        {
            // 0, 37 inside bets pay 35:1
            // This will probably become a button class property
            for (int i = 0; i < 38; i++)
            {
                payouts[i] = straight;
            }

            //player1 = new Player();
            Wallet = defaultWallet;
            //BetList = new List<int[]>();
        }

        public void PlaceBet()  //(Player p, int boardID, int betAmount)
        {
            // 
            
            // Params: Bet square, bet amount
            // Check if Player.bet == 0
            // Prompt for bet placement
            // else, place previously stored bet
            // player1.Bet[36] = 10;

            BetIndex = 36;
            //BetAmount = 10;
        }

        //private void AddBet(int boardId, int betAmount)
        //{
        //    int[] listItem = new int[] { boardId, betAmount };
        //    BetList.Add(listItem);
        //}
        
        public void Spin(int winIndexInput)
        {
            // Random number generation
            // Pay out bets
            // Reset betting table

            Wallet -= BetAmount;
            winIndex = winIndexInput;
            Payout();
            //return winIndex;
        }

        private void Payout()
        {
            // Add to players' totals
            // Show what players lost
            // Clear markers on screen

            // Start with local winning number
            // int winIndex = 36;
            // if player1.bet == winIndex then
            // player1.wallet += bet * betPayoutFactor[winIndex]

            // temporary local win factor
            int winFactor = 35;

            if (BetIndex == winIndex)
            {
                BetPayout = BetAmount * winFactor;
                Wallet += (BetAmount * winFactor) + BetAmount;
            }
            else
            {
                BetPayout = 0;
            }
        }

    }
}
