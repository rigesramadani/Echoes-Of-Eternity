using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    [Header("Movement")] 
    [SerializeField] private float speed;
    [SerializeField] private float deceleration;
    [SerializeField] private Transform orientation;
    private float horizontalInput;
    private float verticalInput;
    private Vector3 movementDirection;
    private Rigidbody rigidbody;

    void Start() {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.freezeRotation = true;
    }

    void Update() {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void FixedUpdate() {
        if (horizontalInput == 0 && verticalInput == 0) {
            rigidbody.velocity = new Vector3(
                Mathf.Lerp(rigidbody.velocity.x, 0, deceleration * Time.deltaTime),
                rigidbody.velocity.y,
                Mathf.Lerp(rigidbody.velocity.z, 0, deceleration * Time.deltaTime)
            );
        }
        else {
            movementDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
            rigidbody.AddForce(movementDirection * (speed * 10f), ForceMode.Force);
        }
    }
}