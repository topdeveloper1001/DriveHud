//-----------------------------------------------------------------------
// <copyright file="PokerBaaziHandBuilder.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Exceptions;
using DriveHUD.Common.Log;
using DriveHUD.Common.Resources;
using DriveHUD.Entities;
using DriveHUD.Importers.PokerBaazi.Model;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DriveHUD.Importers.PokerBaazi
{
    internal class PokerBaaziHandBuilder : IPokerBaaziHandBuilder
    {
        private Dictionary<uint, List<PokerBaaziPackage>> roomPackages = new Dictionary<uint, List<PokerBaaziPackage>>();
        private Dictionary<uint, PokerBaaziInitResponse> roomsInitResponses = new Dictionary<uint, PokerBaaziInitResponse>();

        private static readonly Currency currency = Currency.INR;
        private static readonly string loggerName = EnumPokerSites.PokerBaazi.ToString();

        /// <summary>
        /// Finds the <see cref="PokerBaaziInitResponse"/> of the specified room
        /// </summary>
        /// <param name="roomId">Room id</param>
        /// <returns><see cref="PokerBaaziInitResponse"/> of the specified room if found; otherwise - null</returns>
        public PokerBaaziInitResponse FindInitResponse(uint roomId)
        {
            roomsInitResponses.TryGetValue(roomId, out PokerBaaziInitResponse response);
            return response;
        }

        /// <summary>
        /// Tries to build hand history by using buffered packages or buffer the specified package for further using
        /// </summary>
        /// <param name="package">Package to buffer</param>
        /// <param name="handHistory">Hand history</param>
        /// <returns>True if hand can be built; otherwise - false</returns>
        public bool TryBuild(PokerBaaziPackage package, out HandHistory handHistory)
        {
            handHistory = null;

            if (package == null)
            {
                return false;
            }

            if (package.PackageType == PokerBaaziPackageType.InitResponse)
            {
                ParsePackage<PokerBaaziInitResponse>(package, x => ProcessInitResponse(x));
                return false;
            }

            if (!roomPackages.TryGetValue(package.RoomId, out List<PokerBaaziPackage> packages))
            {
                packages = new List<PokerBaaziPackage>();
                roomPackages.Add(package.RoomId, packages);
            }

            packages.Add(package);




            return handHistory != null && handHistory.Players.Count > 0;
        }

        /// <summary>
        /// Processes the specified <see cref="PokerBaaziInitResponse"/>
        /// </summary>
        /// <param name="response">Response to process</param>
        private void ProcessInitResponse(PokerBaaziInitResponse response)
        {
            if (!roomsInitResponses.ContainsKey(response.RoomId))
            {
                roomsInitResponses.Add(response.RoomId, response);
                LogProvider.Log.Info(this, $"Init data of room {response.RoomId} has been stored. [{loggerName}]");
                return;
            }

            roomsInitResponses[response.RoomId] = response;

            LogProvider.Log.Info(this, $"Init data of room {response.RoomId} has been updated. [{loggerName}]");
        }

        /// <summary>
        /// Parses <see cref="PokerBaaziPackage"/> into response object, then performs the specified action on that response object
        /// </summary>
        /// <typeparam name="T">Type of response object</typeparam>
        /// <param name="package">Package to parse</param>
        /// <param name="action">Action to perform on response object</param>
        private void ParsePackage<T>(PokerBaaziPackage package, Action<T> action) where T : class
        {
            PokerBaaziResponse<T> response;

            try
            {
                response = JsonConvert.DeserializeObject<PokerBaaziResponse<T>>(package.JsonData);
            }
            catch
            {
                throw new DHInternalException(new NonLocalizableString($"Failed to deserialize package of {package.PackageType} type."));
            }

            action?.Invoke(response.ClassObj);
        }
    }
}