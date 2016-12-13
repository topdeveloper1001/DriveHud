using System.Collections.Generic;
using System.Threading.Tasks;
using DriveHUD.Entities;

namespace Model.Interfaces
{
    public interface ITopPlayersService
    {
        Task<IList<Playerstatistic>> GetTop();
        void UpdateStatistics(IList<Playerstatistic> getAllPlayerStats);
    }
}
