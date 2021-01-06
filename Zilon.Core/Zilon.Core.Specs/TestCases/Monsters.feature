Feature: Monsters

@dev1 @monsters
Scenario: Монстры бродят по комнате без выброса исключений.
	Given Есть карта размером 10
	And Есть монстр класса rat Id:100 в ячейке (0, 0)
	And Есть монстр класса rat Id:200 в ячейке (1, 3)
	And Есть монстр класса rat Id:300 в ячейке (2, 6)
	When Следующая итерация сектора 100 раз

@dev1 @monsters
Scenario: Не замирает в конечной точке патруллирования.
	Given Есть карта размером 5
	And Есть монстр класса rat Id:100 в ячейке (0, 0)
	And Для монстра Id:100 задан маршрут
	| x | y |
	| 0 | 0 |
	| 4 | 4 |
	When Следующая итерация сектора 12 раз
	Then Монстр Id:100 не стоит в узле (4, 4)

@monsters @dev1 @perks
Scenario: Для монстров не поддерживается развитие. Для них не должно выбрасываться исключение при попытке прокачать перки.
	Given Есть карта размером 2
	And Есть актёр игрока класса human-person в ячейке (0, 0)
	# bat has 1d3 attack efficient. In test all efficient is divided by 2. Bat attack efficient divided by 2 is 1.
	# It is enought to kill the player person with 1 HP.
	And Есть монстр класса bat Id:100 в ячейке (1, 0)
	And Актёр игрока имеет Hp: 1
	And Монстр Id:100 имеет Hp 1000
	# Monster must to attack and kill the player person
	When Я жду 1 итерацию
	Then Актёр игрока мертв