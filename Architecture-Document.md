# PagePulse - Scalable Architecture Design

## Overview

The current PagePulse service performs URL audits synchronously. To support:

- 10,000 audits/day
- 500 concurrent request bursts
- Customer-facing response time SLA

the service needs asynchronous processing and scalable components.

The main goal is to keep the API fast while moving heavy audit processing to background workers.

---

# Architecture Diagram

```
                    Users
                      |
                      |
               API Gateway
                      |
                      |
              PagePulse API
                      |
        -----------------------------
        |                           |
        |                           |
    Redis Cache                Message Queue
                                    |
                                    |
                            Audit Worker Service
                                    |
                                    |
                            URL Fetch Service
                                    |
                                    |
                           External Websites


                                    |
                                    |
                              PostgreSQL DB
```

---

# Components

## 1. API Service

Responsible for:

- Receiving audit requests
- Validating input
- Applying rate limits
- Creating audit jobs
- Returning audit status

The API should not directly perform long-running website audits.

---

## 2. Message Queue

A queue is introduced between the API and workers.

Reason:

- Handles traffic spikes
- Prevents API blocking
- Allows independent scaling of workers
- Provides retry support

Example technology:

- RabbitMQ
- Azure Service Bus
- AWS SQS

---

## 3. Audit Workers

Background workers consume jobs from the queue.

Responsibilities:

- Fetch website data
- Process audit
- Store results
- Update job status

During high traffic, more workers can be added.

---

## 4. Redis Cache

Used for temporary and frequently accessed data.

Stores:

- Recent audit results
- Rate limiting information
- Temporary job status

Benefits:

- Faster responses
- Reduced repeated website calls

---

## 5. PostgreSQL Database

Stores permanent data:

- Audit history
- User requests
- Results
- Errors

---

# Data Flow

1. User sends URL audit request.

2. API validates the request.

3. API checks Redis cache.

4. If result exists:
   
   - Return cached response.

5. If not:

   - Create audit job.
   - Send job to queue.

6. Worker consumes the job.

7. Worker fetches the website.

8. Result is stored in PostgreSQL.

9. Result is cached in Redis.

10. User receives the final result.

---

# Queue Strategy

A queue-based approach is preferred instead of synchronous processing.

Without a queue:

- 500 concurrent requests could overload the API.
- Slow websites could block requests.
- Failures become harder to recover.

Queue features:

- Retry failed jobs
- Dead-letter queue for permanent failures
- Track failed audits
- Scale workers independently

---

# State Management

## Redis

Temporary state:

- Cache
- Rate limit counters
- Active audit status

Data has expiry time.

---

## PostgreSQL

Permanent state:

- Completed audits
- Audit history
- Error records

---

# Technology Decisions

| Component | Selected | Alternative | Reason |
|---|---|---|---|
| Backend | ASP.NET Core | Node.js | Good performance and existing implementation |
| Queue | RabbitMQ | Direct API processing | Better handling of traffic bursts |
| Cache | Redis | Memory Cache | Works across multiple API instances |
| Database | PostgreSQL | MongoDB | Better fit for structured audit data |
| Deployment | Docker | Single VM | Easier scaling and deployment |

---

# Failure Mode Analysis

## 1. External Website Timeout

Problem:

Some websites may respond slowly or become unavailable.

Impact:

- Worker delays
- Increased queue time

Mitigation:

- HTTP timeout
- Retry policy
- Circuit breaker
- Failed job tracking

---

## 2. Queue Overload

Problem:

Incoming requests are higher than processing capacity.

Impact:

- Increased audit completion time

Mitigation:

- Add more workers
- Monitor queue length
- Apply backpressure

---

## 3. Database Failure

Problem:

Database becomes unavailable.

Impact:

- Audit results cannot be stored

Mitigation:

- Database backups
- Connection retry
- Database replication/failover

---

# Observability Plan

Monitor:

## API

- Request count
- Response time
- Error rate
- Rate limit failures

## Queue

- Queue size
- Processing time
- Failed jobs

## Workers

- CPU usage
- Memory usage
- Worker failures
- Audit processing time

## Database

- Query latency
- Connection usage

## Cache

- Cache hit rate
- Redis availability

---

# Alerting

Alerts should trigger for:

- High API response latency
- Increasing queue backlog
- High error rate
- Worker failures
- Database connection failures
- Cache failures

---

# Deployment and Rollback Strategy

## Deployment

Use a rolling or blue-green deployment approach.

Process:

1. Deploy new version.
2. Run health checks.
3. Send traffic to new version.
4. Monitor metrics.

---

## Rollback

If deployment causes issues:

1. Stop traffic to the new version.
2. Switch back to the previous stable version.
3. Review logs and metrics.
4. Fix and redeploy.

---

# Credit

Built for Digital Heroes Training Task

https://digitalheroesco.com
