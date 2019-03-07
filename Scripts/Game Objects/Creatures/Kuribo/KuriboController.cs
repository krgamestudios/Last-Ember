using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Creatures {
	public class KuriboController : MonoBehaviour, ICreature {
		//components
		Rigidbody2D rigidBody;

		//gameplay
		const float moveForce = 10f;
		const float maxSpeed = 2.5f;

		public int HorizontalMoveDirection { get; set; }
		public int VerticalMoveDirection { get; set; }
		public int DamageValue { get; set; }

		int _healthValue;
		public int HealthValue {
			get {
				return _healthValue;
			}
			set {
				_healthValue = value;
				if (_healthValue <= 0) {
					Destroy(gameObject);
				}
			}
		}

		//internals
		DamagerController damagerController;

		void Awake() {
			rigidBody = GetComponent<Rigidbody2D>();

			HorizontalMoveDirection = -1;
			VerticalMoveDirection = 0;

			HealthValue = 1;
			DamageValue = 1;
		}

		void Start() {
			damagerController = GetComponentInChildren<DamagerController>();

			damagerController.PushOnTriggerEnter((Collider2D collider) => {
				if (collider.gameObject.tag == "Player") {
					//deal damage to the player
					collider.gameObject.GetComponent<PlayerController>().HealthValue -= DamageValue;

					//NOTE: not every damager will deal damage
				}
			});
		}

		void FixedUpdate() {
			HandleMovement();
		}

		void OnCollisionEnter2D(Collision2D collision) {
			//handle bouncing on a monster
			Vector2 normal = collision.GetContact(0).normal;

			if (collision.gameObject.tag == "Monster") {
				if (normal == Vector2.up) {
					//bounce
					rigidBody.AddForce(new Vector2(0f, 480f));
				} else {
					//turn around
					if (SameSign(collision.gameObject.GetComponent<ICreature>().HorizontalMoveDirection, HorizontalMoveDirection) || collision.gameObject.GetComponent<ICreature>().HorizontalMoveDirection == 0) {
						rigidBody.velocity = new Vector2(0f, rigidBody.velocity.y);
						HorizontalMoveDirection = -HorizontalMoveDirection;
					}
				}
			}

			//collision with the player (when the player is not bouncing)
			if (collision.gameObject.tag == "Player" && normal != Vector2.down) {
				//turn around
				rigidBody.velocity = new Vector2(0f, rigidBody.velocity.y);
				HorizontalMoveDirection = -HorizontalMoveDirection;
			}
		}

		void HandleMovement() {
			//turn around if stopped
			if (Mathf.Abs(rigidBody.velocity.x) < 0.1f) {
				StartCoroutine(SetDirectionIfNotMovingAfter(-HorizontalMoveDirection, 0.1f));
			}

			//move the entity in this direction, if not at max speed
			if (Mathf.Abs(rigidBody.velocity.x) < maxSpeed) {
				rigidBody.AddForce(Vector2.right * HorizontalMoveDirection * moveForce);
			}

			//slow the entity down when it's travelling too fast
			if (Mathf.Abs (rigidBody.velocity.x) > maxSpeed) {
				rigidBody.velocity = new Vector2 (Mathf.Sign (rigidBody.velocity.x) * maxSpeed, rigidBody.velocity.y);
			}

			if (Mathf.Abs (rigidBody.velocity.y) > maxSpeed) {
				rigidBody.velocity = new Vector2 (rigidBody.velocity.x, Mathf.Sign (rigidBody.velocity.y) * maxSpeed);
			}
		}

		//utilities
		IEnumerator SetDirectionIfNotMovingAfter(int direction, float delay) {
			yield return new WaitForSeconds(delay);
			//turn around if stopped
			if (Mathf.Abs(rigidBody.velocity.x) < 0.1f) {
				HorizontalMoveDirection = direction;
			}
		}

		bool SameSign(float num1, float num2) {
			if (num1 > 0 && num2 > 0) {
				return true;
			}
			if (num1 < 0 && num2 < 0) {
				return true;
			}
			//if either is zero, return false
			return false;
		}
	}
}