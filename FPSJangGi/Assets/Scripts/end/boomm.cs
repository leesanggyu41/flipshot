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
        // ������ ����
        Vector3 randomDirection = Random.onUnitSphere.normalized;

        // ������ ����
        float randomStrength = Random.Range(minForce, maxForce);

        rb.isKinematic = false;
        // �� ����
        rb.AddForce(randomDirection * randomStrength, ForceMode.Impulse);
        Invoke("oncol", 0.5f);
    }

    void oncol()
    {
        cool.enabled = true;   
    }
}
