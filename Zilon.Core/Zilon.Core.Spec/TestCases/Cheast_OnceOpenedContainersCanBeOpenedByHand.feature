Feature: Cheast_OnceOpenedContainersCanBeOpenedByHand
	In order to avoid silly mistakes
	As a math idiot
	I want to be told the sum of two numbers

@mytag
Scenario: Открытые контейнеры должны открываться при помощи рук (без использования ключей, отмычек и т.д.)
	Given I have entered 50 into the calculator
	And I have entered 70 into the calculator
	When I press add
	Then the result should be 120 on the screen
