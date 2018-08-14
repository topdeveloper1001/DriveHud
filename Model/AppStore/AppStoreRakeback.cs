//-----------------------------------------------------------------------
// <copyright file="AppStoreRakeback.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Prism.Mvvm;
using System;

namespace Model.AppStore
{
    /// <summary>
    /// Model of rakeback in shop
    /// </summary>    
    [Serializable]
    public class AppStoreRakeback : BindableBase
    {
        private int rakeback;

        public int Rakeback
        {
            get
            {
                return rakeback;
            }
            set
            {
                SetProperty(ref rakeback, value);
            }
        }

        private string network;

        public string Network
        {
            get
            {
                return network;
            }
            set
            {
                SetProperty(ref network, value);
            }
        }


        private string description;

        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                SetProperty(ref description, value);
            }
        }

        private string signUpCode;

        public string SignUpCode
        {
            get
            {
                return signUpCode;
            }
            set
            {
                SetProperty(ref signUpCode, value);
            }

        }

        private string signUpLink;

        public string SignUpLink
        {
            get
            {
                return signUpLink;
            }
            set
            {
                SetProperty(ref signUpLink, value);
            }
        }

        private string imageLink;

        public string ImageLink
        {
            get
            {
                return imageLink;
            }
            set
            {
                SetProperty(ref imageLink, value);
            }
        }

        private double imageWidth;

        public double ImageWidth
        {
            get
            {
                return imageWidth;
            }
            set
            {
                SetProperty(ref imageWidth, value);
            }
        }

        private double imageHeight;

        public double ImageHeight
        {
            get
            {
                return imageHeight;
            }
            set
            {
                SetProperty(ref imageHeight, value);
            }
        }
    }
}