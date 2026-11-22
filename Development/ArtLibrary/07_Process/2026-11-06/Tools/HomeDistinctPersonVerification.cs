using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
public static class HomeDistinctPersonVerification
{
 const BindingFlags F=BindingFlags.NonPublic|BindingFlags.Instance;
 public static string Run()
 {
  var h=UnityEngine.Object.FindObjectOfType<ProductHomeWorldHost>(true);
  if(h==null||!h.WorldVisible||!h.VisualPrototypeEnabled)throw new Exception("Real product B Home required");
  if(VillageRuntimeBridge.Time.Speed!=VillageTime.VillageTimeSpeed.Paused)throw new Exception("Pause product clock first");
  var r=new HomePresentationResolver{PrototypeEnabled=true};
  var type=typeof(HomeWorldEntityView);var target=type.GetField("_target",F);var back=type.GetField("_facingBack",F);var flip=type.GetField("_facingFlip",F);
  string before=JsonUtility.ToJson(VillageRuntimeBridge.BuildSaveData());
  int directions=0,selection=0;var occluded=new System.Collections.Generic.List<string>();
  try{
   foreach(var p in h.Snapshot.Persons)
   {
    var v=h.PersonViews[p.PersonId];
    string appearance=PersonVisualResolver.ResolveAppearanceKey(VillageRuntimeBridge.SimulationPersonSource,p.PersonId);
    string key="village.person.art01."+appearance+".se";
    if(!r.TryResolve(key,out var front,out _)||!r.TryResolve("village.person.art01."+appearance+".ne",out var rear,out _))throw new Exception("Missing pair: "+appearance);
    var body=v.transform.Find("Body").GetComponent<SpriteRenderer>();var ring=v.transform.Find("Body/Selection").GetComponent<SpriteRenderer>();
    if(body.sprite!=front&&body.sprite!=rear)throw new Exception("Product identity mismatch: "+p.DisplayName);
    var oldTarget=target.GetValue(v);var oldBack=back.GetValue(v);var oldFlip=flip.GetValue(v);var position=v.transform.position;
    try{
     foreach(var d in new[]{new Vector3(1,1),new Vector3(-1,1),new Vector3(-1,-1),new Vector3(1,-1)}){
      v.transform.position=position;target.SetValue(v,position+d);v.StepPresentation(.01f);
      h.RefreshNow();
      var expected=d.y>0?rear:front;
      if(body.sprite!=expected||body.flipX!=(d.x<0)||ring.sprite!=expected||ring.flipX!=(d.x<0))throw new Exception("Directional sprite/mirror mismatch: "+appearance+" "+d);
      directions++;
     }
    }finally{target.SetValue(v,oldTarget);back.SetValue(v,oldBack);flip.SetValue(v,oldFlip);v.transform.position=position;v.RefreshVisual(key,r);}
    Physics2D.SyncTransforms();
    var collider=v.GetComponent<BoxCollider2D>();
    bool picked=false;
    foreach(var offset in new[]{Vector3.zero,new Vector3(.12f,0),new Vector3(-.12f,0),new Vector3(0,.12f),new Vector3(0,-.12f)}){
     var point=collider.bounds.center+offset;
     var screen=h.WorldCamera.ViewCamera.WorldToScreenPoint(point);
     if(h.PointerOverUI(screen))continue;
     h.HandleWorldClick(screen);
     if(h.Selection.Kind==HomeSelectionKind.Person&&h.Selection.Id==p.PersonId){picked=true;break;}
    }
    if(!picked){var hits=Physics2D.OverlapPointAll(collider.bounds.center,1<<ProductHomeWorldHost.WorldLayer);if(hits.Any(c=>{var other=c.GetComponentInParent<HomeWorldEntityView>();return other!=null&&other.Kind==HomeSelectionKind.Person&&other.Identity!=p.PersonId;}))occluded.Add(p.DisplayName);else throw new Exception("Unobstructed person picking failed: "+p.DisplayName);}else selection++;
   }
   if(occluded.Count>0)throw new Exception("Overlapping persons remain: "+string.Join(",",occluded));
   h.Selection.Clear();h.RefreshNow();
   h.SetVisualPrototype(false);h.RefreshNow();
   foreach(var p in h.Snapshot.Persons){var v=h.PersonViews[p.PersonId];var expectedKey=r.PresentationKey(p);var a=new HomePresentationResolver();try{if(!a.TryResolve("village.person."+p.PresentationIntent,out var original,out _)||v.transform.Find("Body").GetComponent<SpriteRenderer>().sprite!=original)throw new Exception("A profile changed");}finally{a.Dispose();}}
   h.SetVisualPrototype(true);h.RefreshNow();
   foreach(var p in h.Snapshot.Persons){var appearance=PersonVisualResolver.ResolveAppearanceKey(VillageRuntimeBridge.SimulationPersonSource,p.PersonId);r.TryResolve("village.person.art01."+appearance+".se",out var front,out _);if(h.PersonViews[p.PersonId].transform.Find("Body").GetComponent<SpriteRenderer>().sprite!=front)throw new Exception("B restore identity mismatch");}
   if(before!=JsonUtility.ToJson(VillageRuntimeBridge.BuildSaveData()))throw new Exception("Presentation checks changed simulation save data");
   return "PASS: actual product people="+h.Snapshot.Persons.Count+"; component directional cases="+directions+"; real screen-coordinate person picks="+selection+"; overlapping persons skipped="+string.Join(",",occluded)+"; original A and restored B verified; selection ring sprites agree; periodic refresh retains facing; domain save JSON unchanged. Direction forcing is MODULE verification, not actual travel or animation acceptance.";
  }finally{h.Selection.Clear();h.SetVisualPrototype(true);h.RefreshNow();r.Dispose();}
 }
}
