using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class Enermy_shikigami : MonoBehaviour
{
    /*public Camera OverViewCamera;//俯瞰かめら
    public float width;//横幅・縦幅 
    float minX,maxX;//スライダー範囲*/
    public GameObject player;
    [SerializeField] bool isJumping;
    [SerializeField] float interval;//ジャンプの間隔
    [SerializeField] float forwardForce;//前方向の力
    [SerializeField] float jumpForce;//上方向の力
    [SerializeField] Rigidbody rb;
    [SerializeField] float velocityY;//y方向の速度
    [SerializeField] float old_velocityY;
    [SerializeField] GameObject destroyObject;//消えるときのエフェクト
    public GameObject pCamera, oCamera;//disappear.csに渡すやつ
    [SerializeField] float jumpHeightPerY = 1.0f;

    void Start()
    {
        /*OverViewCamera = GameObject.FindWithTag("OverViewCamera").GetComponent<Camera>();
        MeshFilter mf = this.GetComponent<MeshFilter>();

        Vector3[] vertices = mf.mesh.vertices;

        minX = float.MaxValue;//左端
        maxX = float.MinValue;//右端

        foreach (Vector3 v in vertices)
        {
            // ローカル→ワールド
            Vector3 world = this.transform.TransformPoint(v);

            // ワールド→スクリーン
            Vector3 screen = OverViewCamera.WorldToScreenPoint(world);

            minX = Mathf.Min(minX, screen.x);
            maxX = Mathf.Max(maxX, screen.x);
        }
        width = maxX - minX;*/
        player = GameObject.FindWithTag("Player");
        rb = GetComponent<Rigidbody>();
        StartCoroutine(JumpLoop());
    }
    void Update()
    {
        velocityY = rb.linearVelocity.y;
        if (velocityY == 0 && old_velocityY == 0)//静止してる時
        {
            isJumping = false;
        }
        old_velocityY = velocityY;
    }
    private IEnumerator JumpLoop()
    {
        while (true)
        {
            if (!isJumping)
            {
                Jump();
            }

            yield return new WaitForSeconds(interval);
        }
    }

    private void Jump()
    {
        isJumping = true;

        // プレイヤー方向
        Vector3 direction = player.transform.position - transform.position;
        direction.y = 0f;
        direction.Normalize();

        // プレイヤーを向く
        transform.rotation = Quaternion.LookRotation(direction);

        // 速度リセット
        rb.linearVelocity = Vector3.zero;

        // 前方向＋上方向へジャンプ
        float yDifference = player.transform.position.y - transform.position.y;
        float currentJumpForce = jumpForce + Mathf.Max(0f, yDifference) * jumpHeightPerY;

        Vector3 force = direction * forwardForce + Vector3.up * currentJumpForce;
        rb.AddForce(force, ForceMode.VelocityChange);
    }
    void OnDestroy()
    {
        StopAllCoroutines();
        var pos = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
        var instance = Instantiate(destroyObject, pos, Quaternion.identity);
        var d = instance.GetComponent<disappear>();
        d.pCamera = pCamera;
        d.oCamera = oCamera;
    }
}