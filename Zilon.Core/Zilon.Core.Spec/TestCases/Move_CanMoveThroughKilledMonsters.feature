Feature: Move_CanMoveThroughKilledMonsters
	In order to avoid silly mistakes
	As a math idiot
	I want to be told the sum of two numbers

@mytag
Scenario: Перемещение через убитых монстров.
	Given Есть актёр игрока и монстр
	And Они стоят в коридоре
	When Актёр игрока убивает монстра
	And Актёр игрока перемещается в узел за монстром
	Then Актёр игрока находится в узле за монстром
