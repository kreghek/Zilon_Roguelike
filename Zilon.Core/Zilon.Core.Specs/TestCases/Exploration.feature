Feature: Exploration

@dev16 @transitions
Scenario: Using transition moves into next sector
	Given the linear globe
	And the player actor in the map node (0, 0)
	When the player person uses current transition
	And Я жду 2 итерацию
	Then the player actor in the map with id:2