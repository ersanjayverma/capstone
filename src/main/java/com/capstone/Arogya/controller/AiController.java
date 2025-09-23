package com.capstone.Arogya.controller;

import com.capstone.Arogya.service.AiService;
import lombok.RequiredArgsConstructor;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;
import com.capstone.Arogya.service.*; 
import org.springframework.security.core.Authentication;
import com.capstone.Arogya.dto.*;
import java.util.List;
import java.util.stream.Collectors;

@RestController
@RequestMapping("/api/ai")
@RequiredArgsConstructor
public class AiController {

    private final AiService aiService;
    private final AuthService authService;
    private final QuestionService questionService;
    @GetMapping("/health-suggestions")
    public ResponseEntity<String> getHealthSuggestions(Authentication authentication) {

        // Get username from the authenticated principal
        String username = authentication.getName();

        // Fetch user ID from AuthService
        Long userId = authService.getUserId(username);
        
        List<AnswerDto> answers = questionService.getAnswersForUser(userId);

        if (answers.isEmpty()) {
            return ResponseEntity.ok("No answers yet.");
        }

        // Construct a simple profile string
        String userProfile = answers.stream()
                .map(a -> {
                    String answerText = a.getQuestionType().name().equals("MCQ") 
                        ? (a.getSelectedOptionText() != null ? a.getSelectedOptionText() : "")
                        : (a.getAnswerText() != null ? a.getAnswerText() 
                                                    : (a.getAnswerInt() != null ? a.getAnswerInt().toString() : ""));
                    return a.getQuestionId() + ": " + answerText;
                })
                .collect(Collectors.joining("; "));

        String suggestions = aiService.getHealthSuggestions(userProfile);
        return ResponseEntity.ok(suggestions);
    }
}
