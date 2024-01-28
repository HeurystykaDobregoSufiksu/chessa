using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chessTile : MonoBehaviour
{
    private Vector2Int _position = new Vector2Int();
    public chessFigure currentFigure;
    public boardManager boardManager;

    public GameObject overlay;
    
    [Header("Selected,PrevMoveStart,PrevMoveFinish,PossibleMove,WrongMove")]
    public List<Material> stateMaterials;
    public int MyProperty { get; set; }
    public Vector2Int position { get => _position; set => _position = value; }
    [Header("Sounds")]
    public List<AudioClip> selectSound;
    public List<AudioClip> dropSound;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void playPickSound() {
        int selected = Random.Range(0, selectSound.Count);
        transform.GetComponent<AudioSource>().clip = selectSound[selected];
        transform.GetComponent<AudioSource>().Play();
    }
    public void playDropSound() {
       int selected = Random.Range(0, dropSound.Count);
       transform.GetComponent<AudioSource>().clip = dropSound[selected];
       transform.GetComponent<AudioSource>().Play();
    }
    public void changeOverlay(int id) { //Selected,PrevMoveStart,PrevMoveFinish,PossibleMove
        id = Mathf.Clamp(id, 0, stateMaterials.Count - 1);
        overlay.active = true;
        overlay.GetComponent<Renderer>().material = stateMaterials[id];
    }
    public void hideOverlay() {
        overlay.active = false;
    }
    public void moveFigureToTile() {
        if (!currentFigure) return;
        currentFigure.transform.position = transform.position;
        float rotation = currentFigure.isWhite ? 1 : -1;
        currentFigure.transform.rotation = Quaternion.Euler(-90, 90 * rotation, 0);
        playDropSound();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
