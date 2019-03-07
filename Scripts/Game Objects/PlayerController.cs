using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpriterDotNetUnity;

public class PlayerController : MonoBehaviour {
	//internal components
	GameObject spriteObject;
	UnityAnimator animator;
	Rigidbody2D rigidBody;
	BoxCollider2D currentBoxCollider;

	//constants
	const float deadZone = 0.25f;

	//gameplay
	[Header("Movement Settings")]
	public float moveForce = 10f;
	public float jumpForce = 400;
	public float maxSpeed = 3f;
	public float fallSpeed = 8f;
	public float dashMultiplier = 2f;
	public float straightJumpVerticalMultiplier = 1.1f;
	public float straightJumpHorizontalMultiplier = 0.5f;
//	public float rollingJumpVerticalMultiplier = 1.0f;
//	public float rollingJumpHorizontalMultiplier = 1.0f;
	public float jumpMaxSpeedMultiplier = 1.1f;
	public float wallJumpMaxSpeedMultiplier = 0.5f;
	public float wallHuggedMultiplier = 0.5f;

	//basic movement
	float horizontalInput = 0f;
	float verticalInput = 0f;
	bool jumping = false;
	bool grounded = false;
	bool wallHugged = false;
	bool wallJumping = false;
	const float groundedProjection = 0.08f;

	//dashing movement
	float dashValue = 0f;
	float dashTime = float.NegativeInfinity;
	bool dashLatch = true;

	//movement modifiers
	float horizontalModifier = 1f;
	float verticalModifier = 1f;
	float dashModifier = 1f;
	float maxSpeedModifier = 1f;

	//combat
	DamagerController[] damagerControllers;
	HUDCanvas hud;
	bool invulnerable = false;

	public int DamageValue { get; set; }
	public int HealthValue {
		set {
			//prevent damage while invulnerable
			if (value < hud.FlameLevel && invulnerable) {
				return;
			}

			//check death
			if (value < 0) {
				Debug.Log("YOU DIED");
			}

			//trigger hitstun if damage taken
			if (value < hud.FlameLevel) {
				StartCoroutine(TriggerHitStun(0.8f));
			}

			//set the graphics
			hud.FlameLevel = value;
		}
		get {
			return hud.FlameLevel;
		}
	}

	//graphical modifiers
	float prevTimeScale;
	float prevLocalScaleX;

	//gameplay
	float friction = 0f;

	void Awake() {
		rigidBody = GetComponent<Rigidbody2D>();
		currentBoxCollider = GetComponent<BoxCollider2D>();
		prevTimeScale = Time.timeScale; //BUGFIX: waking when unpaused
		prevLocalScaleX = transform.localScale.x;
	}

	void Start() {
		//BUGFIX: Set the sprite sorting order
		GetComponentsInChildren<SpriterDotNetBehaviour>()[0].SortingLayer = "Player";

		//get the HUD
		hud = Object.FindObjectOfType<HUDCanvas>();

		DamageValue = 1;
		HealthValue = 4;

		//get and disable the damagers
		damagerControllers = GetComponentsInChildren<DamagerController>();
		foreach(DamagerController dmgr in damagerControllers) {
			dmgr.gameObject.SetActive(false);

			dmgr.PushOnTriggerEnter((Collider2D collider) => {
				if (collider.gameObject.tag == "Monster" && collider.gameObject.GetComponent<Creatures.ICreature>().HealthValue > 0) {
					collider.gameObject.GetComponent<Creatures.ICreature>().HealthValue -= DamageValue;
					hud.SparkLevel++;

					//bounce on attack
					Vector2 normal = collider.bounds.ClosestPoint(transform.position) - transform.position;
					normal.Normalize();

					if (normal.y < 0f) {
						jumping = true;
					}
				}
			});
		}
	}

	void Update() {
		if (Time.timeScale > 0f && Time.timeScale == prevTimeScale && HandleAnimation()) {
			HandleInput();
		}
		prevTimeScale = Time.timeScale;

		//Debug.LogFormat("{0} {1}", animator.CurrentAnimation.Name, currentBoxCollider.size.y);

		//handle gameplay
		if (friction >= 1f) {
			hud.SparkLevel++;
			friction -= 1f;
		}

		if (animator.CurrentAnimation.Name != "Run" && animator.CurrentAnimation.Name != "Wall Slide") {
			friction = 0f;
		}
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
				jumping = true;
			}
		}
	}

	void HandleInput() {
		//determine if on the ground (using coyote time)
		bool trueGrounded;
		trueGrounded  = Physics2D.Linecast(transform.position + new Vector3(currentBoxCollider.offset.x, currentBoxCollider.offset.y, 0), transform.position + new Vector3(transform.localScale.x * (currentBoxCollider.offset.x), -groundedProjection, 0), 1 << LayerMask.NameToLayer("Ground"));
		trueGrounded |= Physics2D.Linecast(transform.position + new Vector3(currentBoxCollider.offset.x, currentBoxCollider.offset.y, 0), transform.position + new Vector3(transform.localScale.x * (currentBoxCollider.offset.x + currentBoxCollider.size.x / 2.1f), -groundedProjection, 0), 1 << LayerMask.NameToLayer("Ground"));
		trueGrounded |= Physics2D.Linecast(transform.position + new Vector3(currentBoxCollider.offset.x, currentBoxCollider.offset.y, 0), transform.position + new Vector3(transform.localScale.x * (currentBoxCollider.offset.x - currentBoxCollider.size.x / 2.1f), -groundedProjection, 0), 1 << LayerMask.NameToLayer("Ground"));

		if (trueGrounded) {
			grounded = true;
		} else {
			StartCoroutine(SetGroundedWithDelay(false, 0.1f)); //coyote physics: 100ms
		}

		//determine wall hugging
		wallHugged  = Physics2D.Linecast(transform.position + new Vector3(currentBoxCollider.offset.x, currentBoxCollider.offset.y, 0), transform.position + new Vector3(transform.localScale.x * (currentBoxCollider.offset.x + currentBoxCollider.size.x / 2 + groundedProjection), currentBoxCollider.size.y * 0.05f, 0), 1 << LayerMask.NameToLayer("Ground"));
		wallHugged &= Physics2D.Linecast(transform.position + new Vector3(currentBoxCollider.offset.x, currentBoxCollider.offset.y, 0), transform.position + new Vector3(transform.localScale.x * (currentBoxCollider.offset.x + currentBoxCollider.size.x / 2 + groundedProjection), currentBoxCollider.size.y * 0.5f, 0), 1 << LayerMask.NameToLayer("Ground"));
		wallHugged &= Physics2D.Linecast(transform.position + new Vector3(currentBoxCollider.offset.x, currentBoxCollider.offset.y, 0), transform.position + new Vector3(transform.localScale.x * (currentBoxCollider.offset.x + currentBoxCollider.size.x / 2 + groundedProjection), currentBoxCollider.size.y * 0.95f, 0), 1 << LayerMask.NameToLayer("Ground"));

		//reset multipliers under regular conditions
		if (trueGrounded && !jumping) {
			horizontalModifier = 1f;
			verticalModifier = 1f;
			maxSpeedModifier = 1f;
		}

		//get inputs
		verticalInput = GamePad.GetAxis(CAxis.LY);
		horizontalInput = GamePad.GetAxis(CAxis.LX);

		//determine vertical input
		if (Mathf.Abs(verticalInput) < deadZone) { //no input
			verticalInput = 0f;

			//handle stop crouching
			if (animator.CurrentAnimation.Name == "Crouch") {
				animator.Play("Crouch to Idle");
			}
		} else { //yes input
			if (verticalInput < 0) { //looking up
				if (grounded && animator.CurrentAnimation.Name == "Idle" && Mathf.Abs(horizontalInput) < deadZone) {
					animator.Play("Idle to Lookup");
				}
			} else { //crouching down
				//from rolling jump to crouch
				if (grounded && animator.CurrentAnimation.Name == "Rolling Jump") {
					animator.Play("Crouch");
				}
				else if (grounded && Mathf.Abs(rigidBody.velocity.y) > 0.0001f) { //explicitly don't check for animation here; "Straight Jump Landing" passes through here for some reason
					animator.Play("Rolling Jump");
				}
				//if not already crouching
				else if (grounded && animator.CurrentAnimation.Name != "Crouch" && animator.CurrentAnimation.Name != "Idle to Crouch") {
					animator.Play("Idle to Crouch");
				}
			}
		}

		//determine if walking
		if (Mathf.Abs(horizontalInput) < deadZone) { //no input
			//reset multipliers under regular conditions
			if (grounded && !jumping) {
				dashModifier = 1f;
				if (animator.CurrentAnimation.Name == "Run") {
					animator.Play("Walk");
				}
			}

			if (dashLatch) {
				//capture the time of last release for dashing
				dashTime = Time.time;
				dashLatch = false;
			}

			//stop walking
			horizontalInput = 0f;
			if (animator.CurrentAnimation.Name == "Walk") {
				//animator.Play("Walk to Idle"); //TODO: enable this
				animator.Play("Idle"); //TODO: remove this (why?)
			}
		} else { //yes input
			if (animator.CurrentAnimation.Name == "Crouch") {
				//TODO: slides/rolls

				horizontalInput = horizontalInput > 0 ? 0.0001f : -0.0001f; //TMP
			} else if (animator.CurrentAnimation.Name == "Lookup") {
				animator.Play("Lookup to Idle");
			} else if (wallHugged && !grounded && (animator.CurrentAnimation.Name != "Brace on Wall" && animator.CurrentAnimation.Name != "Wall Slide" && animator.CurrentAnimation.Name != "Wall Kick")) {
				animator.Play("Brace on Wall");
			} else {
				//check if dashing
				if (Time.time - dashTime < 0.2f && SameSign(horizontalInput, dashValue) && grounded && !dashLatch) {
					dashModifier = dashMultiplier;
					horizontalInput = maxSpeed * dashModifier * (horizontalInput > 0 ? 1 : -1);
					animator.Play("Run");
					StartCoroutine(BuildingFriction("Run", 0.05f));
				}

				//capture the value for dashing
				dashValue = horizontalInput;

				//BUGFIX
				dashLatch = true;
			}

			//BUGFIX: landing into a run
			if (animator.CurrentAnimation.Name == "Idle" && dashModifier != 1f) {
				animator.Play("Run");
				StartCoroutine(BuildingFriction("Run", 0.05f));
			}

			//start walking
			if (animator.CurrentAnimation.Name == "Idle") {
				animator.Play("Idle to Walk");
			}

			//flip direction
			if (Time.timeScale > 0f) {
				prevLocalScaleX = transform.localScale.x;
				transform.localScale = new Vector3(horizontalInput > 0 ? 1 : -1, 1, 1);

				//play turning animations
				if (prevLocalScaleX != transform.localScale.x) {
					if (animator.CurrentAnimation.Name == "Idle" || animator.CurrentAnimation.Name == "Idle to Walk" || animator.CurrentAnimation.Name == "Walk" || animator.CurrentAnimation.Name == "Lookup to Idle") {
						animator.Play("Ground Turn");
					}

					if (animator.CurrentAnimation.Name == "Straight Jump Rising" || animator.CurrentAnimation.Name == "Straight Jump Crest"  || animator.CurrentAnimation.Name == "Straight Jump Falling") {
						animator.Play("Straight Jump Turn");
					}

					if (animator.CurrentAnimation.Name == "Rolling Jump") {
						animator.Play("Rolling Jump Turn");
					}
				}
			}
		}

		//determine if jumping
		if (GamePad.GetState().Pressed(CButton.A) && grounded) {
			jumping = true;

			maxSpeedModifier = jumpMaxSpeedMultiplier;

			if (animator.CurrentAnimation.Name == "Crouch") {
				animator.Play("Rolling Jump");
			}
			else if (Mathf.Abs(horizontalInput) < deadZone) {
				animator.Play("Begin Straight Jump");
				horizontalModifier = straightJumpHorizontalMultiplier;
				verticalModifier = straightJumpVerticalMultiplier;
			} else {
				animator.Play("Begin Rolling Jump");
				//horizontalModifier = rollingJumpHorizontalMultiplier;
				//verticalModifier = rollingJumpVerticalMultiplier;
			}
		}

		if (GamePad.GetState().Pressed(CButton.A) && !grounded && wallHugged && (animator.CurrentAnimation.Name == "Brace on Wall" || animator.CurrentAnimation.Name == "Wall Slide")) {
			wallJumping = true;

			maxSpeedModifier = jumpMaxSpeedMultiplier;

			animator.Play("Wall Kick");
		}

		//determine if attacking on the ground
		if (GamePad.GetState().Pressed(CButton.B) && grounded) {
			if (verticalInput < -deadZone) { //yes up input
				if (animator.CurrentAnimation.Name == "Run") {
					animator.Play("Grounded Upward Slash"); //TODO: replace with "Running Upward Slash"
				} else {
					animator.Play("Grounded Upward Slash");
				}
				StartCoroutine(EnableDamagerForPeriod(0, 0.3f));
			} else { //no vertical input
				if (animator.CurrentAnimation.Name == "Run") {
					animator.Play("Grounded Forward Slash"); //TODO: replace with "Running Forward Slash"
				} else {
					animator.Play("Grounded Forward Slash");
				}
				StartCoroutine(EnableDamagerForPeriod(1, 0.3f));
			}
		}

		//determine if attacking in the air
		if (GamePad.GetState().Pressed(CButton.B) && !grounded) {
			if (verticalInput < -deadZone) { //yes up input
				animator.Play("Airborn Upward Slash");
				StartCoroutine(EnableDamagerForPeriod(0, 0.3f));
			} else if (verticalInput > deadZone) { //yes down input
				animator.Play("Airborn Downward Slash");
				StartCoroutine(EnableDamagerForPeriod(2, 0.3f));
			} else { //no vertical input
				animator.Play("Airborn Forward Slash");
				StartCoroutine(EnableDamagerForPeriod(1, 0.3f));
			}
		}

		//determine if releasing the attack button
		if (GamePad.GetState().Released(CButton.B)) {
			foreach (DamagerController dmgr in damagerControllers) {
				dmgr.gameObject.SetActive(false);
			}
		}

		//BUGFIX: prevent crouch-gliding and slash-gliding
		if (grounded && (animator.CurrentAnimation.Name == "Crouch")) {
			horizontalInput = 0f;
			dashModifier = 1f;
		}

		//BUGFIX: falling animations after walking/running off a cliff
		if (rigidBody.velocity.y < -0.0001f && (animator.CurrentAnimation.Name == "Walk" || animator.CurrentAnimation.Name == "Run")) {
			animator.Play("Straight Jump Falling");
		}
	}

	void HandleMovement() {
		//stop the player if input in that direction has been removed
		if (horizontalInput * rigidBody.velocity.x <= 0 && grounded) {
			rigidBody.velocity = new Vector2 (rigidBody.velocity.x * 0.85f, rigidBody.velocity.y);
		}

		//move in the inputted direction, if not at max speed
		if (horizontalInput * rigidBody.velocity.x < maxSpeed * dashModifier * maxSpeedModifier) {
			rigidBody.AddForce (Vector2.right * horizontalInput * moveForce * horizontalModifier);
		}

		//slow the player down when it's travelling too fast
		if (Mathf.Abs (rigidBody.velocity.x) > maxSpeed * dashModifier * maxSpeedModifier) {
			rigidBody.velocity = new Vector2 (Mathf.Sign (rigidBody.velocity.x) * maxSpeed * dashModifier * maxSpeedModifier, rigidBody.velocity.y);
		}

		if (rigidBody.velocity.y < -fallSpeed * (wallHugged ? wallHuggedMultiplier : 1f)) {
			rigidBody.velocity = new Vector2 (rigidBody.velocity.x, Mathf.Sign (rigidBody.velocity.y) * fallSpeed * (wallHugged ? wallHuggedMultiplier : 1f));
		}

		//jump up
		if (jumping) {
			rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0f); //max v-jump speed
			rigidBody.AddForce (new Vector2 (0f, jumpForce * verticalModifier));
			jumping = false;
		}

		if (wallJumping) {
			rigidBody.velocity = new Vector2(0f, 0f); //wall-jump from zero
			rigidBody.AddForce (new Vector2 (-transform.localScale.x * maxSpeed * jumpForce, jumpForce * verticalModifier));
			maxSpeedModifier = wallJumpMaxSpeedMultiplier;
			wallJumping = false;
		}
	}

	bool HandleAnimation() {
		if (spriteObject == null) {
			foreach (Transform child in transform) {
				if (child.name ==  "Ember") {
					spriteObject = child.gameObject;
					break;
				}
			}
		}

		if (animator == null && spriteObject != null) {
			animator = spriteObject.GetComponent<SpriterDotNetBehaviour>().Animator;
			animator.AnimationFinished += HandleAnimationTransitions;
		}

		//determine statue state
		if (!PauseManager.Instance.Paused && animator.CurrentAnimation.Name == "Statue" &&
			(
				//NOTE: carbon input really needs an "any key"
				//any face button
				GamePad.GetState().Pressed(CButton.A) || GamePad.GetState().Pressed(CButton.B) || GamePad.GetState().Pressed(CButton.X) || GamePad.GetState().Pressed(CButton.Y) || //only pressed this loop
				//any axis
				Mathf.Abs(GamePad.GetAxis(CAxis.LX)) >= deadZone || Mathf.Abs(GamePad.GetAxis(CAxis.LY)) >= deadZone || Mathf.Abs(GamePad.GetAxis(CAxis.RX)) >= deadZone || Mathf.Abs(GamePad.GetAxis(CAxis.RY)) >= deadZone
			)
		) {
			animator.Play("Statue to Idle");
			return false;
		}

		if (animator.CurrentAnimation.Name == "Statue to Idle") {
			return false;
		}

		//switch from regular animations to transition animations
		if (animator.CurrentAnimation.Name == "Straight Jump Rising" && rigidBody.velocity.y <= 0) {
			animator.Play("Straight Jump Crest");
		}

		if (animator.CurrentAnimation.Name == "Straight Jump Falling" && rigidBody.velocity.y >= 0) {
			animator.Play("Straight Jump Landing");
		}

		if (animator.CurrentAnimation.Name == "Rolling Jump" && grounded) {
			animator.Play("Straight Jump Landing"); //deliberately reuse this
		}

		//start looking up
		if (animator.CurrentAnimation.Name == "Idle" && verticalInput < 0 && Mathf.Abs(rigidBody.velocity.x) < 0.0001f) {
			animator.Play("Idle to Lookup");
		}

		//stop looking up
		if (animator.CurrentAnimation.Name == "Lookup" && verticalInput >= 0) {
			animator.Play("Lookup to Idle");
		}

		//BUGFIX: bouncing off of a wall
		if (!grounded && !wallHugged && animator.CurrentAnimation.Name == "Wall Slide") {
			animator.Play("Rolling Jump");
		}

		//if ever simply falling (or attacking)
		if (!grounded && !wallHugged && rigidBody.velocity.y < 0 && animator.CurrentAnimation.Name != "Straight Jump Crest" && animator.CurrentAnimation.Name != "Straight Jump Turn" && animator.CurrentAnimation.Name != "Rolling Jump Turn" && animator.CurrentAnimation.Name != "Rolling Jump" && animator.CurrentAnimation.Name != "Airborn Upward Slash" && animator.CurrentAnimation.Name != "Airborn Downward Slash" && animator.CurrentAnimation.Name != "Airborn Forward Slash") {
			animator.Play("Straight Jump Falling");
		}

		//hit a wall
		if (animator.CurrentAnimation.Name != "Brace on Wall" && animator.CurrentAnimation.Name != "Wall Slide" && animator.CurrentAnimation.Name != "Wall Kick" && wallHugged && !grounded) {
			animator.Play("Brace on Wall");
		}

		//reach the bottom of a wall-slide (or hit the ground when turning)
		if ((animator.CurrentAnimation.Name == "Brace on Wall" || animator.CurrentAnimation.Name == "Wall Slide" || animator.CurrentAnimation.Name == "Straight Jump Turn" || animator.CurrentAnimation.Name == "Rolling Jump Turn") && grounded) {
			animator.Play("Straight Jump Landing");
		}

		//if bouncing
		if (animator.CurrentAnimation.Name == "Straight Jump Landing" && !grounded && rigidBody.velocity.y > 0f) {
			animator.Play("Begin Rolling Jump");
		}

		//handle bounding box
		if (
			animator.CurrentAnimation.Name == "Crouch" ||
			animator.CurrentAnimation.Name == "Idle to Crouch" ||
			animator.CurrentAnimation.Name == "Rolling Jump" ||
			animator.CurrentAnimation.Name == "Begin Rolling Jump" ||
			(rigidBody.velocity.y > 0 && animator.CurrentAnimation.Name == "Straight Jump Landing") ||
			animator.CurrentAnimation.Name == "Brace on Wall" ||
			animator.CurrentAnimation.Name == "Wall Slide" ||
			animator.CurrentAnimation.Name == "Wall Kick"
		) {
			//half box
			if (currentBoxCollider != GetComponents<BoxCollider2D>()[1]) {
				currentBoxCollider.enabled = false;
				currentBoxCollider = GetComponents<BoxCollider2D>()[1];
				currentBoxCollider.enabled = true;
			}
		} else {
			//full box
			if (currentBoxCollider != GetComponents<BoxCollider2D>()[0]) {
				currentBoxCollider.enabled = false;
				currentBoxCollider = GetComponents<BoxCollider2D>()[0];
				currentBoxCollider.enabled = true;
			}
		}

		//peek with the camera
		CameraController camController = Object.FindObjectOfType<CameraController>();
		if (camController.GetPeek() == Vector2.zero) {
			if (animator.CurrentAnimation.Name == "Lookup") {
				camController.SetPeek(new Vector2(0f, 4f));
			}
			if (animator.CurrentAnimation.Name == "Crouch") {
				camController.SetPeek(new Vector2(0f, -4f));
			}
		}
		if (camController.GetPeek() != Vector2.zero) {
			if (animator.CurrentAnimation.Name != "Lookup" && animator.CurrentAnimation.Name != "Crouch") {
				camController.ResetPeek();
			}
		}

		return true;
	}

	//internal callbacks
	void HandleAnimationTransitions(string name) {
		//NOTE: This handles the end of transitional animations
		switch(name) {
			case "Statue to Idle":
				animator.Play("Idle");
				break;

			case "Idle to Walk":
				animator.Play("Walk");
				break;

			case "Walk to Idle":
				animator.Play("Idle");
				break;

			case "Begin Straight Jump":
				animator.Play("Straight Jump Rising");
				break;

			case "Straight Jump Crest":
				animator.Play("Straight Jump Falling");
				break;

			case "Straight Jump Landing":
				animator.Play("Idle");
				break;

			case "Begin Rolling Jump":
				animator.Play("Rolling Jump");
				break;

			case "Idle to Crouch":
				animator.Play("Crouch");
				break;

			case "Crouch to Idle":
				animator.Play("Idle");
				break;

			case "Idle to Lookup":
				animator.Play("Lookup");
				break;

			case "Lookup to Idle":
				animator.Play("Idle");
				break;

			case "Brace on Wall":
				animator.Play("Wall Slide");
				StartCoroutine(BuildingFriction("Wall Slide", 0.1f));
				break;

			case "Wall Kick":
				animator.Play("Rolling Jump");
				break;

			case "Grounded Forward Slash":
				animator.Play("Idle");
				break;

			case "Grounded Upward Slash":
				if (verticalInput < -deadZone && Mathf.Abs(rigidBody.velocity.x) < 0.0001f) {
					animator.Play("Lookup");
				} else {
					if (dashModifier != 1f) {
						animator.Play("Run");
						StartCoroutine(BuildingFriction("Run", 0.05f));
					} else {
						animator.Play("Idle");
					}
				}
				break;

			case "Airborn Upward Slash":
			case "Airborn Downward Slash":
			case "Airborn Forward Slash":
				if (rigidBody.velocity.y > 0f) {
					animator.Play("Straight Jump Rising");
				} else {
					animator.Play("Straight Jump Falling");
				}
				break;

			case "Ground Turn":
				animator.Play("Idle");
				break;

			case "Straight Jump Turn":
				if (rigidBody.velocity.y > 0f) {
					animator.Play("Straight Jump Rising");
				} else {
					animator.Play("Straight Jump Falling");
				}
				break;

			case "Rolling Jump Turn":
				animator.Play("Rolling Jump");
				break;
		}
	}

	//utilities
	void OnDrawGizmos() {
		if (currentBoxCollider != null) {
			Gizmos.color = Color.red;
			Gizmos.DrawLine(transform.position + new Vector3(currentBoxCollider.offset.x, currentBoxCollider.offset.y, 0), transform.position + new Vector3(transform.localScale.x * (currentBoxCollider.offset.x), -groundedProjection, 0));
			Gizmos.DrawLine(transform.position + new Vector3(currentBoxCollider.offset.x, currentBoxCollider.offset.y, 0), transform.position + new Vector3(transform.localScale.x * (currentBoxCollider.offset.x + currentBoxCollider.size.x / 2.1f), -groundedProjection, 0));
			Gizmos.DrawLine(transform.position + new Vector3(currentBoxCollider.offset.x, currentBoxCollider.offset.y, 0), transform.position + new Vector3(transform.localScale.x * (currentBoxCollider.offset.x - currentBoxCollider.size.x / 2.1f), -groundedProjection, 0));

			Gizmos.color = Color.red;
			Gizmos.DrawLine(transform.position + new Vector3(currentBoxCollider.offset.x, currentBoxCollider.offset.y, 0), transform.position + new Vector3(transform.localScale.x * (currentBoxCollider.offset.x + currentBoxCollider.size.x / 2 + groundedProjection), currentBoxCollider.size.y * 0.05f, 0));
			Gizmos.DrawLine(transform.position + new Vector3(currentBoxCollider.offset.x, currentBoxCollider.offset.y, 0), transform.position + new Vector3(transform.localScale.x * (currentBoxCollider.offset.x + currentBoxCollider.size.x / 2 + groundedProjection), currentBoxCollider.size.y * 0.5f, 0));
			Gizmos.DrawLine(transform.position + new Vector3(currentBoxCollider.offset.x, currentBoxCollider.offset.y, 0), transform.position + new Vector3(transform.localScale.x * (currentBoxCollider.offset.x + currentBoxCollider.size.x / 2 + groundedProjection), currentBoxCollider.size.y * 0.95f, 0));
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

	IEnumerator SetGroundedWithDelay(bool value, float delay) {
		yield return new WaitForSeconds(delay);
		grounded = value;
	}

	IEnumerator EnableDamagerForPeriod(int index, float delay) {
		damagerControllers[index].gameObject.SetActive(true);
		yield return new WaitForSeconds(delay);
		damagerControllers[index].gameObject.SetActive(false);
	}

	IEnumerator BuildingFriction(string animation, float increment) {
		while (animator.CurrentAnimation.Name == animation) {
			friction += increment;
			yield return new WaitForSeconds(0.1f);
		}
	}

	IEnumerator TriggerHitStun(float delay) {
		invulnerable = true;
		StartCoroutine(TriggerHitStunGraphic(0.1f, delay - 0.1f));
		yield return new WaitForSeconds(delay);
		invulnerable = false;
	}

	IEnumerator TriggerHitStunGraphic(float redDelay, float opacityDelay) {
		SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

		//flash red
		foreach (var renderer in spriteRenderers) {
			renderer.color = Color.red;
		}

		yield return new WaitForSeconds(redDelay);

		foreach (var renderer in spriteRenderers) {
			renderer.color = Color.white;
			renderer.material.color = new Color(1f, 1f, 1f, 0.75f);
		}

		yield return new WaitForSeconds(opacityDelay);

		foreach (var renderer in spriteRenderers) {
			renderer.material.color = Color.white;
		}		
	}
}
