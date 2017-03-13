using DriveHUD.API.DataContracts;
using HandHistories.Objects.Hand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace DriveHUD.API.Interfaces
{
    [ServiceContract]
    public interface IAPIService
    {
        [OperationContract]
        [WebGet(UriTemplate = "hand/{id}/{pokerSite=Unknown}", ResponseFormat = WebMessageFormat.Xml)]
        [XmlSerializerFormat]
        HandHistory GetHandById(string id, string pokerSite);

        [OperationContract]
        [WebGet(UriTemplate = "hands/{date=Unknown}", ResponseFormat = WebMessageFormat.Xml)]
        [XmlSerializerFormat]
        HandHistory[] GetHands(string date);

        [OperationContract]
        [WebGet(UriTemplate = "hands/{playerName}/{pokerSite}/{date=Unknown}", ResponseFormat = WebMessageFormat.Xml)]
        [XmlSerializerFormat]
        HandHistory[] GetPlayerHands(string playerName, string pokerSite, string date);

        [OperationContract]
        [WebGet(UriTemplate = "list/hands/{date=Unknown}", ResponseFormat = WebMessageFormat.Json)]
        HandInfoDataContract[] GetHandsList(string date);

        [OperationContract]
        [WebGet(UriTemplate = "list/hands/{playerName}/{pokerSite}/{date=Unknown}", ResponseFormat = WebMessageFormat.Json)]
        HandInfoDataContract[] GetPlayerHandsList(string playerName, string pokerSite, string date);

    }
}
