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
            Debug.LogError("An instance of TorchManager already exists!");
            return;
        }

        Instance = this;

        selectedControllerConfigurator = useTorchOnLeftController ? leftControllerConfigurator : rightControllerConfigurator;

        Instantiate(torchPrefabGameObject, selectedControllerConfigurator.transform);
    }
}
