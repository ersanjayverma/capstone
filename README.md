🩺 Arogya API – Intelligent Health Microservice

Arogya is a modern, modular Spring Boot application that blends microservice architecture, secure stateless authentication, and AI-driven health insights powered by Retrieval-Augmented Generation (RAG).

It demonstrates industry-standard design across API development, JWT-based security, OpenAPI documentation, third-party API integration, and vector-based AI augmentation.

📘 Table of Contents

Overview

Architecture

Core Modules

Key Features

Security Architecture (JWT)

API Documentation (Swagger)

Third-Party API Resilience

AI Intelligence Layer (RAG)

Tech Stack

Setup & Configuration

Best Practices

License

🧩 Overview

Arogya is designed to serve as a scalable, stateless health intelligence platform, exposing RESTful APIs for:

User registration and authentication

Questionnaire management

Answer submission and storage

AI-driven personalized health suggestions

It combines traditional REST design with AI augmentation, offering contextual, data-backed responses using RAG.

🏗️ Architecture

Arogya follows modular microservice principles:

┌──────────────────────────┐
│        API Layer         │  ← Controllers (Auth, Questions, Answers, AI)
└─────────────┬────────────┘
              │
┌─────────────┴────────────┐
│       Service Layer       │  ← Business logic & integrations
└─────────────┬────────────┘
              │
┌─────────────┴────────────┐
│     Persistence Layer     │  ← MySQL for structured data
│     Vector Layer (Qdrant) │  ← For embeddings & AI retrieval
└──────────────────────────┘


Each layer maintains clear separation of concerns, improving testability, scalability, and maintainability.

⚙️ Core Modules
Module	Description
Authentication	User registration and login via JWT
Questions	Retrieves user-specific health questions
Answers	Captures and stores user responses
AI	Generates personalized health insights via RAG
Security	Stateless authentication using JWT & Spring Security
Swagger	Auto-generated, interactive API documentation
🔐 Security Architecture (JWT)

Arogya employs stateless authentication with JSON Web Tokens.
Each request is self-contained — carrying all required identity information for validation.

Components

JwtAuthenticationFilter → Extracts and validates tokens per request

JwtUtil → Generates, signs, and parses JWTs

SecurityConfig → Defines security filter chain and route permissions

Flow

User logs in → receives a signed JWT

Every request includes Authorization: Bearer <token>

Filter validates signature, expiration, and roles

Valid tokens populate SecurityContextHolder for RBAC enforcement

Benefits

Fully stateless → no session storage

Scalable across distributed instances

Supports Role-Based Access Control (RBAC)

📖 API Documentation (Swagger)

Arogya integrates Swagger (OpenAPI 3) for real-time documentation.

Accessible endpoints:

/v3/api-docs
/swagger-ui/index.html

Highlights

Interactive “Try it out” testing

JWT Bearer authentication within UI

Auto-generated models for all DTOs

Logical grouping by tags: Authentication, Questions, Answers, AI

SecurityConfig explicitly permits Swagger endpoints while securing operational routes.

🌐 Third-Party API Resilience

The AiService module integrates with external AI endpoints (e.g., https://ai.blackhatbadshah.com/api/generate).

Resilience Design

Centralized service for outbound calls

Retry and timeout handling

Circuit breaker pattern (Resilience4j)

Configurable endpoints and thresholds

Structured logging with correlation IDs

Example Configuration
ai.api.url=https://ai.blackhatbadshah.com/api/generate
ai.api.timeout=5000
ai.api.retries=3


If the AI endpoint fails, the system degrades gracefully instead of breaking downstream flows.

🧠 AI Intelligence Layer (RAG)

Arogya’s AI module uses Retrieval-Augmented Generation (RAG) for context-aware health recommendations.

Process

Ingest domain data (questions, answers, knowledge sources).

Chunk text into 500–800 token fragments.

Embed chunks into semantic vectors.

Store embeddings in Qdrant; metadata in MySQL.

Retrieve top-N similar vectors on user query.

Augment prompt with context and user profile.

Generate personalized AI suggestions.

Benefits

Prevents hallucination

Produces factual, explainable responses

Ensures traceability and accuracy

🧰 Tech Stack
Category	Technology
Backend	Spring Boot 3.x
Language	Java 17+
Security	Spring Security, JWT
Docs	Swagger / OpenAPI 3
Database	MySQL
Vector DB	Qdrant / LanceDB
AI Integration	Ollama / REST API
Build Tool	Maven
Resilience	Resilience4j, RestTemplate / WebClient
⚙️ Setup & Configuration

Clone Repository

git clone https://github.com/<your-org>/arogya-api.git
cd arogya-api


Set Environment Variables

export JWT_SECRET=<secure-random-key>
export JWT_EXPIRATION_MS=3600000
export AI_API_URL=https://ai.blackhatbadshah.com/api/generate


Run Application

mvn spring-boot:run


Access Swagger UI

http://localhost:8080/swagger-ui/index.html

🧩 Best Practices

Use DTOs to decouple domain and API contracts.

Keep JWT secrets secure and tokens short-lived.

Refresh embeddings when data changes.

Enable circuit breakers and timeouts on all API calls.

Version APIs under /api/v1/.

Disable Swagger in production unless authenticated.

Log AI context, queries, and outputs for audit.

🪪 License

© 2025 Blackhatbadshah
All rights reserved.

This project and its documentation are intended for educational and enterprise use.
