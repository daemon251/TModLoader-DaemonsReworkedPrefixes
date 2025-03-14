using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using System.Collections.Generic;
using Terraria.Localization;
using Terraria.Utilities;
using Microsoft.Xna.Framework;
using daemonReforge.Content.weaponPrefixes;

namespace daemonReforge.Content.accessoryPrefixes
{ 
	public class aaa : Projectile
	{
		public void func()
		{
			//Main.NewText(minionSlots);
		}
	}
	
	public class daemonReforgeModPlayer : ModPlayer
	{
		public float multMeleeSpeed = 1.00f; 
		public float multMoveSpeed = 1.00f; 
		public float multDamage = 1.00f; 
		public float multManaCost = 1.00f;
		
		public override void PostUpdateRunSpeeds() //this method needs to be used because hermes boots and its derivatives are annoying and need this for the speed prefixes to do anything while they are equipped
		{
			if (multMoveSpeed != 1.00f) 
			{
				Player.maxRunSpeed *= multMoveSpeed;
				Player.accRunSpeed *= multMoveSpeed; //this is for making hermes boots work
				Player.runAcceleration *= multMoveSpeed;
			}
		}
		
		public override void PostUpdateEquips()
		{
			if (multDamage != 1.00f) {Player.GetDamage(DamageClass.Generic) *= multDamage;}
			if (multMeleeSpeed != 1.00f) {Player.GetAttackSpeed(DamageClass.Generic) *= multMeleeSpeed; Player.pickSpeed *= (1 / multMeleeSpeed);}
			if (multManaCost != 1.00f) {Player.manaCost *= multManaCost;}
		}
		
		public override void PreUpdate()
		{
			multMeleeSpeed = 1.00f; 
			multMoveSpeed = 1.00f; 
			multDamage = 1.00f;
			multManaCost = 1.00f;
		}
	}

	public class daemonReforgeAccs : GlobalItem
	{
		public override void Load()
		{
			On_Player.GrantPrefixBenefits += modifyAccessories; 
		}
		
		public override void VerticalWingSpeeds(Item item, Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
		{
			constantAscend *= player.GetModPlayer<daemonReforgeModPlayer>().multMoveSpeed; //accel
			maxAscentMultiplier *= player.GetModPlayer<daemonReforgeModPlayer>().multMoveSpeed; //max speed
			player.GetModPlayer<daemonReforgeModPlayer>().multMoveSpeed = 1.00f;
		}
		
		private static void modifyAccessories(On_Player.orig_GrantPrefixBenefits orig, Player player, Item item) 
		{
			int critIncrease = 0;
			
			if (item.prefix == PrefixID.Wild) {player.GetModPlayer<daemonReforgeModPlayer>().multMeleeSpeed += 0.02f;}
			else if (item.prefix == PrefixID.Rash) {player.GetModPlayer<daemonReforgeModPlayer>().multMeleeSpeed += 0.04f;}
			else if (item.prefix == PrefixID.Intrepid) {player.GetModPlayer<daemonReforgeModPlayer>().multMeleeSpeed += 0.06f;}
			else if (item.prefix == PrefixID.Violent) {player.GetModPlayer<daemonReforgeModPlayer>().multMeleeSpeed += 0.08f;}
			else if (item.prefix == PrefixID.Brisk) {player.GetModPlayer<daemonReforgeModPlayer>().multMoveSpeed += 0.02f;}
			else if (item.prefix == PrefixID.Fleeting) {player.GetModPlayer<daemonReforgeModPlayer>().multMoveSpeed += 0.04f;}
			else if (item.prefix == PrefixID.Hasty2) {player.GetModPlayer<daemonReforgeModPlayer>().multMoveSpeed += 0.06f;}
			else if (item.prefix == PrefixID.Quick2) {player.GetModPlayer<daemonReforgeModPlayer>().multMoveSpeed += 0.08f;}
			else if (item.prefix == PrefixID.Jagged) {player.GetModPlayer<daemonReforgeModPlayer>().multDamage += 0.015f;}
			else if (item.prefix == PrefixID.Spiked) {player.GetModPlayer<daemonReforgeModPlayer>().multDamage += 0.03f;}
			else if (item.prefix == PrefixID.Angry) {player.GetModPlayer<daemonReforgeModPlayer>().multDamage += 0.045f;}
			else if (item.prefix == PrefixID.Menacing) {player.GetModPlayer<daemonReforgeModPlayer>().multDamage += 0.06f;}
			
			else if (item.prefix == PrefixID.Precise) {critIncrease = 4;}
			else if (item.prefix == PrefixID.Lucky) {critIncrease = 8;}
			else if (item.prefix == PrefixID.Arcane) {player.GetModPlayer<daemonReforgeModPlayer>().multManaCost += -0.12f;}
			else {orig(player, item);}
			
			if (critIncrease != 0) 
			{
				player.GetCritChance(DamageClass.Generic) += critIncrease;
			}
		}
			
		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) 
		{
            string bonus = "0";
			
			if (item.prefix == PrefixID.Wild) {bonus = "2";}
			else if (item.prefix == PrefixID.Rash) {bonus = "4";}
			else if (item.prefix == PrefixID.Intrepid) {bonus = "6";}
			else if (item.prefix == PrefixID.Violent) {bonus = "8";}
			else if (item.prefix == PrefixID.Brisk) {bonus = "2";}
			else if (item.prefix == PrefixID.Fleeting) {bonus = "4";}
			else if (item.prefix == PrefixID.Hasty2) {bonus = "6";} 
			else if (item.prefix == PrefixID.Quick2) {bonus = "8";} 
			else if (item.prefix == PrefixID.Jagged) {bonus = "1.5";}
			else if (item.prefix == PrefixID.Spiked) {bonus = "3";}
			else if (item.prefix == PrefixID.Angry) {bonus = "4.5";}
			else if (item.prefix == PrefixID.Menacing) {bonus = "6";}
			else if (item.prefix == PrefixID.Precise) {bonus = "4";}
			else if (item.prefix == PrefixID.Lucky) {bonus = "8";}
			else if (item.prefix == PrefixID.Arcane) {bonus = "12";}
			
			foreach (var line in tooltips) 
			{
                if (line.Name == "PrefixAccMeleeSpeed") 
				{
                    //line.Text = "+" + bonus + Language.GetText("LegacyTooltip.40"); //WORKS... 47 is melee speed BUUUT NOT PRECISE LANGUAGE
					line.Text = "+" + bonus + "% Weapon Speed";
                }
				if (line.Name == "PrefixAccDamage") 
				{
                    //line.Text = "+" + bonus + Language.GetText("LegacyTooltip.39"); //WORKS BUUUT NOT PRECISE LANGUAGE
					line.Text = "+" + bonus + "% Final Damage";
                }
				if (line.Name == "PrefixAccCritChance") 
				{
                    line.Text = "+" + bonus + Language.GetText("LegacyTooltip.41"); //WORKS
                }
				if (line.Name == "PrefixAccMoveSpeed") 
				{
                    line.Text = "+" + bonus + Language.GetText("LegacyTooltip.46"); //WORKS
                }
				if (line.Name == "PrefixAccMaxMana") 
				{
                    line.Text = "-" + bonus + "% Mana Cost";  
                }
				
            }
        }
		
	}
}
