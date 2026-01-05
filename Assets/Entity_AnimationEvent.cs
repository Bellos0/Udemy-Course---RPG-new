using UnityEngine;
/// <summary>
/// script nay dung de marking cac diem enent trong animation de goi cac method tuong ung trong player.cs
/// </summary>
public class Entity_AnimationEvent : MonoBehaviour
{
    private Entity EntityService;

    private void Awake()
    {
        EntityService = GetComponentInParent<Entity>(); // vi player nam trong parent ta dung getcomponentinparent de truy cap no
    }

   
    /// <summary>
    /// method nay duoc setup trong animation cua unity, khi o 1 frame nao do thi ta bat event nay len de goi method nay
    /// </summary>
    private void DisableMove_and_Jump()
    {
        EntityService.EnableMotion(false);
    }
    private void EndableMove_and_Jump()

       => EntityService.EnableMotion(true); // viet kieu lamda expression, neu trong method chi co 1 dong code thi co the viet the nay cho nhanh
    public void DamageTarget()
    {
    EntityService.DamageTarget();
        //Debug.Log("player animation event damage enemy called");
    }

}
