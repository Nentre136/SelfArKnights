using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_EnemyTips : MonoBehaviour
{
    [SerializeField] float speed;
    public GameObject path;
    private Transform[] paths;//路径
    private GameObject endNode;//终点
    private int index = 1;
    void Update()
    {
        Moving();
    }
    void Start()
    {
        paths = path.GetComponentsInChildren<Transform>();//获取的是所有的子物体，包括父物体本身
        endNode = paths[paths.Length - 1].gameObject;
    }
    protected void Moving()
    {
        //移动方向
        Vector3 moveDirection = (paths[index].position - transform.position).normalized;
        Vector3 movePosition = moveDirection * speed * Time.deltaTime;
        //移动  
        transform.position += movePosition;
        //enemyBody.MovePosition(enemyBody.position + movePosition);
        //计算两个向量的距离，小于一定程度就算到达，则前往下一位置
        if (Vector3.Distance(paths[index].position, transform.position) < 0.1f)
            index++;
        if (Vector3.Distance(paths[paths.Length - 1].position, transform.position) < 0.5f)
        {

            GameObject.Destroy(gameObject);
        }
    }

}
