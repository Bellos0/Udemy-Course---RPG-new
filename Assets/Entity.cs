using System;
using System.Collections;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class Entity : MonoBehaviour
{
    protected Rigidbody2D rb;
    protected Animator anim;
    protected Collider2D col;   // can thiep vao collider cho method die()
    protected SpriteRenderer sr; // tuong tac voi spriterendder de thay doi hieu ung khi nhan damage hoac chet

   // protected float xInput;


    [Header("Movement details")]
    [SerializeField] protected float moveSpeed = 1f;

    [SerializeField] protected float jumpForce = 1f;
    

    [Header("Health details")]
    [SerializeField] private int maxHealth = 1;
    [SerializeField] private int currentHealth;
    [SerializeField] private Material damMaterial;
    [SerializeField] private float damFeedbackDuration;
    private Coroutine damFeedbackCoroutine;


    [Header("Collsion details")]
    [SerializeField]
    private float groundCheckDistance = 0.2f;
    [SerializeField] protected bool isGrounded;
    [SerializeField] protected LayerMask whatisGround;
    protected int faceDir = 1; // face to right

    protected bool faceRight = true;

    protected bool canMove = true;

    //protected bool canJump = true;
    //------------------
    //public Collider2D[] enemyCollider; chuyen vao ben trong ham de tiet kiem bo nho
    [Header("Attack details")]
    [SerializeField] protected float attackRadius; // tao o ben unity nhin cho truc quan hon
    [SerializeField] protected Transform attackPoint; // tao 1 gameobject con ben trong player, vi player la nguoi can attackpoint
    [SerializeField] protected LayerMask whatisTarget; // setlayer name thoi, khong co gi phuc tap ca


    /// <summary>
    /// khoi tao chay 1 lan duy nhat
    /// </summary>
    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>(); // khai bao de rb co the truy cap duoc vao ben trong rigid cua game object. khai bao 1 lan la duoc, nen de ben trong ham awake
        anim = GetComponentInChildren<Animator>(); // khai bao de bien anim co the truy cap duoc vao trong cac setup trong animator. vi animator duoc tao trong folder con cua main object player nen phai dung getcomponeninchildrend de co the phan quyen truy cap cho no
                                                   // whatisEnemies = LayerMask.GetMask("Enemies"); // set layer mask cho bien whatisenemies de co the su dung no trong overlaycircleall

        col = GetComponent<Collider2D>();
        sr = GetComponentInChildren<SpriteRenderer>(); // inchild vi minh dang can tuong tac den sprite render ben trong animator
        currentHealth = maxHealth;
    }

    // Start is called before the first frame update
    void Start()
    {

    }



    // Update is called once per frame
    protected virtual void Update()
    {

        HandleCollision();
        HandleFlip();
        HanldeAnimator();
        HandleAttack();
        //HandleMovement();
       // TrytoJump();
    }

    /// <summary>
    /// su dung dieu kien va cham(collider) de nhan biet enemy de co the thuc hien thao tac gay sat thuong.
    /// thuoc tinh overlaycircleall can co 3 tham so quan trong: diem danh, ban kinh sat thuong, layer the hien enemy.
    /// khi ay thuoc tinh tra ve 1 mang the hien su va cham da duoc ghi nhan
    /// </summary>
    public void DamageTarget()
    {
        Collider2D[] enemyCollider = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, whatisTarget);

        foreach (Collider2D enemy in enemyCollider)
        {


            Entity EntityService = enemy.GetComponent<Entity>();
            EntityService.TakeDamage();
        }
    }

    protected void TakeDamage()
    {
        //throw new NotImplementedException();

        currentHealth = currentHealth - 1;
        PlayDamageFeedbackCO();

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    protected virtual void Die()
    {
        //throw new NotImplementedException();
        anim.enabled = false; // tat toan bo animation khi game object chet
        col.enabled = false; // tat collider de gameobject drop ra ngoai platform
        rb.gravityScale = 12; // tang khoi luong de drop cho nhanh
        rb.velocity = new Vector2(rb.velocity.x, 15); // khi chet thi nhay len 1 chut
        Destroy(gameObject, 2);
    }


    /// <summary>
    /// method nay giup cho viec hien thi hieu ung khi nhan damage
    /// object se nhay mau trang trong vong 0.2s roi chuyen ve mau goc
    /// </summary>
    private void PlayDamageFeedbackCO()
    {
        if (DamFeedbackCO() != null) // kiem tra trang thai cac corutine co dang chay hay khong, neu co thi tat di, tranh chay cac method chong chat len nhau
        {
            StopCoroutine(DamFeedbackCO());
        }
        StartCoroutine(DamFeedbackCO());
    }


    /// <summary>
    /// method the hien tuan tu tung buoc chuyen doi hieu ung khi nhan damage.
    /// doc comment tung dong de hieu ro hon
    /// con dung class ienumerator de tao coroutine boi vi trong start coroutine tham so truyen vao phai la 1 ienumerator
    /// </summary>
    /// <returns></returns>
    private IEnumerator DamFeedbackCO()
    {
        Material Orimaterial = sr.material; // get materila cua object luc ban dau
        sr.material = damMaterial; // thay doi material thanh material khi bi damage
        yield return new WaitForSeconds(damFeedbackDuration); // cho thoi gian hien thi
        sr.material = Orimaterial; // tro ve material ban dau.
    }

    
    /// <summary>
    /// ham truyen trung gian de turn on/off chuc nang di chuyen va nhay
    /// </summary>
    /// <param name="endable"></param>
    public virtual void EnableMotion(bool endable)
    {
        canMove = endable;
    }

    protected virtual void HanldeAnimator()
    {

        #region old code chuyen doi animator
        //throw new NotImplementedException();
        //bool isMoving = false;
        //if (rb.velocity.x != 0)
        //{
        //    isMoving = true;
        //}
        //else
        //{
        //    isMoving = false;
        //}
        //anim.SetBool("isMoving", isMoving); // isMoving la parameter duoc tao ra ben trong the animator, goi dung tren thi moi co the kich hoat su kien chuyen canh 
        #endregion

        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVel", rb.velocity.y);
        anim.SetFloat("xVel", rb.velocity.x);
    }

    /// <summary>
    /// hang muc nay chi su dung cho player
    /// nen la se chuyen no sang class player de clean entity
    /// </summary>
    protected virtual void HandleMovement()
    {
        //xInput = Input.GetAxisRaw("Horizontal"); // thay doi huong nhanh hon, thuc te gia tri xInput khi run chay tu -1,0,1 gia tri la 1 trong 3 gia tri nay. toc do thay doi phuong di chuyen
        //xInput = Input.GetAxis("Horizontal");
        // thay doi huong di chuyen binh thuong, thuc te gia tri xInput khi run chay tu [-1,1] gia tri nam trong doan nay. toc do thay doi phuong di chuyen
        //xInput = Input.GetAxis("Horizontal");
        //if (canMove)
        //{
        //    rb.velocity = new Vector2(xInput * moveSpeed, rb.velocity.y);

        //}

    }






    
    /// <summary>
    /// kiem tra player da cham dat hay chua.
    /// gia tri groundcheckdistance can phai chinh xac de tranh viec player bi treo khi di chuyen tren cac dia hinh khac nhau
    /// groundcheckdistance phu thuoc vao ham OnDrawGizmos.
    /// </summary>
    protected virtual void HandleCollision()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatisGround);
        /*
         * ban chat method raycast khong phai la bool method, nhung trong unity co su ngam chuyen doi sang bool de co the detech collision la true or false.
         * detect player co cham dat hay khong,
         * detect hit, hay player danh co trung muc tieu hay khong
         */
    }

    protected virtual void HandleAttack()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            anim.SetTrigger("attack");
            rb.velocity = new Vector2(0, rb.velocity.y); // khi tan cong thi khong the di chuyen theo phuong x nua

        }

    }


    protected virtual void HandleFlip()
    {
        if (rb.velocity.x > 0 && !faceRight)
        {
            Flip();
        }
        if (rb.velocity.x < 0 && faceRight)
        {
            Flip();
        }
    }


    /// <summary>
    /// su dung flip bang cach rigid.rotate, game 2D quay y 180 la ok.
    /// luu y doan quay mat
    /// </summary>
    [ContextMenu("flip test")]
    protected void Flip()
    {
        transform.Rotate(0, 180, 0);
        faceRight = !faceRight;
        faceDir = faceDir * -1;
    }

    /// <summary>
    /// OnDrawGizmos is a special Unity callback method that lets you draw editor-only visual aids (called Gizmos) in the Scene view. It’s super handy for debugging, visualizing ranges, paths, or object relationships while you’re building your game.
    /// Note: dung ke gizmos phai giao voi boxcollider2D hoac collider 3d thi gia tri groundcheckdistance moi chinh xac
    /// </summary>
    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, -groundCheckDistance)); // ke duong tham chieu xuong duoi de kiem tra dieu kien cua collision
        if (attackPoint != null)
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius); // ve hinh tron de biet duoc vung tan cong
    }
    //private void FixedUpdate()
    //{

    //}
}
