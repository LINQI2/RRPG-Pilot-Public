using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class CrossSceneArtProbe
{
    const BindingFlags F=BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic;
    static object Read(object target,string name)
    {
        var field=target.GetType().GetField(name,F);
        return field!=null ? field.GetValue(target) : target.GetType().GetProperty(name,F).GetValue(target);
    }
    public static string EnterBattle()
    {
        var owner=UnityEngine.Object.FindObjectOfType<ExpeditionUI>(true);
        var adventure=Read(owner,"_currentAdventure");
        if(adventure==null)return "No active product adventure; stopped";
        var progression=Read(adventure,"Progression");
        var nodes=((IEnumerable)progression.GetType().GetMethod("GetAvailableNextNodes",F).Invoke(progression,null)).Cast<object>().ToArray();
        var node=nodes.FirstOrDefault(n=>new[]{"Battle","Elite","Boss"}.Contains(Read(n,"NodeKind").ToString()));
        if(node==null)return "No available combat node; stopped. Available="+string.Join(",",nodes.Select(n=>Read(n,"NodeKind").ToString()));
        var battle=UnityEngine.Object.FindObjectOfType<BattleUI>(true);
        var result=owner.GetType().GetMethod("HandleNodeClick",F).Invoke(owner,new[]{adventure,Read(node,"NodeId"),battle});
        return "Product HandleNodeClick="+result+" Battle initialized="+Read(battle,"_initializedFromExpedition")+" NodeKind="+Read(node,"NodeKind");
    }
    public static string SaveCheck()
    {
        string hash;using(var sha=System.Security.Cryptography.SHA256.Create())hash=BitConverter.ToString(sha.ComputeHash(System.IO.File.ReadAllBytes(SaveSystem.GetSavePath()))).Replace("-","");
        return "SaveUnchanged="+(hash==UnityEditor.SessionState.GetString("RRPG.Art.SaveSHA","missing"))+" SHA="+hash;
    }
}
