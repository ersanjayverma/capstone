# ---------------------------------------------------
# Stage 1 — Build the app using Maven & JDK 21
# ---------------------------------------------------
FROM ubuntu:24.04 AS builder

# Install dependencies
RUN apt-get update && apt-get install -y \
    openjdk-21-jdk \
    maven \
    && rm -rf /var/lib/apt/lists/*

# Set work directory
WORKDIR /app

# Copy Maven project files
COPY pom.xml .
COPY src ./src

# Build application
RUN mvn -U clean package -DskipTests

# ---------------------------------------------------
# Stage 2 — Runtime image (Ubuntu with JRE only)
# ---------------------------------------------------
FROM ubuntu:24.04

# Install only JRE for smaller footprint
RUN apt-get update && apt-get install -y openjdk-21-jre-headless && \
    rm -rf /var/lib/apt/lists/*

# Environment variables
ENV APP_HOME=/opt/arogya \
    JAVA_OPTS="-Xms256m -Xmx512m"

# Create app directory
WORKDIR $APP_HOME

# Copy the built jar from builder stage
COPY --from=builder /app/target/Arogya-0.0.1-SNAPSHOT.jar $APP_HOME/app.jar

# Expose default Spring Boot port
EXPOSE 8080

# Entry point
ENTRYPOINT ["sh", "-c", "java $JAVA_OPTS -jar app.jar"]
