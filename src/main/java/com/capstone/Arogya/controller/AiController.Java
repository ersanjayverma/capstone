package com.capstone.Arogya.controller;

import org.springframework.http.MediaType;
import org.springframework.web.bind.annotation.*;
// import your service classes here
// import reactor.core.publisher.Flux; // if streaming responses (WebFlux)

@RestController
@RequestMapping("/api/ai")
public class AIController {


    private final AiService aiService;

    public AIController(AiService aiService) {
        this.aiService = aiService;
    }

    /**
     * Basic GET endpoint example
     */
    @GetMapping("/status")
    public String getStatus() {
        return "AI Controller is running.";
    }

}