using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Waves//记录波次
{
    public GameObject path;//路径
    public GameObject enemy;//敌人类型
    public GameObject startNode;//出生点
    public int count;//数量
    public float spacingTime;//每波间隔时间
    public float lastTime;//距离下波时间
    
}
