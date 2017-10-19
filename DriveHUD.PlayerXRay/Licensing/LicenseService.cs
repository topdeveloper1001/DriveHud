﻿//-----------------------------------------------------------------------
// <copyright file="LicenseService.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DeployLX.Licensing.v5;
using DriveHUD.Common.Exceptions;
using DriveHUD.Common.Log;
using DriveHUD.Common.Resources;
using DriveHUD.Common.Security;
using DriveHUD.Common.Utils;
using Microsoft.Practices.ServiceLocation;
using Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DriveHUD.PlayerXRay.Licensing
{
    /// <summary>
    /// License service
    /// </summary>
    internal class LicenseService : ILicenseService
    {
        private const string serialRSAKey = "<RSAKeyValue><Modulus>qFM2NXCFclFl9kvhxFr2sXTvl6aWy7n2oVeGo8WM8FpfKz5zbTeYKlqdhczwW0li4wtoHKKykPOiGGpAoChjB95YYyJpOPIL2hQWsEunbJlnDBceBPg+9tsqT1HVCJe52fI5EWEBBGrGvKaYqi2lMdNh96cd19EIs7lmy6IPN+M=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        private readonly List<ILicenseInfo> licenseInfos;

        private bool isInitialized;

        public LicenseService()
        {
            licenseInfos = new List<ILicenseInfo>();
        }

        /// <summary>
        /// License validation
        /// </summary>
        public bool Validate()
        {
            licenseInfos.Clear();

            // workaround for obfuscation
            var licenseTypes = new LicenseType[]
            {
                LicenseType.Trial,
                LicenseType.Holdem,
                LicenseType.Omaha,
                LicenseType.Combo
            };

            // validate each possible type 
            foreach (LicenseType licenseType in licenseTypes)
            {
                SecureLicense license = null;

                NoLicenseException validationException = null;

                var isExpired = false;
                var serial = string.Empty;

                var licenseManager = ServiceLocator.Current.GetInstance<ILicenseManager>(licenseType.ToString());

                try
                {
                    var requestInfo = new LicenseValidationRequestInfo
                    {
                        DontShowForms = true
                    };

                    license = licenseManager.Validate(requestInfo);

                    if (!license.IsTrial)
                    {
                        LogProvider.Log.Info(CustomModulesNames.PlayerXRay, string.Format("Found license: {0}-*", license.SerialNumber.Substring(0, 4)));
                    }
                }
                catch (NoLicenseException ex)
                {
                    validationException = ex;

                    var exceptionData = ParseException(ex);

                    if (exceptionData != null &&
                            exceptionData.ContainsKey("errorCode") && exceptionData["errorCode"].Equals("LCS_EXP"))
                    {
                        isExpired = true;
                        serial = exceptionData["serial"];
                    }
                }
                catch (Exception ex)
                {
                    LogProvider.Log.Error(CustomModulesNames.PlayerXRay, "Validation: License validation error", ex);
                }

                // Trial license - check if real trial is expired or does not exists
                if (license != null && license.IsTrial)
                {
                    licenseManager.ResetCacheForLicense(license);

                    var requestInfo = new LicenseValidationRequestInfo
                    {
                        DontShowForms = true,
                        DisableTrials = true
                    };

                    try
                    {
                        license = licenseManager.Validate(requestInfo);
                    }
                    catch (NoLicenseException ex)
                    {
                        validationException = ex;
                    }
                }

                var licenseInfo = new LicenseInfo(license, licenseType);
                licenseInfo.ValidationException = validationException;

                // if license expired we must specify that
                if (isExpired && license == null)
                {
                    licenseInfo.IsExpired = true;
                    licenseInfo.Serial = serial;
                }

                if (!licenseInfo.IsTrial || licenseInfos.All(x => !x.IsTrial))
                {
                    licenseInfos.Add(licenseInfo);
                }
            }

            isInitialized = true;

            UpdateExpirationDates(licenseInfos);

            var isValid = licenseInfos.Any(x => x.IsRegistered);

            return isValid;
        }

        /// <summary>
        /// Register with specified serial number
        /// </summary>
        /// <param name="serial">Serial number</param>
        public bool Register(string serial, string email)
        {
            if (string.IsNullOrWhiteSpace(serial))
            {
                throw new DHBusinessException(new NonLocalizableString("Serial is not defined."));
            }

            var licenseType = GetTypeFromSerial(serial);

            if (!licenseType.HasValue)
            {
                throw new DHBusinessException(new NonLocalizableString("Serial is not defined."));
            }

            var licenseManager = ServiceLocator.Current.GetInstance<ILicenseManager>(licenseType.ToString());

            var licenseInfo = licenseInfos.FirstOrDefault(x => x.LicenseType == licenseType);

            var license = licenseInfo != null ? licenseInfo.License : null;

            if (license != null)
            {
                licenseManager.ResetCacheForLicense(license);
            }

            if (licenseInfo != null)
            {
                licenseInfos.Remove(licenseInfo);
            }

            var requestInfo = new LicenseValidationRequestInfo
            {
                DontShowForms = true,
                SerialNumbers = new string[] { serial },
                SaveExternalSerials = true,
                DisableTrials = true,
                DisableCache = true,
                ShouldGetNewSerialNumber = true
            };

            requestInfo.AdditionalServerProperties.Add("email", email);

            var isExpired = false;

            try
            {
                license = licenseManager.Validate(requestInfo);

                if (license == null || license.IsTrial)
                {
                    throw new LicenseInvalidSerialException("License not found or expired");
                }

                if (license.IsActivation && !license.IsActivated)
                {
                    throw new LicenseCouldNotActivateException();
                }

                LogProvider.Log.Info(CustomModulesNames.PlayerXRay, $"License has been registered: {license.SerialNumber.Substring(0, 5)}");

                return true;
            }
            catch (NoLicenseException ex)
            {
                // check for expiration first
                var exceptionData = ParseException(ex);

                if (exceptionData != null &&
                         exceptionData.ContainsKey("errorCode") && exceptionData["errorCode"].Equals("LCS_EXP"))
                {
                    isExpired = true;

                    LogProvider.Log.Error(CustomModulesNames.PlayerXRay, "Registration: License expired", ex);
                    throw new LicenseExpiredException();
                }

                // check for activation errors
                var errorCodes = Utils.GetErrorCodes(ex);

                if (errorCodes.Any(x => x.Equals("E_CouldNotActivateAtServer") || x.Equals("E_CannotValidateAtServer")))
                {
                    LogProvider.Log.Error(CustomModulesNames.PlayerXRay, "Registration: License wasn't activated", ex);
                    throw new LicenseCouldNotActivateException();
                }

                LogProvider.Log.Error(CustomModulesNames.PlayerXRay, "Registration: License not found or expired", ex);
                throw new LicenseInvalidSerialException("License not found or expired", ex);
            }
            catch (LicenseCouldNotActivateException)
            {
                LogProvider.Log.Error(CustomModulesNames.PlayerXRay, "Registration: License wasn't activated");
                throw;
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(CustomModulesNames.PlayerXRay, "Registration: License validation error", ex);
                throw new LicenseException("Unexpected license validation exception", ex);
            }
            finally
            {
                licenseInfo = new LicenseInfo(license, licenseType.Value);
                licenseInfos.Add(licenseInfo);

                // if license expired we must specify that
                if (isExpired && license == null)
                {
                    licenseInfo.IsExpired = true;
                    licenseInfo.Serial = serial;
                }

                LogProvider.Log.Info(CustomModulesNames.PlayerXRay, $"License limits: Cash={licenseInfo.CashLimit}, Tournament={licenseInfo.TournamentLimit}");

                UpdateExpirationDates(new[] { licenseInfo });
            }
        }

        /// <summary>
        /// Parse exception to get predefined data
        /// </summary>
        /// <param name="ex">Exception to be parsed</param>
        /// <returns>Return dictionary of exception data, if exception can not be parsed returns null</returns>
        private Dictionary<string, string> ParseException(Exception ex)
        {
            Dictionary<string, string> parseResult = null;

            if (ex == null)
            {
                return parseResult;
            }

            var errorMsg = ex.ToString();

            try
            {
                if (errorMsg.Contains("|JSONErrorStart|"))
                {
                    errorMsg = errorMsg.Substring(errorMsg.IndexOf("|JSONErrorStart|"));
                    errorMsg = errorMsg.Substring(0, errorMsg.IndexOf("|JSONErrorEnd|")).Replace("|JSONErrorStart|", "");

                    parseResult = JsonConvert.DeserializeObject<Dictionary<string, string>>(errorMsg);
                    return parseResult;
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(CustomModulesNames.PlayerXRay, "Could not parse exception", e);
            }

            return parseResult;
        }

        /// <summary>
        /// Encrypts the specified email
        /// </summary>
        /// <param name="email">Email to encrypt</param>
        /// <returns>Encrypted email</returns>
        public string EncryptEmail(string email)
        {
            var random = new Random();

            var salt = random.Next(100000, 999999).ToString();

            var emailWithSalt = email + salt;

            var encryptedEmail = SecurityUtils.EncryptStringRSA(emailWithSalt, serialRSAKey);
            return encryptedEmail;
        }

        /// <summary>
        /// Update expiration dates for specified licenses
        /// </summary>
        /// <param name="licenseInfos">License to be updated</param>
        private void UpdateExpirationDates(IEnumerable<ILicenseInfo> licenses)
        {
            var valuableLicenses = licenses.Where(x => !string.IsNullOrWhiteSpace(x.Serial) && !x.IsTrial).ToArray();

            var serialHashes = valuableLicenses.Select(x => SecurityUtils.EncryptStringRSA(x.Serial, serialRSAKey)).ToArray();

            if (serialHashes.Length < 1)
            {
                return;
            }

            var expiryDates = GetExpiryDates(serialHashes);

            if (serialHashes.Length != expiryDates.Length)
            {
                LogProvider.Log.Error(CustomModulesNames.PlayerXRay, "Getting expiration date failed - collections do not match");
                return;
            }

            for (var i = 0; i < serialHashes.Length; i++)
            {
                valuableLicenses[i].ExpiryDate = expiryDates[i];
            }
        }

        /// <summary>
        /// Get expiration date for current serial number
        /// </summary>
        /// <param name="serial"></param>
        /// <returns></returns>
        private DateTime[] GetExpiryDates(string[] serialHashes)
        {
            // to do: read from local settings


            // read from server            
            try
            {
                using (var server = new DeployLXLicensingServer())
                {
                    var expiryDates = server.GetExpiryDate(serialHashes);
                    return expiryDates;
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(CustomModulesNames.PlayerXRay, "Could not obtain expiry dates", e);
                var expiryDates = serialHashes.Select(x => DateTime.MaxValue).ToArray();
                return expiryDates;
            }
        }

        /// <summary>
        /// Returns active licenses, if licenses weren't initialized exception will be thrown
        /// </summary>
        public IEnumerable<ILicenseInfo> LicenseInfos
        {
            get
            {
                if (!isInitialized)
                {
                    throw new DHInternalException(new NonLocalizableString("License has not been initialized"));
                }

                return licenseInfos;
            }
        }

        /// <summary>
        /// If any of license is registered
        /// </summary>
        public bool IsRegistered
        {
            get
            {
                return isInitialized && licenseInfos.Any(x => x.IsRegistered);
            }
        }

        /// <summary>
        /// Only one license is registered and it's a trial
        /// </summary>
        public bool IsTrial
        {
            get
            {
                var registered = licenseInfos.Where(x => x.IsRegistered).ToArray();
                return isInitialized && registered.Length > 0 && registered.All(x => x.IsTrial);
            }
        }

        /// <summary>
        /// Trial expired
        /// </summary>
        public bool IsTrialExpired
        {
            get
            {
                return !IsRegistered && licenseInfos.Any(x => x.IsTrialExpired);
            }
        }

        /// <summary>
        /// True if one of existing licenses is expiring soon
        /// </summary>
        public bool IsExpiringSoon
        {
            get
            {
                return IsRegistered && licenseInfos.Any(x => x.IsExpiringSoon);
            }
        }

        /// <summary>
        /// Determine when any of licenses is expired
        /// </summary>
        public bool IsExpired
        {
            get
            {
                return licenseInfos.Any(x => x.IsExpired);
            }
        }

        /// <summary>
        /// Determines if user can upgrade his license
        /// </summary>
        public bool IsUpgradable
        {
            get
            {
                return !licenseInfos.Any(x => x.IsRegistered && !x.IsExpired && x.Serial.StartsWith("DHCP"));
            }
        }

        /// <summary>
        /// Get license type by its serial number
        /// </summary>
        /// <param name="serial">Serial number</param>
        /// <returns>Type of license</returns>
        public LicenseType? GetTypeFromSerial(string serial)
        {
            if (serial.StartsWith("XRH", StringComparison.InvariantCulture))
            {
                return LicenseType.Holdem;
            }

            if (serial.StartsWith("XRO", StringComparison.InvariantCulture))
            {
                return LicenseType.Omaha;
            }

            if (serial.StartsWith("XRCT", StringComparison.InvariantCulture))
            {
                return LicenseType.Trial;
            }

            if (serial.StartsWith("XRC", StringComparison.InvariantCulture))
            {
                return LicenseType.Combo;
            }

            return null;
        }
    }
}