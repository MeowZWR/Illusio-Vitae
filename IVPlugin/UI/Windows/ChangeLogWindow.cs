using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using IVPlugin.Services;
using IVPlugin.UI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace IVPlugin.UI.Windows
{
    public static class ChangeLogWindow
    {
        public static bool IsOpen = false;

        public static void TryShow()
        {
            if(IllusioVitae.configuration.LastSeenVersion == -1)
            {
                Show();
                IllusioVitae.configuration.LastSeenVersion = version;
                return;
            }

            if(IllusioVitae.configuration.LastSeenVersion < version)
            {
                Show();
                IllusioVitae.configuration.LastSeenVersion = version;
                return;
            }
        }
        public static void Show() => IsOpen = true;
        public static void Toggle() => IsOpen = !IsOpen;

        public const int version = 1; //change to show an update to changelog

        public static void Draw()
        {
            if (!IsOpen) return;

            var size = new Vector2(-1, -1);
            ImGui.SetNextWindowSize(size, ImGuiCond.FirstUseEver);

            ImGui.SetNextWindowSizeConstraints(new Vector2(700, 700), new Vector2(1000, 1000));

            if (ImGui.Begin($"Illusio Vitae 更新日志", ref IsOpen, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
            {
                using (ImRaii.Child("##changelogCHILD", new(0, 620)))
                {
                    Ver1();

                    ImGui.Spacing();
                    ImGui.Spacing();

                    ver0();
                }
                

                ImGui.SetCursorPos(new(ImGui.GetWindowContentRegionMax().X/2 - ImGui.CalcTextSize("关闭").X/2, ImGui.GetWindowContentRegionMax().Y - 25));
                if (ImGui.Button("关闭"))
                {
                    Toggle();
                }
            }
        }

        #region Logs

        private static void Ver1()
        {
            BearGUI.Text("[08/09/24] - Version 1.0.5:", 2, 0xFFFF00FF);
            BearGUI.Text("- 添加对创建IVMP文件的支持");
            BearGUI.Text("- 修复一些小Bug");
        }

        private static void ver0()
        {
            BearGUI.Text("[07/24/24] - 版本 1.0：", 2, 0xFFFF00FF);
            BearGUI.Text("- Illusio Vitae 插件已正式发布。");
            BearGUI.Text("- 添加了自定义情感动作标签和 Mod 管理器。");
            BearGUI.Text("  现在可以为情感动作设置独特的斜杠命令，并且不会替换现有文件。");
            BearGUI.Text("- 添加了对特定 VFX 情感动作的支持，");
            BearGUI.Text("  允许玩家角色周围始终存在特殊的持久效果。");
            BearGUI.Text("- 添加了多种体验改善功能，如从自定义情感动作中切换音乐、VFX 或相机。");
            BearGUI.Text("- 添加了CM标签、外观编辑器、骨骼编辑器和动画控制器。");
            BearGUI.Text("- 在外观编辑器中添加了完整的角色自定义、装备切换和部分着色器操作。");
            BearGUI.Text("- 添加了在集体动作中相机跟随角色移动的能力。");
            BearGUI.Text("- 添加了使用骨骼编辑器保存和加载姿势的功能。（支持 Brio 姿势导入）");
            BearGUI.Text("- 为骨骼编辑器添加了改善体验的筛选项。");
            BearGUI.Text("- 添加了角色场景，可以保存和加载多个角色、他们当前的姿势，");
            BearGUI.Text("  以及他们当前的世界位置。");
            BearGUI.Text("- 添加了自定义角色生成。这些角色可以在外观编辑器中编辑，");
            BearGUI.Text("  并且可以与上述提到的角色场景功能一起使用。");
            BearGUI.Text("- 在CM中添加了外观锁定切换。");
            BearGUI.Text("  这将尝试在加载区域之间冻结当前的自定义数据。");
            BearGUI.Text("- 在动画控制器中添加了动画切换、暂停、速度调整和滚动功能。");
            BearGUI.Text("- 在动画控制器中添加了种族动画覆盖下拉菜单。");
            BearGUI.Text("  其功能与原始CMTool中的“数据路径”相同。");
            BearGUI.Text("- 添加了查看和调整当前选择的角色每个动画槽速度的功能，");
            BearGUI.Text("  并且可以在集体动作中自由操控相机，精确调节通常无法访问的设置。");
            BearGUI.Text("- 世界控制标签、时间与天气控制器、相机控制器，");
            BearGUI.Text("  以及自定义情感动作相机控制器已添加。");
            BearGUI.Text("- 添加了调整当前时间、天气和天空盒纹理的能力。");
            BearGUI.Text("- 添加了自由加载和播放 XCP 文件的功能，");
            BearGUI.Text("  无论现有情感动作是否包含相机文件。");
            BearGUI.Text("- Illusio Vitae 自定义骨架 (IVCS) 已更新至 2.0，现完全兼容 Dawntrail。");
            BearGUI.Text("- IVCS 现在有多种物理启用的骨骼，可以在任何支持的身体 Mod 中使用。");
            BearGUI.Text("- IVCS 现在利用变形功能。使用 IVCS 不再需要额外的 Mod 或开销。");
            BearGUI.Text("  所有身体 Mod 现在可以像基础骨架一样加权到 IVCS。");
        }

        #endregion
    }
}
