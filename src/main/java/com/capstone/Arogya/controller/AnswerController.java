package com.capstone.Arogya.controller;

import com.capstone.Arogya.dto.AnswerDto;
import com.capstone.Arogya.dto.SubmitAnswerDto;
import com.capstone.Arogya.service.QuestionService;
import lombok.RequiredArgsConstructor;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;
import com.capstone.Arogya.service.AuthService; 
import org.springframework.security.core.Authentication;

import java.util.List;

@RestController
@RequestMapping("/api/answers")
@RequiredArgsConstructor
public class AnswerController {

    private final QuestionService questionService;
    private final AuthService authService;
    @PostMapping
    public ResponseEntity<AnswerDto> submitAnswer(@RequestBody SubmitAnswerDto dto,
                                                Authentication authentication) {
        // Get username from JWT token
        String username = authentication.getName();

        // Get userId from AuthService
        Long userId = authService.getUserId(username);

        // Pass userId to the service along with DTO
        AnswerDto saved = questionService.submitAnswer(dto, userId);

        return ResponseEntity.ok(saved);
    }
}
