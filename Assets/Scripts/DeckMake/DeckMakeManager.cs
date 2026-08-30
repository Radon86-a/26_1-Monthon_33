using UnityEngine;
using UnityEngine.UI;

public class DeckMakeManager : MonoBehaviour
{
    [SerializeField] private Button returnButton;
    void Start()
    {
        if (returnButton != null)
        {
            returnButton.onClick.AddListener(OnReturnButtonClicked);
        }
    }

    private void OnReturnButtonClicked()
    {
        SceneManager.MoveScene(1);
    }
    
}
