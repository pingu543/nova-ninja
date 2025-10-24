using UnityEngine;
using UnityEngine.UIElements;

public class UIScript : MonoBehaviour
{
    private Button Button_Restart;
    private Label Label_CurrentGrade;
    private VisualElement Container_Result;
    private VisualElement root;

    [SerializeField] private GameManager gameManager;

    private void Awake()
    {
        // GameManager must be assigned in the Inspector.
        if (gameManager == null)
            Debug.LogWarning("UIScript: GameManager not assigned in inspector. Assign the GameManager reference on the UIScript component.");

        // UIDocument must be on the same GameObject as this script
        var uiDoc = GetComponent<UIDocument>();
        if (uiDoc == null)
        {
            Debug.LogWarning("UIScript: UIDocument component not found on this GameObject.");
            return;
        }

        root = uiDoc.rootVisualElement;
        if (root == null)
        {
            Debug.LogWarning("UIScript: rootVisualElement is null.");
            return;
        }

        // Query by name from the UI Builder / UXML (names must match)
        Button_Restart = root.Q<Button>("Button_Restart");
        Label_CurrentGrade = root.Q<Label>("Label_CurrentGrade");
        Container_Result = root.Q<VisualElement>("Container_Result");

        if (Button_Restart == null)
            Debug.LogWarning("UIScript: Button_Restart not found in UIDocument. Verify element name.");
        else
            Button_Restart.clicked += OnRestartClicked;

        if (Label_CurrentGrade == null)
            Debug.LogWarning("UIScript: Label_CurrentGrade not found in UIDocument. Verify element name.");
    }

    private void OnDestroy()
    {
        if (Button_Restart != null)
            Button_Restart.clicked -= OnRestartClicked;
    }

    private void Start()
    {
        RefreshUI();
    }

    private void Update()
    {
        // Keep UI in sync. For better performance, update only when game state changes.
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (gameManager == null || root == null) return;

        if (Label_CurrentGrade != null)
            Label_CurrentGrade.text = gameManager.CurrentLetterGrade;

        if (Container_Result != null)
            Container_Result.visible = gameManager.IsGameOver; // hidden when IsGameOver is false
    }

    private void OnRestartClicked()
    {
        if (gameManager == null)
        {
            Debug.LogWarning("UIScript: Cannot restart - GameManager reference is missing.");
            return;
        }

        gameManager.ResetGame();
    }
}
