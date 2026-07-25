# PagePulse - Task B: Scalable Architecture Design

## 1. Overview

The current PagePulse API performs URL audits directly through the API request. To support 10,000 audits/day and bursts of 500 concurrent requests, the service needs asynchronous processing, scalable workers, and better monitoring.

The main idea is to keep the API fast and move heavy URL processing to background workers.

---

# Architecture

```
                 Users
                   |
                   |
            ASP.NET Core API
                   |
        ------------------------
        |                      |
      Cache                 Queue
                               |
                               |
                    .NET Worker Service
                               |
                               |
                       URL Audit Process
                               |
                               |
                    External Websites
                               |
                               |
                       Elasticsearch
                               |
                               |
                            Kibana
```

---

# 2. Components and Data Flow

## ASP.NET Core API

Responsibilities:

- Receive audit requests
- Validate URLs
- Apply rate limiting
- Check cache
- Create audit jobs

The API does not perform long-running audits directly.

---

## Queue

A queue is used between API and workers.

Why:

- Handles traffic spikes
- Prevents API blocking
- Allows worker scaling
- Supports retries

---

## .NET Worker Service

Workers process audit jobs in the background.

They:

- Fetch website information
- Calculate audit details
- Store results

More workers can be added when traffic increases.

---

## Elasticsearch + Kibana

Elasticsearch stores audit results as documents.

Example data:

```
URL
Status Code
Response Time
Timestamp
Errors
```

Kibana is used to visualize:

- Audit success/failure rate
- Response time
- Error trends
- Traffic patterns

---

## Data Flow

1. User sends audit request.
2. API validates the request.
3. API checks cache.
4. If result exists, return cached response.
5. Otherwise, create an audit job.
6. Worker consumes the job.
7. Worker performs URL audit.
8. Result is stored in Elasticsearch.
9. Result is returned and cached.

---

# 3. Technology Decisions

| Component | Choice | Alternative | Reason |
|---|---|---|---|
| Backend | ASP.NET Core | Node.js | Existing implementation and strong async support |
| Processing | .NET Worker Service | Direct API processing | Avoids blocking API requests |
| Storage | Elasticsearch | SQL Database | Audit data is document-based and searchable |
| Monitoring | Kibana | Custom dashboard | Works directly with Elasticsearch data |
| Queue | RabbitMQ | Synchronous processing | Better handling of request bursts |

---

# 4. Failure Mode Analysis

## External Website Timeout

Problem:
Some websites may respond slowly or fail.

Impact:
Audit processing becomes slow.

Solution:

- Add request timeout
- Retry failed requests
- Track failed audits

---

## Queue Overload

Problem:
More audit requests arrive than workers can process.

Impact:
Higher waiting time.

Solution:

- Increase worker instances
- Monitor queue size
- Apply request limits

---

## Elasticsearch Failure

Problem:
Audit results cannot be stored.

Impact:
Data availability issues.

Solution:

- Retry failed indexing
- Monitor cluster health
- Maintain backups

---

# 5. Monitoring and Rollback Plan

## Monitoring

Track:

API:
- Request latency
- Error rate
- Request count

Workers:
- Processing time
- Failed jobs
- Worker availability

Queue:
- Queue length
- Processing delay

Elasticsearch:
- Indexing errors
- Query latency
- Cluster status

Kibana dashboards can be used for visualization.

---

## Rollback Strategy

For a bad deployment:

1. Deploy the new version.
2. Run health checks.
3. Monitor errors and latency.
4. If problems occur, switch back to the previous stable version.
5. Fix issues and redeploy.

---

# Credit

Built for Digital Heroes Training Task

https://digitalheroesco.com
