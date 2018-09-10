Feature: Cheast_TakeWeapon
	In order to avoid silly mistakes
	As a math idiot
	I want to be told the sum of two numbers

@mytag
Scenario: Если забираем из сундука орудие, то не должно выбрасываться исключений.
	Given Есть карта размером 2
	And Есть актёр игрока класса captain в ячейке (0, 0)
	And Есть сундук Id:500 в ячейке (0, 1)
	And Сундук содержит Id:500 экипировку pistol
	When Я выбираю сундук Id:500
	And Я забираю из сундука экипировку pistol
	Then У актёра в инвентаре есть pistol
	And В сундуке Id:500 нет экипировки pistol
