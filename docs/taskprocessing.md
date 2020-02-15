---
title: Task processing
layout: "default"
nav_order: 1
---
## Task processing
Processing tasks is the core element of Broadcast and is done with just 2 lines of code
```csharp
var broadcaster = new Broadcaster();
broadcaster.Send(() => Trace.WriteLine("This is a basic task"));
```

## Processing Mode
In the inside tasks are processed by Taksprocessors. These are definded by the Interface ITaskProcessor.

Broadcast has 3 different builtin Modes for processing tasks
1. Async
2. Background
3. Serial

async is the default if none other is defined.

### Async Processing
Each task is processed in a new thread
```csharp
var broadcaster = new Broadcaster(ProcessorMode.Async);
broadcaster.Send(() => Trace.WriteLine("This is a basic task"));
```

Because async is the default it is not neccessary to pass the ProcessorMode
```csharp
var broadcaster = new Broadcaster();
broadcaster.Send(() => Trace.WriteLine("This is a basic task"));
```

### Background Processing
All tasks are added to a Queue for processing. The Processor starts a thread to process all tasks in the Queue.
```csharp
var broadcaster = new Broadcaster(ProcessorMode.Background);
broadcaster.Send(() => Trace.WriteLine("This is a basic task"));
```

### Background Serial
All tasks are automaticaly processed. This processingmode only makes sense in testmode to guarantee all tasks are processed befor continuing.
```csharp
var broadcaster = new Broadcaster(ProcessorMode.Background);
broadcaster.Send(() => Trace.WriteLine("This is a basic task"));
```