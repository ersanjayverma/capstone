package com.capstone.Arogya.service;

import com.capstone.Arogya.dto.AiResponse;
import lombok.RequiredArgsConstructor;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.boot.web.client.RestTemplateBuilder;
import org.springframework.http.*;
import org.springframework.stereotype.Service;
import org.springframework.web.client.RestClientResponseException;
import org.springframework.web.client.RestTemplate;
import com.fasterxml.jackson.databind.ObjectMapper;

import java.time.Duration;

@Service
@RequiredArgsConstructor
public class AiService {

    private final RestTemplateBuilder restTemplateBuilder;
    private final ObjectMapper objectMapper;

    @Value("${ai.ollama.url:https://ai.blackhatbadshah.com/api/generate}")
    private String ollamaApiUrl;

    @Value("${ai.ollama.model:Blackhatbadshah}")
    private String modelName;

    private RestTemplate restTemplate() {
        return restTemplateBuilder
                .setConnectTimeout(Duration.ofSeconds(5))
                .setReadTimeout(Duration.ofSeconds(30))
                .build();
    }

    public String getHealthSuggestions(String userProfile) {
        HttpHeaders headers = new HttpHeaders();
        headers.setContentType(MediaType.APPLICATION_JSON);

        String prompt = "Suggest practical, safe, and personalized health improvements based on: " + userProfile;
        String payload = """
                {"model":"%s","prompt":"%s","stream":false}
                """.formatted(modelName, escape(prompt));

        HttpEntity<String> request = new HttpEntity<>(payload, headers);

        try {
            ResponseEntity<String> response = restTemplate().postForEntity(ollamaApiUrl, request, String.class);
            if (!response.getStatusCode().is2xxSuccessful() || response.getBody() == null) {
                throw new IllegalStateException("Upstream error: " + response.getStatusCode());
            }
            AiResponse ai = objectMapper.readValue(response.getBody(), AiResponse.class);
            return ai.getResponse() != null ? ai.getResponse() : "";
        } catch (RestClientResponseException ex) {
            throw new IllegalStateException("Upstream error: " + ex.getRawStatusCode() + " " + ex.getResponseBodyAsString(), ex);
        } catch (Exception ex) {
            throw new IllegalStateException("Failed to get AI suggestions", ex);
        }
    }

    private static String escape(String s) {
        return s.replace("\\", "\\\\").replace("\"", "\\\"");
    }
}
