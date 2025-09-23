package com.capstone.Arogya.service;

import com.capstone.Arogya.dto.*;
import lombok.RequiredArgsConstructor;
import org.springframework.http.*;
import org.springframework.stereotype.Service;
import org.springframework.web.client.RestTemplate;

@Service
@RequiredArgsConstructor
public class AiService {

    private final RestTemplate restTemplate = new RestTemplate();
    private final String OLLAMA_API_URL = "https://ai.blackhatbadshah.com/api/generate"; // replace with actual endpoint

    /**
     * Get personalized health suggestions for a user.
     */
    public String getHealthSuggestions(String userProfile) {
        HttpHeaders headers = new HttpHeaders();
        headers.setContentType(MediaType.APPLICATION_JSON);

        // Request body for Ollama AI
        String body = "{ \"model\": \"Blackhatbadshah\", \"prompt\": \"Suggest ways to improve health for: " + userProfile + "\", \"stream\": false }";
            HttpEntity<String> request = new HttpEntity<>(body, headers);

            ResponseEntity<AiResponse> response = restTemplate.postForEntity(
                    OLLAMA_API_URL,
                    request,
                    AiResponse.class
            );
        if (response.getStatusCode().is2xxSuccessful()) {
            return response.getBody() != null ? response.getBody().getResponse() : "";
        } else {
            throw new RuntimeException("Failed to get AI suggestions: " + response.getStatusCode());
        }
    }
}
