using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private Animator anim;
    public string ID;
    public bool checkPointActive;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    /// <summary>
    /// 生成检查点的ID
    /// </summary>
    [ContextMenu("生成检查点的ID")]
    private void GenerateID()
    {
        ID = System.Guid.NewGuid().ToString();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<player>() != null)
        {
            ActiveCheckPoint();
        }
    }

    /// <summary>
    /// 设置检查点为激活状态
    /// </summary>
    public void ActiveCheckPoint()
    {
        //如果检查点没激活并且玩家和检测点碰撞则播放检测点激活音效，目前暂时没有音效素材
        //if (!checkPointActive)
        //    AudioManager.instance.PlaySFX(0);
        checkPointActive = true;
        anim.SetBool("active", true);
    }
}
