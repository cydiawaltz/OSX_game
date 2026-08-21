using UnityEngine;
using UnityEngine.Video;

public class QT : MonoBehaviour
{
    [SerializeField] VideoPlayer vp;
    [SerializeField] Renderer PlayButtonUp;//上に乗ってる方（停止中だけオン）
    [SerializeField] UIButton PlayButtonDown;//下にある方（再生中だけオン）
    //[SerializeField] Renderer PlayButtonDown;
    //[SerializeField] Texture[] playingTex;//0:普通 1:押されてる
    //[SerializeField] Texture[] stopTex;//0:普通 1:押されてる

    public bool isPlaying = true;
    //再生停止はUIButton.csのOnClickで
    void Start()
    {
        //vp = this.GetComponent<VideoPlayer>();
        PlayButtonUp.enabled = false;
    }
    public void PlayVideo()
    {
        if(isPlaying) return;
        vp.Play();
        isPlaying = true;
        PlayButtonUp.enabled = false;
        PlayButtonDown.enabled = true;
    }
    public void StopVideo()
    {
        if(!isPlaying) return;
        vp.Pause();
        isPlaying = false;
        PlayButtonUp.enabled = true;
        PlayButtonDown.enabled = false;
    }
}
