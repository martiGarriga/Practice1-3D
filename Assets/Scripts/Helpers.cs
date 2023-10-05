using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helpers : MonoBehaviour {

    public float m_DestroyOnTime; 

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyObjectOnTimeCoroutine());
    }

    // Update is called once per frame
    IEnumerator DestroyObjectOnTimeCoroutine()
    {
        yield return new WaitForSeconds(m_DestroyOnTime);
        GameObject.Destroy(gameObject);
    }
}
