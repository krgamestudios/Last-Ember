using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Creatures {
	public class CockatooProjectileController : MonoBehaviour {
		//public access members
		public int HorizontalMoveDirection { get; set; }
		public int VerticalMoveDirection { get; set; }
		public int DamageValue { get; set; }

		//internal members
		Rigidbody2D rigidBody;
		const float moveForce = 10f;
		const float maxSpeed = 5f;

		DamagerController damagerController;

		void Start() {
			rigidBody = GetComponent<Rigidbody2D>();

			damagerController = GetComponent<DamagerController>();

			damagerController.PushOnTriggerEnter((Collider2D collider) => {
				if (collider.gameObject.tag == "Player") {
					collider.gameObject.GetComponent<PlayerController>().HealthValue -= DamageValue;
				}

				Destroy(gameObject);
			});
		}

		void FixedUpdate() {
			HandleMovement();

			//handle grapphics
			transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x) * HorizontalMoveDirection, transform.localScale.y);
		}

		void HandleMovement() {
			//move the entity in this direction, if not at max speed
			if (Mathf.Abs(rigidBody.velocity.x) < maxSpeed) {
				rigidBody.AddForce(Vector2.right * HorizontalMoveDirection * moveForce);
			}

			if (Mathf.Abs(rigidBody.velocity.y) < maxSpeed) {
				rigidBody.AddForce(Vector2.up * VerticalMoveDirection * moveForce);
			}

			//slow the entity down when it's travelling too fast
			if (Mathf.Abs (rigidBody.velocity.x) > maxSpeed) {
				rigidBody.velocity = new Vector2 (Mathf.Sign (rigidBody.velocity.x) * maxSpeed, rigidBody.velocity.y);
			}

			if (Mathf.Abs (rigidBody.velocity.y) > maxSpeed) {
				rigidBody.velocity = new Vector2 (rigidBody.velocity.x, Mathf.Sign (rigidBody.velocity.y) * maxSpeed);
			}
		}
	}
}