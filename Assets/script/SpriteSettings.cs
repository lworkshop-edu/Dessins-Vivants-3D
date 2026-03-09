using UnityEngine;

public class SpriteSettings : MonoBehaviour
{
   public Sprite sprite ;
   public string assetPath;
    public float speed = 5f;
    public Color color = Color.white;
    public int sortingOrder = 0;

    private SpriteRenderer spriteRenderer;

    void Start()
    {

        // spriteRenderer.MeshPrefab.Create();
        // spriteRenderer.Create();
        //spriteRenderer.SpriteConfigData = color;
        // var spriteAssist = GetComponent<SpriteProcessor>();
        // spriteRenderer.SpriteProcessor.Create();
        // spriteAssist.Create();
        //Debug.Log(spriteAssist);
    }


}
