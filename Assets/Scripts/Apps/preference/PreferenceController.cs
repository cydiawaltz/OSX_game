using UnityEngine;

public class PreferenceController : MonoBehaviour
{
    [SerializeField] GameObject[] objects;

    public void SetActiveOnly(int index)
    {
        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].SetActive(i == index);
        }
    }
}
