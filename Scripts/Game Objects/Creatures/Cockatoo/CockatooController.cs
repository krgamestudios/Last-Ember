using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Creatures {
	public class CockatooController : MonoBehaviour, ICreature {
		//public access members
		public GameObject projectilePrefab;

		//components
		Rigidbody2D rigidBody;

		//gameplay
		const float moveForce = 10f;
		const float maxSpeed = 2.5f;

		int _horizontalMoveDirection;
		public int HorizontalMoveDirection {
			get {
				return _horizontalMoveDirection;
			}
			set {
				_horizontalMoveDirection = value;
				if (_horizontalMoveDirection >= 0) {
					transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y));
				} else {
					transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y));
				}
			}
		}

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
		float initialPositionY;

		bool detectedPlayer;
		float detectionDistance = 4f;
		float lastDetection = float.NegativeInfinity;
		float detectionDelay = 2f;

		void Awake() {
			rigidBody = GetComponent<Rigidbody2D>();

			HorizontalMoveDirection = -1;
			VerticalMoveDirection = 0;

			HealthValue = 1;
			DamageValue = 1;

			initialPositionY = rigidBody.position.y;

			rigidBody.position = new Vector2(rigidBody.position.x, rigidBody.position.y + 0.5f);

			StartCoroutine(BigFlap(5f));
		}

		void FixedUpdate() {
			HandleDetection();
			HandleVerticalMoveDirection();
			HandleMovement();
		}

		void OnCollisionEnter2D(Collision2D collision) {
			Vector2 normal = collision.GetContact(0).normal;

			if (collision.gameObject.tag == "Monster") {
				//turn around
				if (SameSign(collision.gameObject.GetComponent<ICreature>().HorizontalMoveDirection, HorizontalMoveDirection) || collision.gameObject.GetComponent<ICreature>().HorizontalMoveDirection == 0) {
					rigidBody.velocity = new Vector2(0f, rigidBody.velocity.y);
					HorizontalMoveDirection = -HorizontalMoveDirection;
				}
			}

			//collision with the player (when the player is not bouncing)
			if (collision.gameObject.tag == "Player" && normal != Vector2.down) {
				//turn around
				rigidBody.velocity = new Vector2(0f, rigidBody.velocity.y);
				HorizontalMoveDirection = -HorizontalMoveDirection;
			}
		}

		void HandleDetection() {
			bool detectedPlayer = Physics2D.Linecast(transform.position, transform.position + new Vector3((transform.localScale.x > 0 ? 1 : -1) * detectionDistance, -detectionDistance, 0), 1 << LayerMask.NameToLayer("Player"));

			if (detectedPlayer && Time.time - lastDetection > detectionDelay) {
				lastDetection = Time.time;

				GameObject go = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

				go.GetComponent<CockatooProjectileController>().HorizontalMoveDirection = HorizontalMoveDirection;
				go.GetComponent<CockatooProjectileController>().VerticalMoveDirection = -1;
				go.GetComponent<CockatooProjectileController>().DamageValue = DamageValue;
			}
		}

		void HandleVerticalMoveDirection() {
			if (rigidBody.position.y >= initialPositionY) {
				VerticalMoveDirection = -1;
			} else if (rigidBody.position.y < initialPositionY) {
				VerticalMoveDirection = 1;
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

			//move the entity in the correct direction vertically
			if (rigidBody.velocity.y * VerticalMoveDirection < maxSpeed) {
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

		//utilities
		IEnumerator SetDirectionIfNotMovingAfter(int direction, float delay) {
			yield return new WaitForSeconds(delay);
			//turn around if stopped
			if (Mathf.Abs(rigidBody.velocity.x) < 0.1f) {
				HorizontalMoveDirection = direction;
			}
		}

		//BUGFIX
		IEnumerator BigFlap(float delay) {
			while (true) {
				yield return new WaitForSeconds(delay);
				rigidBody.AddForce(Vector2.up * VerticalMoveDirection * moveForce * 5f);
			}
		}

		void OnDrawGizmos() {
			Gizmos.color = Color.red;
			Gizmos.DrawLine(transform.position, transform.position + new Vector3((transform.localScale.x > 0 ? 1 : -1) * detectionDistance, -detectionDistance, 0));
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