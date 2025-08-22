using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlsRebind : MonoBehaviour
{
    [SerializeField] private InputActionAsset actions;
    private string filePath;

    private void Awake()
    {
        filePath = Path.Combine(Application.persistentDataPath, "Controls.json");

        LoadRebinds();
    }

    public void SaveRebinds()
    {
        string json = actions.SaveBindingOverridesAsJson();
        File.WriteAllText(filePath, json);
        Debug.Log("Rebinds saved to: " + filePath);
    }

    public void LoadRebinds()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            actions.LoadBindingOverridesFromJson(json);
            Debug.Log("Rebinds loaded from: " + filePath);
        }
    }

    public void ResetRebinds()
    {
        actions.RemoveAllBindingOverrides();
        if (File.Exists(filePath))
            File.Delete(filePath);
    }

    public void StartRebind(InputActionReference actionToRebind, int bindingIndex)
    {
        actionToRebind.action.Disable();

        actionToRebind.action.PerformInteractiveRebinding(bindingIndex)
            .WithControlsExcluding("<Mouse>/position")
            .OnComplete(operation =>
            {
                actionToRebind.action.Enable();
                operation.Dispose();

                // od razu zapis po zmianie
                SaveRebinds();
            })
            .Start();
    }
}