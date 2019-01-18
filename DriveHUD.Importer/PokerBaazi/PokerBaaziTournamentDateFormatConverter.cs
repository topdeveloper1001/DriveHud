//-----------------------------------------------------------------------
// <copyright file="PokerBaaziTournamentDateFormatConverter.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Globalization;

namespace DriveHUD.Importers.PokerBaazi
{
    internal class PokerBaaziTournamentDateFormatConverter : IsoDateTimeConverter
    {
        private const int DateTimeOffset = -3;

        public PokerBaaziTournamentDateFormatConverter(string format)
        {
            Culture = CultureInfo.InvariantCulture;
            DateTimeFormat = format;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var result = base.ReadJson(reader, objectType, existingValue, serializer);

            if (result is DateTime dateTime)
            {
                dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);

                if (dateTime >= DateTime.MinValue.AddHours(-DateTimeOffset))
                {
                    dateTime = dateTime.AddHours(DateTimeOffset);
                }

                return dateTime;
            }

            return result;
        }
    }
}