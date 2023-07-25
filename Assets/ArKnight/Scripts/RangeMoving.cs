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
            //Time.unscaledDeltaTime不受时间因子缩放影响的时间
            offset = offset - Time.unscaledDeltaTime * moveSpeed;
            offset %= 1;
            range.mainTextureOffset = new Vector2(offset, 0);
        }
    }
}
