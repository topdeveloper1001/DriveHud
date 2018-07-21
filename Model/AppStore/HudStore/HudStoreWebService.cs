﻿//-----------------------------------------------------------------------
// <copyright file="HudStoreWebService.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using DriveHUD.Common.Exceptions;
using DriveHUD.Common.Log;
using DriveHUD.Common.Resources;
using DriveHUD.Common.Security;
using Model.AppStore.HudStore.Model;
using Model.AppStore.HudStore.ServiceResponses;
using Newtonsoft.Json;

namespace Model.AppStore.HudStore
{
    /// <summary>
    /// Service to communicate with HUD store web service
    /// </summary>
    public class HudStoreWebService : IHudStoreWebService
    {
        private const string encryptKey = "<RSAKeyValue><Modulus>t9FLl4PZ92qPz459kGw6dDM76tNNGFhNjZFnhbuk5oI/pEGx6idF/zbXBIh/4NuVnH1skKx1s1A1z7H/081k9H4ojB7lr1R7vdzzIndZFmj9Or3p84UgUqResuuTppwrkZZkyNzyJHUl9LvPsx2GhW3JXXXgX3lWuhPCaKPUreV70j3xvmL49pQChU66RG/l+UojT6KjWU/N1Gk8XU1hqpao377mA5UUcBq18co6uQ6yVJ67xeg4Zs/E8fdlq9vIVju7KWDZN1XShHPTm3X6b7KjePXl4/stbNPFK+rm9lElb6u4/ZcerIGppCTCIabtTko3RykCMjGDSIRhDJG3CQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        private static readonly string serverAddress;

        private static readonly HttpClient httpClient;

        static HudStoreWebService()
        {
            serverAddress = CommonResourceManager.Instance.GetResourceString("Settings_HudStoreService");
            httpClient = new HttpClient();
        }

        /// <summary>
        /// Gets info for uploading form
        /// </summary>
        /// <returns><see cref="HudStoreData"/></returns>
        /// <exception cref="DHInternalException" />
        public HudStoreData GetUploadInfo()
        {
            try
            {
                var hudStoreData = Get<HudStoreData>(HudStoreServiceNames.InfoService, HudStoreServiceCommands.Get);
                return hudStoreData;
            }
            catch (Exception e)
            {
                throw new DHInternalException(new NonLocalizableString("Couldn't get upload info from the hud store web service."), e);
            }
        }

        /// <summary>
        /// Uploads data to the HUD store
        /// </summary>
        /// <param name="uploadInfo">Data to upload</param>
        /// <exception cref="DHInternalException" />
        public void Upload(HudStoreUploadInfo uploadInfo)
        {
            if (uploadInfo == null)
            {
                throw new ArgumentNullException(nameof(uploadInfo));
            }

            try
            {
                uploadInfo.Serial = SecurityUtils.EncryptStringRSA(uploadInfo.Serial, encryptKey);

                var multiPartContent = new MultipartFormDataContent();

                for (var i = 0; i < uploadInfo.Images.Length; i++)
                {
                    var imageInfo = uploadInfo.Images[i];

                    if (!MediaTypeHeaderValue.TryParse(MimeMapping.GetMimeMapping(imageInfo.Path), out MediaTypeHeaderValue contentType))
                    {
                        throw new DHBusinessException(new LocalizableString("Error_HudStore_Upload_BadImageFormat", imageInfo.Path));
                    }

                    var streamContent = new StreamContent(File.OpenRead(imageInfo.Path));
                    streamContent.Headers.ContentType = contentType;

                    multiPartContent.Add(streamContent, $"images[]", Path.GetFileName(imageInfo.Path));
                }

                var jsonString = JsonConvert.SerializeObject(uploadInfo);

                multiPartContent.Add(new StringContent(jsonString, Encoding.UTF8), "data");

                Post<bool>(HudStoreServiceNames.HudsService, HudStoreServiceCommands.Upload, multiPartContent);
            }
            catch (Exception e)
            {
                throw new DHInternalException(new NonLocalizableString("Couldn't upload data to hud store web service."), e);
            }
        }

        private T Post<T>(string serviceName, string command, HttpContent content)
        {
            try
            {
                var uri = new Uri(new Uri(serverAddress), $"{serviceName}/{command}");

                var response = httpClient.PostAsync(uri, content).Result;

                response.EnsureSuccessStatusCode();

                var responseContent = response.Content.ReadAsStringAsync().Result;

                var serviceResponse = JsonConvert.DeserializeObject<HudStoreServiceResponse<T>>(responseContent);

                if (serviceResponse.Errors == null || serviceResponse.Errors.Length == 0)
                {
                    return serviceResponse.Result;
                }


            }
            catch (Exception e)
            {
                throw new DHInternalException(new NonLocalizableString("Couldn't post data to hud store web service."), e);
            }

            return default(T);
        }

        private T Get<T>(string serviceName, string command) where T : class
        {
            try
            {
                var uri = new Uri(new Uri(serverAddress), $"{serviceName}/{command}");

                var response = httpClient.GetAsync(uri).Result;
                var responseContent = response.Content.ReadAsStringAsync().Result;

                var serviceResponse = JsonConvert.DeserializeObject<HudStoreServiceResponse<T>>(responseContent);

                if (serviceResponse.Errors == null || serviceResponse.Errors.Length == 0)
                {
                    return serviceResponse.Result;
                }
            }
            catch (Exception e)
            {
                throw new DHInternalException(new NonLocalizableString("Couldn't get data from hud store web service."), e);
            }

            return null;
        }
    }
}