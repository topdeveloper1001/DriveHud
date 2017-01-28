//-----------------------------------------------------------------------
// <copyright file="ShopProduct.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
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

namespace Model.Shop
{
    /// <summary>
    /// Model of product in shop
    /// </summary>    
    [Serializable]
    public class AppStoreProduct : BindableBase
    {
        private string productName;

        public string ProductName
        {
            get
            {
                return productName;
            }
            set
            {
                if (productName == value)
                {
                    return;
                }

                productName = value;
                OnPropertyChanged();
            }
        }

        private string productDescription;

        public string ProductDescription
        {
            get
            {
                return productDescription;
            }
            set
            {
                if (productDescription == value)
                {
                    return;
                }

                productDescription = value;
                OnPropertyChanged();
            }
        }

        private string price;

        public string Price
        {
            get
            {
                return price;
            }
            set
            {
                if (price == value)
                {
                    return;
                }

                price = value;
                OnPropertyChanged();
            }
        }

        private string cartLink;

        public string CartLink
        {
            get
            {
                return cartLink;
            }
            set
            {
                if (cartLink == value)
                {
                    return;
                }

                cartLink = value;
                OnPropertyChanged();
            }
        }

        private string learnMoreLink;

        public string LearnMoreLink
        {
            get
            {
                return learnMoreLink;
            }
            set
            {
                if (learnMoreLink == value)
                {
                    return;
                }

                learnMoreLink = value;
                OnPropertyChanged();
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
                if (imageLink == value)
                {
                    return;
                }

                imageLink = value;
                OnPropertyChanged();
            }
        }

        private bool isAnimatedGif;

        public bool IsAnimatedGif
        {
            get
            {
                return isAnimatedGif;
            }
            set
            {
                if (isAnimatedGif == value)
                {
                    return;
                }

                isAnimatedGif = value;
                OnPropertyChanged();
            }
        }
    }
}