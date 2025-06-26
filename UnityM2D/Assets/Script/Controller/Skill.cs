using UnityEngine;

public abstract class Skill 
{
    public string SkillName;
    public float Cooldown;
    public float ManaCost;

    protected float _currentCooldown;

    protected bool _init = false;
    void Start()
    {
        Init();
    }

    public virtual bool Init()
    {
        if (_init)
            return false;

        return _init = true;
    }

    public virtual void OnSkillUse(BaseController caster)
    {
        // 공통 로직 (예: 마나 소모, 쿨다운 시작)
    }

    public virtual void OnSkillUpdate(float deltaTime)
    {
        if (_currentCooldown > 0)
        {
            _currentCooldown -= deltaTime;
            if (_currentCooldown < 0) _currentCooldown = 0;
        }
    }

    public abstract void ExecuteSkill(BaseController _attacker, BaseController _targeter);
}
