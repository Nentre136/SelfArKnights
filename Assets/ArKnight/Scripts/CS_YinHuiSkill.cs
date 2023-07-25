using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_YinHuiSkill : MonoBehaviour
{

    [SerializeField] Sprite[] skill;
    private SpriteRenderer spriteRanderder;
    public float animaRate = 0.1f;
    void Start()
    {
        spriteRanderder = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
    }
    public IEnumerator SkillAnimation()
    {
        for(int index = 0;index<skill.Length ;index++ )
        {
            spriteRanderder.sprite = skill[index];
            yield return new WaitForSeconds(animaRate);
        }
        spriteRanderder.enabled = false;
        yield break;
    }
}
