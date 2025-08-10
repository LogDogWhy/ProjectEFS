
using Content.Shared.Verbs;
using Robust.Shared.Utility;
using Content.Shared.Examine;
using Content.Shared.Inventory;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.Prototypes;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Item;
using Content.Shared.Tag;


namespace Content.Shared.Weapons.Ranged.Systems;
public partial class SharedAttachmentSystem : EntitySystem
{
    [Dependency] private readonly ExamineSystemShared _examine = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly TagSystem _tagSystem = default!;

    public override void Initialize()
    {

            base.Initialize();

            // Добавление логирования для отладки
            Logger.InfoS("system-init", "Initializing SharedAttachmentSystem...");

            try
            {
                SubscribeLocalEvent<AttachmentComponent, GetVerbsEvent<ExamineVerb>>(OnAttachmentVerbExamine);


                Logger.InfoS("system-init", "Successfully subscribed to AttachmentComponent events.");
            }
            catch (Exception ex)
            {
                Logger.ErrorS("system-init", $"Error during initialization of SharedAttachmentSystem: {ex.Message}");
            }

    }

    private void OnAttachmentVerbExamine(EntityUid uid, AttachmentComponent component, GetVerbsEvent<ExamineVerb> args)
    {
        if (!args.CanInteract || !args.CanAccess)
            return;

        var examine = GetAttachmentExamine(component);
        var examine2 = GetAttachmentModExamine(component);

        var ev = new AttachmentExamineEvent(examine);
        RaiseLocalEvent(uid, ref ev);

         _examine.AddDetailedExamineVerb(args, component, examine,
            Loc.GetString("gg-attachment-verb-text"), "/Textures/Interface/VerbIcons/plus.svg.192dpi.png",
            Loc.GetString("gg-attachment-verb-message"));

         _examine.AddDetailedExamineVerb(args, component, examine2,
            Loc.GetString("gg-attachment-mod-verb-text"), "/Textures/Interface/VerbIcons/information.svg.192dpi.png",
            Loc.GetString("gg-attachment-mod-verb-message"));


    }


    public Dictionary<string, float> GetAllStats(AttachmentComponent component)
    {

        Dictionary<string, float> modify = new();
         if (component.MinAngle != 0)
        {
            var shit = (float)(component.MinAngle * (180.0 / Math.PI));
            modify.Add("minangle", shit);
        }

        if (component.MaxAngle != 0)
        {
            var shit = (float)(component.MaxAngle * (180.0 / Math.PI));
            modify.Add("maxangle", shit);
        }

        if (component.AngleIncrease != 0)
        {
            var shit = (float)(component.AngleIncrease * (180.0 / Math.PI));
            modify.Add("angleincrease", shit);
        }

        if (component.AngleDecay != 0)
        {
            var shit = (float)(component.AngleDecay * (180.0 / Math.PI));
            modify.Add("angledecay", shit);
        }

        if (component.FireRate != 0f)
        {
            modify.Add("firerate", component.FireRate);
        }

        if (component.CameraRecoilScalar != 0f)
        {
            modify.Add("camerarecoilscalar", component.CameraRecoilScalar);
        }

        return modify;
    }

    public void TryModifyStats(GunComponent gun, Dictionary<string, float> stats)
    {

            foreach (var item in stats)
            {
                switch(item.Key)
                {
                    case "minangle":
                    {
                        gun.MinAngle += Angle.FromDegrees(item.Value);
                        break;
                    }
                    case "maxangle":
                    {
                        gun.MaxAngle += Angle.FromDegrees(item.Value);
                        break;
                    }
                    case "firerate":
                    {
                        gun.FireRate += item.Value;
                        break;
                    }
                    case "angledecay":
                    {
                        gun.AngleDecay += Angle.FromDegrees(item.Value);
                        break;
                    }
                    case "angleincrease":
                    {
                        gun.AngleIncrease += Angle.FromDegrees(item.Value);
                        break;
                    }
                    case "camerarecoilscalar":
                    {
                        gun.CameraRecoilScalar += item.Value;
                        break;
                    }
                }
            }
    }

    public void TryUnmodifyStats(GunComponent gun, Dictionary<string, float> stats)
    {
            foreach (var item in stats)
            {
                switch(item.Key)
                {
                    case "minangle":
                    {
                        gun.MinAngle -= Angle.FromDegrees(item.Value);
                        break;
                    }
                    case "maxangle":
                    {
                        gun.MaxAngle -= Angle.FromDegrees(item.Value);
                        break;
                    }
                    case "firerate":
                    {
                        gun.FireRate -= item.Value;
                        break;
                    }
                    case "angledecay":
                    {
                        gun.AngleDecay -= Angle.FromDegrees(item.Value);
                        break;
                    }
                    case "angleincrease":
                    {
                        gun.AngleIncrease -= Angle.FromDegrees(item.Value);
                        break;
                    }
                    case "camerarecoilscalar":
                    {
                        gun.CameraRecoilScalar -= item.Value;
                        break;
                    }
                }
            }

    }

    private FormattedMessage GetAttachmentExamine(AttachmentComponent component)
    {
        var msg = new FormattedMessage();
        var modify = GetAllStats(component);


        msg.AddMarkup(Loc.GetString("gg-attachment-examine"));

        foreach (var number in modify)
        {
            msg.PushNewline();
            var plus = number.Value < 0f ? "" : "+";
            var modType = Loc.GetString("gg-attachment-type-" + number.Key.ToLower());
            msg.AddMarkup(Loc.GetString("gg-attachment-value",
            ("type", modType),
            ("plus", plus),
            ("value", MathF.Round(number.Value))));

        }


        if (component.NewSlot != null && component.Tags != null)
        {
            msg.PushNewline();
            var slotType = Loc.GetString("gg-attachment-type-" + component.NewSlot.ToLower());

            var shit = Loc.GetString("gg-attachment-tag-" + component.Tags[0].Id.ToLower());
            msg.AddMarkup(Loc.GetString("gg-attachment-slot",
            ("type", slotType),
            ("tag", shit)));

        }

        if (component.Sil == true)
        {
            msg.PushNewline();
            msg.AddMarkup(Loc.GetString("gg-attachment-silencer"));

        }


        return msg;
    }
    private FormattedMessage GetAttachmentModExamine(AttachmentComponent component)
    {
        var msg = new FormattedMessage();

        // Получаем теги предмета с компонентом Attachment
        if (!TryComp<TagComponent>(component.Owner, out var tagComponent) || tagComponent.Tags.Count == 0)
        {
            msg.AddText(Loc.GetString("gg-attachment-no-tags"));
            return msg;
        }

        // Создаем список для хранения найденных прототипов.
        List<string> matchingPrototypes = new();

        // Проходим по каждому прототипу сущности и проверяем слоты с нужными тегами.
        foreach (var prototype in _prototypeManager.EnumeratePrototypes<EntityPrototype>())
        {
            // Проверяем, содержит ли прототип слоты с необходимыми тегами.
            if (prototype.TryGetComponent<ItemSlotsComponent>(out var itemSlotsComponent))
            {
                foreach (var slot in itemSlotsComponent.Slots.Values)
                {
                    if (slot.Whitelist?.Tags == null || slot.Whitelist.Tags.Contains("EFSDebugTag"))
                        continue;

                    // Проверяем, совпадают ли теги слота с тегами предмета, исключая тег EFSDebugTag.
                    foreach (var tag in tagComponent.Tags)
                    {
                        if (slot.Whitelist.Tags.Contains(tag))
                        {
                            matchingPrototypes.Add(prototype.Name);
                            break;
                        }
                    }
                }
            }

            // Проверяем, содержит ли прототип компонент AttachmentComponent с нужными тегами.
            if (prototype.TryGetComponent<AttachmentComponent>(out var attachmentComponent))
            {
                if (attachmentComponent.Tags != null)
                {
                    foreach (var tag in tagComponent.Tags)
                    {
                        if (attachmentComponent.Tags.Contains(tag))
                        {
                            matchingPrototypes.Add(prototype.Name);
                            break;
                        }
                    }
                }
            }
        }

        // Добавляем найденные прототипы в сообщение.
        if (matchingPrototypes.Count > 0)
        {
            msg.AddMarkup(Loc.GetString("gg-attachment-matching-prototypes"));
            foreach (var proto in matchingPrototypes)
            {
                msg.PushNewline();
                msg.AddMarkup(Loc.GetString("gg-attachment-matching-found", ("name", proto)));
            }
        }
        else
        {
            msg.PushNewline();
            msg.AddText(Loc.GetString("gg-attachment-no-matching-prototypes"));
        }

        return msg;
    }


}
