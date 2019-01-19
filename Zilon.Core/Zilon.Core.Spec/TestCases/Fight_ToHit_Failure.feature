Feature: Fight_ToHit_Failure
	Чтобы было противостояние наступление и обороны
	Как разработчику
	Мне нужно, чтобы персонаж не мог попадать по противнику, если тот умеет оборонятся против типа нападения действия.

@fight @dev0
Scenario Outline: Промахи действий с определённым типом нападения, если у противника высокий уровень обороны проив этого действия.
	Given Есть карта размером 10
	And Есть актёр игрока класса <personSid> в ячейке (0, 0)
	And Есть монстр класса <monsterSid> Id:<monsterId> в ячейке (<monsterNodeX>, <monsterNodeY>)
	And Актёр игрока экипирован предметом <equipmentSid> в слот Index: 2
	And В инвентаре у актёра есть ресурс: mana количество: 10
	When Актёр игрока атакует монстра Id:<monsterId>
	Then Монстр Id:<monsterId> успешно обороняется

Examples: 
| personSid | equipmentSid | monsterSid | monsterId | monsterNodeX | monsterNodeY |
| captain   | short-sword  | pretorian  | 1000      | 0            | 1            |
| captain   | battle-axe   | pretorian  | 1000      | 0            | 1            |
| captain   | shadow-staff | hound      | 1000      | 0            | 1            |
| captain   | stilleto     | hound      | 1000      | 0            | 1            |
| captain   | sniper-rifle | jocker     | 1000      | 0            | 1            |
| captain   | minigun      | jocker     | 1000      | 0            | 1            |
