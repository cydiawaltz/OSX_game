using UnityEngine;
using System.Collections;

public class PlayingZone : MonoBehaviour//iTunesのプレイ中の音楽を表示するとこ
{
    [SerializeField] BGMController bgm;
    [SerializeField] Texture[] textures;
    [SerializeField] Renderer target;
    [SerializeField] int targetMaterialIndex;
    [SerializeField] float minpos,maxpos;
    float space;
    [SerializeField] float interval;
    [SerializeField] float musicLength,playingTime;
    void Start()
    {
        bgm = BGMController.bgmBase;
        ChangeTex(0);
        bgm.changeBGMState+=ChangeBGM;
        space = maxpos-minpos;

    }
    void Update()
    {
        playingTime = bgm.source.time;
    }
    IEnumerator UpdatePlayingState()
    {
        while(true)
        {
            yield return null;
        }
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
            musicLength = bgm.source.clip.length;
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
