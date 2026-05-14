using UnityEngine;
using System.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;

public class Enermy : MonoBehaviour
{
    [SerializeField] GameObject bullet;//prefab インスペクターでアサイン
    [SerializeField] GameObject player;
    [SerializeField] float bulletSpeed;
    [SerializeField] bool isNear;
    [SerializeField] float targetDistance;
    public float distance;
    public int HP;
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        ShootingLoop();
    }
    void Update()
    {
        distance = Vector3.Distance(player.transform.position,this.transform.position);
        if(distance <= targetDistance)
        {
            isNear = true;
        }
        else{isNear = false;}
    }
    async void ShootingLoop()
    {
        while(true)
        {
            if(isNear)
            {
                Vector3 targetPos = player.transform.position;
                targetPos.y = transform.position.y;
                transform.DOLookAt(targetPos,0.5f);
                await Task.Delay(500);
                InjectBullet();
            }
            await Task.Delay(2000);
        }
    }
    public void InjectBullet()
    {
        //Vector3 lookatXZ = new Vector3(player.transform.position.x,0,player.transform.position.z);
        //this.transform.LookAt(player.transform,Vector3.zero);
        
        GameObject shell = Instantiate(bullet,gameObject.transform.position,Quaternion.identity);
        Rigidbody rb = shell.GetComponent<Rigidbody>();
        //rb.AddForce((player.transform.position-transform.forward).normalized*bulletSpeed);
        rb.AddForce(transform.forward*bulletSpeed);
        Destroy(shell,8.0f);
    }
}