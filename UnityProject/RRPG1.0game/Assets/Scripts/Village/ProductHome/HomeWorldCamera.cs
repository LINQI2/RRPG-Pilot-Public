using UnityEngine;

/// <summary>Presentation camera only; no clock or save dependency.</summary>
public sealed class HomeWorldCamera : MonoBehaviour
{
    public Camera ViewCamera { get; private set; }
    private Vector3 _lastMouse;
    public bool PrototypeEnabled { get; private set; }
    public void SetPrototype(bool enabled)
    {
        PrototypeEnabled=enabled;
        ViewCamera.backgroundColor=enabled ? new Color(.24f,.34f,.29f) : new Color(.075f,.13f,.13f);
        Zoom(0);
    }
    public void Initialize()
    {
        ViewCamera=gameObject.AddComponent<Camera>();
        ViewCamera.orthographic=true; ViewCamera.orthographicSize=7.5f;
        ViewCamera.cullingMask=1<<ProductHomeWorldHost.WorldLayer; ViewCamera.depth=0;
        ViewCamera.clearFlags=CameraClearFlags.SolidColor; ViewCamera.backgroundColor=new Color(.075f,.13f,.13f);
        ViewCamera.nearClipPlane=.1f; ViewCamera.farClipPlane=100;
        transform.position=ProductHomeWorldHost.WorldOrigin+new Vector3(0,0,-10);
    }
    public void Focus(Vector3 point) => transform.position=new Vector3(point.x,point.y,transform.position.z);
    public void Pan(Vector2 delta)
    {
        var point=transform.position+new Vector3(delta.x,delta.y,0);
        if(PrototypeEnabled)
        {
            point.x=Mathf.Clamp(point.x,ProductHomeWorldHost.WorldOrigin.x-18,ProductHomeWorldHost.WorldOrigin.x+18);
            point.y=Mathf.Clamp(point.y,-12,12);
        }
        transform.position=point;
    }
    public void Zoom(float delta) => ViewCamera.orthographicSize=Mathf.Clamp(ViewCamera.orthographicSize+delta,PrototypeEnabled ? 4f : 2.5f,PrototypeEnabled ? 11f : 18f);
    /// <summary>相机消费的只读输入快照；生产默认由 Unity legacy Input 逐字段采样。</summary>
    private readonly struct InputSnapshot
    {
        private readonly Vector3 _mouse;
        private readonly bool _rightHeld,_rightDown,_middleHeld,_middleDown;
        private readonly float _scrollY;
        internal InputSnapshot(Vector3 mouse,bool rightHeld,bool rightDown,bool middleHeld,bool middleDown,float scrollY)
        {
            _mouse=mouse;_rightHeld=rightHeld;_rightDown=rightDown;_middleHeld=middleHeld;_middleDown=middleDown;_scrollY=scrollY;
        }
        internal Vector3 Mouse => _mouse;
        internal bool RightHeld => _rightHeld;
        internal bool RightDown => _rightDown;
        internal bool MiddleHeld => _middleHeld;
        internal bool MiddleDown => _middleDown;
        internal float ScrollY => _scrollY;
    }
#if UNITY_EDITOR
    // 仅 Editor 编译存在的实例级可逆测试采样覆盖（Player 编译中本字段、方法、分支全部不存在）。
    // 不写盘、不改任何 Authority；null = 恢复生产 legacy Input 采样。
    private InputSnapshot? _inputSampleForTests;
#endif
    public void ReadInput(bool overUI,bool suppressRightPan=false)
    {
        InputSnapshot input=SampleInput();
        if (!overUI)
        {
            if ((!suppressRightPan && input.RightHeld) || input.MiddleHeld)
            {
                if (!input.RightDown && !input.MiddleDown)
                {
                    float scale=ViewCamera.orthographicSize*2/Mathf.Max(1,Screen.height);
                    Pan((Vector2)(_lastMouse-input.Mouse)*scale);
                }
            }
            Zoom(-input.ScrollY*.55f);
        }
        _lastMouse=input.Mouse;
    }
    private InputSnapshot SampleInput()
    {
#if UNITY_EDITOR
        // 与生产默认逐句等价：无测试采样时仍只读 Input.mousePosition / GetMouseButton(1/2) /
        // GetMouseButtonDown(1/2) / mouseScrollDelta，不新增第二套输入系统。
        if (_inputSampleForTests.HasValue) return _inputSampleForTests.Value;
#endif
        return new InputSnapshot(Input.mousePosition,Input.GetMouseButton(1),Input.GetMouseButtonDown(1),Input.GetMouseButton(2),Input.GetMouseButtonDown(2),Input.mouseScrollDelta.y);
    }
#if UNITY_EDITOR
    /// <summary>
    /// 【Editor-only internal 实例级测试缝】用合成 legacy Input 采样覆盖本相机下一次及后续
    /// <see cref="ReadInput"/> 的输入快照；不绕过 overUI / suppressRightPan 门禁，不新增公共 API。
    /// 只替换 Unity legacy Input 的采样数据，产品 Host.Update 与相机判定代码保持原路径。
    /// 可逆：<see cref="ClearInputSampleForTests"/> 恢复生产 Input；测试必须在 finally 清理。
    /// </summary>
    internal void SetInputSampleForTests(Vector3 mousePosition,bool rightHeld,bool rightDown,bool middleHeld,bool middleDown,float scrollY)
    {
        _inputSampleForTests=new InputSnapshot(mousePosition,rightHeld,rightDown,middleHeld,middleDown,scrollY);
    }
    /// <summary>【Editor-only internal】清除测试采样，恢复生产 legacy Input 采样（可逆、幂等）。</summary>
    internal void ClearInputSampleForTests()
    {
        _inputSampleForTests=null;
    }
#endif
}
