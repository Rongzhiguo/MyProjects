using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_FadeScene : MonoBehaviour
{
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    /// <summary>
    /// µ­³ö
    /// </summary>
    public void FadeOut() => anim.SetTrigger("fadeOut");
    public void FadeIn() => anim.SetTrigger("fadeIn");
}
