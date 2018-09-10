Feature: Chest_TakeWeapon
	Чтобы была возможность забирать предметы
	As a math idiot
	I want to be told the sum of two numbers

@chest @dev0 @loot
Scenario: Если забираем из сундука оружие, то оно должно быть перемещено из сундука в инвентарь.
	Given Есть карта размером 2
	And Есть актёр игрока класса captain в ячейке (0, 0)
	And Есть сундук Id:500 в ячейке (0, 1)
	And Сундук содержит Id:500 экипировку pistol
	When Я выбираю сундук Id:500
	And Я забираю из сундука экипировку pistol
	Then У актёра в инвентаре есть pistol
	And В сундуке Id:500 нет экипировки pistol
