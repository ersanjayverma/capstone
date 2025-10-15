package com.capstone.Arogya.config;

import io.swagger.v3.oas.annotations.OpenAPIDefinition;
import io.swagger.v3.oas.annotations.enums.SecuritySchemeType;
import io.swagger.v3.oas.annotations.info.Info;
import io.swagger.v3.oas.annotations.security.*;
import io.swagger.v3.oas.annotations.servers.Server;
import org.springframework.context.annotation.Configuration;

@OpenAPIDefinition(
    info = @Info(title = "Arogya API", version = "v1", description = "APIs for Arogya"),
    security = @SecurityRequirement(name = "bearerAuth"),
    servers = {
        @Server(url = "https://ai.blackhatbadshah.com", description = "Production (HTTPS)")
    }
)
@SecurityScheme(
    name = "bearerAuth",
    type = SecuritySchemeType.HTTP,
    scheme = "bearer",
    bearerFormat = "JWT",
    description = "Provide the JWT token. Example: 'Bearer eyJhbGciOi...'"
)
@Configuration
public class OpenApiConfig {
    // no beans required for basic setup
}
