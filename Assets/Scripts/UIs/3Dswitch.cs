using UnityEngine;

public class TDswitch : MonoBehaviour
{
    public WindowManagerTest Manager;
    [SerializeField] GameObject[] FlatObjects;//平面オブジェクト　3Dモードで消す　下とインデックス対応させよ
    [SerializeField] GameObject[] Objects;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Manager = GameObject.FindWithTag("Manager").GetComponent<WindowManagerTest>();
        Manager.changeVisualState += Switch;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnDestroy()
    {
        Manager.changeVisualState -= Switch;
    }
    void Switch()
    {
        if(Manager.IsOverView)
        {
            for(int i = 0;i<=FlatObjects.Length-1;i++)
            {
                FlatObjects[i].SetActive(false);
            }
            for(int i = 0;i<=Objects.Length-1;i++)
            {
                Objects[i].SetActive(true);
            }
        }
        else
        {
            for(int i = 0;i<=FlatObjects.Length-1;i++)
            {
                FlatObjects[i].SetActive(true);
            }
            for(int i = 0;i<=Objects.Length-1;i++)
            {
                Objects[i].SetActive(false);
            }
        }
    }
}
