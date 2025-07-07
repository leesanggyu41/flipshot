using UnityEngine;
using Photon.Pun;

public class boomm : MonoBehaviour
{
    public float minForce = 3f;
    public float maxForce = 3.5f;

    private Rigidbody rb;
    private Collider cool;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cool = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Start()
    {
        // 무작위 방향
        Vector3 randomDirection = Random.onUnitSphere.normalized;

        // 무작위 세기
        float randomStrength = Random.Range(minForce, maxForce);

        rb.isKinematic = false;
        // 힘 적용
        rb.AddForce(randomDirection * randomStrength, ForceMode.Impulse);
        Invoke("oncol", 0.5f);
    }

    void oncol()
    {
        cool.enabled = true;   
    }
}
