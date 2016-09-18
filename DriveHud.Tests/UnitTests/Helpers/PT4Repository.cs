using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace DriveHud.Tests.UnitTests
{
    internal class PT4Repository
    {
        private NpgsqlConnection connection;

        public PT4Repository(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException();
            }

            connection = new NpgsqlConnection(connectionString);
        }

        public IEnumerable<HandHistory> GetAllTournamentsHands()
        {
            var handHistorySchema = new HandHistorySchema
            {
                HandHistoriesTable = DbConfiguration.TourneyHandHistoriesTable,
                HandSummaryTable = DbConfiguration.TourneyHandSummaryTable
            };

            var hands = GetAllHands(handHistorySchema);

            return hands;
        }

        public IEnumerable<HandHistory> GetAllCashHands()
        {
            var handHistorySchema = new HandHistorySchema
            {
                HandHistoriesTable = DbConfiguration.CashHandHistoriesTable,
                HandSummaryTable = DbConfiguration.CashHandSummaryTable
            };

            var hands = GetAllHands(handHistorySchema);

            return hands;
        }

        private IEnumerable<HandHistory> GetAllHands(HandHistorySchema handHistorySchema)
        {
            connection.Open();

            var sql = string.Format("SELECT chh.id_hand, chh.history FROM {0} chh INNER JOIN {1} chs on chh.id_hand=chs.id_hand where chs.id_site=100", handHistorySchema.HandHistoriesTable, handHistorySchema.HandSummaryTable);

            var command = new NpgsqlCommand(sql, connection);

            var dataReader = command.ExecuteReader();

            var result = new List<HandHistory>();

            while (dataReader.Read())
            {
                var cashHandHistory = new HandHistory
                {
                    Id = Convert.ToInt32(dataReader["id_hand"]),
                    History = dataReader["history"].ToString()
                };

                result.Add(cashHandHistory);
            }

            connection.Close();

            return result;
        }

        public IEnumerable<string> GetAllDHHands()
        {
            connection.Open();

            var sql = "SELECT \"HandHistory\" FROM \"HandHistories\"";

            var command = new NpgsqlCommand(sql, connection);

            var dataReader = command.ExecuteReader();

            var result = new List<string>();

            while (dataReader.Read())
            {              
                result.Add(dataReader["HandHistory"].ToString());
            }

            connection.Close();

            return result;
        }
    }
}