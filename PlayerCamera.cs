// Decompiled with JetBrains decompiler
// Type: PlayerCamera
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 93635EE7-440D-45FB-9828-DA116675EBE0
// Assembly location: C:\Users\carlb\Downloads\MonoBleedingEdge\All First Person Movement Test_Data\All First Person Movement Test_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class PlayerCamera : MonoBehaviour
{
  public float sensY;
  public float sensX;
  public Transform orientation;
  private float xRotation;
  private float yRotation;

  private void Start()
  {
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
  }

  private void Update()
  {
    float num1 = Input.GetAxisRaw("Mouse X") * Time.deltaTime * this.sensX;
    float num2 = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * this.sensY;
    this.yRotation += num1;
    this.xRotation -= num2;
    this.xRotation = Mathf.Clamp(this.xRotation, -90f, 90f);
    this.transform.rotation = Quaternion.Euler(this.xRotation, this.yRotation, 0.0f);
    this.orientation.rotation = Quaternion.Euler(0.0f, this.yRotation, 0.0f);
  }
}
