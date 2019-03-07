using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Structures {
	public class HazardController : MonoBehaviour {
		//internals
		int DamageValue { get; set; }
		DamagerController damagerController;

		void Awake() {
			DamageValue = 1;
		}

		void Start() {
			damagerController = GetComponent<DamagerController>();

			damagerController.PushOnTriggerStay((Collider2D collider) => {
				if (collider.gameObject.tag == "Player") {
					//deal damage to the player
					collider.gameObject.GetComponent<PlayerController>().HealthValue -= DamageValue;

					//NOTE: not every damager will deal damage
				}
			});
		}
	}
}