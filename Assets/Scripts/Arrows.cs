using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Arrows : MonoBehaviour
{
    [SerializeField] GameObject[] arrows;
    [SerializeField] float flickTime = .666f;

    private void OnEnable() => StartCoroutine(StateChanger());
    IEnumerator StateChanger()
    {
        arrows.ToList().ForEach(x => x.SetActive(true));

        while (true)
        {
            yield return new WaitForSecondsRealtime(flickTime);

            arrows.ToList().ForEach(x => x.SetActive(!x.activeSelf));
        }
    }

}
