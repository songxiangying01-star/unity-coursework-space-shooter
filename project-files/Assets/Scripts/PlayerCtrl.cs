using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCtrl : MonoBehaviour
{
    private Transform tr;
    private Animation anim;

    public float moveSpeed = 10f;
    public float turnSpeed = 115f;
    public float jumpHeight = 2.0f;
    public float jumpDuration = 0.55f;
    
    private readonly float initHP = 1000.0f;
    public float currHP;
    private Image hpBar;
    private bool isJumping;

    public delegate void PlayerDieHandler();
    public static event PlayerDieHandler OnPlayerDie;

    IEnumerator Start()
    {
        hpBar = GameObject.FindGameObjectWithTag("HP_BAR")?.GetComponent<Image>();

        currHP = initHP;

        tr = GetComponent<Transform>();
        anim = GetComponent<Animation>();

        anim.Play("Idle");

        turnSpeed = 0.0f;
        yield return new WaitForSeconds(0.3f);
        turnSpeed = 115.0f;
    }

    void Update()
    {
        if (GameManager.instance != null && GameManager.instance.IsGameOver) {
            return;
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        float r = Input.GetAxis("Mouse X");

        Vector3 moveDir = (Vector3.forward* v) + (Vector3.right) * h;

        tr.Translate(moveDir.normalized * moveSpeed * Time.deltaTime);

        tr.Rotate(Vector3.up * turnSpeed * Time.deltaTime * r);

        if (Input.GetKeyDown(KeyCode.Space) && !isJumping) {
            StartCoroutine(Jump());
        }
    
        PlayerAnim(h, v);
    }

    void PlayerAnim(float h, float v) {
        if (isJumping) {
            return;
        }

        if (v >= 0.1f) {
            anim.CrossFade("RunF", 0.25f);
        }
        else if (v <= -0.1f) {
            anim.CrossFade("RunB", 0.25f);
        }
        else if (h >= 0.1f) {
            anim.CrossFade("RunR", 0.25f);
        }
        else if (h <= -0.1f) {
            anim.CrossFade("RunL", 0.25f);
        }
        else {
            anim.CrossFade("Idle", 0.25f);
        }
    }
    
    void OnTriggerEnter(Collider coll) {
        if (currHP >= 0.0f && coll.CompareTag("PUNCH")) {
            currHP -= 10.0f;
            DisplayHealth();

            Debug.Log($"Player hp = {currHP/initHP}");

            if (currHP <= 0.0f) {
                PlayerDie();
            }
        }
    }

    void PlayerDie() {
        Debug.Log("Player Die !");

        OnPlayerDie?.Invoke();

        GameManager.instance.IsGameOver = true;
    }

    void DisplayHealth() {
        hpBar.fillAmount = currHP/initHP;
    }

    IEnumerator Jump() {
        isJumping = true;
        anim.CrossFade("Idle", 0.1f);

        float elapsed = 0.0f;
        Vector3 startPos = tr.position;

        while (elapsed < jumpDuration) {
            float progress = elapsed / jumpDuration;
            float yOffset = Mathf.Sin(progress * Mathf.PI) * jumpHeight;
            tr.position = new Vector3(tr.position.x, startPos.y + yOffset, tr.position.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        tr.position = new Vector3(tr.position.x, startPos.y, tr.position.z);
        isJumping = false;
    }
}
