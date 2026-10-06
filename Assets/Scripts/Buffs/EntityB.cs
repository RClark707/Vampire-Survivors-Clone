using System.Collections.Generic;
using UnityEngine;

public abstract class EntityB : MonoBehaviour
{
    protected float health;

    [System.Serializable]
    public class Buff
    {
        public BuffData buffData;
        public float remainingDuration, nextTick;
        public int variant;

        public Buff(BuffData d, int variant = 0, float durationMutiplier = 1f)
        {
            buffData = d;
            BuffData.Stats buffStats = d.Get(variant);
            remainingDuration = buffStats.duration * durationMutiplier;
            nextTick = buffStats.tickInterval;
            this.variant = variant;
        }

        public BuffData.Stats GetData()
        {
            return buffData.Get(variant);
        }
    }

    protected List<Buff> activeBuffs = new List<Buff>();

    [System.Serializable]
    public class BuffInfo
    {
        public BuffData data;
        public int variant;
        [Range(0f, 1f)] public float probability = 1f;
    }

    public virtual Buff GetBuff(BuffData data, int variant = -1)
    {
        foreach (Buff b in activeBuffs)
        {
            if (b.buffData = data)
            {
                if (variant >= 0)
                {
                    if (b.variant == variant) return b;
                }
                else
                {
                    return b;
                }
            }
        }
        return null;
    }

    public virtual bool ApplyBuff(BuffInfo info, float durationMultiplier = 1f)
    {
        if (Random.value <= info.probability)
        {
            return ApplyBuff(info.data, info.variant, durationMultiplier);
        }
        return false;
    }

    public virtual bool ApplyBuff(BuffData data, int variant = 0, float durationMultiplier = 1f)
    {
        Buff b;
        BuffData.Stats s = data.Get(variant);

        switch (s.stackBehavior)
        {
            case BuffData.BuffStackBehavior.StacksFully:
                activeBuffs.Add(new Buff(data, variant, durationMultiplier));
                RecalculateStats();
                return true;
            case BuffData.BuffStackBehavior.RefreshDurationOnly:
                b = GetBuff(data, variant);
                if (b != null)
                {
                    b.remainingDuration = s.duration * durationMultiplier;
                }
                else
                {
                    activeBuffs.Add(new Buff(data, variant, durationMultiplier));
                    RecalculateStats();
                }
                return true;
            case BuffData.BuffStackBehavior.DoesNotStack:
                b = GetBuff(data, variant);
                if (b != null)
                {
                    activeBuffs.Add(new Buff(data, variant, durationMultiplier));
                    RecalculateStats();
                    return true;
                }
                return false;
        }
        return false;
    }

    public virtual bool RemoveBuff(BuffData data, int variant = -1)
    {
        return false;
    }

    void RecalculateStats()
    {
        return;
    }
}
