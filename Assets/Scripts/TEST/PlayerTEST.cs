using UnityEngine;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    public float JumpPower = 10;
    public float gravity = 20;
    public float speed = 6.0f;
    public Transform cameraTransform;

    private Vector3 moveDirection = Vector3.zero;
    CharacterController controller;
    float x,y,z;

    [SerializeField] GameObject bullet;
    [SerializeField] float bulletSpeed;
    [SerializeField] Vector3 originalPos;
    [SerializeField] Image HPbarFront;
    public int HP;//ユーザーのヒットポイント
    public int maxHP;//ユーザーの最大ヒットポイント
    [SerializeField] WindowManager manager;
    [Header("カメラはインスペクターでアサイン")]
    [SerializeField] Camera maincam;
    [SerializeField] Camera overViewCam;
    public Vector3 offset_main,offset_over;//カメラの位置調整用
    [SerializeField] RectTransform bar;

    void Start()
    {
        manager = GameObject.FindWithTag("Manager").GetComponent<WindowManager>();
        controller = GetComponent<CharacterController>();
        originalPos = cameraTransform.position - transform.position;
        maxHP = HP;
        manager.changeVisualState+= Switch;
    }

    void Update()
    {
        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");
        if (controller.isGrounded)
        {
            
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection) * speed;

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
        if(controller.enabled)
        {
            controller.Move(moveDirection * Time.deltaTime);
        }

        cameraTransform.position = new Vector3(
            transform.position.x + originalPos.x,
            transform.position.y + originalPos.y,
            transform.position.z + originalPos.z
        );

        if (Input.GetKeyDown(KeyCode.P))
        {
            InjectBullet();
        }
        if(!manager.IsOverView)
        {
            Vector3 screenPos = maincam.WorldToScreenPoint(this.transform.position);
            bar.position = screenPos+offset_main;
        }
    }
    void Switch()
    {
        if(manager.IsOverView)
        {
            bar.gameObject.SetActive(false);
            //Vector3 screenPos = overViewCam.WorldToScreenPoint(this.transform.position);
            //bar.position = screenPos+offset_over;
        }
        else
        {
            bar.gameObject.SetActive(true);
            //Vector3 screenPos = maincam.WorldToScreenPoint(this.transform.position);
            //bar.position = screenPos+offset_main;
        }
    }

    public void InjectBullet()
    {
        GameObject shell = Instantiate(bullet, gameObject.transform.position, Quaternion.identity);
        Rigidbody rb = shell.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * bulletSpeed);
        Destroy(shell, 8.0f);
    }
    void OnCollisionEnter(Collision other)
    {
        try{
        if (other.gameObject.CompareTag("E_shikigami"))
        {
            HP -= 1; // ヒットポイントを減らす
            Destroy(other.gameObject); // 弾を破壊
            if (HP <= 0)
            {
                // GameOver処理
                Debug.Log("Game Over(bullet)");
            }
        }
        else if(other.gameObject.CompareTag("Enemy"))
        {
            HP -= 2;
            if (HP <= 0)
            {
                // GameOver処理
                Debug.Log("Game Over(Enemy)");
            }
        }
        }
        catch(Exception e)
        {
            Debug.Log("Player.cs:OnCollisionEnter:Exception:"+e);
        }
        HPbarFront.fillAmount = (float)HP / maxHP; // HPバーの更新（例: HPが100の場合）
    }
}