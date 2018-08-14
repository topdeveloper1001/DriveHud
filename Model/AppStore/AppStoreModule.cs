//-----------------------------------------------------------------------
// <copyright file="AppStoreModule.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

namespace Model.AppStore
{
    public class AppStoreModule : AppStoreProduct
    {
        private string moduleName;

        public string ModuleName
        {
            get
            {
                return moduleName;
            }
            set
            {
                SetProperty(ref moduleName, value);
            }
        }

        private string windowIconSource;

        public string WindowIconSource
        {
            get
            {
                return windowIconSource;
            }
            set
            {
                SetProperty(ref windowIconSource, value);
            }
        }

        private double windowWidth;

        public double WindowWidth
        {
            get
            {
                return windowWidth;
            }
            set
            {
                SetProperty(ref windowWidth, value);
            }
        }

        private double windowHeight;

        public double WindowHeight
        {
            get
            {
                return windowHeight;
            }
            set
            {
                SetProperty(ref windowHeight, value);
            }
        }
    }
}