using Structs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerSpriteBody : MonoBehaviour
    {
        [SerializeField] private float _animFrameTime = 0.2f;
    
        private SpriteRenderer _spriteRenderer;

        private Structs.MoveDir _moveDir = Structs.MoveDir.Right;
        private Structs.MoveState _moveState = Structs.MoveState.Idle;
        private Structs.AttackStates _attackState = AttackStates.None;
    
        private int _animFrame = 0;
        private float _currAnimeFrameTime = 0.0f;
    
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // Update is called once per frame
        void Update()
        {
            _currAnimeFrameTime += Time.deltaTime;
        
            // Update sprite frames
            if (_currAnimeFrameTime >= _animFrameTime)
            {
                _animFrame = (_animFrame + 1) % 8;
                Sprite newSprite = Managers.SpriteManager.Instance.GetPlayerSprite(_moveState, _moveDir, _animFrame);
                _spriteRenderer.sprite = newSprite;
                _currAnimeFrameTime = 0.0f;
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            Vector2 input = context.ReadValue<Vector2>();
        
            // Check if movement should be processed
            float absX = Mathf.Abs(input.x);
            float absY = Mathf.Abs(input.y);
            if (absX < 0.05f & absY < 0.05f)
            {
                _moveState = Structs.MoveState.Idle;
                _animFrame = 0;
                return;
            }
        
            // Sprite logic
            // k = tan(22.5°) ≈ 0.4142
            // s = tan(67.5°) ≈ 2.4142
            const float k = 0.41421356f;
            const float s = 2.41421356f;
            if (absY <= k * absX)
            {
                _moveDir = input.x >= 0 ? Structs.MoveDir.Right : Structs.MoveDir.Left;
            }
            else if (absY >= s * absX)
            {
                _moveDir = input.y >= 0 ? Structs.MoveDir.Up : Structs.MoveDir.Down;
            }
            else
            {
                _moveDir = input.x >= 0 ? (input.y >= 0 ? Structs.MoveDir.RightUp : Structs.MoveDir.DownRight)
                    : (input.y >= 0 ? Structs.MoveDir.LeftUp : Structs.MoveDir.LeftDown);
				
            }
		
            if (_moveState != MoveState.Attack)
            {
                _moveState = Structs.MoveState.Move;
            }

            Sprite newSprite = Managers.SpriteManager.Instance.GetPlayerSprite(_moveState, _moveDir, _animFrame);
            _spriteRenderer.sprite = newSprite;
        }

        public void SetAttackState(Structs.AttackStates newState)
        {
            _attackState =  newState;
            if (newState == AttackStates.None)
            {
                _moveState = MoveState.Move;
                _animFrameTime = 0.2f;
            }
            else
            {
                _animFrameTime = 0.08f;
                _moveState = MoveState.Attack;
            }

            Sprite newSprite = Managers.SpriteManager.Instance.GetPlayerSprite(_moveState, _moveDir, _animFrame);
            _spriteRenderer.sprite = newSprite;
        }
    }
}
