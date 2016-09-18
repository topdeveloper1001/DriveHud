using HandHistories.Objects.GameDescription;
using Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// Contains information about tournament winnings by place
    /// </summary>
    public class TournamentResultModel
    {
        /// <summary>
        /// Tournament Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Collection of prizes
        /// </summary>
        public TournamentPrize[] Prizes { get; set; }

        /// <summary>
        /// Tournament Buy-In
        /// </summary>
        public int BuyinInCents { get; set; }

        /// <summary>
        /// Players in tournament
        /// </summary>
        public int TotalPlayers { get; set; }

        /// <summary>
        /// Tournament currency
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// Gets collection of predefined tournaments (TODO: split by siteId when siteId detection will be implemented)
        /// </summary>
        /// <returns></returns>
        public static IList<TournamentResultModel> GetPredefinedTournaments()
        {
            var resultList = new List<TournamentResultModel>();

            /* bovada */
            resultList.Add(new TournamentResultModel() { Name = "2-Table (6-Handed)", BuyinInCents = 1000, Currency = Currency.USD, TotalPlayers = 12, Prizes = TournamentPrize.CreatePrizesArray(6000, 3600, 2400) });
            resultList.Add(new TournamentResultModel() { Name = "3-Table (Beginner)", BuyinInCents = 200, Currency = Currency.USD, TotalPlayers = 27, Prizes = TournamentPrize.CreatePrizesArray(1215, 945, 594, 486, 405, 324, 270, 216, 189) });
            resultList.Add(new TournamentResultModel() { Name = "3-Table Sit & Go", BuyinInCents = 1000, Currency = Currency.USD, TotalPlayers = 27, Prizes = TournamentPrize.CreatePrizesArray(9720, 7020, 4860, 3240, 2160) });
            resultList.Add(new TournamentResultModel() { Name = "3-Table Sit & Go", BuyinInCents = 100, Currency = Currency.USD, TotalPlayers = 27, Prizes = TournamentPrize.CreatePrizesArray(972, 702, 486, 324, 216) });
            resultList.Add(new TournamentResultModel() { Name = "All-In Sit & Go (6-Handed)", BuyinInCents = 1000, Currency = Currency.USD, TotalPlayers = 6, Prizes = TournamentPrize.CreatePrizesArray(6000) });
            resultList.Add(new TournamentResultModel() { Name = "All-In Sit & Go (Heads-Up)", BuyinInCents = 5000, Currency = Currency.USD, TotalPlayers = 4, Prizes = TournamentPrize.CreatePrizesArray(15000, 5000) });
            resultList.Add(new TournamentResultModel() { Name = "All-In Sit & Go (Heads-Up)", BuyinInCents = 2000, Currency = Currency.USD, TotalPlayers = 4, Prizes = TournamentPrize.CreatePrizesArray(6000, 2000) });
            resultList.Add(new TournamentResultModel() { Name = "Nightly $25K PTS Qualifier", BuyinInCents = 0, Currency = Currency.USD, TotalPlayers = 9, Prizes = TournamentPrize.CreatePrizesArray(5000) });
            resultList.Add(new TournamentResultModel() { Name = "Nightly $25k Quarterfinal", BuyinInCents = 250, Currency = Currency.USD, TotalPlayers = 6, Prizes = TournamentPrize.CreatePrizesArray(1000, 400) });
            resultList.Add(new TournamentResultModel() { Name = "Nightly $25K Semifinal", BuyinInCents = 1000, Currency = Currency.USD, TotalPlayers = 6, Prizes = TournamentPrize.CreatePrizesArray(5000, 0) });
            resultList.Add(new TournamentResultModel() { Name = "Nightly $5K PLO Hi/Lo Quarterfinal", BuyinInCents = 150, Currency = Currency.USD, TotalPlayers = 6, Prizes = TournamentPrize.CreatePrizesArray(600, 240) });
            resultList.Add(new TournamentResultModel() { Name = "Nightly $5K PLO Hi/Lo Semifinal", BuyinInCents = 600, Currency = Currency.USD, TotalPlayers = 6, Prizes = TournamentPrize.CreatePrizesArray(3000, 300) });
            resultList.Add(new TournamentResultModel() { Name = "Sat. to Any $100+$9 SMPO3 Event (All-In Heads-Up)", BuyinInCents = 3000, Currency = Currency.USD, TotalPlayers = 4, Prizes = TournamentPrize.CreatePrizesArray(10000, 1100) });
            resultList.Add(new TournamentResultModel() { Name = "Satellite To Any $10 + $1", BuyinInCents = 367, Currency = Currency.USD, TotalPlayers = 9, Prizes = TournamentPrize.CreatePrizesArray(1000, 1000, 1000, 3) });
            resultList.Add(new TournamentResultModel() { Name = "SMPO3 $150K Quarterfinal (All-In 6-Handed)", BuyinInCents = 500, Currency = Currency.USD, TotalPlayers = 6, Prizes = TournamentPrize.CreatePrizesArray(2700, 165) });
            resultList.Add(new TournamentResultModel() { Name = "SMPO3 $150K Semifinal (All-In 6-Handed)", BuyinInCents = 2700, Currency = Currency.USD, TotalPlayers = 6, Prizes = TournamentPrize.CreatePrizesArray(15000) });
            resultList.Add(new TournamentResultModel() { Name = "SMPO3 Main Event Quarterfinal ((All-In Heads-Up)", BuyinInCents = 2000, Currency = Currency.USD, TotalPlayers = 4, Prizes = TournamentPrize.CreatePrizesArray(7500, 150) });
            resultList.Add(new TournamentResultModel() { Name = "SMPO3 Main Event Semifinal (All-In 6-Handed)", BuyinInCents = 7500, Currency = Currency.USD, TotalPlayers = 6, Prizes = TournamentPrize.CreatePrizesArray(42500) });
            resultList.Add(new TournamentResultModel() { Name = "SMPO3 Mini Main Event Satellite (All-In Heads-Up)", BuyinInCents = 2500, Currency = Currency.USD, TotalPlayers = 4, Prizes = TournamentPrize.CreatePrizesArray(7500, 1800) });
            resultList.Add(new TournamentResultModel() { Name = "Sunday $50K Quarterfinal", BuyinInCents = 1100, Currency = Currency.USD, TotalPlayers = 6, Prizes = TournamentPrize.CreatePrizesArray(4000, 1100, 1000) });
            resultList.Add(new TournamentResultModel() { Name = "Sunday $50K Semifinal", BuyinInCents = 4000, Currency = Currency.USD, TotalPlayers = 6, Prizes = TournamentPrize.CreatePrizesArray(20000, 1100, 100) });

            /* bet online */
            resultList.Add(new TournamentResultModel() { Name = "12 Player Sit n Go #2", BuyinInCents = 55, Currency = Currency.USD, TotalPlayers = 12, Prizes = TournamentPrize.CreatePrizesArray(300, 180, 120) });
            resultList.Add(new TournamentResultModel() { Name = "12 Player Sit n Go #2", BuyinInCents = 550, Currency = Currency.USD, TotalPlayers = 12, Prizes = TournamentPrize.CreatePrizesArray(3000, 1800, 1200) });
            resultList.Add(new TournamentResultModel() { Name = "12 Player Sit n Go #2", BuyinInCents = 165, Currency = Currency.USD, TotalPlayers = 12, Prizes = TournamentPrize.CreatePrizesArray(900, 540, 360) });
            resultList.Add(new TournamentResultModel() { Name = "18 Player Sit n Go #2", BuyinInCents = 50, Currency = Currency.USD, TotalPlayers = 18, Prizes = TournamentPrize.CreatePrizesArray(360, 270, 180, 90) });
            resultList.Add(new TournamentResultModel() { Name = "18 Player Sit n Go #2", BuyinInCents = 110, Currency = Currency.USD, TotalPlayers = 18, Prizes = TournamentPrize.CreatePrizesArray(720, 540, 360, 180) });
            resultList.Add(new TournamentResultModel() { Name = "18 Player Sit n Go #1", BuyinInCents = 5, Currency = Currency.USD, TotalPlayers = 18, Prizes = TournamentPrize.CreatePrizesArray(36, 27, 18, 9) });
            resultList.Add(new TournamentResultModel() { Name = "4 Player Heads Up Sit n Go #2", BuyinInCents = 110, Currency = Currency.USD, TotalPlayers = 4, Prizes = TournamentPrize.CreatePrizesArray(400) });
            resultList.Add(new TournamentResultModel() { Name = "4 Player Heads Up Sit n Go #2", BuyinInCents = 55, Currency = Currency.USD, TotalPlayers = 4, Prizes = TournamentPrize.CreatePrizesArray(200) });
            resultList.Add(new TournamentResultModel() { Name = "4 Player Heads Up Sit n Go #2", BuyinInCents = 330, Currency = Currency.USD, TotalPlayers = 4, Prizes = TournamentPrize.CreatePrizesArray(1200) });

            resultList.Add(new TournamentResultModel() { Name = "$50,000 GT Satelite Step 1 #2", BuyinInCents = 25, Currency = Currency.USD, TotalPlayers = 6, Prizes = TournamentPrize.CreatePrizesArray(69, 69) });
            resultList.Add(new TournamentResultModel() { Name = "$50,000 GT Satelite Step 2 #1", BuyinInCents = 69, Currency = Currency.USD, TotalPlayers = 6, Prizes = TournamentPrize.CreatePrizesArray(333, 45) });
            resultList.Add(new TournamentResultModel() { Name = "$50,000 GT Satelite Step 3 #1", BuyinInCents = 333, Currency = Currency.USD, TotalPlayers = 6, Prizes = TournamentPrize.CreatePrizesArray(1815, 3) });
            resultList.Add(new TournamentResultModel() { Name = "$50,000 GT Satelite Step 4 #1", BuyinInCents = 1815, Currency = Currency.USD, TotalPlayers = 6, Prizes = TournamentPrize.CreatePrizesArray(9900) });

            return resultList;

        }
    }

    /// <summary>
    /// Represents tournament prize for the specific place
    /// </summary>
    public class TournamentPrize
    {
        public int Place { get; set; }
        public int WinningsInCents { get; set; }

        public TournamentPrize() : this(0, 0)
        {
        }

        public TournamentPrize(int place, int winningsInCents)
        {
            Place = place;
            WinningsInCents = winningsInCents;
        }

        /// <summary>
        /// Creates array of prizes
        /// </summary>
        /// <param name="winnings">array of winnings starting from the first place</param>
        /// <returns>array of prizes</returns>
        public static TournamentPrize[] CreatePrizesArray(params int[] winnings)
        {
            var resultList = new List<TournamentPrize>();
            for (int i = 0; i < winnings.Length; i++)
            {
                resultList.Add(new TournamentPrize(i + 1, winnings[i]));
            }

            return resultList.ToArray();
        }

    }
}
