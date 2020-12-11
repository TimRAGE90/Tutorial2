using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{

    private Rigidbody2D rd2d;
    
    AudioSource audioSource;
    public AudioClip musicClipOne;
    public AudioClip musicClipTwo;
    public AudioClip musicClipThree;
    public AudioClip talkSound;
    public AudioClip painSound;
    public AudioClip hitSound;
    public AudioClip collectSound;
    public AudioClip winSound;
    public AudioClip jumpSound;
    
    private bool gameOver;
    public static int level;

    Animator anim;

    Vector2 lookDirection = new Vector2(1,0);

    private float speed;

    public Text score;
    public Text lives;
    private int livesValue = 5;
    private int scoreValue = 8;
    private bool facingRight = true;
    public Text winText;
    private bool isJumping = true;



    // Start is called before the first frame update
    void Start()
    {
        rd2d = GetComponent<Rigidbody2D>();
        score.text = "Bottles Left: " + scoreValue.ToString();
        lives.text = "Health: " + livesValue.ToString();
        winText.text = "";
        anim = GetComponent<Animator>();
        speed = 8.0f;
        gameOver = false;
        audioSource = GetComponent<AudioSource>();
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Level1"))
        {
            level = 1;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            anim.SetInteger("State", 1); 
        }

        if (Input.GetKeyUp(KeyCode.D))
        {
            anim.SetInteger("State", 0);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            anim.SetInteger("State", 1);
        }

        if (Input.GetKeyUp(KeyCode.A))
        {
            anim.SetInteger("State", 0);
        }


        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rd2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                Debug.Log("Raycast has hit the object " + hit.collider.gameObject);
                NonPlayableCharacter character = hit.collider.GetComponent<NonPlayableCharacter>();
                if (character != null)
                {
                    character.DisplayDialog();
                    PlaySound(talkSound);
                }
            }
        }

        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }

    }


    // Update is called once per frame
    void FixedUpdate()
    {
        float hozMovement = Input.GetAxis("Horizontal");
        float verMovement = Input.GetAxis("Vertical");

        rd2d.AddForce(new Vector2(hozMovement * speed, verMovement * speed));

        if (facingRight == false && hozMovement > 0)
        {
            Flip();
        }
        else if (facingRight == true && hozMovement < 0)
        {
            Flip();
        }


        if (isJumping == true && verMovement > 0)
        {
            anim.SetInteger("State", 2);
        }
        else if (isJumping == false && verMovement == 0)
        {
            anim.SetInteger("State", 0);
        }

        if (Input.GetKey(KeyCode.R))
        {
            if (gameOver == true)
            {
                SceneManager.LoadScene("Level1");
            }
        }   

    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector2 Scaler = transform.localScale;
        Scaler.x = Scaler.x * -1;
        transform.localScale = Scaler;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == "Coin")
        {
            scoreValue -= 1;
            score.text = "Bottles Left: " + scoreValue.ToString();
            Destroy(collision.collider.gameObject);
            PlaySound(collectSound);

            if (scoreValue == 0)
            {
                winText.text = "A Clean Bill of Health! Press [R] to Restart Level!";
                audioSource.clip = musicClipTwo;
                audioSource.Play();
                audioSource.loop = false;
                gameOver = true;
                PlaySound(winSound);
            }
        }

        if(collision.collider.tag == "Enemy")
        {
            livesValue -= 1;
            lives.text = "Health: " + livesValue.ToString();
            Destroy(collision.collider.gameObject);
            anim.SetInteger("State", 3);
            PlaySound(hitSound);
            PlaySound(painSound);

            if (livesValue == 0)
            {
                winText.text = "Quacked Under Pressure! Press [R] to Restart Level!";
                gameOver = true;
                speed = 0.0f;
                audioSource.clip = musicClipThree;
                audioSource.Play();
                audioSource.loop = false;
                GetComponent<Collider2D>().enabled = false;

            }

        }
    }

    public void PlaySound(AudioClip clip)
        {
            audioSource.PlayOneShot(clip);
        }


    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.collider.tag == "Ground")
        {
            if(Input.GetKey(KeyCode.W))
            {
                rd2d.AddForce(new Vector2(0, 3), ForceMode2D.Impulse);
                PlaySound(jumpSound);
            }

        }
    }

}
