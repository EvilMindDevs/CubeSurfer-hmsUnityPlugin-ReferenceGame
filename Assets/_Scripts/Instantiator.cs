using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instantiator : MonoBehaviour
{
  [SerializeField] int count = 0;
  [SerializeField] GameObject objectPrefab;

  //Yerleri belirli nesnelerin dinamik bir şekilde girilen sayıda oluşturulması ve y ekseninde üst üste eklemek için kullanılan fonksiyon. (Cubeler için)
  private void OnEnable()
  {
    for (int i = 0; i < count; i++)
    {
      var newTransform = new Vector3(transform.position.x, transform.position.y + i, transform.position.z);
      Instantiate(objectPrefab, newTransform, Quaternion.identity, transform);
    }
  }
}
