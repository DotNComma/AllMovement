// Decompiled with JetBrains decompiler
// Type: WallRunning
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 93635EE7-440D-45FB-9828-DA116675EBE0
// Assembly location: C:\Users\carlb\Downloads\MonoBleedingEdge\All First Person Movement Test_Data\All First Person Movement Test_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class WallRunning : MonoBehaviour
{
  [Header("Wallrunning")]
  public LayerMask whatIsWall;
  public LayerMask whatIsGround;
  public float wallJumpUpForce;
  public float wallJumpSideForce;
  public float wallRunForce;
  public float wallClimbSpeed;
  public float maxWallRunTimer;
  private float wallRunTimer;
  [Header("Input")]
  public KeyCode jumpKey = KeyCode.Space;
  public KeyCode upwardsRunKey = KeyCode.LeftShift;
  public KeyCode downwardsRunKey = KeyCode.LeftControl;
  private bool upwardsRunning;
  private bool downwardsRunning;
  private float horizontalInput;
  private float verticalInput;
  [Header("Detection")]
  public float wallCheckDistance;
  public float minJumpHeight;
  private RaycastHit leftWallhit;
  private RaycastHit rightWallhit;
  private bool wallLeft;
  private bool wallRight;
  [Header("Exiting")]
  private bool exitingWall;
  public float exitWallTime;
  private float exitWallTimer;
  [Header("Gravity")]
  public bool useGravity;
  public float gravityCounterForce;
  [Header("Reference")]
  public Transform orientation;
  private PlayerMovement pm;
  private Rigidbody rb;

  private void Start()
  {
    this.rb = this.GetComponent<Rigidbody>();
    this.pm = this.GetComponent<PlayerMovement>();
  }

  private void Update()
  {
    this.CheckForWall();
    this.StateMachine();
  }

  private void FixedUpdate()
  {
    if (!this.pm.wallrunning)
      return;
    this.WallRunningMovement();
  }

  private void CheckForWall()
  {
    this.wallRight = Physics.Raycast(this.transform.position, this.orientation.right, out this.rightWallhit, this.wallCheckDistance, (int) this.whatIsWall);
    this.wallLeft = Physics.Raycast(this.transform.position, -this.orientation.right, out this.leftWallhit, this.wallCheckDistance, (int) this.whatIsWall);
  }

  private bool AboveGround()
  {
    return !Physics.Raycast(this.transform.position, Vector3.down, this.minJumpHeight, (int) this.whatIsGround);
  }

  private void StateMachine()
  {
    this.horizontalInput = Input.GetAxisRaw("Horizontal");
    this.verticalInput = Input.GetAxisRaw("Vertical");
    this.upwardsRunning = Input.GetKey(this.upwardsRunKey);
    this.downwardsRunning = Input.GetKey(this.downwardsRunKey);
    if ((this.wallRight || this.wallLeft) && ((double) this.verticalInput > 0.0 || (double) this.horizontalInput > 0.0 || (double) this.horizontalInput <= 0.0) && this.AboveGround() && !this.exitingWall)
    {
      if (!this.pm.wallrunning)
        this.StartWallRun();
      if ((double) this.wallRunTimer > 0.0)
        this.wallRunTimer -= Time.deltaTime;
      if ((double) this.wallRunTimer <= 0.0)
      {
        this.exitingWall = true;
        this.exitWallTimer = this.exitWallTime;
      }
      if (!Input.GetKeyDown(this.jumpKey))
        return;
      this.WallJump();
    }
    else if (this.exitingWall)
    {
      if (this.pm.wallrunning)
        this.StopWallRun();
      if ((double) this.exitWallTimer > 0.0)
        this.exitWallTimer -= Time.deltaTime;
      if ((double) this.exitWallTimer > 0.0)
        return;
      this.exitingWall = false;
    }
    else
    {
      if (!this.pm.wallrunning)
        return;
      this.StopWallRun();
    }
  }

  private void StartWallRun()
  {
    this.pm.wallrunning = true;
    this.wallRunTimer = this.maxWallRunTimer;
    this.rb.velocity = new Vector3(this.rb.velocity.x, 0.0f, this.rb.velocity.z);
  }

  private void WallRunningMovement()
  {
    this.rb.useGravity = this.useGravity;
    Vector3 lhs = this.wallRight ? this.rightWallhit.normal : this.leftWallhit.normal;
    Vector3 vector3 = Vector3.Cross(lhs, this.transform.up);
    if ((double) (this.orientation.forward - vector3).magnitude > (double) (this.orientation.forward - -vector3).magnitude)
      vector3 = -vector3;
    this.rb.AddForce(vector3 * this.wallRunForce, ForceMode.Force);
    if (this.upwardsRunning)
      this.rb.velocity = new Vector3(this.rb.velocity.x, this.wallClimbSpeed, this.rb.velocity.z);
    if (this.downwardsRunning)
      this.rb.velocity = new Vector3(this.rb.velocity.x, -this.wallClimbSpeed, this.rb.velocity.z);
    if ((!this.wallLeft || (double) this.horizontalInput <= 0.0) && (!this.wallRight || (double) this.horizontalInput >= 0.0))
      this.rb.AddForce(-lhs * 50f, ForceMode.Force);
    if (!this.useGravity)
      return;
    this.rb.AddForce(this.transform.up * this.gravityCounterForce, ForceMode.Force);
  }

  private void StopWallRun() => this.pm.wallrunning = false;

  private void WallJump()
  {
    this.exitingWall = true;
    this.exitWallTimer = this.exitWallTime;
    Vector3 force = this.transform.up * this.wallJumpUpForce + (this.wallRight ? this.rightWallhit.normal : this.leftWallhit.normal) * this.wallJumpSideForce;
    this.rb.velocity = new Vector3(this.rb.velocity.x, 0.0f, this.rb.velocity.z);
    this.rb.AddForce(force, ForceMode.Impulse);
  }
}
