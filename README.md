# ReqRes API Client in ASP.NET Core

This project demonstrates a clean, testable service that interacts with the [reqres.in](https://reqres.in) API. It fetches users via HTTP and demonstrates proper API client practices including:

- `HttpClient` with `IHttpClientFactory`
- Async/await usage
- Error handling and custom exceptions
- API key usage (`x-api-key`)
- In-memory caching
- JSON deserialization and data mapping

---

## 🚀 Features

- `GET /api/User/users` to get paginated users
- `GET /api/User/getuser/{id}` to get single user details
- Caching for user and all-user queries
- Custom exception for 404 handling
- Configurable API base URL and key

---

## 🔧 Setup Instructions

### 1. Clone the Repository

```bash
git clone https://github.com/MrAbhiAgg/TestApp.git
