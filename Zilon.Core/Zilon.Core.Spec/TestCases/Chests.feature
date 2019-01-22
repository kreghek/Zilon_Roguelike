Feature: Chests

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

@chest @dev1 @loot
Scenario: Если забираем из сундука ресурсы, то оно должно быть перемещено из сундука в инвентарь.
	Given Есть карта размером 2
	And Есть актёр игрока класса captain в ячейке (0, 0)
	And Есть сундук Id:500 в ячейке (0, 1)
	And Сундук содержит Id:500 ресурс bottle в количестве 1
	When Я выбираю сундук Id:500
	And Я забираю из сундука рерурс bottle в количестве 1
	Then У актёра в инвентаре есть bottle
	And В сундуке Id:500 нет предмета bottle

Scenario: Открытые контейнеры должны открываться при помощи рук
#(без использования ключей, отмычек и т.д.)
#на открытые сундуки не тратятся ходы
	Given I have entered 50 into the calculator
	And I have entered 70 into the calculator
	When I press add
	Then the result should be 120 on the screen

@mytag
Scenario: Одинаковый дроп объединяется в один стак.
# Одинаковый дроп может появиться в результате 2+ проходов по одной таблице дропа
# или двух таблиц дропа с одинаковым ресурсом
	Given I have entered 50 into the calculator
	And I have entered 70 into the calculator
	When I press add
	Then the result should be 120 on the screen
