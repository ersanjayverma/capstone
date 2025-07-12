# Zero Trust Access Control System (ZTACS)

# 💡 Combines cybersecurity, DevOps, and CI to secure enterprise systems.

# 

# 🚀 Project Summary:

# Build a Zero Trust access control system that uses behavioral analytics to grant or deny access to services (like internal APIs, cloud resources, or microservices). All access is treated as untrusted, even from inside the network.

# 

# 🧱 Key Features:

# User \& Service Authentication: JWT/OAuth2 + Device Fingerprinting.

# 

# Real-time Behavior Monitoring: Track login locations, access times, and unusual API usage patterns.

# 

# Policy Engine: Dynamic access control policies based on user role, risk score, and context.

# 

# Audit Trail Dashboard: Full user/service activity logs in Grafana or a custom UI.

# 

# 🔧 Tech Stack:

# Area	Stack

# Backend	C# (.NET)

# Frontend	Blazor

# Authentication	OAuth2 / JWT

# Data Store	MYSQL , Redis (session), ElasticSearch (logs)

# DevOps	Docker, Kubernetes, GitHub Actions Monitoring Prometheus, Grafana



✅ PROGRESS VS VISION: A QUICK MATCH

ZTACS Component	Status	Notes

Authentication (JWT/OAuth2)	✅ Done	Keycloak integrated with claims

Device Fingerprinting	⚠️ Pending	Currently only "BlazorWASM" string — extend to fingerprint

Real-time Behavior Monitoring	✅ Core Done	Login logs, threat scoring, enrichment complete

Threat Scoring System	✅ Done	Risk-based scores + reasons

Policy Engine (Role + Context)	🟡 Started	Role-based logic placeholder; dynamic policies next

Audit Dashboard	✅ Basic Done	Working logs, profiles — UI powered

Realtime Updates (SignalR)	❌ Not yet	Next milestone

Export / Reporting	🟡 CSV Done	PDF exports, compliance reports pending

DevOps (CI/CD, Containers)	🟡 Docker OK	Needs GitHub Actions + Kubernetes setup

Monitoring (Grafana, Prometheus)	❌ Not yet	Final milestone

