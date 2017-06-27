﻿//-----------------------------------------------------------------------
// <copyright file="MigrationUtils.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace DriveHUD.Application.MigrationService.Migrators
{
    /// <summary>
    /// Determines methods to help with migration
    /// </summary>
    public class MigrationUtils
    {
        /// <summary>
        ///  Determines whenever the specified file is a serialized <see cref="{T}"/> 
        /// </summary>
        /// <typeparam name="T">Type of serialized object</typeparam>
        /// <param name="fileName">Path to the specified file</param>
        /// <returns>True if file is a serialized <see cref="{T}"/>, otherwise - false</returns>
        public static bool CanDeserialize<T>(string fileName)
        {
            using (var stream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var reader = new XmlTextReader(stream);
                var xmlSerializer = new XmlSerializer(typeof(T));
                return xmlSerializer.CanDeserialize(reader);
            }
        }
    }
}