using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define.CreatureState;

public class MyPlayerController : PlayerController
{
    protected override void Init()
    {
        base.Init();
    }
    
    protected override void UpdateController()
    {
        switch (State)
        {
            case Idle:
                GetDirInput();
                break;
            case Moving:
                GetDirInput();
                break;
        }
		
        base.UpdateController();
    }
    
    // 키보드 입력
    void GetDirInput()
    {
        if (Input.GetKey(KeyCode.W))
        {
            Dir = Define.MoveDir.Up;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            Dir = Define.MoveDir.Down;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            Dir = Define.MoveDir.Left;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Dir = Define.MoveDir.Right;
        }
        else
        {
            Dir = Define.MoveDir.None;			
        }
    }
    
    void LateUpdate()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }
    
    protected override void UpdateIdle()
    {
        // 이동 상태로 갈지 확인
        if (Dir != Define.MoveDir.None)
        {
            State = Define.CreatureState.Moving;
            return;
        }

        // 스킬 상태로 갈지 확인
        if (Input.GetKey(KeyCode.Space))
        {
            State = Define.CreatureState.Skill;
            //_coSkill = StartCoroutine("CoStartPunch");
            _coSkill = StartCoroutine("CoStartShootArrow");
        }
    }

}
