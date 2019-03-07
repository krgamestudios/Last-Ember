using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Creatures {
	public interface ICreature {
		//flags used to control other monsters
		int HorizontalMoveDirection { get; set; }
		int VerticalMoveDirection { get; set; }

		//used by the combad system
		int DamageValue { get; set; }
		int HealthValue { get; set; }
	}
}