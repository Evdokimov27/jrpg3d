﻿using System.Collections;
using UnityEngine;

namespace RPGCharacterAnims
{
    public enum RPGCharacterState
    {
        Idle = 0,
        Move = 1,
        Jump = 2,
        DoubleJump = 3,
        Fall = 4,
        Swim = 5,
        Block = 6,
        ClimbLadder = 7,
        Roll = 8
    }

    public class RPGCharacterMovementController:SuperStateMachine
    {
        public RPGCharacterState rpgCharacterState;

        //Components.
        [HideInInspector] public UnityEngine.AI.NavMeshAgent navMeshAgent;
        private SuperCharacterController superCharacterController;
        private RPGCharacterController rpgCharacterController;
        private RPGCharacterInputController rpgCharacterInputController;
        private Rigidbody rb;
        private Animator animator;
        private CapsuleCollider capCollider;

        [HideInInspector] public bool useMeshNav = false;
        [HideInInspector] public Vector3 lookDirection { get; private set; }
        [HideInInspector] public bool isKnockback;
        public float knockbackMultiplier = 1f;

		[Header("Jumping")]
        public float gravity = 25.0f;
        public float jumpAcceleration = 5.0f;
        public float jumpHeight = 3.0f;
        public float doubleJumpHeight = 4f;
        private bool doublejumped = false;
		[HideInInspector] public bool canJump;
        [HideInInspector] public bool canDoubleJump = false;

		[Header("Movement")]
        public float movementAcceleration = 90.0f;
        public float walkSpeed = 3.5f;
        public float runSpeed = 6f;
        public float sprintSpeed = 12;
        private readonly float rotationSpeed = 40f;
        public float groundFriction = 50f;
		[HideInInspector] public Vector3 currentVelocity;
        [HideInInspector] public bool isMoving = false;
        [HideInInspector] public bool canMove = true;
        [HideInInspector] public bool crouch;
        [HideInInspector] public bool isSprinting;

		[Header("Rolling")]
        public float rollSpeed = 8;
        public float rollduration = 0.35f;
        private int rollNumber;
		[HideInInspector] public bool isRolling = false;

		[Header("Air Control")]
		public float inAirSpeed = 6f;

		[Header("Swimming")]
		public float swimSpeed = 4f;
        public float waterFriction = 3f;

        private void Start()
        {
			//Get other RPG Character components.
            superCharacterController = GetComponent<SuperCharacterController>();
            rpgCharacterController = GetComponent<RPGCharacterController>();
            rpgCharacterInputController = GetComponent<RPGCharacterInputController>();
            navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();

			//Check if Animator exists, otherwise pause script.
            animator = GetComponentInChildren<Animator>();
			if(animator == null)
			{
				Destroy(this);
				return;
			}

			//Setup Collider and Rigidbody for collisions.
            capCollider = GetComponent<CapsuleCollider>();
            rb = GetComponent<Rigidbody>();
            if(rb != null)
            {
                //Set restraints on startup if using Rigidbody.
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            }

            //Set currentState to idle on startup.
            currentState = RPGCharacterState.Idle;
            rpgCharacterState = RPGCharacterState.Idle;

			SwitchCollisionOn();
        }

		#region Updates

		/*
		Update is normally run once on every frame update. We won't be using it in this case, since the SuperCharacterController component sends a callback Update called SuperUpdate. 
		SuperUpdate is recieved by the SuperStateMachine, and then fires further callbacks depending on the state
		void Update() 
		{
		}
		*/

		//Put any code in here you want to run BEFORE the state's update function. This is run regardless of what state you're in.
		protected override void EarlyGlobalSuperUpdate()
        {
        }

        //Put any code in here you want to run AFTER the state's update function.  This is run regardless of what state you're in.
        protected override void LateGlobalSuperUpdate()
        {
            //Move the player by our velocity every frame.
            transform.position += currentVelocity * superCharacterController.deltaTime;
            //If using Navmesh nagivation, update values.
            if(navMeshAgent != null)
            {
                if(useMeshNav)
                {
					isSprinting = false;
                    if(navMeshAgent.velocity.sqrMagnitude > 0)
                    {
						animator.SetBool("Moving", true);
                        animator.SetFloat("Velocity Z", navMeshAgent.velocity.magnitude);
						return;
                    }
                    else
                    {
                        animator.SetFloat("Velocity Z", 0);
                    }
                }
            }
            //If alive and is moving, set animator.
            if(!useMeshNav && !rpgCharacterController.isDead && canMove)
            {
                if(currentVelocity.magnitude > 0 && rpgCharacterInputController.HasMoveInput())
                {
                    isMoving = true;
                    animator.SetBool("Moving", true);
                    //Swimming.
                    if(rpgCharacterState == RPGCharacterState.Swim)
                    {
                        animator.SetFloat("Velocity Z", transform.InverseTransformDirection(currentVelocity).z);
                    }
                    //Not swimming.
                    else
                    {
                        animator.SetFloat("Velocity Z", currentVelocity.magnitude);
                    }
                }
                else
                {
                    isMoving = false;
                    animator.SetBool("Moving", false);
                }
                //Sprinting.
                if(isSprinting)
                {
                    animator.SetBool("Sprint", true);
                }
                else
                {
                    animator.SetBool("Sprint", false);
                }
            }
            //Aiming.
            if(rpgCharacterController.isFacing && !isSprinting)
            {
                Facing();
            }
            else
            {
				//Rotate towards movement direction.
				if(!rpgCharacterController.isTargeting || rpgCharacterController.injured || isSprinting)
				{
					if(rpgCharacterInputController.HasMoveInput() && canMove)
					{
						RotateTowardsMovementDir();
					}
				}
				//Otherwise rotate towards Target. (strafing)
				else
				{
					RotateTowardsTarget(rpgCharacterController.target.transform.position);
				}
			}

			//Update animator with local movement values.
			animator.SetFloat("Velocity X", transform.InverseTransformDirection(currentVelocity).x);
			animator.SetFloat("Velocity Z", transform.InverseTransformDirection(currentVelocity).z);
		}

		#endregion

		private bool AcquiringGround()
        {
            return superCharacterController.currentGround.IsGrounded(false, 0.01f);
        }

        public bool MaintainingGround()
        {
            return superCharacterController.currentGround.IsGrounded(true, 0.5f);
        }

        public void RotateGravity(Vector3 up)
        {
            lookDirection = Quaternion.FromToRotation(transform.up, up) * lookDirection;
        }

        // Calculate the initial velocity of a jump based off gravity and desired maximum height attained
        private float CalculateJumpSpeed(float jumpHeight, float gravity)
        {
            return Mathf.Sqrt(2 * jumpHeight * gravity);
        }

		#region States

		//Below are the state functions. Each one is called based on the name of the state, so when currentState = Idle, we call Idle_EnterState. If currentState = Jump, we call Jump_SuperUpdate()
		private void Idle_EnterState()
        {
            superCharacterController.EnableSlopeLimit();
            superCharacterController.EnableClamping();
            canJump = true;
            doublejumped = false;
            canDoubleJump = false;
            animator.SetInteger("Jumping", 0);
        }

        //Run every frame we are in the idle state.
        private void Idle_SuperUpdate()
        {
            //If Jump.
            if(rpgCharacterInputController.allowedInput && rpgCharacterController.Jump())
            {
                currentState = RPGCharacterState.Jump;
                rpgCharacterState = RPGCharacterState.Jump;
                return;
            }
            if(!MaintainingGround())
            {
                currentState = RPGCharacterState.Fall;
                rpgCharacterState = RPGCharacterState.Fall;
                return;
            }
            if(rpgCharacterInputController.HasMoveInput() && canMove)
            {
                currentState = RPGCharacterState.Move;
                rpgCharacterState = RPGCharacterState.Move;
                return;
            }
            //Apply friction to slow to a halt.
            currentVelocity = Vector3.MoveTowards(currentVelocity, Vector3.zero, groundFriction * superCharacterController.deltaTime);
        }

        //Run once when exit the idle state.
        private void Idle_ExitState()
        {
        }

        private void Move_SuperUpdate()
        {
            //If Jump.
            if(rpgCharacterInputController.allowedInput && rpgCharacterController.Jump())
            {
                currentState = RPGCharacterState.Jump;
                rpgCharacterState = RPGCharacterState.Jump;
                return;
            }
			//Falling.
            if(!MaintainingGround())
            {
                currentState = RPGCharacterState.Fall;
                rpgCharacterState = RPGCharacterState.Fall;
                return;
            }
            //Set speed determined by movement type.
            if(rpgCharacterInputController.HasMoveInput() && canMove)
            {
                //Keep strafing animations from playing.
                animator.SetFloat("Velocity X", 0F);
                //Injured limp.
                if(rpgCharacterController.injured)
                {
                    currentVelocity = Vector3.MoveTowards(currentVelocity, rpgCharacterInputController.moveInput * 1.35f, movementAcceleration * superCharacterController.deltaTime);
                    return;
                }
                //Sprinting.
                if(isSprinting)
                {
                    currentVelocity = Vector3.MoveTowards(currentVelocity, rpgCharacterInputController.moveInput * sprintSpeed, movementAcceleration * superCharacterController.deltaTime);
                    return;
                }
                //Strafing or Walking.
                if(rpgCharacterController.isTargeting)
                {
                    currentVelocity = Vector3.MoveTowards(currentVelocity, rpgCharacterInputController.moveInput * walkSpeed, movementAcceleration * superCharacterController.deltaTime);
                    if(rpgCharacterController.weapon != Weapon.RELAX)
                    {
                        RotateTowardsTarget(rpgCharacterController.target.transform.position);
                    }
                    return;
                }
                //Run.
                currentVelocity = Vector3.MoveTowards(currentVelocity, rpgCharacterInputController.moveInput * runSpeed, movementAcceleration * superCharacterController.deltaTime);
            }
			//No movement, go to Idle state.
            else
            {
                currentState = RPGCharacterState.Idle;
                rpgCharacterState = RPGCharacterState.Idle;
                return;
            }
        }

        private void Jump_EnterState()
        {
            superCharacterController.DisableClamping();
            superCharacterController.DisableSlopeLimit();
            currentVelocity += superCharacterController.up * CalculateJumpSpeed(jumpHeight, gravity);
            canJump = false;
            animator.SetInteger("Jumping", 1);
            animator.SetTrigger("JumpTrigger");
        }

		private void Jump_SuperUpdate()
		{
			Vector3 planarMoveDirection = Math3d.ProjectVectorOnPlane(superCharacterController.up, currentVelocity);
			Vector3 verticalMoveDirection = currentVelocity - planarMoveDirection;
			//Falling.
			if(currentVelocity.y < 0)
			{
				currentVelocity = planarMoveDirection;
				currentState = RPGCharacterState.Fall;
				rpgCharacterState = RPGCharacterState.Fall;
				return;
			}
			planarMoveDirection = Vector3.MoveTowards(planarMoveDirection, rpgCharacterInputController.moveInput * inAirSpeed, jumpAcceleration * superCharacterController.deltaTime);
			verticalMoveDirection -= superCharacterController.up * gravity * superCharacterController.deltaTime;
			currentVelocity = planarMoveDirection + verticalMoveDirection;
		}

		private void DoubleJump_EnterState()
        {
            currentVelocity += superCharacterController.up * CalculateJumpSpeed(doubleJumpHeight, gravity);
            canDoubleJump = false;
            doublejumped = true;
            animator.SetInteger("Jumping", 3);
            animator.SetTrigger("JumpTrigger");
        }

        private void DoubleJump_SuperUpdate()
        {
            Jump_SuperUpdate();
        }

        private void DoubleJump()
        {
            if(!doublejumped)
            {
                canDoubleJump = true;
            }
            if(rpgCharacterInputController.inputJump && canDoubleJump && !doublejumped)
            {
                currentState = RPGCharacterState.DoubleJump;
                rpgCharacterState = RPGCharacterState.DoubleJump;
            }
        }

        private void Fall_EnterState()
        {
            if(!doublejumped)
            {
                canDoubleJump = true;
            }
            superCharacterController.DisableClamping();
            superCharacterController.DisableSlopeLimit();
            canJump = false;
            animator.SetInteger("Jumping", 2);
            animator.SetTrigger("JumpTrigger");
        }

        private void Fall_SuperUpdate()
        {
            if(AcquiringGround())
            {
                currentVelocity = Math3d.ProjectVectorOnPlane(superCharacterController.up, currentVelocity);
                currentState = RPGCharacterState.Idle;
                rpgCharacterState = RPGCharacterState.Idle;
                return;
            }
            DoubleJump();
            currentVelocity -= superCharacterController.up * gravity * superCharacterController.deltaTime;
        }

        private void Swim_EnterState()
        {
			superCharacterController.DisableClamping();
			superCharacterController.DisableSlopeLimit();
			rpgCharacterController.isTargeting = false;
			rpgCharacterController.isFacing = false;
			rpgCharacterController.canAction = false;
			animator.SetBool("Targeting", false);
			animator.SetBool("Facing", false);
			animator.SetTrigger("SwimTrigger");
			animator.SetBool("Swimming", true);
			if(rpgCharacterController.rpgCharacterWeaponController != null)
			{
				StartCoroutine(rpgCharacterController.rpgCharacterWeaponController._HideAllWeapons(false, true));
			}

			//Scale collider to match position of character.
			superCharacterController.radius = 1.5f;
			if(capCollider != null)
			{
				capCollider.radius = 1.5f;
			}
		}

        private void Swim_ExitState()
        {
            if(capCollider != null)
            {
                capCollider.radius = 0.6f;
            }
            superCharacterController.radius = 0.6f;
            rpgCharacterController.canAction = true;
        }

        private void Swim_SuperUpdate()
        {
            //Swim movement.
            currentVelocity = Vector3.MoveTowards(currentVelocity, rpgCharacterInputController.moveInput * swimSpeed, waterFriction * superCharacterController.deltaTime);

            //Apply friction to slow character to a halt on vertical axis.
            currentVelocity = Vector3.MoveTowards(currentVelocity, new Vector3(currentVelocity.x, 0, currentVelocity.z), (waterFriction * 5) * superCharacterController.deltaTime);

            if(rpgCharacterInputController.inputJump && (!rpgCharacterInputController.inputTarget || !rpgCharacterInputController.HasBlockInput()))
            {
                SwimDescent();
            }
            else if(rpgCharacterInputController.inputJump)
            {
                SwimAscend();
            }
        }

		private void ClimbLadder_EnterState()
		{
			if(capCollider != null)
			{
				capCollider.center = new Vector3(0, 0.75f, 0);
			}
		}

		private void ClimbLadder_ExitState()
		{
			if(capCollider != null)
			{
				capCollider.center = new Vector3(0, 1.25f, 0);
			}
		}

		#endregion

		public void SwimAscend()
        {
            currentVelocity += superCharacterController.up * CalculateJumpSpeed(1, gravity);
			animator.SetInteger("Action", 1);
            animator.SetTrigger("JumpTrigger");
        }

        public void SwimDescent()
        {
            currentVelocity -= superCharacterController.up * CalculateJumpSpeed(1, gravity);
			animator.SetInteger("Action", 2);
			animator.SetTrigger("JumpTrigger");
        }

        public void DirectionalRoll()
        {
            //Check which way the dash is pressed relative to the character facing.
            float angle = Vector3.Angle(rpgCharacterInputController.moveInput, -transform.forward);
            float sign = Mathf.Sign(Vector3.Dot(transform.up, Vector3.Cross(rpgCharacterInputController.inputFacing, transform.forward)));
            //Angle in [-179,180].
            float signed_angle = angle * sign;
            //Angle in 0-360.
            float angle360 = (signed_angle + 180) % 360;
            //Deternime the animation to play based on the angle.
            if(angle360 > 315 || angle360 < 45)
            {
                StartCoroutine(_Roll(1));
            }
            if(angle360 > 45 && angle360 < 135)
            {
                StartCoroutine(_Roll(2));
            }
            if(angle360 > 135 && angle360 < 225)
            {
                StartCoroutine(_Roll(3));
            }
            if(angle360 > 225 && angle360 < 315)
            {
                StartCoroutine(_Roll(4));
            }
        }

        /// <summary>
        /// Character Roll.
        /// </summary>
        /// <param name="1">Forward.</param>
        /// <param name="2">Right.</param>
        /// <param name="3">Backward.</param>
        /// <param name="4">Left.</param>
        public IEnumerator _Roll(int roll)
        {
            rollNumber = roll;

            currentState = RPGCharacterState.Roll;
            rpgCharacterState = RPGCharacterState.Roll;

            animator.SetInteger("Action", rollNumber);
            animator.SetTrigger("RollTrigger");
            isRolling = true;
			rpgCharacterController.Lock(true, true, true, 0, 0.5f);
			yield return new WaitForSeconds(rollduration);
            isRolling = false;

            currentState = RPGCharacterState.Idle;
            rpgCharacterState = RPGCharacterState.Idle;
        }

        public void SwitchCollisionOff()
        {
            canMove = false;
            superCharacterController.enabled = false;
            animator.applyRootMotion = true;
            if(rb != null)
            {
                rb.isKinematic = false;
            }
        }

        public void SwitchCollisionOn()
        {
            canMove = true;
            superCharacterController.enabled = true;
            animator.applyRootMotion = false;
            if(rb != null)
            {
                rb.isKinematic = true;
            }
        }

        private void RotateTowardsMovementDir()
        {
			if(rpgCharacterInputController.moveInput != Vector3.zero)
			{
				transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(rpgCharacterInputController.moveInput), Time.deltaTime * rotationSpeed);
			}
		}

        private void RotateTowardsTarget(Vector3 targetPosition)
        {
			Quaternion targetRotation = Quaternion.LookRotation(targetPosition - new Vector3(transform.position.x, 0, transform.position.z));
			transform.eulerAngles = Vector3.up * Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetRotation.eulerAngles.y, (rotationSpeed * Time.deltaTime) * rotationSpeed);
		}

        private void Facing()
        {
			//If right joystick is moved, use that for facing.
			if(rpgCharacterInputController.HasFacingInput())
			{
				Vector3 joyDirection = new Vector3(rpgCharacterInputController.inputFacing.x, 0, -rpgCharacterInputController.inputFacing.y);
				Quaternion joyRotation = Quaternion.LookRotation(joyDirection.normalized);
				transform.rotation = joyRotation;
			}
			//Otherwise, look at mouse pointer.
			else
			{
				// Generate a plane that intersects the transform's position with an upwards normal.
				Plane playerPlane = new Plane(Vector3.up, transform.position);

				// Generate a ray from the cursor position
				Ray ray = Camera.main.ScreenPointToRay(rpgCharacterInputController.inputMouseFacing);

				// Determine the point where the cursor ray intersects the plane.
				// This will be the point that the object must look towards to be looking at the mouse.
				// Raycasting to a Plane object only gives us a distance, so we'll have to take the distance,
				// then find the point along that ray that meets that distance.  This will be the point
				// to look at.
				float hitdist = 0.0f;

				// If the ray is parallel to the plane, Raycast will return false.
				if(playerPlane.Raycast(ray, out hitdist))
				{
					// Get the point along the ray that hits the calculated distance.
					Vector3 targetPoint = ray.GetPoint(hitdist);

					//Convert the point to a Vector2 along the ground plane.
					//// Determine the target rotation.  This is the rotation if the transform looks at the target point.
					Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);

					//// Rotate towards the target point.
					transform.rotation = targetRotation;
				}
			}
        }

        public IEnumerator _Knockback(Vector3 knockDirection, int knockBackAmount, int variableAmount)
        {
            isKnockback = true;
            StartCoroutine(_KnockbackForce(knockDirection, knockBackAmount, variableAmount));
            yield return new WaitForSeconds(.1f);
            isKnockback = false;
        }

        private IEnumerator _KnockbackForce(Vector3 knockDirection, int knockBackAmount, int variableAmount)
        {
            while(isKnockback)
            {
                rb.AddForce(knockDirection * ((knockBackAmount + Random.Range(-variableAmount, variableAmount)) * (knockbackMultiplier * 10)), ForceMode.Impulse);
                yield return null;
            }
        }

        private void OnTriggerEnter(Collider collide)
        {
            //Entering a water volume.
            if(collide.gameObject.layer == 4)
            {
                currentState = RPGCharacterState.Swim;
                rpgCharacterState = RPGCharacterState.Swim;
            }
            //Near a ladder.
            else if(collide.transform.parent != null)
            {
                if(collide.transform.parent.name.Contains("Ladder"))
                {
                    rpgCharacterController.isNearLadder = true;
                    rpgCharacterController.ladder = collide.gameObject;
                }
            }
            //Near a cliff.
            else if(collide.transform.name.Contains("Cliff"))
            {
                rpgCharacterController.isNearCliff = true;
                rpgCharacterController.cliff = collide.gameObject;
            }
        }

        private void OnTriggerExit(Collider collide)
        {
            //Leaving a water volume.
            if(collide.gameObject.layer == 4)
            {
                animator.SetBool("Swimming", false);
                currentState = RPGCharacterState.Jump;
                rpgCharacterState = RPGCharacterState.Jump;
            }
            //Leaving a ladder.
            else if(collide.transform.parent != null)
            {
                if(collide.transform.parent.name.Contains("Ladder"))
                {
                    rpgCharacterController.isNearLadder = false;
                    rpgCharacterController.ladder = null;
                }
            }
            //leaving a cliff.
            else if(collide.transform.name.Contains("Cliff"))
            {
                rpgCharacterController.isNearCliff = false;
                rpgCharacterController.cliff = null;
            }
        }

        //Keep character from moving.
        public void LockMovement()
        {
			canMove = false;
            animator.SetBool("Moving", false);
            animator.applyRootMotion = true;
            currentVelocity = new Vector3(0, 0, 0);
        }

        public void UnlockMovement()
        {
            canMove = true;
            animator.applyRootMotion = false;
        }
    }
}