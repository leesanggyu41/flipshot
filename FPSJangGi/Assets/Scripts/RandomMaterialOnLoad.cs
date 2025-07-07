using UnityEngine;

public class RandomMaterialOnLoad : MonoBehaviour
{
    [Tooltip("적용할 머티리얼 목록")]
    public Material[] materialOptions;

    private Renderer objectRenderer;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();

        if (materialOptions.Length == 0 || objectRenderer == null)
        {
            Debug.LogWarning("머티리얼이 없거나 Renderer를 찾지 못했습니다.");
            return;
        }

        int randomIndex = Random.Range(0, materialOptions.Length);
        objectRenderer.material = materialOptions[randomIndex];
    }
}