//-----------------------------------------------------------------------
// <copyright file="SerializerHelper.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using NUnit.Framework;
using ProtoBuf;
using System.IO;

namespace DriveHud.Tests.UnitTests.Helpers
{
    internal class SerializerHelper
    {
        /// <summary>
        /// Gets the deserialized object of the serialized version of the specified object of type <see cref="{T}"/>
        /// </summary>
        /// <typeparam name="T">Type of object to be serialized/deserialized</typeparam>
        /// <param name="hudLayoutToolExpected">Object to be serialized/deserialized</param>
        /// <returns>Deserialized object of type <see cref="{T}"/></returns>
        public static T GetSerializedDeserializedObject<T>(T expectedObject) where T : class
        {
            byte[] data = null;

            Assert.DoesNotThrow(() =>
            {
                using (var msTestString = new MemoryStream())
                {
                    Serializer.Serialize(msTestString, expectedObject);
                    data = msTestString.ToArray();
                }
            });

            Assert.IsNotNull(data);

            T actualObject = null;

            Assert.DoesNotThrow(() =>
            {
                using (var afterStream = new MemoryStream(data))
                {
                    actualObject = Serializer.Deserialize<T>(afterStream);
                }
            });

            Assert.IsNotNull(actualObject);

            return actualObject;
        }
    }
}