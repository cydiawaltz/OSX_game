using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    public float JumpPower = 10;
    public float gravity = 20;
    public float speed = 6.0f;
    public Transform cameraTransform;

    private Vector3 moveDirection = Vector3.zero;
    CharacterController controller;
    float x, y, z;

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
    public Vector3 offset_main, offset_over;//カメラの位置調整用
    [SerializeField] GameObject enemy;
    [SerializeField] RectTransform bar;
    [SerializeField] GameObject kai;
    [SerializeField] float zangekiSpeed = 20f;
    [SerializeField] Image Syouin;//宿儺の手
    [SerializeField] Image SekaizanText;//「世界を断つ斬撃!!」
    [SerializeField] Image Eisyou;//「番の流星」
    [SerializeField] float enableTime;//斬撃が有効な時間
    [SerializeField] GameObject disappear;
    [SerializeField] Renderer[] playerrenderers;
    bool isWinned;

    void Start()
    {
        manager = GameObject.FindWithTag("Manager").GetComponent<WindowManager>();
        controller = GetComponent<CharacterController>();
        originalPos = cameraTransform.position - transform.position;
        maxHP = HP;
        manager.changeVisualState += Switch;
        Syouin.enabled = false; SekaizanText.enabled = false; Eisyou.enabled = false;
    }

    void Update()
    {
        if(enemy == null)
        {
            if(!isWinned)
            {
                isWinned = true;
                //StartCoroutine(OnWin());
            }
        }
        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");
        if(this.transform.position.y <= -10)
        {
            controller.enabled = false;
            Debug.Log("respawn");
            this.transform.position = new Vector3(0,20,0);
            controller.enabled = true;
        }
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
        if (controller.enabled)
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
        if (Input.GetKeyDown(KeyCode.M))
        {
            StartCoroutine(Sekaizan());
        }
        if (!manager.IsOverView)
        {
            Vector3 screenPos = maincam.WorldToScreenPoint(this.transform.position);
            bar.position = screenPos + offset_main;
        }
        if(HP == 0)
        {
            StartCoroutine(OnDeath());
            HP--;
        }
    }
    void Switch()
    {
        if (manager.IsOverView)
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
        try
        {
            if (other.gameObject.CompareTag("E_shikigami"))
            {
                HP -= 1; // ヒットポイントを減らす
                Destroy(other.gameObject); // 弾を破壊
                if (HP <= 0)
                {
                    Debug.Log("Game Over(bullet)");
                }
            }
            else if (other.gameObject.CompareTag("Enemy"))
            {
                HP -= 1;
                if (HP <= 0)
                {
                    Debug.Log("Game Over(Enemy)");
                }
            }
            else if(other.gameObject.CompareTag("door"))
            {
                Debug.Log("EnterDoor");
                StartCoroutine(OnWin());
            }
        }
        catch (Exception e)
        {
            Debug.Log("Player.cs:OnCollisionEnter:Exception:" + e);
        }
        HPbarFront.fillAmount = (float)HP / maxHP; // HPバーの更新（例: HPが100の場合）
    }
    IEnumerator Sekaizan()//イースターエッグ的な
    {
        GameObject enemy = GameObject.FindWithTag("Enemy");
        if (enemy == null) yield break;
        Syouin.enabled = true; SekaizanText.enabled = true; Eisyou.enabled = true;
        yield return new WaitForSeconds(0.3f);
        GameObject slash = Instantiate(kai, transform.position, Quaternion.identity);
        Vector3 direction = (enemy.transform.position - slash.transform.position).normalized;
        Rigidbody rb = slash.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("kaiにRigidbodyがありません");
            Destroy(slash);
            yield break;
        }
        int frame = 0;
        rb.linearVelocity = direction * zangekiSpeed;
        while (slash != null && enemy != null && frame <= enableTime * 60)//60fps想定　どうせ飛ぶのでそんな変わらんて
        {
            //slash.transform.position +=
            //direction * zangekiSpeed * Time.deltaTime;
            frame++;
            yield return null;
        }
        if (slash != null) Destroy(slash);
        Syouin.enabled = false; SekaizanText.enabled = false; Eisyou.enabled = false;
    }
    IEnumerator OnDeath()
    {
        Debug.Log("you died");
        maincam.transform.parent = null;
        manager.OnChangeView();
        foreach(var p in playerrenderers)
        {
            p.enabled = false;
        }
        var pos = new Vector3(transform.position.x, transform.position.y+0.5f, transform.position.z);
        var instance = Instantiate(disappear, pos, Quaternion.identity);
        var d = instance.GetComponent<disappear>();
        d.pCamera = maincam.gameObject;
        d.oCamera = overViewCam.gameObject;
        //this.gameObject.SetActive(false);
        yield return new WaitForSeconds(1.3f);
        StartCoroutine(manager.CloseGame(false));
        //StopAllCoroutines();
    }
    public IEnumerator OnWin()
    {
        Debug.Log("you died");
        maincam.transform.parent = null;
        manager.OnChangeView();
        foreach(var p in playerrenderers)
        {
            p.enabled = false;
        }
        var pos = new Vector3(transform.position.x, transform.position.y+0.5f, transform.position.z);
        var instance = Instantiate(disappear, pos, Quaternion.identity);
        var d = instance.GetComponent<disappear>();
        d.pCamera = maincam.gameObject;
        d.oCamera = overViewCam.gameObject;
        //this.gameObject.SetActive(false);
        yield return new WaitForSeconds(1.3f);
        StartCoroutine(manager.CloseGame(true));
        //StopAllCoroutines();
    }
}