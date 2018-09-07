Feature: Move_CanMoveOnlyInNeighborNode
	In order to avoid silly mistakes
	As a math idiot
	I want to be told the sum of two numbers

@mytag
Scenario: Можно перемещаться только в соседние узлы. То есть этот тест толжен проверять выброс ошибки, если выбран не соседний узел.
	Given I have entered 50 into the calculator
	And I have entered 70 into the calculator
	When I press add
	Then the result should be 120 on the screen
