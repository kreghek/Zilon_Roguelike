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

        [Given(@"Между ячейками \((.*), (.*)\) и \((.*), (.*)\) есть стена")]
        public void GivenМеждуЯчейкамиИЕстьСтена(int x1, int y1, int x2, int y2)
        {
            _context.AddWall(x1, y1, x2, y2);
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
        public void GivenАктёрИмеетHp(float startHp)
        {
            var actor = _context.GetActiveActor();
            actor.State.SetHpForce(startHp);
        }

        [Given(@"Есть монстр класса (.*) Id:(.*) в ячейке \((.*), (.*)\)")]
        public void GivenЕстьМонстрКлассаRatВЯчейке(string monsterSid, int monsterId, int x, int y)
        {
            _context.AddMonsterActor(monsterSid, monsterId, new OffsetCoords(x, y));
        }

        [Given(@"Монстр Id:(.*) имеет Hp (.*)")]
        public void GivenМонстрIdИмеетHp(int monsterId, float monsterHp)
        {
            var monster = _context.GetMonsterById(monsterId);

            monster.State.SetHpForce(monsterHp);
        }

        [When(@"Я выбираю ячейку \((.*), (.*)\)")]
        public void WhenЯВыбираюЯчейку(int x, int y)
        {
            _context.HoverNode(x, y);
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
        public void ThenАктёрИмеетЗадасHp(float expectedHp)
        {
            var actor = _context.GetActiveActor();
            actor.State.Hp.Should().Be(expectedHp);
        }

        [Then(@"Монстр Id:(.*) имеет Hp (.*)")]
        public void ThenМонстрIdИмеетHp(int monsterId, float expectedMonsterHp)
        {
            var monster = _context.GetMonsterById(monsterId);

            monster.State.Hp.Should().Be(expectedMonsterHp);
        }

    }
}
