package com.capstone.Arogya.controller;

import com.capstone.Arogya.dto.AnswerDto;
import com.capstone.Arogya.dto.SubmitAnswerDto;
import com.capstone.Arogya.service.QuestionService;
import lombok.RequiredArgsConstructor;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/api/questions")
@RequiredArgsConstructor
public class QuestionController {

    private final QuestionService questionService;


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
}
