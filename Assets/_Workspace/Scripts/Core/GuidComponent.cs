using UnityEngine;

[ExecuteInEditMode]
public class GuidComponent : MonoBehaviour
{
    [SerializeField]
    private string _guid = "";
    public string Guid => _guid;

    private void Awake()
    {
        if (string.IsNullOrEmpty(_guid))
        {
            GenerateGuid();
        }
    }

    [ContextMenu("Generate New GUID")]
    private void GenerateGuid()
    {
        _guid = System.Guid.NewGuid().ToString();
    }
}