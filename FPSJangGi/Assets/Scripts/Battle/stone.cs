using UnityEngine;

public class stone : MonoBehaviour
{
   private Rigidbody rb;
    void Start()
    {
        rb = transform.GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Ground")
        {
            rb.constraints = RigidbodyConstraints.FreezePositionY;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(collision.transform.tag == "Ground")
        {
            rb.constraints = RigidbodyConstraints.None;
        }
    }
}
