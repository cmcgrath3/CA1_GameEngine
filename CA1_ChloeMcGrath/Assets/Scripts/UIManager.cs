using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;

    public void SetScore(int score)
    {
        // Convert score integer to String and set it as the text
        scoreText.SetText(score.ToString());
    }

}
