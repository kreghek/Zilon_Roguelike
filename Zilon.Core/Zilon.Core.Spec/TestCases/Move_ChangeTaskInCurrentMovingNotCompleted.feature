Feature: Move_ChangeTaskInCurrentMovingNotCompleted
	In order to avoid silly mistakes
	As a math idiot
	I want to be told the sum of two numbers

@mytag
Scenario: Указали вперед 3 клетки, указали двигаться в другом направлении или что-нибудь съели.
	Given I have entered 50 into the calculator
	And I have entered 70 into the calculator
	When I press add
	Then the result should be 120 on the screen
