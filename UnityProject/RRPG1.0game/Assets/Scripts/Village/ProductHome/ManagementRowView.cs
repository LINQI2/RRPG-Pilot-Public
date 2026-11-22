using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// V17-R3-B 集中管理列表行（Authored Prefab 绑定的小型行视图）。
///
/// 只负责一行固定子控件的赋值与点击回调：标题、次要说明、按钮、选中底色。
/// 不读取任何 Authority、不保存当前人物、不生成第二份列表；行的创建/回收由 ManagementView 统一负责。
/// 固定结构必须由 Editor Prefab 绑定，运行时只做赋值。
/// </summary>
public sealed class ManagementRowView : MonoBehaviour
{
    [SerializeField] private TMP_Text label;
    [SerializeField] private TMP_Text detail;
    [SerializeField] private Button button;
    [SerializeField] private Image background;
    [SerializeField] private LayoutElement layout;

    private static readonly Color NormalColor = new Color(.14f, .24f, .24f, 1);
    private static readonly Color SelectedColor = new Color(.28f, .43f, .36f, 1);

    public Button Button => button;

    public void Bind(string title, string description, Action onClick, bool interactable, bool selected,
        float preferredHeight = 0f)
    {
        if (label != null) label.text = title ?? string.Empty;
        if (detail != null) detail.text = description ?? string.Empty;
        if (background != null) background.color = selected ? SelectedColor : NormalColor;
        if (layout != null && preferredHeight > 0f) layout.preferredHeight = preferredHeight;
        if (button == null) return;
        button.onClick.RemoveAllListeners();
        if (onClick != null) button.onClick.AddListener(() => onClick());
        button.interactable = interactable;
    }
}
