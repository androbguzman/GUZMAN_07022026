# ABGFileProcessorAPI

A secure, containerized RESTful web service built using ASP.NET Core 8.0. This application provides secure endpoints to process uploaded files in both JSON and CSV formats, features an in-memory execution tracker, and utilizes custom middleware for API Key verification.

## Features
- Dual Format Processing: Automatically detects and routes files based on extension such as '.json' or '.csv'.
- JSON Validation: Validates JSON structural integrity.
- CSV Aggregation: Automatically calculates structural data averages (which calculates the average of numerical parameters passed in the second column).
- Custom Security Middleware: Secures processing endpoints via an incoming request header token check ('X-API-KEY').
- Live Performance Reporting: Tracks files processed dynamically via an in-memory singleton tracker.
- Dockerized Architecture: Built with a clean multi-stage Docker environment for portable deployments.

---

## Technical Architecture Overview

The application processes requests through a decoupled, multi-layered architecture:
1. Client Request: The client triggers an HTTP Request to the API port.
2. Middleware Layer ('ApiKeyMiddleware): Inspects headers for authentication. If the key is missing or invalid, it returns '401 Unauthorized' before reaching controllers (excludes Swagger endpoints for accessibility).
3. Controller Layer ('FileController'): Manages endpoints, determines incoming file extensions, and runs core routing.
4. Service Layer ('TrackingService'): A thread-safe Singleton store ('ConcurrentBag') that logs execution stats throughout the application lifecycle.

---

## Local Development Execution

To run this application locally without Docker, ensure you have the .NET 8 SDK installed:

'''bash
# Navigate to the project root
cd ABGFileProcessorAPI

# Restore dependencies and execute the application
dotnet run

Check the localhost and go to browser. Change the local host on the example link below.
Ex. http://localhost:5107/swagger/index.html

Header Key: X-API-KEY
Header Value: ABG_Secure_Secret_Key_123

--

## Complete Docker Deployment & Postman Testing Guide

Follow these sequential steps to build the Docker container and test your endpoints immediately using Postman:

- Step 1: API Security Requirements
  All secured endpoints inside the Docker container require an authentication key. Ensure you supply this exact custom header configuration in Postman for all operational requests:
  Header Key: X-API-KEY
  Header Value: ABG_Secure_Secret_Key_123

- Step 2: Build and Run inside Docker
  Run these commands in your terminal deployment environment (do not forget the trailing period . in the build instruction):
  Bash
  # 1. Build the lightweight production Docker Image
  docker build -t abg-file-api .
  # 2. Spin up the container and map it to local network port 8080
  docker run -d -p 8080:8080 --name abg_service abg-file-api
  Your secured API service is now active and exposed at: http://localhost:8080

- Step 3: Verify Initial Status in Postman (GET)
  Ensure your clean service state is healthy and responsive before uploading data:
  Open a new request tab in Postman and select the GET method dropdown.
  Input the reporting endpoint URL: http://localhost:8080/api/file/report
  Navigate to the Headers sub-tab and enter your security credential:
  Key: X-API-KEY
  Value: ABG_Secure_Secret_Key_123
  Click Send.

- Step 4: Upload and Parse a JSON File (POST)
  Let's feed a structured JSON document to your running Docker container:
  Open another new tab in Postman and select the POST method dropdown.
  Input the target upload URL: http://localhost:8080/api/file/upload
  Go to the Headers tab and re-apply your security header token:
  Key: X-API-KEY
  Value: ABG_Secure_Secret_Key_123
  Switch to the Body tab and select the form-data option.
  In the Key field, type exactly file. Hover your cursor over the right side of that input cell and change the type selector from Text to File.
  In the Value column, click Select Files and choose your local JSON asset (e.g., test.json).
  Sample Valid JSON content layout:
  JSON
  {
    "status": "working"
  }
  Click Send.

- Step 5: Upload and Compute Aggregates for a CSV File (POST)
  Using your active POST interface in Postman, swap your configuration to evaluate tabular computing:
  Under your Body -> form-data asset picker grid, clear out your previous JSON file.
  Click Select Files again and choose your local tabular numeric file (e.g., data.csv).
  Sample Valid CSV data structure:
  Item,Score
  ProductA,100
  ProductB,50
  Click Send.

- Step 6: Review Cumulative Container Execution Statistics (GET)
  Re-focus on your initial Postman terminal window running the GET report query.
  Click Send a second time to inspect state changes.