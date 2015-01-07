using System;
using System.Collections.Generic;
using PoeHUD.Models.Enums;

using SharpDX;

namespace PoeHUD.Hud.Loot
{
	public class ItemUsefulProperties {

		public string Name;
		public string DisplayName { get { return (Quality > 0 ? "Superior " : String.Empty) + Name; } }
		public bool IsCurrency;
		public bool IsSkillGem;
		public ItemRarity Rarity;
		public bool WorthChrome;

		public bool IsCraftingBase;
		
		public int NumSockets;
		public int NumLinks;

		public int ItemLevel;
		public int Quality;
		public int MapLevel;
		public bool IsVaalFragment;

        public bool IsWorthAlertingPlayer(HashSet<string> currencyNames, ItemAlertSettings settings)
		{
            if (Rarity == ItemRarity.Rare && settings.Rares)
				return true;
            if (Rarity == ItemRarity.Unique && settings.Uniques)
				return true;
            if ((MapLevel > 0 || IsVaalFragment) && settings.Maps)
				return true;
            if (NumLinks >= settings.MinLinks)
				return true;
            if (IsCurrency && settings.Currency)
            {
				if (currencyNames == null) {
					if( !Name.Contains("Portal") && Name.Contains("Wisdom") )
						return true;
				}
				else if (currencyNames.Contains(Name))
					return true;
			}

            if (IsSkillGem && settings.SkillGems) return true;
            if (IsSkillGem && settings.QualitySkillGems && Quality >= settings.QualitySkillGemsLevel) return true;
            if (WorthChrome && settings.Rgb) return true;
            if (settings.QualityItems.Enable)
            {
                var qualitySettings = settings.QualityItems;
                if (qualitySettings.Weapon.Enable && IsWeapon && Quality >= qualitySettings.Weapon.MinQuality
                    || qualitySettings.Armour.Enable && IsArmour && Quality >= qualitySettings.Armour.MinQuality
                    || qualitySettings.Flask.Enable && IsFlask && Quality >= qualitySettings.Flask.MinQuality)
                {
                    return true;
                }
            }
            return NumSockets >= settings.MinSockets || IsCraftingBase;
		}

        public bool IsWeapon { get; set; }

        public bool IsArmour { get; set; }

        public bool IsFlask { get; set; }

		internal AlertDrawStyle GetDrawStyle()
		{
			Color color = Color.White;
			switch(this.Rarity) {
				case ItemRarity.White : color = Color.White; break;
				case ItemRarity.Magic: color = HudSkin.MagicColor; break;
				case ItemRarity.Rare: color = HudSkin.RareColor; break;
				case ItemRarity.Unique: color = HudSkin.UniqueColor; break;
			}
			if( IsSkillGem )
				color = HudSkin.SkillGemColor;
			if (IsCurrency)
				color = HudSkin.CurrencyColor;

			int iconIndex = -1;
			if (WorthChrome)
				iconIndex = 1;
			if (NumSockets == 6)
				iconIndex = 0;
			if (IsCraftingBase)
				iconIndex = 2;
			if (NumLinks == 6)
				iconIndex = 3;

			return new AlertDrawStyle()
			{
				color = color,
				FrameWidth = MapLevel > 0 || IsVaalFragment ? 1 : 0,
				Text = DisplayName,
				IconIndex = iconIndex
			};
		}
	}
}
