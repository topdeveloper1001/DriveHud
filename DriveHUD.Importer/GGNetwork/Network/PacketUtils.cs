//-----------------------------------------------------------------------
// <copyright file="PacketUtils.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Exceptions;
using DriveHUD.Common.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace DriveHUD.Importers.GGNetwork.Network
{
    /// <summary>
    /// Implements methods to operate with GGN packets
    /// </summary>
    internal class PacketUtils
    {
        /// <summary>
        /// Compresses data with zip
        /// </summary>
        /// <param name="data">Data to compress</param>
        /// <returns>Compressed data</returns>
        /// <exception cref="DHInternalException" />
        public static byte[] CompressData(byte[] data)
        {
            try
            {
                using (var ms = new MemoryStream())
                {
                    using (var zip = new GZipStream(ms, CompressionMode.Compress))
                    {
                        zip.Write(data, 0, data.Length);
                        zip.Close();
                        return ms.ToArray();
                    }
                }
            }
            catch (Exception e)
            {
                throw new DHInternalException(new NonLocalizableString("Data hasn't been compressed."), e);
            }
        }

        /// <summary>
        /// Decompresses data compressed with zip
        /// </summary>
        /// <param name="data">Data to decompress</param>
        /// <returns>Decompressed data</returns>
        /// <exception cref="DHInternalException" />
        public static byte[] DecompressData(byte[] data)
        {
            try
            {
                using (var ms = new MemoryStream(data))
                {
                    using (var zip = new GZipStream(ms, CompressionMode.Decompress))
                    {
                        using (var result = new MemoryStream())
                        {
                            zip.CopyTo(result);
                            return result.ToArray();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new DHInternalException(new NonLocalizableString("Data hasn't been decompressed."), e);
            }
        }

        /// <summary>
        /// Builds packet for sending to GGN server
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <param name="protocolId">Protocol identifier</param>
        /// <param name="magicKey">Magic key</param>
        /// <param name="serialMode">Serial mode</param>
        /// <param name="packetIdSeq">Packet id</param>
        /// <param name="relayid">Relay id</param>
        /// <param name="junkData">Junk data</param>
        /// <param name="sendHeaderSize">The size of the header</param>
        /// <returns>Packet as array of bytes</returns>
        /// <exception cref="DHInternalException" />
        public static byte[] BuildPacket(string message, ProtocolId protocolId, int magicKey, int serialMode,
            ref int packetIdSeq, int relayid, int junkData, int sendHeaderSize)
        {
            try
            {
                var payloadData = CompressData(Encoding.UTF8.GetBytes(message));

                var dataBufferSize = payloadData.Length + sendHeaderSize;

                var dataBuffer = new List<byte>(dataBufferSize);

                dataBuffer.AddRange(BitConverter.GetBytes(payloadData.Length));
                dataBuffer.AddRange(BitConverter.GetBytes((int)protocolId));
                dataBuffer.AddRange(BitConverter.GetBytes(magicKey));
                dataBuffer.AddRange(BitConverter.GetBytes(serialMode));
                dataBuffer.AddRange(BitConverter.GetBytes(packetIdSeq++));
                dataBuffer.AddRange(BitConverter.GetBytes(relayid));

                dataBuffer.AddRange(BitConverter.GetBytes(junkData));
                dataBuffer.AddRange(BitConverter.GetBytes(junkData));
                dataBuffer.AddRange(BitConverter.GetBytes(junkData));

                dataBuffer.AddRange(payloadData);

                return dataBuffer.ToArray();
            }
            catch (Exception e)
            {
                throw new DHInternalException(new NonLocalizableString("Packet hasn't been built."), e);
            }
        }

        /// <summary>
        /// Extracts data from packet
        /// </summary>
        /// <param name="buffer">Packet as array of bytes to extract data</param>
        /// <param name="recvHeaderSize">The size of the header</param>
        /// <returns>Extracted data</returns>
        /// <exception cref="DHInternalException" />
        public static string ExtractData(byte[] buffer, int recvHeaderSize)
        {
            try
            {
                var payloadData = buffer.Skip(recvHeaderSize).Take(buffer.Length - recvHeaderSize)
                    .ToArray();

                var data = Encoding.UTF8.GetString(DecompressData(payloadData));

                return data;
            }
            catch (Exception e)
            {
                throw new DHInternalException(new NonLocalizableString("Data hasn't been extracted."), e);
            }
        }
    }
}