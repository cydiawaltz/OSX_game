using UnityEngine;

public class threeDUIs : MonoBehaviour//機能まとめたやつみたいな　
{
    public GameObject Button;
    public bool isUsingButton;
    [SerializeField] bool isLookAt;//LookAt機能を使うか？
    [SerializeField] bool isChangeBetView;//視点変更時に3dモデルと2dUIを切り替えるか？ アタッチしてるオブジェクトは3D
    [Header("以下は使うもののみアサイン")]
    [SerializeField] Renderer flatObj;//2d用の平面オブジェクト
    [SerializeField] Renderer obj3d;//3d用　このスクリプトがアサインされている可能性あるのでrendererで切り替え
    [SerializeField] GameObject OverViewCamera;
    [SerializeField] GameObject playerCamera;
    [SerializeField] WindowManagerTest Manager;
    public Quaternion origin;
    int frameCount;
    
    public bool old_isOverView;
    void Start()
    {
        isUsingButton = Button != null;
        if(isLookAt)
        {
            Manager = GameObject.FindWithTag("Manager").GetComponent<WindowManagerTest>();
            Manager.changeVisualState += Switch;
        }
        if(isChangeBetView)
        {
            Manager = GameObject.FindWithTag("Manager").GetComponent<WindowManagerTest>();
        }
        origin = this.transform.rotation;
        Set3DState(false);//2dからスタート
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
        if(isChangeBetView&&!Manager.IsOverView==old_isOverView)//状態変更時
        {
            Set3DState(Manager.IsOverView);
        }

        //最後
        old_isOverView = Manager.IsOverView;
    }
    void Switch()
    {
        if(isLookAt)
        {
            frameCount = 10;
        }
    }
    void Set3DState(bool to3D)
    {
        if(!(flatObj == null))
        {
            if(!to3D)
            {
                obj3d.enabled = true;
                flatObj.enabled = false;
            }
            else
            {
                obj3d.enabled = false;
                flatObj.enabled = true;
            }
        }
        else
        {
            Debug.LogWarning("flatobj is null");
        }
    }
}