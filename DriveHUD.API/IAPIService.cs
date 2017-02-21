using HandHistories.Objects.Hand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace DriveHUD.API
{
    [ServiceContract]
    public interface IAPIService
    {
        [OperationContract]
        [WebGet(UriTemplate = "GetHandById/{id}", ResponseFormat = WebMessageFormat.Xml)]
        [XmlSerializerFormat]
        string GetHandById(string id);

        [OperationContract]
        [WebGet(UriTemplate = "GetPlayerHands/{playerName}/{pokersiteId}", ResponseFormat = WebMessageFormat.Xml)]
        [XmlSerializerFormat]
        HandHistory[] GetPlayerHands(string playerName, string pokersiteId);
    }
}
