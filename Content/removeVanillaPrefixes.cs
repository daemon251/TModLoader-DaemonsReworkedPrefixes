using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using daemonReforge.Content.accessoryPrefixes;

namespace daemonReforge.Content.removeVanillaPrefixes
{ 
	//TO DO: 
	//spears / other melee weapons get the cool prefixes :D
	//use vanilla IDs
	//summoner minion slot reforge
	public class removeVanillaPrefixes : GlobalItem
	{
		//not actually removing the prefixes. is this bad?
		public override bool AllowPrefix(Item item, int pre)
		{
			if (item.accessory == true) {return true;}
			if (pre < 1) {return true;}
			if (pre <= 84) {return false;}
			return true;
		}
	}
}
