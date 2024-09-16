using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using ImGuiNET;
using IVPlugin.ActorData;
using IVPlugin.Camera;
using IVPlugin.Core;
using IVPlugin.Core.Extentions;
using IVPlugin.Mods;
using IVPlugin.Services;
using IVPlugin.UI;
using IVPlugin.UI.Helpers;
using IVPlugin.UI.Windows;
using Lumina.Excel.GeneratedSheets;
using Penumbra.Api.IpcSubscribers;

namespace IVPlugin.Windows;

public static class ConfigWindow
{
    public static bool IsOpen = false;
    public static void Show() => IsOpen = true;

    public static void Hide() => IsOpen = false;
    public static void Toggle() => IsOpen = !IsOpen;

    private static bool NPCHack = true;
    private static bool SkeleColors = true;
    private static bool installIVCS = true;
    private static bool aSceneLocalSpace = true;
    private static bool aSceneWarningShow = true;
    private static bool FadeInOnAnimation = true;

    private static bool IVCSRequiresUpdate = false;
    public static void Draw()
    {
        if (!IsOpen) return;

        var size = new Vector2(-1, -1);
        ImGui.SetNextWindowSize(size, ImGuiCond.FirstUseEver);

        ImGui.SetNextWindowSizeConstraints(new Vector2(395, 0), new Vector2(500, 1080));

        NPCHack = IllusioVitae.configuration.UseNPCHack;
        SkeleColors = IllusioVitae.configuration.UseSkeletonColors;
        installIVCS = IllusioVitae.configuration.installIVCS;
        aSceneLocalSpace = IllusioVitae.configuration.ActorSceneLocalSpace;
        aSceneWarningShow = IllusioVitae.configuration.ActorSceneWarningShow;
        FadeInOnAnimation = IllusioVitae.configuration.FadeInOnAnimation;

        if (ImGui.Begin($"Illusio Vitae: 配置设置", ref IsOpen, ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
        {

            if (ImGui.BeginTabBar("ConfigTabBar"))
            {
                if (ImGui.BeginTabItem("常规设置"))
                {
                    GeneralTabDraw();

                    ImGui.EndTabItem();
                }

                if(ImGui.BeginTabItem("NPC预设"))
                {
                    AppearancesDraw();

                    ImGui.EndTabItem();
                }

                ImGui.EndTabBar();
            }
        }
    }

    private static void GeneralTabDraw()
    {
        BearGUI.Text("自定义情感动作安装目录", 1.1f);

        BearGUI.Text(IllusioVitae.configuration.ModLocation, .85f);

        ImGui.Spacing();

        if (ImGui.Button("更改位置"))
        {
            WindowsManager.Instance.fileDialogManager.OpenFolderDialog("选择自定义情感动作安装目录", (confirm, path) =>
            {
                if (confirm)
                {
                    IllusioVitae.configuration.ModLocation = path;
                }
            });
        }

        ImGui.Spacing();

        ImGui.Separator();

        if (ImGui.Checkbox("“显示角色场景世界空间警告”", ref aSceneWarningShow))
        {
            IllusioVitae.configuration.ActorSceneWarningShow = aSceneWarningShow;
        }

        ImGui.Spacing();

        if (ImGui.Checkbox("启用来自Concept Matrix的NPC外观数据", ref NPCHack))
        {
            IllusioVitae.configuration.UseNPCHack = NPCHack;
        }

        ImGui.Spacing();

        if (ImGui.Checkbox("启用骨架编辑器颜色编码", ref SkeleColors))
        {
            IllusioVitae.configuration.UseSkeletonColors = SkeleColors;
        }

        ImGui.Spacing();

        using (ImRaii.Disabled(!DalamudServices.penumbraServices.CheckAvailablity()))
        {

            if (ImGui.Button("安装 IVCS"))
            {
                ModManager.Instance.InstallIVCSMod();
            }
        }

        ImGui.Spacing();

        if (ImGui.Button("显示更新日志"))
        {
            ChangeLogWindow.Show();
        }
    }

    public static int id = -1;

    private static void AppearancesDraw()
    {
        BearGUI.Text("用于情感动作的NPC外观预设", 1.1f);

        ImGui.Spacing();

        using(var listbox = ImRaii.ListBox("##Custom Preset", new(279, 100)))
        {
            if (listbox.Success)
            {
                for(var i = 0; i < IllusioVitae.configuration.PresetActors.Length; i++)
                {
                    var currentActor = IllusioVitae.configuration.PresetActors[i];

                    var selected = ImGui.Selectable($"{currentActor.Name}##{i}", id == i);

                    if(selected)
                    {
                        id = i;
                    }
                }  
            }
        }

        ImGui.SameLine();

        ImGui.BeginGroup();

        if (ImGui.Button("创建预设"))
        {
            IllusioVitae.configuration.PresetActors = IllusioVitae.configuration.PresetActors.Append(new($"Illusio Actor", null)).ToArray();
        }

        ImGui.Spacing();
        ImGui.Spacing();
        using (ImRaii.PushFont(UiBuilder.IconFont))
        {
            if (id != -1)
            {
                var selectedActor = IllusioVitae.configuration.PresetActors[id];

                using (ImRaii.Disabled(id == 0))
                {
                    if (ImGui.Button($"{FontAwesomeIcon.AngleUp.ToIconString()}##MoveUp"))
                    {
                        var prevActor = IllusioVitae.configuration.PresetActors[id - 1];

                        IllusioVitae.configuration.PresetActors[id - 1] = selectedActor;

                        IllusioVitae.configuration.PresetActors[id] = prevActor;

                        id = id - 1;
                    }
                }

                using (ImRaii.Disabled(IllusioVitae.configuration.PresetActors.Length - 1 == id))
                {
                    if (ImGui.Button($"{FontAwesomeIcon.AngleDown.ToIconString()}##MoveDown"))
                    {
                        var nextActor = IllusioVitae.configuration.PresetActors[id + 1];

                        IllusioVitae.configuration.PresetActors[id + 1] = selectedActor;

                        IllusioVitae.configuration.PresetActors[id] = nextActor;

                        id = id + 1;
                    }
                }
            }
            else
            {
                using (ImRaii.Disabled(id == -1))
                {
                    if (ImGui.Button($"{FontAwesomeIcon.AngleUp.ToIconString()}##MoveUpFake")) { }
                }

                using (ImRaii.Disabled(id == -1))
                {
                    if (ImGui.Button($"{FontAwesomeIcon.AngleDown.ToIconString()}##MoveDownFake")) { }
                }
            }
        }

        ImGui.EndGroup();

        if (id == -1) return;
        ImGui.Separator();
        ImGui.Spacing();

        string actorName = IllusioVitae.configuration.PresetActors[id].Name;

        ImGui.Text("重命名角色预设：");

        ImGui.SetNextItemWidth(279);
        if(ImGui.InputText("##nameinput", ref actorName, 50))
        {
            if (CheckValidName(actorName))
            {
                IllusioVitae.configuration.PresetActors[id].Name = actorName.Captialize();
            }
        }

        ImGui.SameLine();

        ImGui.TextColored(CheckValidName(actorName) ? IVColors.Green : IVColors.Red,
           CheckValidName(actorName) ? "有效名称" : "无效名称");

        ImGui.Spacing();

        var currentCharaLocation = IllusioVitae.configuration.PresetActors[id].charaPath;
            
        ImGui.Text("选择的文件：" + (string.IsNullOrEmpty(currentCharaLocation) ? "未设置" : Path.GetFileName(currentCharaLocation)));

        if (ImGui.Button("选择角色数据"))
        {
            WindowsManager.Instance.fileDialogManager.OpenFileDialog("导入角色数据文件", ".chara", (confirm, path) =>
            {
                if (confirm)
                {
                    IllusioVitae.configuration.PresetActors[id].charaPath = path;
                }
            });
        }

        ImGui.SameLine();

        if(ImGui.Button("删除选择的预设"))
        {
            var list = IllusioVitae.configuration.PresetActors.ToList();

            list.Remove(IllusioVitae.configuration.PresetActors[id]);

            IllusioVitae.configuration.PresetActors = list.ToArray();

            id = -1;
        }
    }

    private static bool CheckValidName(string newName)
    {
        var strings = newName.Split(" ");


        if (strings.Length > 2) return false;

        if (strings.Length < 2) return false;

        foreach (var s in strings)
        {
            if (HasSpecialChars(s)) return false;

            int capitalized = 0;

            if (s.Length < 2) return false;

            for (var i = 1; i < s.Length; i++)
            {
                if (char.IsUpper(s[i]))
                {
                    capitalized++;
                }
            }

            if (capitalized > 0) return false;
        }

        return true;
    }

    private static bool HasSpecialChars(string yourString)
    {
        return yourString.Any(ch => (!char.IsLetter(ch) && (ch != '-' && ch != '\'')));
    }
}
