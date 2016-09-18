using System.Linq;
using DriveHUD.Inserter.Domain;
using NHibernate;
using NHibernate.Linq;

namespace DriveHUD.Inserter
{
    public class DriveHUDDataLayer : IDataLayer
    {
        public void Store(long gameNumber, string handHistory, int pokerClient = 1)
        {
            if(string.IsNullOrWhiteSpace(handHistory))
                return;

            using (var session = ModelEntities.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var exisits = Exist(session, gameNumber, pokerClient);
                    if(exisits)
                        return;

                    Hands hand = new Hands
                    {
                        PokersiteId = (short)pokerClient,
                        Gamenumber = gameNumber
                    };

                    Handhistory hh = new Handhistory
                    {
                        Filename = "Outer",
                        Gamenumber = gameNumber,
                        GametypeId = 0,
                        PokersiteId = (short)pokerClient,
                        HandhistoryVal = handHistory,
                    };

                    session.Save(hand);
                    session.Save(hh);

                    transaction.Commit();
                }
            }
        }

        private bool Exist(ISession session,long gameNumber, int pokerClient)
        {
            return session.Query<Hands>()
                .Any(x =>
                    x.Gamenumber == gameNumber &&
                    x.PokersiteId == pokerClient);
        }
    }
}