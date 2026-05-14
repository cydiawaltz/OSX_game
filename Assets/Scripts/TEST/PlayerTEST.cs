using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CharacterController))]
public class PlayerTEST : MonoBehaviour
{
    public float JumpPower = 10;
    public float gravity = 20;
    public float speed = 6.0f;
    public Transform cameraTransform;
    public float x,y,z;

    private Vector3 moveDirection = Vector3.zero;

    CharacterController controller;
    public int HP ;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (controller.isGrounded)
        {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 
                                0,
                                Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= speed;

            x = Input.GetAxis("Horizontal");
            z = Input.GetAxis("Vertical");
            moveDirection.z = z * speed;
            moveDirection.x = x * speed;
            moveDirection = transform.TransformDirection(moveDirection);
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                moveDirection.y = JumpPower;
            }
        }
        else
        {
            moveDirection.z = z * speed;
            moveDirection.x = x * speed;
            moveDirection = transform.TransformDirection(moveDirection);
        }

        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);

        cameraTransform.position = new Vector3(
           transform.position.x,
           transform.position.y + 4.0f,
           transform.position.z - 3.0f
       );
    }
}
