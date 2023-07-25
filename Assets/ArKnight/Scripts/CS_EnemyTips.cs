using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_EnemyTips : MonoBehaviour
{
    [SerializeField] float speed;
    public GameObject path;
    private Transform[] paths;//·��
    private GameObject endNode;//�յ�
    private int index = 1;
    void Update()
    {
        Moving();
    }
    void Start()
    {
        paths = path.GetComponentsInChildren<Transform>();//��ȡ�������е������壬���������屾��
        endNode = paths[paths.Length - 1].gameObject;
    }
    protected void Moving()
    {
        //�ƶ�����
        Vector3 moveDirection = (paths[index].position - transform.position).normalized;
        Vector3 movePosition = moveDirection * speed * Time.deltaTime;
        //�ƶ�  
        transform.position += movePosition;
        //enemyBody.MovePosition(enemyBody.position + movePosition);
        //�������������ľ��룬С��һ���̶Ⱦ��㵽���ǰ����һλ��
        if (Vector3.Distance(paths[index].position, transform.position) < 0.1f)
            index++;
        if (Vector3.Distance(paths[paths.Length - 1].position, transform.position) < 0.5f)
        {

            GameObject.Destroy(gameObject);
        }
    }

}
