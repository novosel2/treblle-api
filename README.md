# Social Network Proxy API

A proxy layer built for a hackathon that sits between clients and my social network API deployed on Azure. Built with ASP.NET Core and PostgreSQL, this adds logging, monitoring, and extra control over the original API endpoints.

## What's This About?

This proxy intercepts requests to the social network API and adds functionality like request logging and problematic activity detection. Think of it as a middleware that watches what's happening and gives you insights into API usage patterns.

The actual API endpoints aren't exposed directly, unfortunately :(. At the end I gave a couple examples for the API endpoints.

## Prerequisites

- Docker and Docker Compose installed
- That's it :)

## Setup

1. **Copy the environment file**

   ```bash
   cp .env_example .env
   ```

2. **Add your credentials**

   Open `.env` and fill in your database password and any other required values:

   ```
   POSTGRES_PASSWORD=your_password_here
   ```

3. **Fire it up**

   ```bash
   docker-compose up --build
   ```

4. **Access the API**

   The proxy runs on `http://localhost:8000`

## Available Endpoints

### Proxied Social Network Endpoints

All social network API calls go through `/api/proxy/{path}` - the proxy handles forwarding to Azure.

**Register**

```http
POST /api/proxy/account/register
Content-Type: application/json

{
"username": "your_username",
"email": "your@email.com",
"password": "your_password",
"confirmPassword": "your_password"
}
```

**Login**

```http
POST /api/proxy/account/login
Content-Type: application/json

{
"name": "your_username_or_email",
"password": "your_password"
}
```

Returns a JWT token. Copy this token and add it to the Authorization header in Swagger for authenticated requests.
Using the token in Swagger:

Click the "Authorize" button at the top
Paste your JWT token in the value field
Click "Authorize" - now all requests will include the token

**Get Popular Feed**

```http
GET /api/proxy/posts/popular-feed
```

The proxy accepts any HTTP method (GET, POST, PUT, DELETE, PATCH) and forwards everything to the actual API. Path, query params, headers, body - all get passed through.

### Monitoring & Analytics

**View All Logs**

```http
GET /api/logs?query-params
```

Query params:

- `page` - page number (default: 0)
- `limit` - results per page, max 200 (default: 20)
- `sortBy` - sort field: `createdAt`, `responseTime`, `statusCode`
- `sortDir` - sort direction: `asc`, `desc`
- `method` - filter by HTTP method (can pass multiple: `method=GET,POST`)
- `statusCode` - filter by status code (can pass multiple: `statusCode=200,404`)
- `responseTime[gte]` - min response time in ms
- `responseTime[lte]` - max response time in ms
- `createdAt[gte]` - filter from date (ISO 8601 format)
- `createdAt[lte]` - filter to date (ISO 8601 format)
- `search` - search in request/response data

Example:

```http
GET /api/logs?query-params
```

**View Problematic Logs**

```http
GET /api/logs/problems?query-params
```

Same query params as regular logs. This endpoint filters for requests that had issues - errors, timeouts, unusual patterns, etc.

## How It Works

The proxy forwards requests to the actual social network API hosted on Azure. Every request gets logged, and certain patterns get flagged as problematic for review. This is useful for:

- Debugging issues in production
- Monitoring API usage
- Catching suspicious activity
- Understanding traffic patterns

## Development

The Docker setup handles everything - database, API, networking. Just make sure your `.env` is configured correctly and you're good to go.

## Notes

- This was built for a hackathon, so some edges might be rough
- The underlying social network API is separate and hosted on Azure
- Logs persist in the database between restarts

## Troubleshooting

**Port already in use?**

```bash
# Change the port in docker-compose.yml
ports:
  - "8001:8000"  # or any other available port
```

Hopefully everything works :)
