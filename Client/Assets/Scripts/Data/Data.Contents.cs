using Protocol;
using System;
using System.Collections.Generic;

namespace Data
{
    #region Skill
    [Serializable]
    public class Skill
    {
        public int id;
        public string name;
        public float coolDown;
        public int damage;
        public SkillType skillType;
        public ProjectileInfo projectile;
    }

    public class ProjectileInfo
    {
        public string name;
        public float speed;
        public int range;
        public string prefab;
    }

    [Serializable]
    public class SkillData : ILoader<int, Skill>
    {
        public List<Skill> skills = new List<Skill>();

        public Dictionary<int, Skill> MakeDict()
        {
            var dict = new Dictionary<int, Skill>();
            foreach (var skill in skills)
                dict.Add(skill.id, skill);
            return dict;
        }
    }
    #endregion
}