using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

///<summary>
/// 管理游戏中所有的Timeline。
///</summary>
public class TimelineManager : SingletonMono<TimelineManager>
{
    private List<TimelineObj> _timelines = new List<TimelineObj>();

    private void FixedUpdate()
    {
        if (this._timelines.Count <= 0) return;

        //依次处理所有Timeline。
        var idx = 0;
        while (idx < this._timelines.Count)
        {
            var wasTimeElapsed = _timelines[idx].TimeElapsed;
            _timelines[idx].TimeElapsed += Time.fixedDeltaTime * _timelines[idx].timeScale;
            
            //执行时间点内的事情。
            for (var i = 0; i < _timelines[idx].Model.Nodes.Length; i++)
            {
                if (_timelines[idx].Model.Nodes[i].TriggerTime < _timelines[idx].TimeElapsed 
                    && _timelines[idx].Model.Nodes[i].TriggerTime >= wasTimeElapsed)
                {
                    _timelines[idx].Model.Nodes[i].DoEvent(_timelines[idx], 
                        _timelines[idx].Model.Nodes[i].eveParams);
                }
            }

            //判断timeline是否终结。
            if (_timelines[idx].Model.Duration <= _timelines[idx].TimeElapsed)
            {
                _timelines.RemoveAt(idx);
            }
            else
            {
                idx++;
            }
        }
    }

    /// <summary>
    /// 实际创建、添加一个Timeline。
    /// </summary>
    /// <param name="timelineModel">需要创建出的TimelineModel</param>
    /// <param name="caster">创建角色、关联者。</param>
    /// <param name="source">创造源。（如为技能创造则是对应的SkillObj）</param>
    public void AddTimeline(TimelineModel timelineModel, GameObject caster, object source)
    {
        if (CasterHasTimeline(caster)) return;
        _timelines.Add(new TimelineObj(timelineModel, caster, source));
    }

    ///<summary>
    /// 实际创建、添加一个Timeline。
    ///<param name="timelineModel">要添加的TimelineModel</param>
    ///</summary>
    public void AddTimeline(TimelineObj timeline)
    {
        if (timeline.Caster && CasterHasTimeline(timeline.Caster)) return;
        this._timelines.Add(timeline);
    }

    /// <summary>
    /// 检查一个角色Caster是否有对应在释放处理中的Timeline。
    /// </summary>
    /// <param name="caster"></param>
    /// <returns></returns>
    public bool CasterHasTimeline(GameObject caster)
    {
        return _timelines.Any(t => t.Caster == caster);
    }
}