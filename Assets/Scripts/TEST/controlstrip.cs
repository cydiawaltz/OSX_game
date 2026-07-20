using UnityEngine;
using UnityEngine.UI;

public class controlstripController : MonoBehaviour
{
    [SerializeField] WindowManager manager;
    [SerializeField] BGMController bgm;
    [SerializeField] GameObject offObj;//上下バーだけ
    [SerializeField] GameObject onObj;
    //[SerializeField] Image[] OnObjects;//0:上下バー 1:降参 2:視点変更 3:ヒント 4:BGM 5:飾り(上)
    //[SerializeField] Image[] Exp; => 各々で管理、IPointerEnterで実装する
    public bool isActive;
    public int bgmindex;
    int maxvalue;//最後はEDで予約済み
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var mane = GameObject.FindWithTag("Manager");
        manager = mane.GetComponent<WindowManager>();
        bgm = mane.GetComponent<BGMController>();
        bgmindex = bgm.bgmindex;
        maxvalue = bgm.clips.Length-2;
        offObj.SetActive(true); onObj.SetActive(false);
        /*foreach(var ex in Exp)
        {
            ex.enabled = false;
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnClick(int index)
    {
        switch(index)
        {
            case 0:
            if(isActive){ onObj.SetActive(true); offObj.SetActive(false); isActive = false;}
            else {offObj.SetActive(true); onObj.SetActive(false); isActive = true;} break;
            case 1:
            break;
            case 2:
            manager.OnChangeView(); break;
            case 4:
            if(bgmindex == maxvalue)
            {
                bgm.StopBGM();
                bgmindex = -1;
            }
            else
            {
                bgmindex++;
                if(bgmindex>maxvalue)
                {
                    bgmindex = bgmindex%maxvalue;
                }
                bgm.ChangeBGM(bgmindex);
            }
            break;

        }
    }
    /*void CloseMessageDialog(int oldindex,int newindex)//menubarと似たような実装
    {
        
    }*/
}
