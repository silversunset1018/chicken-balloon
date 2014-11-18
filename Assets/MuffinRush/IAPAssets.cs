/// Copyright (C) 2012-2014 Soomla Inc.
///
/// Licensed under the Apache License, Version 2.0 (the "License");
/// you may not use this file except in compliance with the License.
/// You may obtain a copy of the License at
///
///      http://www.apache.org/licenses/LICENSE-2.0
///
/// Unless required by applicable law or agreed to in writing, software
/// distributed under the License is distributed on an "AS IS" BASIS,
/// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
/// See the License for the specific language governing permissions and
/// limitations under the License.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Soomla.Store;

namespace Soomla.Example {

	/// <summary>
	/// This class defines our game's economy, which includes virtual goods, virtual currencies
	/// and currency packs, virtual categories, and non-consumable items.
	/// </summary>
	public class IAPAssets : IStoreAssets{

		/// <summary>
		/// see parent.
		/// </summary>
		public int GetVersion() {
			return 0;
		}

		/// <summary>
		/// see parent.
		/// </summary>
		public VirtualCurrency[] GetCurrencies() {
			return new VirtualCurrency[]{BALLOON_CURRENCY};
		}

		/// <summary>
		/// see parent.
		/// </summary>
	    public VirtualGood[] GetGoods() {
			return new VirtualGood[] {CONTINUE_GOOD1, CONTINUE_GOOD3, CONTINUE_GOOD5, CONTINUE_GOOD7, CONTINUE_GOOD9};
		}

		/// <summary>
		/// see parent.
		/// </summary>
	    public VirtualCurrencyPack[] GetCurrencyPacks() {
			return new VirtualCurrencyPack[] {BALLOON7_PACK, BALLOON40_PACK, BALLOON100_PACK};
		}

		/// <summary>
		/// see parent.
		/// </summary>
	    public VirtualCategory[] GetCategories() {
			return new VirtualCategory[]{GENERAL_CATEGORY};
		}

		/// <summary>
		/// see parent.
		/// </summary>
	    public NonConsumableItem[] GetNonConsumableItems() {
			return new NonConsumableItem[]{};
		}
		
	    /** Static Final Members **/
	
	    public const string CURRENCY_ITEM_ID      = "currency_balloon";

		public const string BALLOON7_PACK_PRODUCT_ID      = "android.test.purchased";
		public const string BALLOON40_PACK_PRODUCT_ID      = "40_pack";
		public const string BALLOON100_PACK_PRODUCT_ID      = "100_pack";

		/*
	    public const string FIFTYMUFF_PACK_PRODUCT_ID    = "android.test.canceled";
		public const string FOURHUNDMUFF_PACK_PRODUCT_ID = "android.test.purchased";
		public const string THOUSANDMUFF_PACK_PRODUCT_ID = "2500_pack";
	    public const string NO_ADDS_NONCONS_PRODUCT_ID   = "no_ads";
		*/
		public const string CONTINUE_ITEM_ID1   = "continue_item1";
		public const string CONTINUE_ITEM_ID3   = "continue_item3";
		public const string CONTINUE_ITEM_ID5   = "continue_item5";
		public const string CONTINUE_ITEM_ID7   = "continue_item7";
		public const string CONTINUE_ITEM_ID9   = "continue_item9";

	
	    /** Virtual Currencies **/

	    public static VirtualCurrency BALLOON_CURRENCY = new VirtualCurrency(
	            "Coins",										// name
	            "",												// description
	            CURRENCY_ITEM_ID							// item id
	    );
		

	    /** Virtual Currency Packs **/
	
	    public static VirtualCurrencyPack BALLOON7_PACK = new VirtualCurrencyPack(
	            "7 Balloons",							// name
	            "Handful Amount of Balloons",		 // description
	            "balloons_7",							// item id
				7,									// number of currencies in the pack
	            CURRENCY_ITEM_ID,					// the currency associated with this pack
	            new PurchaseWithMarket(BALLOON7_PACK_PRODUCT_ID, 0.99)
		);

		public static VirtualCurrencyPack BALLOON40_PACK = new VirtualCurrencyPack(
			"40 Balloons",							// name
			"Thankful Amount of Balloons",		 // description
			"balloons_40",							// item id
			40,									// number of currencies in the pack
			CURRENCY_ITEM_ID,					// the currency associated with this pack
			new PurchaseWithMarket(BALLOON40_PACK_PRODUCT_ID, 4.99)
		);
		
		public static VirtualCurrencyPack BALLOON100_PACK = new VirtualCurrencyPack(
			"100 Balloons",							// name
			"Thankful Amount of Balloons",		 // description
			"balloons_100",							// item id
			100,									// number of currencies in the pack
			CURRENCY_ITEM_ID,					// the currency associated with this pack
			new PurchaseWithMarket(BALLOON100_PACK_PRODUCT_ID, 9.99)
		);



	    /** Virtual Goods **/
		
	    public static VirtualGood CONTINUE_GOOD1 = new SingleUseVG(
	            "Continue Item1",                                       		// name
	            "Continue to game", // description
	            CONTINUE_ITEM_ID1,                                       		// item id
	            new PurchaseWithVirtualItem(CURRENCY_ITEM_ID, 1)); // the way this virtual good is purchased

		public static VirtualGood CONTINUE_GOOD3 = new SingleUseVG(
			"Continue Item3",                                       		// name
			"Continue to game", // description
			CONTINUE_ITEM_ID3,                                       		// item id
			new PurchaseWithVirtualItem(CURRENCY_ITEM_ID, 3));

		public static VirtualGood CONTINUE_GOOD5 = new SingleUseVG(
			"Continue Item5",                                       		// name
			"Continue to game", // description
			CONTINUE_ITEM_ID5,                                       		// item id
			new PurchaseWithVirtualItem(CURRENCY_ITEM_ID, 5));
		
		public static VirtualGood CONTINUE_GOOD7 = new SingleUseVG(
			"Continue Item7",                                       		// name
			"Continue to game", // description
			CONTINUE_ITEM_ID7,                                       		// item id
			new PurchaseWithVirtualItem(CURRENCY_ITEM_ID, 7));
		
		public static VirtualGood CONTINUE_GOOD9 = new SingleUseVG(
			"Continue Item9",                                       		// name
			"Continue to game", // description
			CONTINUE_ITEM_ID9,                                       		// item id
			new PurchaseWithVirtualItem(CURRENCY_ITEM_ID, 9));

	
		
	    /** Virtual Categories **/
	    // The muffin rush theme doesn't support categories, so we just put everything under a general category.
	    public static VirtualCategory GENERAL_CATEGORY = new VirtualCategory(
			"General", new List<string>(new string[] { CONTINUE_ITEM_ID1, CONTINUE_ITEM_ID3, CONTINUE_ITEM_ID5, CONTINUE_ITEM_ID7, CONTINUE_ITEM_ID9}) 
	    );
		
		
	    /** Market MANAGED Items **/
		/*
	    public static NonConsumableItem NO_ADDS_NONCONS  = new NonConsumableItem(
            "No Ads",
            "Test purchase of MANAGED item.",
            "no_ads",
            new PurchaseWithMarket(new MarketItem(NO_ADDS_NONCONS_PRODUCT_ID, MarketItem.Consumable.NONCONSUMABLE , 1.99))
    	);
		*/
	}
	
}