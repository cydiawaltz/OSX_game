using UnityEngine;
using UnityEngine.Video;
using System.Collections;

public class classicController : MonoBehaviour
{
    [SerializeField] Enemy enemy;
    [SerializeField] VideoPlayer vp;
    public Material videoMaterial;
    [SerializeField] Renderer videoRenderer;
    WindowManager Manager;
    [SerializeField] Window classicWindow;
    public int materialIndex;
    [SerializeField] Window goal;
    [SerializeField] Icon classicIcon;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Manager = GameObject.FindWithTag("Manager").GetComponent<WindowManager>();
        enemy.OnDeath += OnEnemyDeath;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnEnemyDeath()
    {
        StartCoroutine(Corutine());
    }
    IEnumerator Corutine()
    {
        Debug.Log("EventHappend");
        //videoRenderer.materials[materialIndex] = videoMaterial;//設計次第なとこはあるので一応
        Material[] materials = videoRenderer.materials;
        materials[materialIndex] = videoMaterial;
        videoRenderer.materials = materials;
        vp.Prepare();
        while(!vp.isPrepared)
        {
            yield return null;
        }
        vp.Play();
        yield return new WaitForSeconds((float)vp.clip.length+1.0f);
        Manager.EnableWindowAsNewWindow(goal.gameObject);
        classicIcon.allowStarting = false;
        Manager.CloseWindow(classicWindow);
    }
}
