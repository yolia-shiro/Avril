using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Magic Skill Manager", menuName = "Magic Skill Manager/New Magic Skill Manager")]
public class MagicSkillManager : ScriptableObject
{
    public List<List<BasicMagicSkill>> allMagicSkills = new List<List<BasicMagicSkill>>();
    public List<BasicMagicSkill> nihilityMagicSkills = new List<BasicMagicSkill>();
    public List<BasicMagicSkill> fireMagicSkills = new List<BasicMagicSkill>();
    public List<BasicMagicSkill> waterMagicSkills = new List<BasicMagicSkill>();
    public List<BasicMagicSkill> woodMagicSkills = new List<BasicMagicSkill>();

    private void OnEnable()
    {
        allMagicSkills.Add(nihilityMagicSkills);
        allMagicSkills.Add(fireMagicSkills);
        allMagicSkills.Add(waterMagicSkills);
        allMagicSkills.Add(woodMagicSkills);
    }
}
