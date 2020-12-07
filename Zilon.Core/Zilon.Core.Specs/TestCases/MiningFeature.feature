Feature: Mining

@mytag
Scenario: Add two numbers
	Given Есть карта размером 2
	And Есть актёр игрока класса human-person в ячейке (0, 0)
	And Есть руда Id:500 в ячейке (0, 1)
	When Актёр игрока атакует объект Id:100
	Then Объект Id:500 исчез