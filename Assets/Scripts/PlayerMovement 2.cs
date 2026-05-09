using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D _rigRigidbody2D;
    private PlayerDialogue _playerDialogue;
    private float _xVelocity = 0f;
    private float _yVelocity = 0f;
    public float speed = 3;
    public bool isInArena = false;

    // Start is called before the first frame update
    private void Start() {
        _rigRigidbody2D = GetComponent<Rigidbody2D>();
        _playerDialogue = GetComponent<PlayerDialogue>();    
    }

    // Update is called once per frame
    private void FixedUpdate() {
        if (isInArena) {
            if (_playerDialogue.IsSpeaking()) {
                _xVelocity = 0;
                _yVelocity = 0;
            }
            else {
                _xVelocity = Input.GetAxis(Structs.Input.horizontal);
                _yVelocity = Input.GetAxis(Structs.Input.vertical);
            }   
        }
        else {
            _xVelocity = Input.GetAxis(Structs.Input.horizontal);
            _yVelocity = Input.GetAxis(Structs.Input.vertical);
        }
        
        _rigRigidbody2D.velocity = new Vector2(_xVelocity, _yVelocity) * speed; 
    }
}
