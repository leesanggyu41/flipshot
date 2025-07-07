using UnityEngine;

public class Cus : MonoBehaviour
{
    Texture2D nomal;
    Texture2D nomal2;
    public void Start()
    {
        nomal = Resources.Load<Texture2D>("NC");
        nomal2 = Resources.Load<Texture2D>("HC");
        Cursor.SetCursor(nomal, new Vector2(0, 0), CursorMode.Auto);
    }
    public void OnMouseOver()
    {
        Cursor.SetCursor(nomal2, new Vector2(nomal.width / 3, 0), CursorMode.Auto);
    }

    public void OnMouseExit()
    {
        Cursor.SetCursor(nomal, new Vector2(0, 0), CursorMode.Auto);
    }
}
