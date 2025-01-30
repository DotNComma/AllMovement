// Decompiled with JetBrains decompiler
// Type: MoveCamera
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 93635EE7-440D-45FB-9828-DA116675EBE0
// Assembly location: C:\Users\carlb\Downloads\MonoBleedingEdge\All First Person Movement Test_Data\All First Person Movement Test_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class MoveCamera : MonoBehaviour
{
  public Transform cameraPosition;

  private void Update() => this.transform.position = this.cameraPosition.position;
}
