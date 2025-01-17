using Protocol;
using Server.Data;
using Server.Game.Room;
using System;

namespace Server.Game.Object
{
    public class Monster : GameObject
    {
        public Monster()
        {
            ObjectType = GameObjectType.Monster;
            Stat.Level = 1;
            Stat.Hp = 100;
            Stat.MaxHp = 100;
            Stat.Speed = 5.0f;

            State = CreatureState.Idle;
        }

        // FSM
        public override void Update()
        {
            switch (State)
            {
                case CreatureState.Idle:
                    UpdateIdle();
                    break;
                case CreatureState.Moving:
                    UpdateMoving();
                    break;
                case CreatureState.Skill:
                    UpdateSkill();
                    break;
                case CreatureState.Dead:
                    UpdateDead();
                    break;

            }
        }

        int _searchCellDist = 10;
        int _chaseCellDist = 20;
        long _nextSearchTick = 0;
        Player _target;
        protected void UpdateIdle()
        {
            if (_nextSearchTick > Environment.TickCount64)
                return;
            _nextSearchTick = Environment.TickCount64 + 1000;

            var target = Room.FindPlayer(p =>
            {
                Vector2Int dir = p.CellPos - CellPos;
                return dir.cellDistFromZero <= _searchCellDist;
            });

            if (target == null)
                return;

            _target = target;
            State = CreatureState.Moving;
        }

        int _skillRange = 1;
        long _nextMoveTick = 0;
        protected void UpdateMoving()
        {
            if (_nextMoveTick > Environment.TickCount64) return;
            int moveTick = (int)(1000 / Speed);
            _nextMoveTick = Environment.TickCount64 + moveTick;

            if (_target == null || _target.Room != Room)
            {
                _target = null;
                State = CreatureState.Idle;
                BroadcastMove();
                return;
            }

            Vector2Int dir = _target.CellPos - CellPos;
            int dist = dir.cellDistFromZero;
            if (dist == 0 || dist > _chaseCellDist)
            {
                _target = null;
                State = CreatureState.Idle;
                BroadcastMove();
                return;
            }

            var path = Room.Map.FindPath(CellPos, _target.CellPos, checkObject: false);
            if (path.Count < 2 || path.Count > _chaseCellDist)
            {
                _target = null;
                State = CreatureState.Idle;
                BroadcastMove();
                return;
            }

            // 스킬로 넘어갈지 체크
            if (dist <= _skillRange && (dir.x == 0 || dir.y == 0))
            {
                _coolTick = 0;
                State = CreatureState.Skill;
                return;
            }

            // 이동
            Dir = GetDirFromVec(path[1] - CellPos);
            Room.Map.ApplyMove(this, path[1]);
            BroadcastMove();
        }

        protected void BroadcastMove()
        {
            // 다른 플레이어 에게 알림
            S_Move s_Move = new S_Move();
            s_Move.ObjectId = Id;
            s_Move.PosInfo = PosInfo;
            Room.Broadcast(s_Move);
        }

        long _coolTick = 0;
        protected void UpdateSkill()
        {
            if (_coolTick == 0)
            {
                // 유효한 타겟인지 
                if (_target == null || _target.Room != Room || _target.Hp == 0)
                {
                    _target = null;
                    State = CreatureState.Moving;
                    BroadcastMove();
                    return;
                }

                // 스킬 사용 가능한지
                var dir = (_target.CellPos - CellPos);
                int dist = dir.cellDistFromZero;
                bool canUseSkill = (dist <= _skillRange && (dir.x == 0 || dir.y == 0));
                if (!canUseSkill)
                {
                    State = CreatureState.Moving;
                    BroadcastMove();
                    return;
                }

                // 타게팅 방향 주시
                MoveDir lookDir = GetDirFromVec(dir);
                if (Dir != lookDir)
                {
                    Dir = lookDir;
                    BroadcastMove();
                }

                // 데미지 판정
                Skill skillData = null;
                DataManager.SkillDict.TryGetValue(1, out skillData);
                _target.OnDamaged(this, skillData.damage + Stat.Attack);

                // 스킬 사용 broadcast
                S_Skill skillPk = new S_Skill() { Info = new SkillInfo() };
                skillPk.ObjectId = Id;
                skillPk.Info.SkillId = skillData.id;
                Room.Broadcast(skillPk);

                // 스킬 쿨타임 적용
                int coolTick = (int)(skillData.coolDown * 1000);
                _coolTick = Environment.TickCount64 + coolTick;
            }

            if (_coolTick > Environment.TickCount64)
                return;

            _coolTick = 0;
        }

        protected void UpdateDead()
        {

        }

    }

}
