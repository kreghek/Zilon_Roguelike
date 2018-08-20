using System.Linq;

using FluentAssertions;

using TechTalk.SpecFlow;

using Zilon.Core.Spec.Contexts;

namespace Zilon.Core.Spec.Steps
{
    [Binding]
    public class CommonSteps: GenericStepsBase<CommonGameActionsContext>
    {
        public CommonSteps(CommonGameActionsContext context) : base(context)
        {
        }

        [Given(@"Есть карта размером (.*)")]
        public void GivenЕстьКартаРазмером(int mapSize)
        {
            _context.CreateSector(mapSize);
        }

        [Given(@"Есть актёр игрока класса (.*) в ячейке \((.*), (.*)\)")]
        public void GivenЕстьАктёрИгрокаКлассаCaptainВЯчейке(string personSid, int nodeX, int nodeY)
        {
            _context.AddHumanActor(personSid, new OffsetCoords(nodeX, nodeY));
        }

        [Given(@"Актёр игрока экипирован (.*)")]
        public void GivenАктёрИгрокаЭкипированEquipmentSid(string equipmentSid)
        {
            var actor = _context.GetActiveActor();

            var equipment = _context.CreateEquipment(equipmentSid);

            actor.Person.EquipmentCarrier.SetEquipment(equipment, 0);
        }

        [Given(@"Актёр игрока имеет Hp: (.*)")]
        public void GivenАктёрИмеетHp(int startHp)
        {
            var actor = _context.GetActiveActor();
            actor.State.SetHpForce(startHp);
        }

        [Then(@"Предмет (.*) отсутствует в инвентаре актёра")]
        public void ThenЕдаСырОтсутствуетВИнвентареПерсонажа(string propSid)
        {
            var actor = _context.GetActiveActor();

            var propsInInventory = actor.Person.Inventory.CalcActualItems();
            var testedProp = propsInInventory.FirstOrDefault(x => x.Scheme.Sid == propSid);

            testedProp.Should().BeNull();
        }

        [Then(@"Актёр игрока имеет запас hp (.*)")]
        public void ThenАктёрИмеетЗадасHp(int expectedHp)
        {
            var actor = _context.GetActiveActor();
            actor.State.Hp.Should().Be(expectedHp);
        }
    }
}
