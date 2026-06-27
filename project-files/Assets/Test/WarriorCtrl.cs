#pragma warning disable IDE0051

using UnityEngine;
using UnityEngine.InputSystem;

public class WarriorCtrl : MonoBehaviour
{
    private Animator anim;
    private new Transform transform;
    private Vector3 moveDir;

    private PlayerInput playerInput;
    private InputActionMap mainActionMap;
    private InputAction moveAction;
    private InputAction attackAction;

    void Start() {
        anim = GetComponent<Animator>();
        transform = GetComponent<Transform>();
        playerInput = GetComponent<PlayerInput>();

        mainActionMap = playerInput.actions.FindActionMap("PlayerActions");

        moveAction = mainActionMap.FindAction("Move");
        attackAction = mainActionMap.FindAction("Attack");

        moveAction.performed += ctx => {
            Vector2 dir = ctx.ReadValue<Vector2>();
            moveDir = new Vector3(dir.x, 0, dir.y);
            anim.SetFloat("Movement", dir.magnitude);
        };
        moveAction.canceled += ctx => {
            moveDir = Vector3.zero;
            anim.SetFloat("Movement", 0.0f);
        };

        attackAction.performed += ctx => {
            Debug.Log("Attack by c# event");
            anim.SetTrigger("Attack");
        };
    }

    void Update() {
        if (moveDir != Vector3.zero) {
            transform.rotation = Quaternion.LookRotation(moveDir);
            transform.Translate(Vector3.forward * Time.deltaTime * 4.0f);
        }
    }

#region SEND_MESSAGE
    void OnMove(InputValue value) {
        Vector2 dir = value.Get<Vector2>();

        moveDir = new Vector3 (dir.x, 0, dir.y);

        anim.SetFloat("Movement", dir.magnitude);
        Debug.Log($"Move = ({dir.x}) {dir.y}");
    }

    void OnAttack() {
        Debug.Log("Attack");
        anim.SetTrigger("Attack");
    }
#endregion SEND_MESSAGE

#region UNITY_EVENTS 
    public void OnMove(InputAction.CallbackContext ctx) {
        Vector2 dir = ctx.ReadValue<Vector2>();
        moveDir = new Vector3 (dir.x, 0, dir.y);

        anim.SetFloat("Movement", dir.magnitude);
        Debug.Log($"Move = ({dir.x}) {dir.y}");
    }
    public void OnAttack(InputAction.CallbackContext ctx) {
        Debug.Log($"ctx.phase={ctx.phase}");

        if (ctx.performed) {
            Debug.Log("Attack");
            anim.SetTrigger("Attack");
        }
    }
#endregion UNITY_EVENTS
}

