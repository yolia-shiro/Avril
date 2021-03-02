//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class SkeletonShieldArea : MonoBehaviour, IDamageable
//{
//    private Animator anim;
//    private Enemy enemy;

//    private void Start()
//    {
//        anim = GetComponentInParent<Animator>();
//        enemy = GetComponentInParent<Enemy>();
//    }

//    private void OnTriggerEnter2D(Collider2D collision)
//    {
//        if (collision.CompareTag("Player"))
//        {
//            anim.SetTrigger("shield");
//        }
//    }

//    public bool GetDamage(float value)
//    {
//        //消耗僵直值
//        enemy.ResumeToughness(value * (1 - enemy.resistance));
//        if (enemy.maxToughness > 0 && enemy.CurToughness < 0)
//        {
//            enemy.CurToughness = 0;
//            enemy.sliderToughness.value = 0;

//            // 进入僵直状态
//            enemy.TranslateToState(enemy.stiffState);
//            return true;
//        }
//        return true;
//    }
//}
