using UnityEngine;

namespace CompleteProject
{
    public class GameOverManager : MonoBehaviour
    {
        public PlayerHealth playerHealth;       // Reference to the player's health.

        Animator anim;                          // Reference to the animator component.

        HighScoreManager highScoreManager;
        ScoreManager scoreManager;

        bool gameOver;

        void Awake ()
        {
            gameOver = false;

            // Set up the reference.
            anim = GetComponent <Animator> ();

            highScoreManager = GameObject.Find("HighScoreText").GetComponent<HighScoreManager>();
            scoreManager = GameObject.Find("ScoreText").GetComponent<ScoreManager>();
        }


        void Update ()
        {
            // If the player has run out of health...
            if(playerHealth.currentHealth <= 0)
            {
                if (!gameOver)
                {
                    gameOver = true;

                    // ... tell the animator the game is over.
                    anim.SetTrigger("GameOver");

                    highScoreManager.NewScore(scoreManager.GetScore());
                }

                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown("joystick button 0"))
                {
                    //gameOver = false;
                }
            }
        }
    }
}