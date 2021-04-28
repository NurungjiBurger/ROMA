using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    enum Direction { Left, Right, Up, Stop };

    [SerializeField]
    private KeyCode jump = KeyCode.D;

    [SerializeField]
    private float moveSpeed = 0.1f;

    [SerializeField]
    private float jumpPower = 0.4f;

    [SerializeField]
    private GameObject prefabTimer;

    private Timer dashTimer;

    private Direction dir = Direction.Stop;

    private bool isMonster = false;

    private float time;

    private bool dashing = false;

    private bool isGround = true;
    private bool jumping = false;
    private bool isPortal = false;

    private bool isGoNext = false;

    private Animator animator;


    public bool GetGoNext()
    {
        return isGoNext;
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        dashTimer = Instantiate(prefabTimer).GetComponent<Timer>();

        dashTimer.SetCooldown(5.0f);

    }

    private void FixedUpdate()
    {
        switch (dir)
        {
            case Direction.Right:
                GetComponent<Rigidbody2D>().AddForce(Vector2.right * moveSpeed);
                break;
            case Direction.Left:
                GetComponent<Rigidbody2D>().AddForce(Vector2.left * moveSpeed);
                break;
            case Direction.Up:
                isGoNext = true;
                break;
            case Direction.Stop:
            default:
                //GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, 0f);
                break;
        }

        if (jumping)
        {
            jumping = false;
            isGround = false;
            GetComponent<Rigidbody2D>().AddForce(Vector2.up * jumpPower);
        }
        if (dashing)
        {
            dashTimer.TimerSetZero();
            dashing = false;
            if (GetComponent<SpriteRenderer>().flipX) GetComponent<Transform>().Translate(1.5f,0,0);
            else GetComponent<Transform>().Translate(-1.5f, 0, 0);
        }

        // �ӵ� ����
        if (GetComponent<Rigidbody2D>().velocity.x >= 2.5f)
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(2.5f, GetComponent<Rigidbody2D>().velocity.y);
        }
        else if (GetComponent<Rigidbody2D>().velocity.x <= -2.5f)
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(-2.5f, GetComponent<Rigidbody2D>().velocity.y);
        }
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("move", false);
        // �������̵�
        if (Input.GetKey(KeyCode.RightArrow))
        {
            dir = Direction.Right;
            GetComponent<SpriteRenderer>().flipX = true;
            animator.SetBool("move", true);
        }
        // �����̵�
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            dir = Direction.Left;
            GetComponent<SpriteRenderer>().flipX = false;
            animator.SetBool("move", true);
        }
        // ��Ż��ȣ�ۿ�?
        if (Input.GetKey(KeyCode.UpArrow) && isPortal)
        {
            dir = Direction.Up;
            animator.SetBool("move", true);
        }
        // ����
        if (Input.GetKeyDown(jump) && isGround)
        {
            jumping = true;
        }
        // �뽬
        if (Input.GetKey(KeyCode.Space) && dashTimer.CooldownCheck())
        {
            dashing = true;
        }
        // ����
        if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.UpArrow))
        {
            dir = Direction.Stop;
        }

        if (isGoNext) isGoNext = false;

        if (GetComponent<Rigidbody2D>().velocity.y > 0)
        {
            GetComponent<Collider2D>().isTrigger = true;
        }

        if (GetComponent<Rigidbody2D>().velocity.y < 0)
        {
            GetComponent<Collider2D>().isTrigger = false;
        }      
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Portal"))
        {
            isPortal = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Portal"))
        {
            isPortal = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGround = true;
        }

        if (collision.collider.CompareTag("Monster"))
        {
            isMonster = true;
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Monster"))
        {
           isMonster = false;
        }
    }

}