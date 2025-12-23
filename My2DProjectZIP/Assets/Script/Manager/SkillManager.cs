using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : SingletonMono<SkillManager>
{
    //public static SkillManager instance;

    public Dash_Skill dash_Skill { get; private set; }

    public Clone_Skill clone_Skill { get; private set; }

    public Sword_Skill sword_Skill { get; private set; }

    public Blackhole_Skill blackhole { get; private set; }

    public Crystal_Skill crystal { get; private set; }

    public Crossed_Skill crossed { get; private set; }

    public Parry_Skill parry { get; private set; }

    public Dodge_Skill dodge { get; private set; }
    //private void Awake()
    //{
    //    if (instance == null)
    //        instance = this;
    //    else
    //        Destroy(instance.gameObject);
    //}

    private void Start()
    {
        dash_Skill = GetComponent<Dash_Skill>();
        clone_Skill = GetComponent<Clone_Skill>();
        sword_Skill = GetComponent<Sword_Skill>();
        blackhole = GetComponent<Blackhole_Skill>();
        crystal = GetComponent<Crystal_Skill>();
        crossed = GetComponent<Crossed_Skill>();
        parry = GetComponent<Parry_Skill>();
        dodge = GetComponent<Dodge_Skill>();
    }
}
