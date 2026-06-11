using UnityEngine;

public class threeDUIs : MonoBehaviour
{
    public GameObject Button;
    public bool isUsingButton;
    [SerializeField] bool isLookAt;//LookAt機能を使うか？
    [SerializeField] GameObject OverViewCamera;
    [SerializeField] GameObject playerCamera;
    [SerializeField] WindowManagerTest Manager;
    public Quaternion origin;
    int frameCount;
    
    void Start()
    {
        isUsingButton = Button != null;
        if(isLookAt)
        {
            Manager = GameObject.FindWithTag("Manager").GetComponent<WindowManagerTest>();
            Manager.changeVisualState += Switch;
        }
        origin = this.transform.rotation;
    }

    void Update()
    {
        if(frameCount == 0&&isLookAt)//偶数fだけ？流石に分からんか
        {
            if(Manager.IsOverView)
            {
                //transform.LookAt(OverViewCamera.transform);
                transform.rotation = origin;
            }
            else
            {
                transform.LookAt(playerCamera.transform);
            }      
            frameCount++;
        }
        else frameCount = 0;
        if(frameCount == 10)
        {
            transform.LookAt(OverViewCamera.transform);
            frameCount++;
        }
        if (Input.GetMouseButtonDown(0) && isUsingButton)
        {
            // ボタンが押されたときの処理をここに記述
        }
    }
    void Switch()
    {
        if(isLookAt)
        {
            frameCount = 10;
        }
    }
}