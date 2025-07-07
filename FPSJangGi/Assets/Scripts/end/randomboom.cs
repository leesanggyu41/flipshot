using UnityEngine;

public class randomboom : MonoBehaviour
{
    public endind ca;
    [SerializeField]
    private int num;

    public float minForce = 3f;
    public float maxForce = 3.5f;

    private Rigidbody rb;

    private bool EKr =false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ca.num == num && !EKr)
        {
            EKr = true;
            // ������ ����
            Vector3 randomDirection = Random.onUnitSphere.normalized;

            // ������ ����
            float randomStrength = Random.Range(minForce, maxForce);

            rb.isKinematic = false;
            // �� ����
            rb.AddForce(randomDirection * randomStrength, ForceMode.Impulse);
        }
    }
}
