using UnityEngine;

public class PlayingZone : MonoBehaviour//iTunesのプレイ中の音楽を表示するとこ
{
    [SerializeField] BGMController bgm;
    [SerializeField] Texture[] textures;
    [SerializeField] Renderer target;
    [SerializeField] int targetMaterialIndex;
    void Start()
    {
        bgm = BGMController.bgmBase;
        ChangeTex(0);
        bgm.changeBGMState+=ChangeBGM;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void ChangeBGM()
    {
        if(bgm.source.isPlaying)
        {
            if(bgm.bgmindex >= textures.Length)
            {
                bgm.bgmindex = bgm.bgmindex%textures.Length;
                Debug.LogWarning("入力値が不正だったから割っといたで♡");
            }
            ChangeTex(bgm.bgmindex);
        }
        else
        {
            ChangeTex(textures.Length-1);
        }
    }
    void ChangeTex(int index)
    {
        target.materials[targetMaterialIndex].mainTexture = textures[index];
    }
}
