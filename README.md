# Crossmint Megaverse Challenge Submission

This project is a C# console application that solves the Megaverse challenge. The algorithm aims to manipulate a map, adjusting it to match a predefined goal map, with a focus on handling API constraints and failures efficiently.

## Algorithm

1. **Get the map goal**
2. **Get the current map**
3. **While (current map != goal map)**:
   - Retrieve the current state of the map.
   - Mapper: Identify which resources need to be deleted and posted.
   - Batch the DELETE and POST operations together.

### Notable Points:
- After DELETE calls, sometimes the API will return a 200 status code without actually performing the delete. Hence, a retry loop is implemented to ensure operations are effective.
- The POST operation may fail intermittently due to external factors (e.g., someone modifying the map while it's being processed).

## Notable Implementation Points

1. **Enum Mapping**:
   - All terms in the map (e.g., `SPACE`, `POLYANET`) are mapped to corresponding enum values for cleaner code and easier operations.

2. **Stress Testing**:
   - The application was stress-tested by randomly "dirtying" the map before running the solution. The algorithm is agnostic to the starting map, meaning it will adapt to any initial state.

3. **Abstraction Layers**:
   - **MegaverseClient**: Handles low-level operations, such as API calls and response handling.
   - **MegaverseService**: Manages high-level logic, including creating or deleting resources. It also maps internal enum terms to resources based on the star enum.

4. **Retries & Task Batching**:
   - **Retries**: The system retries operations up to 200 times per second when receiving a `429` status (rate limit).
   - **Batching**: A maximum of 5 operations are processed at once, followed by a 10-second wait before the next batch. This reduces the likelihood of the API being rate-limited.

## Learnings

1. **API Header Analysis**:
   - The API uses the `application/x-www-form-urlencoded` content type, which was discovered through curl requests and header analysis.

2. **Rate Limiting**:
   - The API is rate-limited after around 8 calls, seemingly employing a sliding window rate limit. The exact behavior appears somewhat random.

3. **DELETE Operations**:
   - DELETE calls are not always successful on the first try and may require multiple attempts to ensure the resource is properly deleted.

