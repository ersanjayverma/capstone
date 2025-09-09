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
public class AnswerController {

    private final QuestionService questionService;

    @PostMapping
    public ResponseEntity<AnswerDto> submitAnswer(@RequestBody SubmitAnswerDto dto) {
        AnswerDto saved = questionService.submitAnswer(dto);
        return ResponseEntity.ok(saved);
    }

    @GetMapping("/question/{questionId}")
    public ResponseEntity<List<AnswerDto>> getAnswersForQuestion(@PathVariable Long questionId) {
        return ResponseEntity.ok(questionService.getAnswersForQuestion(questionId));
    }
}
