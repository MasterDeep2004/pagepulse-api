# PagePulse API

Production-grade URL audit service built with ASP.NET Core 9.0.

PagePulse audits URLs by validating requests, fetching webpage information, and returning structured audit results. The service is designed with production features like caching, rate limiting, timeout handling, structured logging, error handling, Docker deployment, and CI automation.

---

## Live Deployment

Live URL:

https://pagepulse-api.onrender.com

---

# API Contract

## Audit URL

### POST

```
/api/audit
```

### Request

```json
{
  "url": "https://example.com"
}
```

### Response

```json
{
  "url": "https://example.com",
  "statusCode": 200,
  "title": "Example Domain",
  "responseTime": 120
}
```

---

# Features Implemented

## Input Validation

- Implemented using FluentValidation
- Validates required fields and URL format
- Returns structured validation errors

---

## Timeout Handling & Concurrency Limits

- Configurable HTTP request timeout
- Prevents slow external websites from blocking resources
- Limits concurrent audits to maintain service stability

---

## Caching

Repeat audits of the same URL are served from cache within a configurable time window.

Benefits:

- Faster response time
- Reduced external requests
- Improved scalability

Example configuration:

```json
{
  "CacheSettings": {
    "DurationMinutes": 5
  }
}
```

---

## Rate Limiting

Implemented using ASP.NET Core Rate Limiting middleware.

Features:

- Fixed window rate limiter
- Configurable request limits
- Protects API from excessive traffic

Example:

```json
{
  "RateLimitSettings": {
    "PermitLimit": 10,
    "WindowSeconds": 60
  }
}
```

---

## Structured Logging & Error Handling

- Global exception middleware
- Structured API error responses
- Unique Request IDs for request tracing and debugging

Example:

```json
{
  "requestId": "abc123",
  "message": "Unexpected error occurred"
}
```

---

# Architecture

```
Client
  |
  v
API Controller
  |
Validation
  |
Audit Service
  |
----------------
|              |
Cache       HttpClient
                |
                v
        External Website
```

---

# Technology Stack

- C#
- ASP.NET Core 9.0
- FluentValidation
- MemoryCache
- ASP.NET Core Rate Limiting
- Swagger/OpenAPI
- Docker
- Render
- GitHub Actions

---

# Project Structure

```
PagePulse
|
├── PagePulse.Api
│   ├── Controllers
│   ├── Services
│   ├── Middleware
│   └── Validators
|
├── PagePulse.Tests
|
├── Dockerfile
|
└── .github
    └── workflows
```

---

# Running Locally

Requirements:

- .NET 9 SDK

Clone:

```bash
git clone https://github.com/MasterDeep2004/pagepulse-api.git
```

Run:

```bash
dotnet run --project PagePulse.Api
```

---

# Docker

Build:

```bash
docker build -t pagepulse-api .
```

Run:

```bash
docker run -p 8080:8080 pagepulse-api
```

---

# Testing & CI

Run tests:

```bash
dotnet test
```

GitHub Actions runs automatically on every push and performs:

- Dependency restore
- Build verification
- Automated tests

---

# AI Usage Disclosure

AI tools were used as a development assistant during this task.

Used for:

- Debugging Docker and Render deployment issues
- Reviewing production configuration decisions
- Improving documentation
- Brainstorming edge cases related to validation, caching, rate limiting, and error handling

All architecture decisions, implementation choices, and final code changes were reviewed and adapted based on my understanding of the requirements.

---

# Credit

Built for Digital Heroes Training Task

Linked to:

https://digitalheroesco.com
