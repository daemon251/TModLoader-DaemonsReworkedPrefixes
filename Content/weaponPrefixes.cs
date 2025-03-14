using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using daemonReforge.Content.accessoryPrefixes;

//https://docs.google.com/spreadsheets/d/1BCzUhU6E_k_-2oB6DDGl4pBhaNarRdYBhMHCxqDCjNk/edit?usp=sharing




namespace daemonReforge.Content.weaponPrefixes
{
	/*public class DaemonMinionCostModifier : GlobalItem
	{
		public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			//Main.NewText(ItemID.Sets.StaffMinionSlotsRequired[item.type]);
			if (item.DamageType.CountsAsClass(DamageClass.Summon) && !item.DamageType.CountsAsClass(DamageClass.SummonMeleeSpeed))
			{
				float addCost = 0.00f; 
				//player.maxMinions
				//player.slotsMinions
				
				int index = item.prefix;
				if (index == ModContent.PrefixType<Tactical>()) {addCost = 1.00f;}
				if (index == ModContent.PrefixType<Relentless>()) {addCost = -0.35f;}
				if (index == ModContent.PrefixType<Uncaring>()) {addCost = 0.30f;}
				//Main.NewText(addCost + 1.00f);
				
				//we are now going to change slotsMinions based on the addCost variable. This is mighty scuffed.
				Main.NewText(player.GetModPlayer<daemonReforgeModPlayer>().currentMinionsUsed + "   " + player.maxMinions);
				if (player.GetModPlayer<daemonReforgeModPlayer>().currentMinionsUsed + addCost < player.maxMinions)
				{
					player.GetModPlayer<daemonReforgeModPlayer>().bonusMinionSlots += -addCost;
					return true;
				}
				else
				{
					return false;
				}
			}
			return true; 

		}
	}*/
	
	public class DaemonAccuracyModifier : GlobalProjectile
	{
		public override bool InstancePerEntity => true;
		
		public override void OnSpawn(Projectile projectile, IEntitySource source)
		{
			int acc = 0; 
			Vector2 vel = Vector2.Zero;
			if (source is EntitySource_ItemUse entitySourceItemUse && entitySourceItemUse.Entity is Terraria.Player player) 
			{
				int index = player.HeldItem.prefix;
				if (index == ModContent.PrefixType<Sighted>()) {acc = 60;}
				if (index == ModContent.PrefixType<Frenzying>()) {acc = -50;}
				if (index == ModContent.PrefixType<Hasty>()) {acc = 25;}
				if (index == ModContent.PrefixType<Awful>()) {acc = -15;}
				if (index == ModContent.PrefixType<Demonic>()) {acc = 10;}
			}
			
			if (acc == 0) {return;}
			else
			{
				if (Main.myPlayer == projectile.owner) 
				{ 
					float speed = (float)Math.Sqrt(projectile.velocity.X * projectile.velocity.X + projectile.velocity.Y * projectile.velocity.Y);
			
					//find the error between the shot fired and the crosshair in radians
					float xDifLook = Main.MouseWorld.X - projectile.Center.X;
					float yDifLook = -(Main.MouseWorld.Y - projectile.Center.Y);
					float angle1 = (float)Math.Atan(yDifLook / xDifLook);
					if (xDifLook < 0) {angle1 = (float)Math.PI + angle1;}
					if (xDifLook > 0 && yDifLook < 0) {angle1 = (float)Math.PI * 2 + angle1;}
						
					float xDifShot = projectile.velocity.X;
					float yDifShot = -projectile.velocity.Y;
					float angle2 = (float)Math.Atan(yDifShot / xDifShot);
					if (xDifShot < 0) {angle2 = (float)Math.PI + angle2;}
					if (xDifShot > 0 && yDifShot < 0) {angle2 = (float)Math.PI * 2 + angle2;}
					
					float angleDif = angle1 - angle2; //angledif > 0 means bullet is below crosshair
					if (angleDif > Math.PI) 
					{
						angleDif += -2f * (float)Math.PI;
					}
					if (angleDif < -Math.PI) 
					{
						angleDif += 2f * (float)Math.PI;
					}
					
					//now we multiply the error using our accuracy stat
					if (acc > 100) {acc = 100;}
					angleDif *= (1 - (float)acc / 100f);	
					
					float randFloat = 0;
					if (acc < 0 && angleDif == 0f) //this is a band-aid solution for accuracy on weapons with perfect spread. One degree of spread both ways per 15% accuracy missing.
					{
						Random random = new Random();
						randFloat = ((float)acc / 15f) * ((float)random.NextDouble() * 2.0f - 1.0f);
						randFloat *= 3.141519f / 180f;
					}
					if (acc > 0 && angleDif == 0f) //band-aid solution: if +acc does nothing, then we just increase the velocity I guess. 
					{
						speed *= (1 + acc / 100f);
					}
					
					vel.Y = -speed * (float)Math.Sin(angleDif + angle1 + randFloat);
					vel.X = speed * (float)Math.Cos(angleDif + angle1 + randFloat);
					projectile.velocity = vel;
					
				}
			}
		}
	}
	
	//*******************
	//**** UNIVERSAL ****
	//*******************
	
	public class Keen : ModPrefix
	{
		public override PrefixCategory Category => PrefixCategory.AnyWeapon;

		public override float RollChance(Item item) 
		{
			return 0.3f;
		}

		public override bool CanRoll(Item item) 
		{
			if (item.CountsAsClass(DamageClass.Summon)) {return false;}
			return true;
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) 
		{
			damageMult *= 1f + 0.00f;
			critBonus += 15;
			knockbackMult *= 1f + 0.50f;
		}

		public override void ModifyValue(ref float valueMult) 
		{
			valueMult *= 1f + -0.3f;
		}

		public override void Apply(Item item) 
		{
			//
		}

		public static LocalizedText PowerTooltip { get; private set; }
		public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

		public override void SetStaticDefaults() {
			_ = AdditionalTooltip;
		}
	}
	
	public class Superior : ModPrefix
	{
		public override PrefixCategory Category => PrefixCategory.AnyWeapon;

		public override float RollChance(Item item) 
		{
			return 0.3f;
		}

		public override bool CanRoll(Item item) 
		{
			if (item.CountsAsClass(DamageClass.Summon)) {return false;}
			return true;
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) 
		{
			damageMult *= 1f + 0.40f;
			critBonus += -30;
			knockbackMult *= 1f + 0.00f;
		}

		public override void ModifyValue(ref float valueMult) 
		{
			valueMult *= 1f + 1.2f;
		}

		public static LocalizedText PowerTooltip { get; private set; }
		public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

		public override void SetStaticDefaults() {
			_ = AdditionalTooltip;
		}
	}
	
	public class Hurtful : ModPrefix
	{
		public override PrefixCategory Category => PrefixCategory.AnyWeapon;

		public override float RollChance(Item item) 
		{
			return 0.3f;
		}

		public override bool CanRoll(Item item) 
		{
			if (item.CountsAsClass(DamageClass.Summon)) {return false;}
			return true;
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) 
		{
			damageMult *= 1f + 0.18f;
			critBonus += 0;
			knockbackMult *= 1f + 0.00f;
		}

		public override void ModifyValue(ref float valueMult) 
		{
			valueMult *= 1f + 0.2f;
		}

		public static LocalizedText PowerTooltip { get; private set; }
		public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

		public override void SetStaticDefaults() {
			_ = AdditionalTooltip;
		}
	}
	
	public class Ruthless : ModPrefix
	{
		public override PrefixCategory Category => PrefixCategory.AnyWeapon;

		public override float RollChance(Item item) 
		{
			return 0.3f;
		}

		public override bool CanRoll(Item item) 
		{
			if (item.CountsAsClass(DamageClass.Summon)) {return false;}
			return true;
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) 
		{
			damageMult *= 1f + 0.22f;
			critBonus += 0;
			knockbackMult *= 1f + -0.50f;
		}

		public override void ModifyValue(ref float valueMult) 
		{
			valueMult *= 1f + 1.0f;
		}

		public static LocalizedText PowerTooltip { get; private set; }
		public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

		public override void SetStaticDefaults() {
			_ = AdditionalTooltip;
		}
	}
	
	public class Zealous : ModPrefix
	{
		public override PrefixCategory Category => PrefixCategory.AnyWeapon;

		public override float RollChance(Item item) 
		{
			return 0.2f;
		}

		public override bool CanRoll(Item item) 
		{
			if (item.CountsAsClass(DamageClass.Summon)) {return false;}
			return true;
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) 
		{
			damageMult *= 1f + -0.15f;
			critBonus += 50;
			knockbackMult *= 1f + 0.00f;
		}

		public override void ModifyValue(ref float valueMult) 
		{
			valueMult *= 0.7f;
		}

		public static LocalizedText PowerTooltip { get; private set; }
		public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

		public override void SetStaticDefaults() {
			_ = AdditionalTooltip;
		}
	}
	
	public class Broken : ModPrefix
	{
		public override PrefixCategory Category => PrefixCategory.AnyWeapon;

		public override float RollChance(Item item) 
		{
			return 0.2f;
		}

		public override bool CanRoll(Item item) 
		{
			if (item.CountsAsClass(DamageClass.Summon)) {return false;}
			return true;
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) 
		{
			damageMult *= 1f + 0.00f;
			critBonus += 0;
			knockbackMult *= 1f + -0.90f;
		}

		public override void ModifyValue(ref float valueMult) 
		{
			valueMult *= 5f;
		}

		public static LocalizedText PowerTooltip { get; private set; }
		public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

		public override void SetStaticDefaults() {
			_ = AdditionalTooltip;
		}
	}
	
	public class Shoddy : ModPrefix
	{
		public override PrefixCategory Category => PrefixCategory.AnyWeapon;

		public override float RollChance(Item item) 
		{
			return 0.2f;
		}

		public override bool CanRoll(Item item) 
		{
			if (item.CountsAsClass(DamageClass.Summon)) {return false;}
			return true;
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) 
		{
			damageMult *= 1f + 0.00f;
			critBonus += -10;
			knockbackMult *= 1f + 0.00f;
		}

		public override void ModifyValue(ref float valueMult) 
		{
			valueMult *= 1f + 0.2f;
		}

		public static LocalizedText PowerTooltip { get; private set; }
		public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

		public override void SetStaticDefaults() {
			_ = AdditionalTooltip;
		}
	}
	
	public class Weak : ModPrefix
	{
		public override PrefixCategory Category => PrefixCategory.AnyWeapon;

		public override float RollChance(Item item) 
		{
			return 0.1f;
		}

		public override bool CanRoll(Item item) 
		{
			if (item.CountsAsClass(DamageClass.Summon)) {return false;}
			return true;
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) 
		{
			damageMult *= 1f + -0.30f;
			critBonus += 0;
			knockbackMult *= 1f + 0.00f;
		}

		public override void ModifyValue(ref float valueMult) 
		{
			valueMult *= 1f;
		}

		public static LocalizedText PowerTooltip { get; private set; }
		public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

		public override void SetStaticDefaults() {
			_ = AdditionalTooltip;
		}
	}
	
//****************
//**** COMMON ****
//****************
	
	public class Godly : ModPrefix
	{
		public override PrefixCategory Category => PrefixCategory.AnyWeapon;

		public override float RollChance(Item item) 
		{
			return 0.2f;
		}

		public override bool CanRoll(Item item) 
		{
			if (item.CountsAsClass(DamageClass.Summon)) {return false;}
			return true;
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) 
		{
			damageMult *= 1f + 0.08f;
			critBonus += 4;
			knockbackMult *= 1f + 0.00f;
			useTimeMult *= 1.00f - 0.10f;
		}

		public override void ModifyValue(ref float valueMult) 
		{
			valueMult *= 1f + 0.2f;
		}

		public static LocalizedText PowerTooltip { get; private set; }
		public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

		public override void SetStaticDefaults() {
			_ = AdditionalTooltip;
		}
	}
	
	public class Quick : ModPrefix
	{
		public override PrefixCategory Category => PrefixCategory.AnyWeapon;

		public override float RollChance(Item item) 
		{
			return 0.2f;
		}

		public override bool CanRoll(Item item) 
		{
			if (item.CountsAsClass(DamageClass.Summon)) {return false;}
			return true;
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) 
		{
			damageMult *= 1f + -0.15f;
			critBonus += 0;
			knockbackMult *= 1f + 0.00f;
			useTimeMult *= 1.00f - 0.30f;
		}

		public override void ModifyValue(ref float valueMult) 
		{
			valueMult *= 1f + 0.2f;
		}

		public static LocalizedText PowerTooltip { get; private set; }
		public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

		public override void SetStaticDefaults() {
			_ = AdditionalTooltip;
		}
	}
	
	public class Deadly : ModPrefix
	{
		public override PrefixCategory Category => PrefixCategory.AnyWeapon;

		public override float RollChance(Item item) 
		{
			return 0.3f;
		}

		public override bool CanRoll(Item item) 
		{
			if (item.CountsAsClass(DamageClass.Summon)) {return false;}
			return true;
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) 
		{
			damageMult *= 1f + 0.00f;
			critBonus += -20;
			knockbackMult *= 1f + -0.00f;
			useTimeMult *= 1.00f - 0.28f;
		}

		public override void ModifyValue(ref float valueMult) 
		{
			valueMult *= 1f + 0.5f;
		}

		public static LocalizedText PowerTooltip { get; private set; }
		public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

		public override void SetStaticDefaults() {
			_ = AdditionalTooltip;
		}
	}
	
	public class Nimble : ModPrefix
	{
		public override PrefixCategory Category => PrefixCategory.AnyWeapon;

		public override float RollChance(Item item) 
		{
			return 0.3f;
		}

		public override bool CanRoll(Item item) 
		{
			if (item.CountsAsClass(DamageClass.Summon)) {return false;}
			return true;
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) 
		{
			damageMult *= 1f + 0.00f;
			critBonus += 0;
			knockbackMult *= 1f + 0.00f;
			useTimeMult *= 1.00f - 0.18f;
		}

		public override void ModifyValue(ref float valueMult) 
		{
			valueMult *= 1f + 0.2f;
		}

		public static LocalizedText PowerTooltip { get; private set; }
		public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

		public override void SetStaticDefaults() {
			_ = AdditionalTooltip;
		}
	}
	
	public class Murderous : ModPrefix
	{
		public override PrefixCategory Category => PrefixCategory.AnyWeapon;

		public override float RollChance(Item item) 
		{
			return 0.2f;
		}

		public override bool CanRoll(Item item) 
		{
			if (item.CountsAsClass(DamageClass.Summon)) {return false;}
			return true;
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) 
		{
			damageMult *= 1f + 0.00f;
			critBonus += 50;
			knockbackMult *= 1f + 0.00f;
			useTimeMult *= 1.00f - -0.35f;
		}

		public override void ModifyValue(ref float valueMult) 
		{
			valueMult *= 1f + 0.2f;
		}

		public static LocalizedText PowerTooltip { get; private set; }
		public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

		public override void SetStaticDefaults() {
			_ = AdditionalTooltip;
		}
	}
	
	public class Nasty : ModPrefix
	{
		public override PrefixCategory Category => PrefixCategory.AnyWeapon;

		public override float RollChance(Item item) 
		{
			return 0.2f;
		}

		public override bool CanRoll(Item item) 
		{
			if (item.CountsAsClass(DamageClass.Summon)) {return false;}
			return true;
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) 
		{
			damageMult *= 1f + 0.40f;
			critBonus += 0;
			knockbackMult *= 1f + 0.00f;
			useTimeMult *= 1.00f + 0.40f;
		}

		public override void ModifyValue(ref float valueMult) 
		{
			valueMult *= 1f + 0.6f;
		}

		public static LocalizedText PowerTooltip { get; private set; }
		public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

		public override void SetStaticDefaults() {
			_ = AdditionalTooltip;
		}
	}
	
	public class Sluggish : ModPrefix
	{
		public override PrefixCategory Category => PrefixCategory.AnyWeapon;

		public override float RollChance(Item item) 
		{
			return 0.2f;
		}

		public override bool CanRoll(Item item) 
		{
			if (item.CountsAsClass(DamageClass.Summon)) {return false;}
			return true;
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) 
		{
			damageMult *= 1f + 0.00f;
			critBonus += 0;
			knockbackMult *= 1f + 0.00f;
			useTimeMult *= 1.00f - -0.20f;
		}

		public override void ModifyValue(ref float valueMult) 
		{
			valueMult *= 1f + 0.2f;
		}

		public static LocalizedText PowerTooltip { get; private set; }
		public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

		public override void SetStaticDefaults() {
			_ = AdditionalTooltip;
		}
	}
	
	public class Lazy : ModPrefix
	{
		public override PrefixCategory Category => PrefixCategory.AnyWeapon;

		public override float RollChance(Item item) 
		{
			return 0.2f;
		}

		public override bool CanRoll(Item item) 
		{
			if (item.CountsAsClass(DamageClass.Summon)) {return false;}
			return true;
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) 
		{
			damageMult *= 1f + 0.00f;
			critBonus += 0;
			knockbackMult *= 1f + -0.15f;
			useTimeMult *= 1.00f - -0.15f;
		}

		public override void ModifyValue(ref float valueMult) 
		{
			valueMult *= 1f + 0.2f;
		}

		public static LocalizedText PowerTooltip { get; private set; }
		public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

		public override void SetStaticDefaults() {
			_ = AdditionalTooltip;
		}
	}
	
	public class Annoying : ModPrefix
	{
		public override PrefixCategory Category => PrefixCategory.AnyWeapon;

		public override float RollChance(Item item) 
		{
			return 0.2f;
		}

		public override bool CanRoll(Item item) 
		{
			if (item.CountsAsClass(DamageClass.Summon)) {return false;}
			return true;
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) 
		{
			damageMult *= 1f + -0.10f;
			critBonus += 0;
			knockbackMult *= 1f + 0.00f;
			useTimeMult *= 1.00f - -0.10f;
		}

		public override void ModifyValue(ref float valueMult) 
		{
			valueMult *= 1f + 0.2f;
		}

		public static LocalizedText PowerTooltip { get; private set; }
		public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

		public override void SetStaticDefaults() {
			_ = AdditionalTooltip;
		}
	}
	
//***************
//**** MELEE ****
//***************
	
	public class Legendary : ModPrefix
	{
		public override PrefixCategory Category => PrefixCategory.AnyWeapon;

		public override float RollChance(Item item) 
		{
			return 0.2f;
		}

		public override bool CanRoll(Item item) 
		{
			if (item.CountsAsClass(DamageClass.Summon)) {return false;}
			return true;
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) 
		{
			damageMult *= 1f + 0.08f;
			critBonus += 4;
			knockbackMult *= 1f + 0.15f;
			useTimeMult *= 1.00f - 0.06f;
			scaleMult *= 1.00f + 0.15f;
		}

		public override void ModifyValue(ref float valueMult) 
		{
			valueMult *= 1f + 0.2f;
		}

		public static LocalizedText PowerTooltip { get; private set; }
		public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

		public override void SetStaticDefaults() {
			_ = AdditionalTooltip;
		}
	}
	
	public class Large : ModPrefix
	{
		public override PrefixCategory Category => PrefixCategory.Melee;

		public override float RollChance(Item item) 
		{
			return 0.3f;
		}

		public override bool CanRoll(Item item) 
		{
			return true;
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) 
		{
			damageMult *= 1f + 0.30f;
			critBonus += 0;
			knockbackMult *= 1f + 0.00f;
			useTimeMult *= 1.00f - -0.40f;
			scaleMult *= 1.00f + 0.30f;
		}

		public override void ModifyValue(ref float valueMult) 
		{
			valueMult *= 1f + 0.2f;
		}

		public static LocalizedText PowerTooltip { get; private set; }
		public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

		public override void SetStaticDefaults() {
			_ = AdditionalTooltip;
		}
	}
	
	public class Bulky : ModPrefix
	{
		public override PrefixCategory Category => PrefixCategory.Melee;

		public override float RollChance(Item item) 
		{
			return 0.2f;
		}

		public override bool CanRoll(Item item) 
		{
			return true;
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) 
		{
			damageMult *= 1f + 0.00f;
			critBonus += 0;
			knockbackMult *= 1f + 1.00f;
			useTimeMult *= 1.00f - -0.10f;
			scaleMult *= 1.00f + 0.30f;
		}

		public override void ModifyValue(ref float valueMult) 
		{
			valueMult *= 1f + -0.4f;
		}

		public static LocalizedText PowerTooltip { get; private set; }
		public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

		public override void SetStaticDefaults() {
			_ = AdditionalTooltip;
		}
	}
	
	public class Savage : ModPrefix
	{
		public override PrefixCategory Category => PrefixCategory.Melee;

		public override float RollChance(Item item) 
		{
			return 0.3f;
		}

		public override bool CanRoll(Item item) 
		{
			return true;
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) 
		{
			damageMult *= 1f + 0.00f;
			critBonus += 0;
			knockbackMult *= 1f + -0.50f;
			useTimeMult *= 1.00f - 0.00f;
			scaleMult *= 1.00f + 0.40f;
		}

		public override void ModifyValue(ref float valueMult) 
		{
			valueMult *= 1f + 1.0f;
		}

		public static LocalizedText PowerTooltip { get; private set; }
		public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

		public override void SetStaticDefaults() {
			_ = AdditionalTooltip;
		}
	}
	
	public class Massive : ModPrefix
	{
		public override PrefixCategory Category => PrefixCategory.Melee;

		public override float RollChance(Item item) 
		{
			return 0.2f;
		}

		public override bool CanRoll(Item item) 
		{
			return true;
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) 
		{
			damageMult *= 1f + 0.05f;
			critBonus += 0;
			knockbackMult *= 1f + 0.30f;
			useTimeMult *= 1.00f + 0.30f;
			scaleMult *= 1.00f + 0.50f;
		}

		public override void ModifyValue(ref float valueMult) 
		{
			valueMult *= 0.8f;
		}

		public static LocalizedText PowerTooltip { get; private set; }
		public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

		public override void SetStaticDefaults() {
			_ = AdditionalTooltip;
		}
	}
	public class Light : ModPrefix
	{
		public override PrefixCategory Category => PrefixCategory.Melee;

		public override float RollChance(Item item) 
		{
			return 0.3f;
		}

		public override bool CanRoll(Item item) 
		{
			return true;
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) 
		{
			damageMult *= 1f + 0.00f;
			critBonus += 0;
			knockbackMult *= 1f + -0.30f;
			useTimeMult *= 1.00f - 0.30f;
			scaleMult *= 1.00f + -0.25f;
		}

		public override void ModifyValue(ref float valueMult) 
		{
			valueMult *= 1f + 0.6f;
		}

		public static LocalizedText PowerTooltip { get; private set; }
		public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

		public override void SetStaticDefaults() {
			_ = AdditionalTooltip;
		}
	}
	public class Tiny : ModPrefix
	{
		public override PrefixCategory Category => PrefixCategory.Melee;

		public override float RollChance(Item item) 
		{
			return 0.2f;
		}

		public override bool CanRoll(Item item) 
		{
			return true;
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) 
		{
			damageMult *= 1f + 0.00f;
			critBonus += 0;
			knockbackMult *= 1f + 0.00f;
			useTimeMult *= 1.00f - 0.00f;
			scaleMult *= 1.00f + -0.35f;
		}

		public override void ModifyValue(ref float valueMult) 
		{
			valueMult *= 1f + 0.2f;
		}

		public static LocalizedText PowerTooltip { get; private set; }
		public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

		public override void SetStaticDefaults() {
			_ = AdditionalTooltip;
		}
	}
	public class Terrible : ModPrefix
	{
		public override PrefixCategory Category => PrefixCategory.Melee;

		public override float RollChance(Item item) 
		{
			return 0.2f;
		}

		public override bool CanRoll(Item item) 
		{
			return true;
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) 
		{
			damageMult *= 1f + -0.15f;
			critBonus += 0;
			knockbackMult *= 1f + 0.00f;
			useTimeMult *= 1.00f - 0.00f;
			scaleMult *= 1.00f + -0.15f;
		}

		public override void ModifyValue(ref float valueMult) 
		{
			valueMult *= 1f + 0.2f;
		}

		public static LocalizedText PowerTooltip { get; private set; }
		public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

		public override void SetStaticDefaults() {
			_ = AdditionalTooltip;
		}
	}
	public class Dull : ModPrefix
	{
		public override PrefixCategory Category => PrefixCategory.Melee;

		public override float RollChance(Item item) 
		{
			return 0.2f;
		}

		public override bool CanRoll(Item item) 
		{
			return true;
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) 
		{
			damageMult *= 1f + -0.10f;
			critBonus += 0;
			knockbackMult *= 1f + 0.00f;
			useTimeMult *= 1.00f - 0.00f;
			scaleMult *= 1.00f + 0.00f;
		}

		public override void ModifyValue(ref float valueMult) 
		{
			valueMult *= 1f + 0.2f;
		}

		public static LocalizedText PowerTooltip { get; private set; }
		public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

		public override void SetStaticDefaults() {
			_ = AdditionalTooltip;
		}
	}
	
//****************
//**** RANGED ****
//****************

	public class Demonic : ModPrefix
	{
		public override PrefixCategory Category => PrefixCategory.Ranged;

		public override float RollChance(Item item) 
		{
			return 0.2f;
		}

		public override bool CanRoll(Item item) 
		{
			return true;
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) 
		{
			damageMult *= 1f + 0.08f;
			critBonus += 6;
			knockbackMult *= 1f + 0.15f;
			useTimeMult *= 1.00f - 0.08f;
			shootSpeedMult *= 1.00f + 0.10f;
			//+10% acc
		}

		public override void ModifyValue(ref float valueMult) 
		{
			valueMult *= 1f + 0.3f;
		}

		public override IEnumerable<TooltipLine> GetTooltipLines(Item item) 
		{
			yield return new TooltipLine(Mod, "", "+10% accuracy")
			{
				IsModifier = true,
			};
		}

		public static LocalizedText PowerTooltip { get; private set; }
		public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

		public override void SetStaticDefaults() {
			_ = AdditionalTooltip;
		}
	}

	public class Sighted : ModPrefix
	{
		public override PrefixCategory Category => PrefixCategory.Ranged;

		public override float RollChance(Item item) 
		{
			return 0.25f;
		}

		public override bool CanRoll(Item item) 
		{
			return true;
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) 
		{
			damageMult *= 1f + 0.10f;
			critBonus += 0;
			knockbackMult *= 1f + 0.00f;
			useTimeMult *= 1.00f - -0.20f;
			shootSpeedMult *= 1.00f + 0.00f;
			//+60% acc
		}

		public override void ModifyValue(ref float valueMult) 
		{
			valueMult *= 1f + 0.3f;
		}

		public override IEnumerable<TooltipLine> GetTooltipLines(Item item) 
		{
			yield return new TooltipLine(Mod, "", "+60% accuracy")
			{
				IsModifier = true,
			};
		}

		public static LocalizedText PowerTooltip { get; private set; }
		public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

		public override void SetStaticDefaults() {
			_ = AdditionalTooltip;
		}
	}
	
	public class Hasty : ModPrefix
	{
		public override PrefixCategory Category => PrefixCategory.Ranged;

		public override float RollChance(Item item) 
		{
			return 0.2f;
		}

		public override bool CanRoll(Item item) 
		{
			return true;
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) 
		{
			damageMult *= 1f + 0.00f;
			critBonus += 0;
			knockbackMult *= 1f + -0.50f;
			useTimeMult *= 1.00f - 0.10f;
			shootSpeedMult *= 1.00f + 0.20f;
			//+25% acc
		}

		public override void ModifyValue(ref float valueMult) 
		{
			valueMult *= 1f + 0.6f;
		}
		
		public override IEnumerable<TooltipLine> GetTooltipLines(Item item) 
		{
			yield return new TooltipLine(Mod, "", "+25% accuracy")
			{
				IsModifier = true,
			};
		}

		public static LocalizedText PowerTooltip { get; private set; }
		public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

		public override void SetStaticDefaults() {
			_ = AdditionalTooltip;
		}
	}
	
	public class Intimidating : ModPrefix
	{
		public override PrefixCategory Category => PrefixCategory.Ranged;

		public override float RollChance(Item item) 
		{
			return 0.2f;
		}

		public override bool CanRoll(Item item) 
		{
			return true;
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) 
		{
			damageMult *= 1f + 0.30f;
			critBonus += 0;
			knockbackMult *= 1f + 0.00f;
			useTimeMult *= 1.00f - 0.00f;
			shootSpeedMult *= 1.00f + -0.50f;
			
		}

		public override void ModifyValue(ref float valueMult) 
		{
			valueMult *= 1f + 1.0f;
		}

		public static LocalizedText PowerTooltip { get; private set; }
		public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

		public override void SetStaticDefaults() {
			_ = AdditionalTooltip;
		}
	}
	
	public class Unreal : ModPrefix
	{
		public override PrefixCategory Category => PrefixCategory.Ranged;

		public override float RollChance(Item item) 
		{
			return 0.2f;
		}

		public override bool CanRoll(Item item) 
		{
			return true;
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) 
		{
			damageMult *= 1f + 0.00f;
			critBonus += 0;
			knockbackMult *= 1f + 0.80f;
			useTimeMult *= 1.00f - 0.00f;
			shootSpeedMult *= 1.00f + 0.60f;
		}

		public override void ModifyValue(ref float valueMult) 
		{
			valueMult *= 0.1f;
		}

		public static LocalizedText PowerTooltip { get; private set; }
		public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

		public override void SetStaticDefaults() {
			_ = AdditionalTooltip;
		}
	}
	
	public class Frenzying : ModPrefix
	{
		public override PrefixCategory Category => PrefixCategory.Ranged;

		public override float RollChance(Item item) 
		{
			return 0.25f;
		}

		public override bool CanRoll(Item item) 
		{
			return true;
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) 
		{
			damageMult *= 1f + -0.12f;
			critBonus += 0;
			knockbackMult *= 1f + 0.00f;
			useTimeMult *= 1.00f - 0.40f;
			shootSpeedMult *= 1.00f + -0.00f;
			//-50% acc
		}

		public override void ModifyValue(ref float valueMult) 
		{
			valueMult *= 1f + 0.2f;
		}
		
		public override IEnumerable<TooltipLine> GetTooltipLines(Item item) 
		{
			yield return new TooltipLine(Mod, "", "-50% accuracy")
			{
				IsModifier = true,
				IsModifierBad = true,
			};
		}

		public static LocalizedText PowerTooltip { get; private set; }
		public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

		public override void SetStaticDefaults() {
			_ = AdditionalTooltip;
		}
	}
	
	public class Akward : ModPrefix
	{
		public override PrefixCategory Category => PrefixCategory.Ranged;

		public override float RollChance(Item item) 
		{
			return 0.2f;
		}

		public override bool CanRoll(Item item) 
		{
			return true;
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) 
		{
			damageMult *= 1f + 0.00f;
			critBonus += 0;
			knockbackMult *= 1f + 0.00f;
			useTimeMult *= 1.00f - -0.15f;
			shootSpeedMult *= 1.00f + -0.15f;
		}

		public override void ModifyValue(ref float valueMult) 
		{
			valueMult *= 1f + 0.2f;
		}

		public static LocalizedText PowerTooltip { get; private set; }
		public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

		public override void SetStaticDefaults() {
			_ = AdditionalTooltip;
		}
	}
	
	public class Awful : ModPrefix
	{
		public override PrefixCategory Category => PrefixCategory.Ranged;

		public override float RollChance(Item item) 
		{
			return 0.2f;
		}

		public override bool CanRoll(Item item) 
		{
			return true;
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) 
		{
			damageMult *= 1f + -0.12f;
			critBonus += 0;
			knockbackMult *= 1f + 0.00f;
			useTimeMult *= 1.00f - -0.08f;
			shootSpeedMult *= 1.00f + -0.08f;
			//-15% acc
		}

		public override void ModifyValue(ref float valueMult) 
		{
			valueMult *= 1f + 0.2f;
		}

		public override IEnumerable<TooltipLine> GetTooltipLines(Item item) 
		{
			yield return new TooltipLine(Mod, "", "-15% accuracy")
			{
				IsModifier = true,
				IsModifierBad = true,
			};
		}

		public static LocalizedText PowerTooltip { get; private set; }
		public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

		public override void SetStaticDefaults() {
			_ = AdditionalTooltip;
		}
	}
	
	public class Lethargic : ModPrefix
	{
		public override PrefixCategory Category => PrefixCategory.Ranged;

		public override float RollChance(Item item) 
		{
			return 0.2f;
		}

		public override bool CanRoll(Item item) 
		{
			return true;
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) 
		{
			damageMult *= 1f + 0.00f;
			critBonus += 0;
			knockbackMult *= 1f + -0.40f;
			useTimeMult *= 1.00f - 0.00f;
			shootSpeedMult *= 1.00f + -0.40f;
		}

		public override void ModifyValue(ref float valueMult) 
		{
			valueMult *= 1f + 0.2f;
		}

		public static LocalizedText PowerTooltip { get; private set; }
		public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

		public override void SetStaticDefaults() {
			_ = AdditionalTooltip;
		}
	}
	
//***************	
//**** MAGIC ****
//***************	

	public class Mystic : ModPrefix
	{
		public override PrefixCategory Category => PrefixCategory.Magic;

		public override float RollChance(Item item) 
		{
			return 0.3f;
		}

		public override bool CanRoll(Item item) 
		{
			if (item.CountsAsClass(DamageClass.Summon)) {return false;}
			return true;
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) 
		{
			damageMult *= 1f + -0.15f;
			critBonus += 0;
			knockbackMult *= 1f + 0.00f;
			useTimeMult *= 1.00f - 0.30f;
			manaMult *= 1.00f + -0.30f;
		}

		public override void ModifyValue(ref float valueMult) 
		{
			valueMult *= 1f + 0.0f;
		}

		public static LocalizedText PowerTooltip { get; private set; }
		public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

		public override void SetStaticDefaults() {
			_ = AdditionalTooltip;
		}
	}
	
	public class Adept : ModPrefix
	{
		public override PrefixCategory Category => PrefixCategory.Magic;

		public override float RollChance(Item item) 
		{
			return 0.3f;
		}

		public override bool CanRoll(Item item) 
		{
			if (item.CountsAsClass(DamageClass.Summon)) {return false;}
			return true;
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) 
		{
			damageMult *= 1f + 0.00f;
			critBonus += 0;
			knockbackMult *= 1f + 0.00f;
			useTimeMult *= 1.00f - -0.12f;
			manaMult *= 1.00f + -0.50f;
		}

		public override void ModifyValue(ref float valueMult) 
		{
			valueMult *= 1f + -0.1f;
		}

		public static LocalizedText PowerTooltip { get; private set; }
		public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

		public override void SetStaticDefaults() {
			_ = AdditionalTooltip;
		}
	}
	
	public class Masterful : ModPrefix
	{
		public override PrefixCategory Category => PrefixCategory.Magic;

		public override float RollChance(Item item) 
		{
			return 0.25f;
		}

		public override bool CanRoll(Item item) 
		{
			if (item.CountsAsClass(DamageClass.Summon)) {return false;}
			return true;
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) 
		{
			damageMult *= 1f + 0.40f;
			critBonus += 0;
			knockbackMult *= 1f + 0.00f;
			useTimeMult *= 1.00f + 0.25f;
			manaMult *= 1.00f + 0.20f;
		}

		public override void ModifyValue(ref float valueMult) 
		{
			valueMult *= 1f + 0.2f;
		}
		 

		public static LocalizedText PowerTooltip { get; private set; }
		public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

		public override void SetStaticDefaults() {
			_ = AdditionalTooltip;
		}
	}
	
	public class Intense : ModPrefix
	{
		public override PrefixCategory Category => PrefixCategory.Magic;

		public override float RollChance(Item item) 
		{
			return 0.2f;
		}

		public override bool CanRoll(Item item) 
		{
			if (item.CountsAsClass(DamageClass.Summon)) {return false;}
			return true;
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) 
		{
			damageMult *= 1f + 0.00f;
			critBonus += 0;
			knockbackMult *= 1f + 1.00f;
			useTimeMult *= 1.00f - 0.00f;
			manaMult *= 1.00f + 0.10f;
		}

		public override void ModifyValue(ref float valueMult) 
		{
			valueMult *= 1f + 0.2f;
		}

		public static LocalizedText PowerTooltip { get; private set; }
		public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

		public override void SetStaticDefaults() {
			_ = AdditionalTooltip;
		}
	}
	
	public class Taboo : ModPrefix
	{
		public override PrefixCategory Category => PrefixCategory.Magic;

		public override float RollChance(Item item) 
		{
			return 0.2f;
		}

		public override bool CanRoll(Item item) 
		{
			if (item.CountsAsClass(DamageClass.Summon)) {return false;}
			return true;
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) 
		{
			damageMult *= 1f + 0.35f;
			critBonus += 0;
			knockbackMult *= 1f + 0.00f;
			useTimeMult *= 1.00f - 0.00f;
			manaMult *= 1.00f + 0.60f;
		}

		public override void ModifyValue(ref float valueMult) 
		{
			valueMult *= 1f + 1.0f;
		}

		public static LocalizedText PowerTooltip { get; private set; }
		public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

		public override void SetStaticDefaults() {
			_ = AdditionalTooltip;
		}
	}
	
	public class Celestial : ModPrefix
	{
		public override PrefixCategory Category => PrefixCategory.Magic;

		public override float RollChance(Item item) 
		{
			return 0.2f;
		}

		public override bool CanRoll(Item item) 
		{
			if (item.CountsAsClass(DamageClass.Summon)) {return false;}
			return true;
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) 
		{
			damageMult *= 1f + -0.20f;
			critBonus += 0;
			knockbackMult *= 1f + 0.00f;
			useTimeMult *= 1.00f - 0.00f;
			manaMult *= 1.00f + -1.00f;
		}

		public override void ModifyValue(ref float valueMult) 
		{
			valueMult *= 0.9f;
		}

		public static LocalizedText PowerTooltip { get; private set; }
		public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

		public override void SetStaticDefaults() {
			_ = AdditionalTooltip;
		}
	}
	
	public class Manic : ModPrefix
	{
		public override PrefixCategory Category => PrefixCategory.Magic;

		public override float RollChance(Item item) 
		{
			return 0.2f;
		}

		public override bool CanRoll(Item item) 
		{
			if (item.CountsAsClass(DamageClass.Summon)) {return false;}
			return true;
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) 
		{
			damageMult *= 1f + 0.00f;
			critBonus += 0;
			knockbackMult *= 1f + 0.00f;
			useTimeMult *= 1.00f - 0.35f;
			manaMult *= 1.00f + 0.30f;
		}

		public override void ModifyValue(ref float valueMult) 
		{
			valueMult *= 1f + 0.2f;
		}

		public static LocalizedText PowerTooltip { get; private set; }
		public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

		public override void SetStaticDefaults() {
			_ = AdditionalTooltip;
		}
	}
	public class Inept : ModPrefix
	{
		public override PrefixCategory Category => PrefixCategory.Magic;

		public override float RollChance(Item item) 
		{
			return 0.2f;
		}

		public override bool CanRoll(Item item) 
		{
			if (item.CountsAsClass(DamageClass.Summon)) {return false;}
			return true;
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) 
		{
			damageMult *= 1f + 0.00f;
			critBonus += 0;
			knockbackMult *= 1f + 0.00f;
			useTimeMult *= 1.00f - 0.00f;
			manaMult *= 1.00f + 0.20f;
		}

		public override void ModifyValue(ref float valueMult) 
		{
			valueMult *= 1f + 0.2f;
		}

		public static LocalizedText PowerTooltip { get; private set; }
		public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

		public override void SetStaticDefaults() {
			_ = AdditionalTooltip;
		}
	}
	public class Ignorant : ModPrefix
	{
		public override PrefixCategory Category => PrefixCategory.Magic;

		public override float RollChance(Item item) 
		{
			return 0.2f;
		}

		public override bool CanRoll(Item item) 
		{
			if (item.CountsAsClass(DamageClass.Summon)) {return false;}
			return true;
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) 
		{
			damageMult *= 1f + -0.05f;
			critBonus += 0;
			knockbackMult *= 1f + 0.00f;
			useTimeMult *= 1.00f - -0.10f;
			manaMult *= 1.00f + 0.05f;
		}

		public override void ModifyValue(ref float valueMult) 
		{
			valueMult *= 1f + 0.2f;
		}

		public static LocalizedText PowerTooltip { get; private set; }
		public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

		public override void SetStaticDefaults() {
			_ = AdditionalTooltip;
		}
	}
	public class Deranged : ModPrefix
	{
		public override PrefixCategory Category => PrefixCategory.Magic;

		public override float RollChance(Item item) 
		{
			return 0.2f;
		}

		public override bool CanRoll(Item item) 
		{
			if (item.CountsAsClass(DamageClass.Summon)) {return false;}
			return true;
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) 
		{
			damageMult *= 1f + -0.25f;
			critBonus += 0;
			knockbackMult *= 1f + 0.00f;
			useTimeMult *= 1.00f - 0.00f;
			manaMult *= 1.00f - 0.15f;
		}

		public override void ModifyValue(ref float valueMult) 
		{
			valueMult *= 1f + 0.2f;
		}

		public static LocalizedText PowerTooltip { get; private set; }
		public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

		public override void SetStaticDefaults() {
			_ = AdditionalTooltip;
		}
	}
	
//******************	
//**** SUMMONER ****
//******************
//does not include whips; those take melee prefixes

//heavy lifting not done in the prefixes for minion cost.

	public class Tactical : ModPrefix
	{
		public override PrefixCategory Category => PrefixCategory.Magic;

		public override float RollChance(Item item) 
		{
			return 0.4f;
		}

		public override bool CanRoll(Item item) 
		{
			if (item.CountsAsClass(DamageClass.Magic)) {return false;}
			return false;
			//return true;
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) 
		{
			damageMult *= 2.00f;
			critBonus += 0;
			knockbackMult *= 1f;
			useTimeMult *= 1.00f;
			manaMult *= 1.00f;
			//+100% minion cost
		}

		public override void ModifyValue(ref float valueMult) 
		{
			valueMult *= 1f + 0.2f;
		}
		
		public override IEnumerable<TooltipLine> GetTooltipLines(Item item) 
		{
			yield return new TooltipLine(Mod, "", "+100% minion cost")
			{
				IsModifier = true,
				IsModifierBad = true,
			};
		}

		public static LocalizedText PowerTooltip { get; private set; }
		public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

		public override void SetStaticDefaults() {
			_ = AdditionalTooltip;
		}
	}

	public class Relentless : ModPrefix
	{
		public override PrefixCategory Category => PrefixCategory.Magic;

		public override float RollChance(Item item) 
		{
			return 0.4f;
		}

		public override bool CanRoll(Item item) 
		{
			if (item.CountsAsClass(DamageClass.Magic)) {return false;}
			return false;
			//return true;
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) 
		{
			damageMult *= 0.90f;
			critBonus += 0;
			knockbackMult *= 1f;
			useTimeMult *= 1.00f;
			manaMult *= 1.00f;
			//-35% minion cost
		}

		public override void ModifyValue(ref float valueMult) 
		{
			valueMult *= 1f + 0.2f;
		}

		public override IEnumerable<TooltipLine> GetTooltipLines(Item item) 
		{
			yield return new TooltipLine(Mod, "", "-35% minion cost")
			{
				IsModifier = true,
			};
		}

		public static LocalizedText PowerTooltip { get; private set; }
		public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

		public override void SetStaticDefaults() {
			_ = AdditionalTooltip;
		}
	}

	public class Strong : ModPrefix
	{
		public override PrefixCategory Category => PrefixCategory.Magic;

		public override float RollChance(Item item) 
		{
			return 0.4f;
		}

		public override bool CanRoll(Item item) 
		{
			if (item.CountsAsClass(DamageClass.Magic)) {return false;}
			return true;
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) 
		{
			damageMult *= 1.00f;
			critBonus += 0;
			knockbackMult *= 1.80f;
			useTimeMult *= 1.00f;
			manaMult *= 1.00f;
		}

		public override void ModifyValue(ref float valueMult) 
		{
			valueMult *= 1f + -0.3f;
		}

		public static LocalizedText PowerTooltip { get; private set; }
		public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

		public override void SetStaticDefaults() {
			_ = AdditionalTooltip;
		}
	}

	public class Dangerous : ModPrefix
	{
		public override PrefixCategory Category => PrefixCategory.Magic;

		public override float RollChance(Item item) 
		{
			return 0.4f;
		}

		public override bool CanRoll(Item item) 
		{
			if (item.CountsAsClass(DamageClass.Magic)) {return false;}
			return true;
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) 
		{
			damageMult *= 1.15f;
			critBonus += 0;
			knockbackMult *= 1f;
			useTimeMult *= 1.00f;
			manaMult *= 1.00f;
		}

		public override void ModifyValue(ref float valueMult) 
		{
			valueMult *= 1f + 0.2f;
		}

		public static LocalizedText PowerTooltip { get; private set; }
		public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

		public override void SetStaticDefaults() {
			_ = AdditionalTooltip;
		}
	}
	
	public class Unpleasant : ModPrefix
	{
		public override PrefixCategory Category => PrefixCategory.Magic;

		public override float RollChance(Item item) 
		{
			return 0.4f;
		}

		public override bool CanRoll(Item item) 
		{
			if (item.CountsAsClass(DamageClass.Magic)) {return false;}
			return true;
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) 
		{
			damageMult *= 1.20f;
			critBonus += 0;
			knockbackMult *= 0.50f;
			useTimeMult *= 1.00f;
			manaMult *= 1.00f;
		}

		public override void ModifyValue(ref float valueMult) 
		{
			valueMult *= 1f + 1.0f;
		}

		public static LocalizedText PowerTooltip { get; private set; }
		public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

		public override void SetStaticDefaults() {
			_ = AdditionalTooltip;
		}
	}
	
	public class Bad : ModPrefix
	{
		public override PrefixCategory Category => PrefixCategory.Magic;

		public override float RollChance(Item item) 
		{
			return 0.2f;
		}

		public override bool CanRoll(Item item) 
		{
			if (item.CountsAsClass(DamageClass.Magic)) {return false;}
			return true;
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) 
		{
			damageMult *= 0.85f;
			critBonus += 0;
			knockbackMult *= 1f;
			useTimeMult *= 1.00f;
			manaMult *= 1.00f;
		}

		public override void ModifyValue(ref float valueMult) 
		{
			valueMult *= 1f + 0.0f;
		}

		public static LocalizedText PowerTooltip { get; private set; }
		public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

		public override void SetStaticDefaults() {
			_ = AdditionalTooltip;
		}
	}
	
	public class Uncaring : ModPrefix
	{
		public override PrefixCategory Category => PrefixCategory.Magic;

		public override float RollChance(Item item) 
		{
			return 0.2f;
		}

		public override bool CanRoll(Item item) 
		{
			if (item.CountsAsClass(DamageClass.Magic)) {return false;}
			return false;
			//return true;
		}

		public override void Apply(Item item) 
		{
			Player player = Main.LocalPlayer; //is it bad that this is localplayer  //myPlayer?
			Main.NewText(item.shoot);
			//player.maxMinions += 10;
			player.GetDamage(DamageClass.Generic) *= 10;
		}
		

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) 
		{
			damageMult *= 1.00f;
			critBonus += 0;
			knockbackMult *= 1f;
			useTimeMult *= 1.00f;
			manaMult *= 1.00f;
			//+30% minion cost
		}

		// This prefix doesn't affect any non-standard stats, so these additional tooltiplines aren't actually necessary, but this pattern can be followed for a prefix that does affect other stats.
		public override IEnumerable<TooltipLine> GetTooltipLines(Item item) 
		{
			yield return new TooltipLine(Mod, "", "+30% minion cost")
			{
				IsModifier = true,
				IsModifierBad = true,
			};
		}

		public override void ModifyValue(ref float valueMult) 
		{
			valueMult *= 1f + 0.2f;
		}

		public static LocalizedText PowerTooltip { get; private set; }
		public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

		public override void SetStaticDefaults() {
			_ = AdditionalTooltip;
		}
	}

}