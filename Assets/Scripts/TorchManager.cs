using UnityEngine;

public class TorchManager : MonoBehaviour {
    public static TorchManager Instance { get; private set; }

    public ControllerConfigurator ControllerConfigurator => selectedControllerConfigurator;

    [SerializeField]
    private ControllerConfigurator leftControllerConfigurator;
    [SerializeField]
    private ControllerConfigurator rightControllerConfigurator;
    [SerializeField]
    private bool useTorchOnLeftController;

    [Space, SerializeField]
    private GameObject torchPrefabGameObject;

    private ControllerConfigurator selectedControllerConfigurator;

    void Awake() {
        if (Instance != null) {
            Debug.LogError($"An instance of {nameof(TorchManager)} already exists!");
            return;
        }

        Instance = this;

        selectedControllerConfigurator = useTorchOnLeftController ? leftControllerConfigurator : rightControllerConfigurator;

        if (selectedControllerConfigurator == null) {
            Debug.LogError($"{nameof(ControllerConfigurator)} not assigned to destination controller!");
            return;
        }
        Instantiate(torchPrefabGameObject, selectedControllerConfigurator.transform);
    }
}
