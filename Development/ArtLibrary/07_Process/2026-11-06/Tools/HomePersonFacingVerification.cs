using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class HomePersonFacingVerification
{
    public static string Run()
    {
        var host=UnityEngine.Object.FindObjectOfType<ProductHomeWorldHost>(true);
        if(host==null || !host.WorldVisible)throw new Exception("Home required");
        if(VillageRuntimeBridge.Time.Speed!=VillageTime.VillageTimeSpeed.Paused)host.Shell.TogglePause();
        host.SetVisualPrototype(true);host.RefreshNow();
        var resolver=new HomePresentationResolver{PrototypeEnabled=true};
        if(!resolver.TryResolve("village.person.r2",out var front,out _) || !resolver.TryResolve("village.person.r2.ne",out var back,out _))throw new Exception("Directional assets missing");
        var view=host.PersonViews.Values.First(v=>v.PresentationVisible);
        var body=view.transform.Find("Body").GetComponent<SpriteRenderer>();
        var ring=view.transform.Find("Body/Selection").GetComponent<SpriteRenderer>();
        const BindingFlags flags=BindingFlags.NonPublic|BindingFlags.Instance;
        var type=typeof(HomeWorldEntityView);
        var target=type.GetField("_target",flags);var backFlag=type.GetField("_facingBack",flags);var flipFlag=type.GetField("_facingFlip",flags);
        var oldTarget=target.GetValue(view);var oldBack=backFlag.GetValue(view);var oldFlip=flipFlag.GetValue(view);var oldPosition=view.transform.position;
        string before=JsonUtility.ToJson(VillageRuntimeBridge.BuildSaveData());
        try
        {
            var directions=new[]{new Vector3(1,1),new Vector3(-1,1),new Vector3(-1,-1),new Vector3(1,-1)};
            for(int i=0;i<directions.Length;i++)
            {
                view.transform.position=oldPosition;target.SetValue(view,oldPosition+directions[i]);
                view.StepPresentation(.01f);
                // The periodic product refresh must retain the chosen view and mirror.
                view.RefreshVisual("village.person.r2",resolver);
                var expected=i<2 ? back : front;bool flip=i==1||i==3;
                if(body.sprite!=expected || body.flipX!=flip || ring.sprite!=expected || ring.flipX!=flip)throw new Exception("Direction mismatch "+i);
            }
            if(before!=JsonUtility.ToJson(VillageRuntimeBridge.BuildSaveData()))throw new Exception("Facing modified domain facts");
            return "PASS: 4 movement quadrants select front/back and mirror; selection sprite agrees; periodic refresh retains orientation; domain save data unchanged. Component probe on real Home view, not a walking-animation acceptance.";
        }
        finally
        {
            target.SetValue(view,oldTarget);backFlag.SetValue(view,oldBack);flipFlag.SetValue(view,oldFlip);view.transform.position=oldPosition;
            view.RefreshVisual("village.person.r2",resolver);resolver.Dispose();
        }
    }
}
