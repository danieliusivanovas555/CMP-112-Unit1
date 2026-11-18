//Dan Ivanovas
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : Health
{

    public string gameOverScene = "GameOver";

    protected override void Die()
    {
        Debug.Log("Game Over!");

        //Load the game over screen

        //When working with different scenes best method for me is to use scenemanagement 
        //Since it makes it very simple and convenient 
        //Events could also be used to call but I prefer this
        SceneManager.LoadScene(gameOverScene);
    }
}