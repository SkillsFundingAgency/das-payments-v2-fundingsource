# Payments V2 Funding Source

<img src="https://avatars.githubusercontent.com/u/9841374?s=200&v=4" align="right" alt="UK Government logo">

[![Build Status](https://dev.azure.com/sfa-gov-uk/DCT/_apis/build/status/GitHub/Service%20Fabric/SkillsFundingAgency.das-payments-v2-fundingsource?branchName=main)](https://dev.azure.com/sfa-gov-uk/DCT/_apis/build/status/GitHub/Service%20Fabric/SkillsFundingAgency.das-payments-v2-fundingsource?branchName=main)
[![Jira Project](https://img.shields.io/badge/Jira-Project-blue)](https://skillsfundingagency.atlassian.net/secure/RapidBoard.jspa?rapidView=782&projectKey=PV2)
[![Confluence Project](https://img.shields.io/badge/Confluence-Project-blue)](https://skillsfundingagency.atlassian.net/wiki/spaces/NDL/pages/3700621400/Provider+and+Employer+Payments+Payments+BAU)
[![License](https://img.shields.io/badge/license-MIT-lightgrey.svg?longCache=true&style=flat-square)](https://en.wikipedia.org/wiki/MIT_License)


The Payments V2 Funding Source ServiceFabric application provides functionality for determining how training provider payments are funded, based on the employer's levy status, levy balance, and the level of ESFA co-investment that applies to the learner.

More information here: https://skillsfundingagency.atlassian.net/wiki/spaces/NDL/pages/533659736/e.+Payment+Funding+Source+Application+DAS+Space

## How It Works

This repository contains stateful and stateless ServiceFabric services that publish events to be consumed by the Payments V2 system for the calculation of payments due for learners, based on their ILR submission data and the funding available in the employer's levy account (if applicable)

The application contains separate services for calculating payments for Levy and Non-Levy employers. For Non-Levy employers, the funding is partially or fully paid by the ESFA, with the employer paying the applicable percentage of co-investment, as determined by the funding rules. For Levy employers, the funding is taken from their levy account balance until that is exhausted, upon which the same co-investment rules apply as for Non-Levy employers.

The application also processes incentive payments as applicable to the funding rules, and transfers of balances between employer levy accounts.

## 🚀 Installation

### Pre-Requisites

Setup instructions can be found at the following link, which will help you set up your environment and access the correct repositories: [https://skillsfundingagency.atlassian.net/wiki/spaces/NDL/pages/950927878/Development+Environment+-+Payments+V2+DAS+Space](https://skillsfundingagency.atlassian.net/wiki/spaces/NDL/pages/4948754681/DAS+Payments+-+Developer+Onboarding+2025)

Select the configuration for the FundingSource application

## 🔗 External Dependencies

N/A

## Technologies

* .NetCore 2.1/3.1/6
* Azure SQL Server
* Azure Functions
* Azure Service Bus
* ServiceFabric

## 🐛 Known Issues

N/A

