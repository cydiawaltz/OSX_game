using UnityEngine;

public class SimpleText : MonoBehaviour
{
    [SerializeField] WindowManager manager;
    [SerializeField] Player player;
    public GameObject target;
    public float dist;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        manager = GameObject.FindWithTag("Manager").GetComponent<WindowManager>();
        //manager.changeVisualState += Switch;
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(player.gameObject.transform.position,target.transform.position)<=dist)
        {
            StartCoroutine(player.OnWin());
        }
    }
    
}
