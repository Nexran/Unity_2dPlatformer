using UnityEngine;
using System.Collections;
using System;

public class Player : MonoBehaviour 
{	
	private static Vector3 _left = new Vector3(-5f, 5f);
	private static Vector3 _right = new Vector3(5f, 5f);


	private static string _animationStateName = "_state";
	private static float _runSpeed = 25f;
	private static float _jumpHeight = 500f;

	private enum PlayerStates
	{
		NONE = 0,
		RUN = 1,
		IDLE = 2,
		JUMP = 3,
		ATTACK = 4
	}

	[SerializeField]
	private GameObject _apple;

	private Rigidbody _rigidBody;
	private Animator _animator; 

	private int _animatorStateID;
	private PlayerStates _playerState;

	public event EventHandler AttackComplete;

	private void Awake()
	{
		Application.runInBackground = true;
		_rigidBody = this.GetComponent<Rigidbody>();
		_animator = this.GetComponent<Animator>();

		_animatorStateID = Animator.StringToHash(_animationStateName);

		AttackComplete += OnAttackComplete;
	}

	private void Destroy()
	{
		AttackComplete -= OnAttackComplete;
	}

	private void Update()
	{
		if(_rigidBody != null && _playerState == PlayerStates.JUMP)
		{
			if(_rigidBody.velocity.y == 0f)
			{
				ChangeState(PlayerStates.IDLE);
			}
		}
	}

	private void FixedUpdate() 
	{
		if(Input.GetKeyDown(KeyCode.W))
		{
			ChangeState(PlayerStates.JUMP);
			return;
		}

		if(Input.GetKeyDown(KeyCode.E))
		{
			ChangeState(PlayerStates.ATTACK);
			return;
		}
			
		if(Input.GetKey(KeyCode.A))
		{
			ChangeState(PlayerStates.RUN);

			this.transform.localScale = _left;
			MovePlayer(-(_runSpeed), 0f);
		}
		else if(Input.GetKey(KeyCode.D))
		{
			ChangeState(PlayerStates.RUN);

			this.transform.localScale = _right;
			MovePlayer(_runSpeed, 0f);
		}
		else
		{
			if(_playerState != PlayerStates.JUMP)
			{
				ChangeState(PlayerStates.IDLE);
			}
		}
	}

	private void ChangeState(PlayerStates state)
	{
		if(_playerState != state)
		{
			StateChanged(state);
		}
	}

	private void StateChanged(PlayerStates newState)
	{
		_playerState = newState;
		EnterState();
	}

	private void SpawnApple()
	{
		GameObject newApple = GameObject.Instantiate(_apple);
		newApple.transform.SetParent(this.transform);
		newApple.transform.localPosition = _apple.transform.localPosition;
		newApple.transform.localScale = Vector3.one;
	}

	private void EnterState()
	{
		if(_animator != null)
		{
			_animator.SetInteger(_animatorStateID, (int)_playerState);
		}
	
		switch(_playerState)
		{
			case PlayerStates.ATTACK:
			{
				StartCoroutine(AnimationComplete(AttackComplete));
			}
			break;

			case PlayerStates.JUMP:
			{
				MovePlayer(0f, _jumpHeight);
			}
			break;
		}
	}

	private void MovePlayer(float x, float y)
	{
		if(_rigidBody != null)
		{
			_rigidBody.AddForce(x, y, 0f);
		}
	}

	private void OnAttackComplete(object sender, EventArgs args)
	{
		_playerState = PlayerStates.IDLE;
		_animator.Play("Idle");
	}

	public IEnumerator AnimationComplete(EventHandler eventComplete, float additionalDelay = 0.0f)
	{
		yield return StartCoroutine(WaitForAnimation(additionalDelay));

		System.EventHandler handler = eventComplete;
		if (handler != null)
		{
			handler(this, System.EventArgs.Empty);
		}
	}

	private IEnumerator WaitForAnimation(float additionalDelay = 0.0f)
	{
		//	we must wait for end of frame to ensure that the new animation has started 
		yield return new WaitForEndOfFrame();

		float duration = 0.0f;
		if (_animator != null)
		{
			AnimatorStateInfo info = _animator.GetCurrentAnimatorStateInfo(0);
			duration = info.length;
		}
		yield return new WaitForSeconds(duration + additionalDelay);
	}
}