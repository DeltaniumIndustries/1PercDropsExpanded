// Originated as a bastardized version of HealOnHit I was trying to use to make a missile weapon that builds walls. That was a stupid idea, but this script has a lot of great uses, namely recovering arrows or making liquid launchers. -Catlike
using System;
using XRL.Rules;
using XRL.World;
using XRL.World.Parts;

namespace XRL.World.Parts
{
	[Serializable]
	public class Cats_liquidCreateOnHit : IPart
	{
		public int Chance; //Percent chance of any object being created. Has no effect on the amount per creation.

		public string Blueprint = "Wooden Arrow"; //You'll generally want to redifine this in your XML as the arrow type. To create plurals, you'll have to make a blueprint that defines multiple items. This can be any object, though hazardous objects created through this class will not be attributed to an owner, failing to properly aggro creatures and joining their default teams.

		public string Amount = "1"; //Applied AFTER chance. Is converted to a string when used, allowing for dice rolls and random amounts per creation. 

		public override void Register(GameObject Object)
		{
			//Object.RegisterPartEvent(this, "WeaponHit"); //Activate this if you want to setup a melee weapon to create objects on hit.
			Object.RegisterPartEvent(this, "ProjectileHit");
			base.Register(Object);
		}

		public override bool FireEvent(Event E)
		{
			if (E.ID == "ProjectileHit") //Insert an or operator with E.ID == "WeaponHit" to add melee functionality, or E.ID == "WeaponThrowHit" for thrown.
			{
				GameObject gameObject = E.GetParameter("Defender") as GameObject;
				if (Chance > 100 || Chance < 1 || Chance == null)
                {
					Chance = 100;
					int i = Stat.Roll(Amount);
					if (gameObject.CurrentCell != null)
                    {
						while (i > 0)
                        {
							gameObject.CurrentCell.AddObject(Blueprint);
							i--;
						}
					}

				}
				else if (Stat.Random(1, 100) <= Chance && gameObject.CurrentCell != null) //Since everything after ProjectileHit occurs AFTER damage calculations, creatures killed by the attack will no longer be able to return their location data, and fail to create the object. If anyone finds a workaround to this, message me on reddit @ u/catlikespectator
				{
					int i = Stat.Roll(Amount);
					while (i > 0)
                    {
						gameObject.CurrentCell.AddObject(Blueprint); //Creates the object in the cell of the object the projectile hit. If it hit a wall, the projectile will be hidden under that wall.
						i--;
					}
				}
			}
			return base.FireEvent(E);
		}
	}
}
