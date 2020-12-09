Feature: Mining

@dev16 @mining
Scenario: Destroy deposit
	Given Есть карта размером 2
	And Есть актёр игрока класса human-person в ячейке (0, 0)
	And the ore with id:100 in the map node (0, 1)
	When the player actor attacks object with id:100 while it exists
	Then Объект Id:100 уничтожен