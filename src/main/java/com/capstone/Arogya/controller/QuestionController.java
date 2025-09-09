package com.capstone.Arogya.controller;

import com.capstone.Arogya.dto.AnswerDto;
import com.capstone.Arogya.dto.SubmitAnswerDto;
import com.capstone.Arogya.service.QuestionService;
import lombok.RequiredArgsConstructor;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/api/answers")
@RequiredArgsConstructor
public class QuestionController {

    private final QuestionService questionService;

    // Submit an answer
    @PostMapping
    public ResponseEntity<AnswerDto> submitAnswer(@RequestBody SubmitAnswerDto dto) {
        AnswerDto saved = questionService.submitAnswer(dto);
        return ResponseEntity.ok(saved);
    }

    // Fetch all answers for a specific user
    @GetMapping("/user/{userId}")
    public ResponseEntity<List<AnswerDto>> getAnswersForUser(@PathVariable Long userId) {
        return ResponseEntity.ok(questionService.getAnswersForUser(userId));
    }
}
