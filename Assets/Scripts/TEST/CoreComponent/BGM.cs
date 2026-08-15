using UnityEngine;
using System;

public class BGMController : MonoBehaviour
{
    public static BGMController bgmBase;
    public AudioSource source;
    public AudioClip[] clips;//外部からは情報の取得だけ...
    [SerializeField] SceneType sceneType;
    enum SceneType{Stage,Welcome,NameEntry}
    /*　-fnky algorthm -perfectplace -morphy randezvous(SO1) -MaximumJoy -YouAintMyFriend -curiosity(fpm) -IRIS -highmax dazzling -daydreamin
    -starsuite2(MG4) -MG2SS(MG4) -eraser(MG) GOLDremix(宇多田) -comeagain(Back2THEFutureRemix) -LIFE
    1曲10mb換算
    10.1 0:Sphere(MG) 1:MG2SS 2:89.5℃ 3:FNKYALGORTHM 4:IRIS ED(5):EVERYDAY LIFE
    10.3 0:Eple 1:morphy 2:TORNADO 3:high max 4:curiosity ED5:daydreamin
    10.4 0:bytecry 1:perfectplace 2:come again(remix) 3:telephone&Whisky 4:PARAGON2  ED5:LIFE
    10.5(vs) 0:Exdous Honey 1:What's your medium?(⭐︎) 2: maximumJoy(SO) 3:starsuite2 4:YouAintMyFriend 5ED:eraser(<=NameEntryと共通 EDなしで今風なリザルト画面+ネームエントリー)
    title Sofa Rockers(video playerの方から管理)=>ステージ選択画面まで継続
    NameEntry No Time!(OS9)<10.1 or Yosemite intro<10.3 or OSX intro<10.4
    */
    [Header("ここは最初につけとけ")]
    [SerializeField] bool isStage;
    public int bgmindex;
    public Action changeBGMState;//停止してるかはsource.isplayingで取れな
    void Start()
    {
        bgmBase = this;
        source.clip = clips[bgmindex];
        source.loop = true;
        switch(sceneType)
        {
            case SceneType.Stage:
            //source.clip = clips[0];
            source.Play();
            break;
        }  
    }

    public void ChangeBGM(int newindex)
    {
        if(newindex >= clips.Length)
        {
            newindex = newindex%clips.Length;
            Debug.LogWarning("入力値が不正だったから割っといたで♡");
        }
        bgmindex = newindex;
        source.clip = clips[newindex];
        source.Play();
        changeBGMState?.Invoke();
    }
    public void StopBGM()
    {
        source.Stop();
        changeBGMState?.Invoke();
    }
}
