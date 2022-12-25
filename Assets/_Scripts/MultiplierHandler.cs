using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplierHandler : MonoBehaviour
{
  
  [SerializeField]
  private int multiplierValue;
  public int MultiplierValue
  {
    get { return multiplierValue; }
    set { multiplierValue = value; }
  }

}
