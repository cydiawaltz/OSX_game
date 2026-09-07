using UnityEngine;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine.UI;
using System;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [SerializeField] GameObject bullet;//prefab インスペクターでアサイン
    [SerializeField] GameObject player;
    [SerializeField] float bulletSpeed;
    [SerializeField] float targetDistance;
    public float distance;
    public int HP;//ユーザーのヒットポイント
    public int maxHP;//ユーザーの最大ヒットポイント
    [SerializeField] WindowManager manager;
    [Header("カメラはインスペクターでアサイン")]
    [SerializeField] Camera maincam;
    [SerializeField] Camera overViewCam;
    public Vector3 offset_main, offset_over;//カメラの位置調整用
    [SerializeField] RectTransform bar;
    [SerializeField] Image HPbarFront;
    [SerializeField] GameObject target;
    [SerializeField] float destroyTime;
    public Action OnDeath;
    public GameObject flatparts;
    bool isdead = false;
    [SerializeField] float minShootInterval = 0.2f;
    [SerializeField] float maxShootInterval = 2.0f;
    public float Difficulty;//外部から設定

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        //ShootingLoop();

        manager = GameObject.FindWithTag("Manager").GetComponent<WindowManager>();
        maxHP = HP;
        manager.changeVisualState += Switch;
        StartCoroutine(ShootingLoop());
    }

    void Update()
    {
        if (player == null) return;
        distance = Vector3.Distance(player.transform.position, this.transform.position);
        if (!manager.IsOverView)
        {
            Vector3 screenPos = maincam.WorldToScreenPoint(target.transform.position);
            bar.position = screenPos + offset_main;
        }
        if (HP <= 0 && !isdead)
        {
            OnDeath?.Invoke();
            StopAllCoroutines();
            isdead = true;
            flatparts.SetActive(false);
            //Destroy(this.gameObject);
            manager.changeVisualState -= Switch;
            bar.gameObject.SetActive(false);
            this.gameObject.SetActive(false);
            this.enabled = false;
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
    IEnumerator ShootingLoop()
{
    while (!isdead)
    {
        yield return null;

        if (distance <= targetDistance)
        {
            if (player != null)
            {
                Vector3 targetPos = player.transform.position;
                targetPos.y = transform.position.y;
                transform.DOLookAt(targetPos, 0.3f);

                yield return new WaitForSeconds(0.2f);

                if (isdead || !gameObject.activeInHierarchy)
                    yield break;

                InjectBullet();
            }

            // 距離に応じて射撃間隔を変更
            float t = Mathf.Clamp01(distance / targetDistance);
            float shootInterval = Mathf.Lerp(minShootInterval, maxShootInterval, t);

            yield return new WaitForSeconds(shootInterval*Difficulty);
        }
        else
        {
            yield return new WaitForSeconds(maxShootInterval*Difficulty);
        }

        yield return null;
    }
}

    public void InjectBullet()
    {
        if (isdead || !gameObject.activeInHierarchy)
            return;

        if (bullet == null)
            return;
        GameObject shell = Instantiate(bullet, gameObject.transform.position, Quaternion.identity);
        Rigidbody rb = shell.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * bulletSpeed);
        shell.GetComponent<Enermy_shikigami>().pCamera = maincam.gameObject;
        shell.GetComponent<Enermy_shikigami>().oCamera = overViewCam.gameObject;
        Destroy(shell, destroyTime);
    }
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("P_shikigami"))
        {
            HP -= 1; // ヒットポイントを減らす
            Destroy(other.gameObject); // 弾を破壊
            if (HP <= 0)
            {
                // GameOver処理
                Debug.Log("Classic is started(bullet)");
            }
        }
        else if (other.gameObject.CompareTag("Player"))
        {
            HP -= 2;
            if (HP <= 0)
            {
                // GameOver処理
                Debug.Log("Classic is started(player)");
            }
        }
        else if (other.gameObject.CompareTag("Mizushi"))
        {
            HP -= 5;
            Destroy(other.gameObject);
            if (HP <= 0)
            {
                // GameOver処理
                Debug.Log("Classic is started(player)");
            }
        }
        HPbarFront.fillAmount = (float)HP / maxHP; // HPバーの更新（例: HPが100の場合）
    }
    void OnDestroy()
    {
        DOTween.Kill(transform);
    }
}