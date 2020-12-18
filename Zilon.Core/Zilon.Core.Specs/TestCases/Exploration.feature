Feature: Exploration

@dev16 @transitions
Scenario: Using transition moves actor into next sector
	Given the linear globe
	And the player actor in the map node (0, 0)
	When I use current transition
	And Я жду 1 итераций