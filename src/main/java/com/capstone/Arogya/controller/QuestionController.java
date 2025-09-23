package com.capstone.Arogya.controller;

import com.capstone.Arogya.dto.*;
import com.capstone.Arogya.service.QuestionService;
import lombok.RequiredArgsConstructor;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;
import com.capstone.Arogya.service.AuthService; 
import org.springframework.security.core.Authentication;

import java.util.List;

@RestController
@RequestMapping("/api/questions")
@RequiredArgsConstructor
public class QuestionController {

    private final QuestionService questionService;
    private final AuthService authService;

    // Fetch all answers for a specific user
    @GetMapping("/user/{userId}")
    public ResponseEntity<List<AnswerDto>> getAnswersForUser(@PathVariable Long userId) {
        return ResponseEntity.ok(questionService.getAnswersForUser(userId));
    }

    // Fetch all answers for a specific question
    @GetMapping("/question/{questionId}")
    public ResponseEntity<List<AnswerDto>> getAnswersForQuestion(@PathVariable Long questionId) {
        return ResponseEntity.ok(questionService.getAnswersForQuestion(questionId));
    }

    
        @GetMapping("/next")
        public ResponseEntity<QuestionDto> getNextQuestion(Authentication authentication) {
            // Get username from the authenticated principal
            String username = authentication.getName();

            // Fetch user ID from AuthService
            Long userId = authService.getUserId(username);

            // Fetch next question
            QuestionDto nextQuestion = questionService.getNextQuestionForUser(userId);

            return ResponseEntity.ok(nextQuestion);
        }

}
