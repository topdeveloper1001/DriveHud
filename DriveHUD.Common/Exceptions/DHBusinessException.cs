//-----------------------------------------------------------------------
// <copyright file="DHBusinessException.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Resources;
using System;

namespace DriveHUD.Common.Exceptions
{
    public class DHBusinessException : LocalizableException, IDetailedException, ITypeSpecifiedException
    {
        public DHBusinessException(ILocalizableString message, Exception innerException, string[] logs = null)
            : base(message, innerException)
        {
            _logs = logs;
        }

        public DHBusinessException(ILocalizableString message, string[] logs = null)
            : base(message, null)
        {
            _logs = logs;
        }

        #region ITypeSpecifiedException Members

        public ErrorPurpose ErrorPurpose
        {
            get
            {
                return ErrorPurpose.Business;
            }
        }

        #endregion

        #region IDetailedException Members

        public ILocalizableString DetailedUIReport
        {
            get;
            set;
        }

        public ILocalizableString DetailedLogReport
        {
            get;
            set;
        }

        #endregion

        #region IOwnLogPoliticsException Members

        private string[] _logs;

        public string[] Logs
        {
            get
            {
                return _logs;
            }
        }

        #endregion     
    }
}