Feature: Levy Account Balance Priority For Co-Invested Learners

When co-investment applies to a learner and course
As a service owner
I want the employer's Levy Account balance to be utilised before making co-invested payments

Scenario: Levy - Employer's Growth and Skills Levy account has enough funds - 100% co-investment
	Given A Levy paying employer has sufficient funds in their Growth and Skills Levy account
	When the SFA contribution percentage is set to 100% for the learner and course
	Then the training provider payments should be fully funded by the employer's Growth and Skills Levy account

Scenario: Levy - Employer's Growth and Skills Levy account has insufficient funds - 100% co-investment
	Given A Levy paying employer has insufficient funds in their Growth and Skills Levy account
	When the SFA contribution percentage is set to 100% for the learner and course
	Then the training provider payments should be partially funded by the employer's Growth and Skills Levy account

Scenario: Levy - Employer's Growth and Skills Levy account has no funds - 100% co-investment
	Given A Levy paying employer has no funds in their Growth and Skills Levy account
	When the SFA contribution percentage is set to 100% for the learner and course
	Then the training provider payments should be fully funded by SFA

Scenario: Non-Levy - Employer's Growth and Skills Levy account has enough funds - 100% co-investment
	Given A Non-Levy paying employer has sufficient funds in their Growth and Skills Levy account
	When the SFA contribution percentage is set to 100% for the learner and course
	Then the training provider payments should be fully funded by the employer's Growth and Skills Levy account

Scenario: Non-Levy - Employer's Growth and Skills Levy account has insufficient funds - 100% co-investment
	Given A Non-Levy paying employer has insufficient funds in their Growth and Skills Levy account
	When the SFA contribution percentage is set to 100% for the learner and course
	Then the training provider payments should be partially funded by the employer's Growth and Skills Levy account

Scenario: Non-Levy - Employer's Growth and Skills Levy account has no funds - 100% co-investment
	Given A Non-Levy paying employer has no funds in their Growth and Skills Levy account
	When the SFA contribution percentage is set to 100% for the learner and course
	Then the training provider payments should be fully funded by SFA

