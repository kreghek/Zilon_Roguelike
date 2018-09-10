Feature: Cheast_TakeWeapon
	In order to avoid silly mistakes
	As a math idiot
	I want to be told the sum of two numbers

@mytag
Scenario: Если забираем из сундука орудие, то не должно выбрасываться исключений.
	Given Есть карта размером 2
	And Есть актёр игрока класса captain в ячейке (0, 0)
	And Есть сундук в ячейке (0, 1)
	And Сундук содержит экипировку pistol
	When Я выбираю сундук в ячейке (0, 1)
	Then У актёра в инвентаре есть 
