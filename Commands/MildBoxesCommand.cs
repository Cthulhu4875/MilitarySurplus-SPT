using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Services.Commerce;

namespace MilitarySurplus;

[Injectable]
public class MildBoxesCommand(MailSendService mailSendService)
{
    public ValueTask<string> Handle(MongoId sessionId)
    {
        // Create the Ammo Case
        var ammoCase = new Item
        {
            Id = new MongoId(Guid.NewGuid().ToString("N")[..24]),
            Template = new MongoId("5aafbde786f774389d0cbc0f"),
            Upd = new Upd()
        };

        // All 25 mild ammo box templates
        var ammoBoxTemplates = new[]
        {
            "657024d2bfc87b3a34093235", // 4.6x30mm Subsonic SX - 40-round box
            "657025161419851aef03e718", // 5.7x28mm L191 - 50-round box
            "57372d4c245977685a3da2a1", // 5.45x39mm PP gs - 120-round box - 2x1
            "6570265f1419851aef03e739", // 5.56x45mm M995 - 100-round box
            "6570254abfc87b3a34093244", // 7.62x25mm TT PT gzh - 25-round box
            "6570260c1419851aef03e727", // 9x18mm PM P gzh - 50-round box
            "65702591c5d7d4cb4d07857c", // 9x19mm AP 6.3 - 50-round box
            "6489875745f9ca4ba51c4808", // 9x21mm BT gzh - 30-round box
            "6570240a1419851aef03e6f7", // .45 ACP Lasermatch FMJ - 50-round box
            "676009ddb623f3b8ba079419", // .50 AE Copper Solid - 20-round box
            "657023b1cfc010a0f50069e5", // .300 BLK M62 Tracer - 50-round box
            "5649ed104bdc2d3d1c8b458b", // 7.62x39mm PS gzh - 20-round box
            "657025dfcfc010a0f5006a3b", // 9x39mm SPP gs - 20-round box
            "657024011419851aef03e6f4", // .366 TKM EKO - 20-round box
            "657023e7c5d7d4cb4d078552", // .357 Magnum JHP - 25-round box
            "67600a42b32eb5d23e0eb459", // 6.8x51mm SIG FMJ - 20-round box
            "65702554bfc87b3a34093247", // 7.62x51mm M62 Tracer - 20-round box
            "65702577cfc010a0f5006a2c", // 7.62x54mm R LPS gzh - 20-round box
            "657023dabfc87b3a3409320d", // .338 Lapua UCW - 20-round box - 2x1
            "6570241bcfc010a0f50069f5", // 12.7x55mm PS12 - 10-round box
            "68e9156326fbff63b30106a0", // .50 BMG HP - 10-round box
            "67657773b83469e4f102dc27", // 20/70 Dangerous Game Slug - 25-round box
            "65702469c5d7d4cb4d07855b", // 12/70 makeshift .50 BMG slug - 25-round box
            "65702432bfc87b3a3409321c", // 12/70 6.5mm Express buckshot - 25-round box
            "657024b31419851aef03e70f"  // 23x75mm Shrapnel-25 - 5-round box - 2x1
        };

        // These three boxes occupy 2 horizontal slots.
        var twoWideBoxes = new HashSet<string>
        {
             "57372d4c245977685a3da2a1", // 5.45x39mm PP gs
             "6570265f1419851aef03e739", // 5.56x45mm M995
             "657023dabfc87b3a3409320d", // .338 Lapua UCW
             "657024b31419851aef03e70f"  // 23x75mm Shrapnel-25
        };
        // Ammo case is 7 slots wide.
        const int caseWidth = 7;

        // Track which grid cells are already occupied.
        var occupied = new HashSet<(int X, int Y)>();

        var ammoBoxItems = new List<Item>();

        foreach (var templateId in ammoBoxTemplates)
        {
            var width = twoWideBoxes.Contains(templateId) ? 2 : 1;
            var height = 1;

            var placed = false;

            // Search the case from top-left to bottom-right
            // until we find a location where the box fits.
            for (var y = 0; y < 20 && !placed; y++)
            {
                for (var x = 0; x <= caseWidth - width; x++)
                {
                    var canFit = true;

                    // Check every grid cell this box would occupy.
                    for (var checkX = 0; checkX < width; checkX++)
                    {
                        for (var checkY = 0; checkY < height; checkY++)
                        {
                            if (occupied.Contains((x + checkX, y + checkY)))
                            {
                                canFit = false;
                                break;
                            }
                        }

                        if (!canFit)
                        {
                            break;
                        }
                    }

                    if (!canFit)
                    {
                        continue;
                    }

                    // Mark the cells as occupied.
                    for (var checkX = 0; checkX < width; checkX++)
                    {
                        for (var checkY = 0; checkY < height; checkY++)
                        {
                            occupied.Add((x + checkX, y + checkY));
                        }
                    }

                    // Create the box.
                    ammoBoxItems.Add(new Item
                    {
                        Id = new MongoId(Guid.NewGuid().ToString("N")[..24]),
                        Template = new MongoId(templateId),
                        ParentId = ammoCase.Id.ToString(),
                        SlotId = "main",
                        Location = new Dictionary<string, object>
                        {
                            ["x"] = x,
                            ["y"] = y,
                            ["r"] = 0
                        },
                        Upd = new Upd
                        {
                            StackObjectsCount = 99999
                        }
                    });

                    placed = true;
                    break;
                }
            }

            if (!placed)
            {
                Console.WriteLine(
                    $"[MilitarySurplus] WARNING: Could not find space for mild ammo box {templateId}."
                );
            }
        }

        var items = new List<Item>
        {
            ammoCase
        };

        items.AddRange(ammoBoxItems);

        mailSendService.SendSystemMessageToPlayer(
            sessionId,
            "Your mild boxes have arrived.",
            items,
            null,
            null
        );

        return ValueTask.FromResult("That's a lot of Mild Boxes!!");
    }
}
