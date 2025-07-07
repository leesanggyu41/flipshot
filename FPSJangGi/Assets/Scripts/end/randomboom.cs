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
            // 무작위 방향
            Vector3 randomDirection = Random.onUnitSphere.normalized;

            // 무작위 세기
            float randomStrength = Random.Range(minForce, maxForce);

            rb.isKinematic = false;
            // 힘 적용
            rb.AddForce(randomDirection * randomStrength, ForceMode.Impulse);
        }
    }
}
