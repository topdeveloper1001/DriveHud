using DriveHUD.Entities;
using System.ServiceModel;

namespace Simulator
{
    [ServiceContract]
    public interface IDHImporterService
    {
        [OperationContract]
        void ImportHandHistory(HandHistoryDto handHistory);
    }
}