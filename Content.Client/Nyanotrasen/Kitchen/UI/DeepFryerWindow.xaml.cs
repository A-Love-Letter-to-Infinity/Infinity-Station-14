using Robust.Client.AutoGenerated;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.UserInterface.CustomControls;
using Robust.Client.UserInterface.XAML;
using Content.Shared.Kitchen.UI;

namespace Content.Client.Kitchen.UI
{
    [GenerateTypedNameReferences]
    [Access(typeof(DeepFryerBoundUserInterface))]
    public sealed partial class DeepFryerWindow : DefaultWindow
    {
        [Dependency] private readonly IEntityManager _entityManager = default!;
        [Dependency] private readonly IClientEntityManager EntMan = default!;

        private static readonly Color WarningColor = Color.FromHsv(new Vector4(0.0f, 1.0f, 0.8f, 1.0f));

        public DeepFryerWindow()
        {
            RobustXamlLoader.Load(this);
            IoCManager.InjectDependencies(this);
        }

        public void UpdateState(DeepFryerBoundUserInterfaceState state)
        {
            OilLevel.Value = (float) state.OilLevel;
            OilPurity.Value = (float) state.OilPurity;

            if (state.OilPurity < state.FryingOilThreshold)
            {
                if (OilPurity.ForegroundStyleBoxOverride == null)
                {
                    OilPurity.ForegroundStyleBoxOverride = new StyleBoxFlat();

                    var oilPurityStyle = (StyleBoxFlat) OilPurity.ForegroundStyleBoxOverride;
                    oilPurityStyle.BackgroundColor = WarningColor;
                }
            }
            else
            {
                OilPurity.ForegroundStyleBoxOverride = null;
            }

            ItemList.Clear();

            foreach (var entity in state.ContainedEntities)
            {

                EntityUid serverEnt = default;

                if (EntMan.Deleted(serverEnt))
                    continue;

                // Duplicated from MicrowaveBoundUserInterface.cs: keep an eye on that file for when it changes.
                Texture? texture;
                if (EntMan.TryGetComponent<IconComponent>(serverEnt, out var iconComponent))
                {
                    texture = EntMan.System<SpriteSystem>().GetIcon(iconComponent);
                }
                else if (EntMan.TryGetComponent<SpriteComponent>(serverEnt, out var spriteComponent))
                {
                    texture = spriteComponent.Icon?.Default;
                }
                else
                {
                    continue;
                }

                 ItemList.AddItem(EntMan.GetComponent<MetaDataComponent>(serverEnt).EntityName, texture);
            }
        }
    }
}
