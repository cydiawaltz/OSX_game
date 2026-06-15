using UnityEngine;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine.UI;

public class Enermy : MonoBehaviour
{
    [SerializeField] GameObject bullet;//prefab インスペクターでアサイン
    [SerializeField] GameObject player;
    [SerializeField] float bulletSpeed;
    [SerializeField] float targetDistance;
    public float distance;
    public int HP;//ユーザーのヒットポイント
    public int maxHP;//ユーザーの最大ヒットポイント
    [SerializeField] WindowManagerTest manager;
    [Header("カメラはインスペクターでアサイン")]
    [SerializeField] Camera maincam;
    [SerializeField] Camera overViewCam;
    public Vector3 offset_main,offset_over;//カメラの位置調整用
    [SerializeField] RectTransform bar;
    [SerializeField] Image HPbarFront;
    [SerializeField] GameObject target;
    [SerializeField] float destroyTime;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        ShootingLoop();
        manager = GameObject.FindWithTag("Manager").GetComponent<WindowManagerTest>();
        maxHP = HP;
    }

    void Update()
    {
        distance = Vector3.Distance(player.transform.position, this.transform.position);
        if(!manager.IsOverView)
        {
            Vector3 screenPos = maincam.WorldToScreenPoint(target.transform.position);
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
    async void ShootingLoop()
    {
        while (true)
        {
            if (distance <= targetDistance)
            {
                Vector3 targetPos = player.transform.position;
                targetPos.y = transform.position.y;
                transform.DOLookAt(targetPos, 0.5f);
                await Task.Delay(500);
                InjectBullet();
            }
            await Task.Delay(2000);
        }
    }

    public void InjectBullet()
    {
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
        else if(other.gameObject.CompareTag("Player"))
        {
            HP -= 2;
            if (HP <= 0)
            {
                // GameOver処理
                Debug.Log("Classic is started(player)");
            }
        }
        HPbarFront.fillAmount = (float)HP / maxHP; // HPバーの更新（例: HPが100の場合）
    }
}