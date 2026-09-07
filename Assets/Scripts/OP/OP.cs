using UnityEngine;
using UnityEngine.Video;
using System.Collections;
using DG.Tweening;

public class OP : MonoBehaviour
{
    [SerializeField] VideoPlayer vp;
    [SerializeField] AudioSource source;
    [SerializeField] float audioStartFrame;
    [SerializeField] float videoLoopTime;
    [SerializeField] float enableBaseTime;
    [SerializeField] GameObject baseObj;
    bool trigger = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(StartSet());
        
    }
    IEnumerator StartSet()
    {
        baseObj.SetActive(false);

        vp.Stop();
        vp.time = 0;

        // æ‚ÉƒCƒxƒ“ƒg‚ð“o˜^
        vp.loopPointReached += Vp_loopPointReached;

        // “®‰æ‚ð€”õ
        vp.Prepare();

        // €”õŠ®—¹‚Ü‚Å‘Ò‚Â
        while (!vp.isPrepared)
            yield return null;

        // 0•b‚©‚çÄ¶
        vp.time = 0;
        vp.Play();

        // ‰¹º‚àŽw’èˆÊ’u‚©‚çÄ¶
        source.time = audioStartFrame;
        source.Play();
    }


    private void Vp_loopPointReached(VideoPlayer source)
    {
        vp.Stop();
        vp.time = videoLoopTime;
        vp.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if(!trigger && vp.isPlaying && vp.time >= enableBaseTime)
        {
            EnableYoukoso();
            trigger = true;
        }
    }
   void EnableYoukoso()
    {
        baseObj.SetActive(true);
        source.DOFade(0.3f, 10f);
    }
}
