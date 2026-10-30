Feature: Short Course Funding Source Payments

As a Training Provider
I would like Apprenticeship Unit payments and clawbacks to keep their course details when they are funded
So that the payment is reported against the correct funding line type

Scenario: Levy funded Apprenticeship Unit payment keeps the short course details
	Given the learning is an Apprenticeship Unit short course funded under the "GSO Short Courses (Apprenticeship Units) Levy" funding line
	And the employer is Levy funded with sufficient funds in their account
	When a levy payment required payment is received for the short course
	And the period end runs
	Then the recorded payments carry the funding line type and the short course details

Scenario: Levy funded Apprenticeship Unit clawback keeps the short course details
	Given the learning is an Apprenticeship Unit short course funded under the "GSO Short Courses (Apprenticeship Units) Levy" funding line
	And the employer is Levy funded with sufficient funds in their account
	When a levy clawback required payment is received for the short course
	And the period end runs
	Then the recorded payments carry the funding line type and the short course details

Scenario: Non-Levy funded Apprenticeship Unit payment keeps the short course details
	Given the learning is an Apprenticeship Unit short course funded under the "GSO Short Courses (Apprenticeship Units) Non-Levy" funding line
	And the employer is Non-Levy funded
	When a co-invested payment required payment is received for the short course
	Then the recorded payments carry the funding line type and the short course details

Scenario: Non-Levy funded Apprenticeship Unit clawback keeps the short course details
	Given the learning is an Apprenticeship Unit short course funded under the "GSO Short Courses (Apprenticeship Units) Non-Levy" funding line
	And the employer is Non-Levy funded
	When a co-invested clawback required payment is received for the short course
	Then the recorded payments carry the funding line type and the short course details
