// Decompiled with JetBrains decompiler
// Type: Sliding
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 93635EE7-440D-45FB-9828-DA116675EBE0
// Assembly location: C:\Users\carlb\Downloads\MonoBleedingEdge\All First Person Movement Test_Data\All First Person Movement Test_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class Sliding : MonoBehaviour
{
  [Header("References")]
  public Transform orientation;
  public Transform playerObject;
  private Rigidbody rb;
  private PlayerMovement pm;
  [Header("Sliding")]
  public float maxSlideTime;
  public float slideForce;
  private float slideTimer;
  public float slideYScale;
  private float startYScale;
  [Header("Input")]
  public KeyCode slideKey = KeyCode.C;
  private float horizontalInput;
  private float verticalInput;

  private void Start()
  {
    this.rb = this.GetComponent<Rigidbody>();
    this.pm = this.GetComponent<PlayerMovement>();
    this.startYScale = this.playerObject.localScale.y;
  }

  private void Update()
  {
    this.horizontalInput = Input.GetAxisRaw("Horizontal");
    this.verticalInput = Input.GetAxisRaw("Vertical");
    if (Input.GetKeyDown(this.slideKey) && ((double) this.horizontalInput != 0.0 || (double) this.verticalInput != 0.0))
      this.StartSlide();
    if (!Input.GetKeyUp(this.slideKey) || !this.pm.sliding)
      return;
    this.StopSlide();
  }

  private void FixedUpdate()
  {
    if (!this.pm.sliding)
      return;
    this.SlidingMovement();
  }

  private void StartSlide()
  {
    this.pm.sliding = true;
    this.playerObject.localScale = new Vector3(this.playerObject.localScale.x, this.slideYScale, this.playerObject.localScale.z);
    this.rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
    this.slideTimer = this.maxSlideTime;
  }

  private void SlidingMovement()
  {
    Vector3 direction = this.orientation.forward * this.verticalInput + this.orientation.right * this.horizontalInput;
    if (!this.pm.OnSlope() || (double) this.rb.velocity.y > -0.10000000149011612)
    {
      this.rb.AddForce(direction.normalized * this.slideForce, ForceMode.Force);
      this.slideTimer -= Time.deltaTime;
    }
    else
      this.rb.AddForce(this.pm.GetSlopeMoveDirection(direction) * this.slideForce, ForceMode.Force);
    if ((double) this.slideTimer > 0.0)
      return;
    this.StopSlide();
  }

  private void StopSlide()
  {
    this.pm.sliding = false;
    this.playerObject.localScale = new Vector3(this.playerObject.localScale.x, this.startYScale, this.playerObject.localScale.z);
  }
}
