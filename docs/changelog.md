---
title: Changelog
nav_order: 99
---

## Broadcast Changelog

### 0.5.0
* Updated the Project to a .NetStandard Library
* Added: Task scheduling - Execute tasks at a certain time instead of directly
* Added: Recurring task execution - Execute tasks multiple times at a desired interval
* Breaking change: The default PorcessorMode is set to Async. If a Parallel Mode is desired, the Broadcaster has to be initialized with the ProcessorMode set to Parallel. 