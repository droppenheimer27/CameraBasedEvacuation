# Camera-Based Evacuation System

This project consists of multiple services that simulate a camera-based evacuation system. It tracks the number of people entering and exiting a premises using updates from cameras. The system processes these updates in real-time, and it includes two main components:

1. **Central Server API**:
    - The central server that receives and processes updates from cameras. It aggregates camera updates to calculate the number of people on-site.
    - This API listens for updates from individual camera instances and uses actor-based messaging for managing real-time data flow.

2. **Camera Management API**:
    - A set of APIs simulating individual remote cameras that send updates about people entering or exiting their respective zones. Each camera operates as an independent service and communicates with the central server.
    - Multiple instances of this service can be run to simulate multiple cameras (C1, C2, C3, etc.), each responsible for tracking activity in a specific area.

## Running All Projects

### Prerequisites

Ensure you have all necessary dependencies installed, including .NET SDK.

### Running All APIs Locally

1. **CentralServer.Api**:
   - Open the solution folder.
   - Run the following command to start the Central Server API:
     ```bash
     dotnet run --launch-profile http --project .\CentralServer.Api
     ```
   - This will start the server on `http://localhost:5266`.

2. **RemoteCamera.Api**:
   - To simulate different cameras, you can start multiple instances of the `RemoteCamera.Api` project with different profiles.
   - Run the following commands to start three cameras (`C1`, `C2`, and `C3`):
     ```bash
     dotnet run --launch-profile C1 --project .\RemoteCamera.Api
     dotnet run --launch-profile C2 --project .\RemoteCamera.Api
     dotnet run --launch-profile C3 --project .\RemoteCamera.Api
     ```

   - This will start the cameras on:
      - Camera 1: `http://localhost:5252`
      - Camera 2: `http://localhost:5253`
      - Camera 3: `http://localhost:5254`

3. **Adding More Cameras**:
   - To add more cameras, simply create a new profile in `launchSettings.json` with a unique `CAMERA_ID`, `ACTOR_PORT`, and a distinct `applicationUrl`.

   Example:
   ```json
   "C4": {
     "commandName": "Project",
     "dotnetRunMessages": true,
     "launchBrowser": true,
     "launchUrl": "swagger",
     "applicationUrl": "http://localhost:5255",
     "environmentVariables": {
       "ASPNETCORE_ENVIRONMENT": "Development",
       "ACTOR_HOSTNAME": "localhost",
       "ACTOR_PORT": "8084",
       "CAMERA_ID": "C4"
     }
   }

## Swagger Usage

Both the **RemoteCamera.Api** and **CentralServer.Api** services come with Swagger documentation, making it easy to interact with the APIs for testing and development purposes. Below are the steps to use Swagger and example queries for the provided endpoints:

### Accessing Swagger

1. Run the projects as described in the **Running All Projects** section.
2. Open your browser and navigate to the following URLs based on the service:
   - **CentralServer.Api**:  
     Swagger URL: `http://localhost:5266/swagger`
   - **RemoteCamera.Api**:  
     Swagger URL: `http://localhost:5252/swagger` (or the corresponding camera's port from the profiles: `5253`, `5254`, etc.)

---

### RemoteCamera.Api Endpoints

![img_1.png](img_1.png)

#### POST `/api/Cameras` - Add a New Camera State

**Request Body** (JSON example):
```json
{
  "timeStamp": "2025-01-01T12:00:00Z",
  "in": 5,
  "out": 3
}
```
---

### CentralServer.Api Endpoints

![img.png](img.png)

#### GET `/api/CameraUpdates` - Retrieve the total number of people currently on-site.
#### GET `/api/CameraUpdates/{cameraId}` - Retrieve the number of people currently reported by a specific camera.

## Architectural Decisions

### Why Akka.NET?

Akka.NET is a great fit for this solution because it simplifies managing multiple real-time camera updates. Here’s why:

1. **Handles Concurrency Easily**: Akka.NET uses actors to process messages one at a time, ensuring updates from cameras don’t conflict with each other.
2. **Built-in Fault Tolerance**: If something goes wrong, Akka.NET can automatically restart parts of the system, keeping things running smoothly.
3. **Scalable**: As the system grows (more cameras, more servers), Akka.NET can scale easily without complicating things.
4. **Event-Driven**: The message-driven design works perfectly for handling camera updates in real-time without unnecessary complexity.
5. **Distributed Ready**: Akka.NET supports communication across different services, making it ideal for a distributed system like this one.

### Why MediatR?

MediatR was a perfect choice for this system because it helps to decouple the services, making the code cleaner and easier to maintain. Here's why it's great:

1. **Decouples Components**: MediatR allows components to communicate without knowing about each other. It simplifies the architecture by removing direct dependencies between services, reducing the risk of tight coupling.

2. **Focus on One Use Case**: With MediatR, you can easily focus on one specific use case at a time. Each handler handles one request, making it clear and simple to follow.

3. **Easier Testing**: Testing becomes simpler with MediatR. The handler is responsible for a single use case and only requires the dependencies necessary for that case, not for unrelated parts of the system. This makes unit testing more straightforward and less prone to errors.

4. **Seamless Integration**: MediatR integrates smoothly with the rest of the application, allowing us to pass requests and responses cleanly without introducing unnecessary complexity.

---

## Solution

The solution is an attempt to follow a **Hexagonal Architecture**, which helps in isolating the core logic from external dependencies. This approach ensures that the core of the system remains independent of frameworks and technologies, making it easier to test, maintain, and extend.

### Folder Structure:

- **Api**: Contains all the HTTP API controllers and related functionality.

- **Application**: This layer contains the business logic, use cases as **MediatR handlers**. It's responsible for coordinating between the domain and the external infrastructure, ensuring the system behaves according to the rules defined in the domain layer.

- **Domain**: Contains the core business logic, entities, and domain models. This layer is independent of any frameworks or technologies and focuses on the problem you're trying to solve.

- **Infrastructure**: Includes the implementation of external services and technical details.

This structure ensures a clear separation of concerns, where each layer has a specific responsibility, and the core logic remains independent from external frameworks or changes.

---

## Considerations

### Resilience 

The resilience part of the solution is a bit tricky. Akka.NET has been used for communication between services, but right now it lacks persistence. This means that if a message sent to the CentralServer gets accepted and then something happens (like a crash or network issue), the message could be lost.

Ideally, the solution would include message acknowledgment and an internal queue for each camera to hold messages until the CameraMonitor actor becomes available again. This would ensure that messages are not lost during temporary issues.

However, there's a bit of a dilemma here: adding such persistence could introduce delays. This system is designed for real-time analytics, and any extra acknowledgment or queuing could significantly increase latency. The solution, therefore, needs some clarification and a decision between maintaining accuracy and reducing latency. The choice depends heavily on the throughput of the system.

Given that this is an evacuation system, accuracy is likely more critical than speed. In an emergency situation, we can't afford to miss even a single update on the number of people in a zone. So, after weighing both sides, it seems like the more accurate approach is probably the right one here, even if it means a little more latency.

That said, this is something that would need to be fine-tuned in a production-ready version, but for now, since this is just a demo, the solution doesn’t include the persistence mechanisms. I’ve considered the trade-offs, but in the case of an evacuation system, accuracy should likely take precedence.

### Zone Overlap

The current solution assumes that each camera covers a distinct, non-overlapping zone. This means the system calculates the number of people in each zone independently, without accounting for any areas covered by multiple cameras.

In cases where zones do overlap, there’s a risk of double-counting people if they are detected by more than one camera. To address this in a production-ready version, advanced techniques like assigning unique IDs to individuals (e.g., through ML-based person detection, object tracking, or face recognition) would be required. These methods would help to accurately track people across multiple zones and eliminate duplication.

For now, since this is a demonstration, the solution is limited to handling non-overlapping zones only. While this approach simplifies calculations, it’s important to note that real-world scenarios with overlapping zones would require additional complexity to ensure accurate results.

### Negative People Count

In the current implementation, there is a potential edge case where the first update received from a camera has In = 0 and Out = 1. This would result in a negative count for people on the site, which is logically incorrect.

Example Scenario
Consider the following initial update:

```json
{
  "CameraId": "C1",
  "Timestamp": "2025-01-24T12:00:00Z",
  "In": 0,
  "Out": 1
}
```

In this situation, the calculated count would be -1.

#### Possible Explanation
This edge case might occur due to anomalies like:

- A person leaving the zone without being detected as entering (e.g., camera malfunction or blind spots).
- External factors like a power failure or a broken light in the area, causing unreliable detection.
- Camera recalibration or data synchronization issues where initial data doesn’t represent actual events.

#### Future Considerations

Handling this scenario requires additional input from domain experts to understand:

- Whether such cases are realistic or acceptable in the context of the evacuation system.
- How the system should respond: should it discard, flag, or adjust such updates?
- The implications of correcting data in real-time versus accepting potential inaccuracies.

