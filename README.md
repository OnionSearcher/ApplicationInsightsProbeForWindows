# ApplicationInsightsProbeForWindows
AI allow you to collect and display several informations of yours applications itselft, but also of the OS (like RAM, CPU, performance counter...)
Sadely, this AI framework is only provided for azure web site or workers, not for the OS by itself.

This Windows Service allow you to monitor any Windows OS itself (Azure VM or not) with all the AI advantage.
For exemple, allow you to monitor the OS that host an SQL Server instance and include several SQL performance counter.

## Requirements

 - .Net 4.6
 - Azure account (free or not)
 - Create an Azure ApplicationInsights (free or not) and get the key