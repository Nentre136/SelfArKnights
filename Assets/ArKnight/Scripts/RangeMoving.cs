using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeMoving : MonoBehaviour
{
    [SerializeField] Material range;
    private float moveSpeed=1f;
    private float offset;
    void Update()
    {
        if (range != null)
        {
            //Time.unscaledDeltaTime����ʱ����������Ӱ���ʱ��
            offset = offset - Time.unscaledDeltaTime * moveSpeed;
            offset %= 1;
            range.mainTextureOffset = new Vector2(offset, 0);
        }
    }
}
