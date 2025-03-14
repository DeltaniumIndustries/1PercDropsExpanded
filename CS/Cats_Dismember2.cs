using System;
using XRL.Rules;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Parts.Skill;

namespace XRL.World.Parts
{
	[Serializable]
	public class Cats_DismemberProjectile : IPart
	{
		public int Chance; //Percent chance. Has no effect on the amount per 

		public bool CanDecapitate = false;

		public string Strength = "20"; //save difficulty

		public string Amount = "1"; //Applied AFTER chance. Is converted to a string when used, allowing for dice rolls and random amounts

		public override void Register(GameObject Object)
		{
			//Object.RegisterPartEvent(this, "WeaponHit"); 
			Object.RegisterPartEvent(this, "ProjectileHit");
			base.Register(Object);
		}

		public override bool FireEvent(Event E)
		{
			if (E.ID == "ProjectileHit") //Insert an or operator with E.ID == "WeaponHit" to add melee functionality, or E.ID == "WeaponThrowHit" for thrown.
			{
				GameObject gameObject = E.GetParameter("Defender") as GameObject; //this tree sets chance to 100 if someone defines it wrong or not at all. I didn't know about using in100 when I made this, but I'm not changing it
				if (Chance > 100 || Chance < 1 || Chance == null)
                {
					Chance = 100;
				}
				if (Stat.Random(1, 100) <= Chance && gameObject.CurrentCell != null) //Since everything after ProjectileHit occurs AFTER damage calculations, creatures killed by the attack will no longer be able to return their location data, cancelling the effect
				{
					int i = Stat.Roll(Amount);
					while (i > 0)
                    {
						int num = Strength.RollCached();
						if (!gameObject.MakeSave("Agility", num, null, null, "Dismember"))
                            {
								Axe_Dismember.Dismember(ParentObject, gameObject, null, null, null, null, assumeDecapitate: CanDecapitate);
							}
						i--;
					}
				}
			}
			return base.FireEvent(E);
		}
	}
}
