using UnityEngine;
using UnityEngine.UI;

public class CountdownDigit : MonoBehaviour {

	public void Setup(string currentNum, float death)
    {
        GetComponentInChildren<TextMesh>().text = currentNum;
        Destroy(gameObject, death);
    }
}
