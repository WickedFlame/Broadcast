---
layout: "default"
---
# Recurring Tasks

## Workflow
- Task is created with flag IsRecurring = true
- RecurringTask is created with a ReferenceId to the current task
- Task gets executed
- task gets cloned and added to the pipeline
- new RecurringTask is created with same name but referenceId to the new task (RecurringTask is updated with next execution and ReferenceId)