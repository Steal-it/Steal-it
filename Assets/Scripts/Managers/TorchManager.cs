using UnityEngine;

public class TorchManager : MonoBehaviour {
    public static TorchManager Instance { get; private set; }

    public TorchControllerConfigurator ControllerConfigurator => selectedControllerConfigurator;

    [SerializeField]
    private TorchControllerConfigurator leftControllerConfigurator;
    [SerializeField]
    private TorchControllerConfigurator rightControllerConfigurator;
    [SerializeField]
    private bool useTorchOnLeftController;

    [Space, SerializeField]
    private GameObject torchPrefabGameObject;

    private TorchControllerConfigurator selectedControllerConfigurator;

    void Awake() {
        if (Instance != null) {
            Debug.LogError($"An instance of {nameof(TorchManager)} already exists!");
            return;
        }

        Instance = this;

        leftControllerConfigurator.ConfigureTorchHandController();
        rightControllerConfigurator.ConfigureTorchHandController();

        selectedControllerConfigurator = useTorchOnLeftController ? leftControllerConfigurator : rightControllerConfigurator;

        Instantiate(torchPrefabGameObject, selectedControllerConfigurator.HandTorchControllerTransform);
    }
}
