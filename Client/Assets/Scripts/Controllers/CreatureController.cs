using Protocol;
using UnityEngine;

public class CreatureController : BaseController
{
    HpBar _hpBar;

    protected void AddHpBar()
    {
        var go = Managers.Resource.Instantiate("UI/HpBar", transform);
        go.transform.localPosition = new Vector3(0, 0.5f, 0);
        go.name = "HpBar";
        _hpBar = go.GetComponent<HpBar>();
        UpdateHpBar();
    }

    public void UpdateHpBar()
    {
        if (_hpBar == null) return;
        float ratio = 0.0f;
        if (Stat.MaxHp > 0)
        {
            ratio = Hp / (float)Stat.MaxHp;
        }
        _hpBar.SetHpBar(ratio);
    }

    public override int Hp
    {
        get
        {
            return Stat.Hp;
        }
        set
        {
            base.Hp = value;
            UpdateHpBar();
        }
    }

    public override StatInfo Stat
    {
        get { return base.Stat; }
        set
        {
            base.Stat = value;
            UpdateHpBar();
        }
    }

    protected override void Init()
    {
        base.Init();
        AddHpBar();
    }

    public virtual void OnDamaged()
    {

    }

    public virtual void OnDead()
    {
        State = CreatureState.Dead;

        GameObject effect = Managers.Resource.Instantiate("Effect/DieEffect");
        effect.transform.position = transform.position;
        effect.GetComponent<Animator>().Play("START");
        GameObject.Destroy(effect, 0.5f);
    }
}
