// Decompiled with JetBrains decompiler
// Type: PlayerMovement
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 93635EE7-440D-45FB-9828-DA116675EBE0
// Assembly location: C:\Users\carlb\Downloads\MonoBleedingEdge\All First Person Movement Test_Data\All First Person Movement Test_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using UnityEngine;

#nullable disable
public class PlayerMovement : MonoBehaviour
{
  [Header("Movement")]
  private float moveSpeed;
  public float walkSpeed;
  public float sprintSpeed;
  public float slideSpeed;
  public float wallRunSpeed;
  private float desiredMoveSpeed;
  private float lastDesiredMoveSpeed;
  public float speedIncreaseMultiplier;
  public float slopeIncreaseMultiplier;
  public float groundDrag;
  [Header("Jumping")]
  public float jumpForce;
  public float jumpCooldown;
  public float airMultiplier;
  private bool readytoJump;
  [Header("Crouching")]
  public float crouchSpeed;
  public float crouchYScale;
  private float startYScale;
  [Header("Keybinds")]
  public KeyCode jumpKey = KeyCode.Space;
  public KeyCode sprintKey = KeyCode.LeftShift;
  public KeyCode crouchKey = KeyCode.C;
  [Header("Ground Check")]
  public float playerHeight;
  public LayerMask whatIsGround;
  private bool grounded;
  [Header("Slope Handling")]
  public float maxSlopeAngle;
  private RaycastHit slopeHit;
  private bool exitingSlope;
  public Transform orientation;
  private float horizontalInput;
  private float verticalInput;
  private Vector3 moveDirection;
  private Rigidbody rb;
  public PlayerMovement.MovementState state;
  public bool sliding;
  public bool crouching;
  public bool wallrunning;

  private void Start()
  {
    this.rb = this.GetComponent<Rigidbody>();
    this.rb.freezeRotation = true;
    this.readytoJump = true;
    this.startYScale = this.transform.localScale.y;
  }

  private void Update()
  {
    this.grounded = Physics.Raycast(this.transform.position, Vector3.down, (float) ((double) this.playerHeight * 0.5 + 0.20000000298023224), (int) this.whatIsGround);
    this.MyInput();
    this.SpeedControl();
    this.StateHandler();
    if (this.grounded)
      this.rb.drag = this.groundDrag;
    else
      this.rb.drag = 0.0f;
  }

  private void FixedUpdate() => this.MovePlayer();

  private void MyInput()
  {
    this.horizontalInput = Input.GetAxisRaw("Horizontal");
    this.verticalInput = Input.GetAxisRaw("Vertical");
    if (Input.GetKey(this.jumpKey) && this.readytoJump && this.grounded)
    {
      this.readytoJump = false;
      this.Jump();
      this.Invoke("ResetJump", this.jumpCooldown);
    }
    if (Input.GetKeyDown(this.crouchKey))
    {
      this.transform.localScale = new Vector3(this.transform.localScale.x, this.crouchYScale, this.transform.localScale.z);
      this.rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
    }
    if (!Input.GetKeyUp(this.crouchKey))
      return;
    this.transform.localScale = new Vector3(this.transform.localScale.x, this.startYScale, this.transform.localScale.z);
  }

  private void StateHandler()
  {
    if (this.wallrunning)
    {
      this.state = PlayerMovement.MovementState.wallrunning;
      this.desiredMoveSpeed = this.wallRunSpeed;
    }
    else if (this.sliding)
    {
      this.state = PlayerMovement.MovementState.sliding;
      this.desiredMoveSpeed = !this.OnSlope() || (double) this.rb.velocity.y >= 0.10000000149011612 ? this.sprintSpeed : this.slideSpeed;
    }
    else if (Input.GetKey(this.crouchKey))
    {
      this.state = PlayerMovement.MovementState.crouching;
      this.desiredMoveSpeed = this.crouchSpeed;
    }
    else if (this.grounded && Input.GetKey(this.sprintKey))
    {
      this.state = PlayerMovement.MovementState.sprinting;
      this.desiredMoveSpeed = this.sprintSpeed;
    }
    else if (this.grounded)
    {
      this.state = PlayerMovement.MovementState.walking;
      this.desiredMoveSpeed = this.walkSpeed;
    }
    else
      this.state = PlayerMovement.MovementState.air;
    if ((double) Mathf.Abs(this.desiredMoveSpeed - this.lastDesiredMoveSpeed) > 4.0 && (double) this.moveSpeed != 0.0)
    {
      this.StopAllCoroutines();
      this.StartCoroutine(this.SmoothlyLerpMoveSpeed());
    }
    else
      this.moveSpeed = this.desiredMoveSpeed;
    this.lastDesiredMoveSpeed = this.desiredMoveSpeed;
  }

  private IEnumerator SmoothlyLerpMoveSpeed()
  {
    float time = 0.0f;
    float difference = Mathf.Abs(this.desiredMoveSpeed - this.moveSpeed);
    float startvalue = this.moveSpeed;
    while ((double) time < (double) difference)
    {
      this.moveSpeed = Mathf.Lerp(startvalue, this.desiredMoveSpeed, time / difference);
      if (this.OnSlope())
      {
        float num = (float) (1.0 + (double) Vector3.Angle(Vector3.up, this.slopeHit.normal) / 90.0);
        time += Time.deltaTime * this.speedIncreaseMultiplier * this.slopeIncreaseMultiplier * num;
      }
      else
        time += Time.deltaTime;
      yield return (object) null;
    }
    this.moveSpeed = this.desiredMoveSpeed;
  }

  private void MovePlayer()
  {
    this.moveDirection = this.orientation.forward * this.verticalInput + this.orientation.right * this.horizontalInput;
    if (this.OnSlope() && !this.exitingSlope)
    {
      this.rb.AddForce(this.GetSlopeMoveDirection(this.moveDirection) * this.moveSpeed * 20f, ForceMode.Force);
      if ((double) this.rb.velocity.y > 0.0)
        this.rb.AddForce(Vector3.down * 80f, ForceMode.Force);
    }
    if (this.grounded)
      this.rb.AddForce(this.moveDirection.normalized * this.moveSpeed * 10f, ForceMode.Force);
    else if (!this.grounded)
      this.rb.AddForce(this.moveDirection.normalized * this.moveSpeed * 10f * this.airMultiplier, ForceMode.Force);
    if (this.wallrunning)
      return;
    this.rb.useGravity = !this.OnSlope();
  }

  private void SpeedControl()
  {
    if (this.OnSlope() && !this.exitingSlope)
    {
      if ((double) this.rb.velocity.magnitude <= (double) this.moveSpeed)
        return;
      this.rb.velocity = this.rb.velocity.normalized * this.moveSpeed;
    }
    else
    {
      Vector3 vector3_1 = new Vector3(this.rb.velocity.x, 0.0f, this.rb.velocity.z);
      if ((double) vector3_1.magnitude <= (double) this.moveSpeed)
        return;
      Vector3 vector3_2 = vector3_1.normalized * this.moveSpeed;
      this.rb.velocity = new Vector3(vector3_2.x, this.rb.velocity.y, vector3_2.z);
    }
  }

  private void Jump()
  {
    this.exitingSlope = true;
    this.rb.velocity = new Vector3(this.rb.velocity.x, 0.0f, this.rb.velocity.z);
    this.rb.AddForce(this.transform.up * this.jumpForce, ForceMode.Impulse);
  }

  private void ResetJump()
  {
    this.readytoJump = true;
    this.exitingSlope = false;
  }

  public bool OnSlope()
  {
    if (!Physics.Raycast(this.transform.position, Vector3.down, out this.slopeHit, (float) ((double) this.playerHeight * 0.5 + 0.30000001192092896)))
      return false;
    float num = Vector3.Angle(Vector3.up, this.slopeHit.normal);
    return (double) num < (double) this.maxSlopeAngle && (double) num != 0.0;
  }

  public Vector3 GetSlopeMoveDirection(Vector3 direction)
  {
    return Vector3.ProjectOnPlane(direction, this.slopeHit.normal).normalized;
  }

  public enum MovementState
  {
    walking,
    sprinting,
    wallrunning,
    crouching,
    sliding,
    air,
  }
}
