using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Services.Commerce;

namespace MilitarySurplus;

[Injectable]
public class SpecialCommand(MailSendService mailSendService)
{
    private static readonly string[] SpecialItems =
    [
        "577e1c9d2459773cd707c525", // Printer paper
        "5d63d33b86f7746ea9275524", // Flat screwdriver
        "573719762459775a626ccbc1", // 9x18mm PM P gzh
        "57347d9c245977448b40fa85", // Can of herring
        "5672cb304bdc2dc2088b456a", // D battery
        "5d4041f086f7743cac3f22a7", // Ortodontox toothpaste
        "5913651986f774432f15d132", // VAZ car key
        "573475fb24597737fb1379e1", // Apollo Soyuz cigarettes
        "5d40412b86f7743cb332ac3a", // Schaman shampoo
        "656df4fec921ad01000481a2", // Pack of instant noodles
        "60098b1705871270cd5352a1", // Emergency Water Ration
        "5672cb124bdc2d1a0f8b4568", // AA Battery
        "59e3556c86f7741776641ac2", // Ox bleach
        "5c13cd2486f774072c757944", // Soap
        "57347b8b24597737dd42e192"  // Classic matches
    ];

    private static readonly Random Random = new();

    public ValueTask<string> Handle(MongoId sessionId)
    {
        // Randomly choose 3, 4, or 5 total items.
        var itemCount = Random.Next(3, 6);

        var items = new List<Item>();

        // Each pick is independent, so duplicates are allowed.
        for (var i = 0; i < itemCount; i++)
        {
            var templateId = SpecialItems[Random.Next(SpecialItems.Length)];

            items.Add(new Item
            {
                Id = new MongoId(Guid.NewGuid().ToString("N")[..24]),
                Template = new MongoId(templateId),
                SlotId = "hideout",
                Upd = new Upd
                {
                    StackObjectsCount = 1
                }
            });
        }

        mailSendService.SendSystemMessageToPlayer(
            sessionId,
            "Sorry something went wrong...",
            items,
            null,
            null
        );

        return ValueTask.FromResult("Special delivery!!");
    }
}